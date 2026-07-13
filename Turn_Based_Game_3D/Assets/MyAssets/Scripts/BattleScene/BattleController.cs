using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;
using Assets.MyAssets.Scripts.BattleScene.Core;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class BattleController : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler _playerInputHandler;    // 플레이어 입력 핸들러
    [SerializeField] private BattleUIController _battleUIController;    // 전투 UI 컨트롤러
    [SerializeField] private TargetSelector     _targetSelector;        // 플레이어가 현재 선택한 타겟을 알려주는 Selector
    [SerializeField] private BattleUnitLinker   _battleUnitLinker;      // 전투 유닛들의 뷰와 해당 이벤트를 구독시켜주는 링커
    [SerializeField] private UnitSpawner        _unitSpawner;           // 죽은 유닛 풀에 반납하기 위해 스포너 참조
    [SerializeField] private BattleBalanceData  _battleBalanceData;     // 데미지 공식 밸런싱 상수(SO)

    private BattleCore _battleCore;    // 턴 진행/승패 판정/데미지 적용을 담당하는 순수 C# 로직

    private List<PlayerUnitView>    _playerUnitViews;           // 플레이어 캐릭터 유닛 뷰 리스트
    private List<EnemyUnitView>     _enemyUnitViews;            // 적 캐릭터 유닛 뷰 리스트

    private bool  _playerActed;           // 플레이어가 현재 턴에서 행동을 완료했는지 여부, true이면 플레이어가 행동을 마쳤음을 나타냄
    private PlayerBattleUnit _actingPlayer;   // 현재 턴에서 행동 중인 플레이어 유닛

    public int TurnCount => _battleCore.TurnCount;
    public List<PlayerBattleUnit> PlayerUnits => _battleCore.PlayerUnits;

    //TODO#: 현재는 OnTurnStart라고 이름을 정했지만, 자신의 차례가 되었을때 카메라 무빙, UI 업데이트 등등의 처리를 하기 위한 이벤트이므로 나중에 이름을 변경할 수도 있음
    public event Action<BattleUnit>         OnTurnStart;            // 턴이 시작될 때마다 호출되는 이벤트, 현재 턴에서 행동할 유닛을 인자로 전달
    public event Action<IDamageable, int>   OnUnitDamaged;          // 유닛이 피해를 입었을 때 호출되는 이벤트, 피해를 입은 유닛과 입은 피해량을 인자로 전달
    public event Action<bool>               OnBattleEnd;            // 전투가 종료될 때 호출되는 이벤트, 플레이어가 승리했는지 여부를 인자로 전달
    public event Action<PlayerBattleUnit>   OnPlayerActionComplete; // 플레이어의 행동이 종료되고 처리할 이벤트(MP갱신 등등)
    public event Action<int>                OnTurnChanged;          // 턴이 변경될 때 처리할 이벤트(턴 UI 업데이트, 유닛 행동 순서 갱신)
    public event Action<BattleUnit>         OnEnemyDied;            // 적이 죽었을 때 처리할 이벤트
    public event Action<List<PlayerBattleUnit>> OnBattleStarted;    // 전투 시작 시 파티 구성을 전달하는 이벤트

    private void Awake() => Initialize();
    private void Initialize()
    {
        _battleCore = new BattleCore(_battleBalanceData != null ? _battleBalanceData.DefenseConstant : 100);

        // BattleCore가 발행하는 이벤트를 동일한 이름의 이벤트로 그대로 전달(forward)
        _battleCore.OnTurnStart            += unit => OnTurnStart?.Invoke(unit);
        _battleCore.OnUnitDamaged          += (target, damage) => OnUnitDamaged?.Invoke(target, damage);
        _battleCore.OnBattleEnd            += isWin => OnBattleEnd?.Invoke(isWin);
        _battleCore.OnPlayerActionComplete += player => OnPlayerActionComplete?.Invoke(player);
        _battleCore.OnTurnChanged          += turn => OnTurnChanged?.Invoke(turn);
        _battleCore.OnEnemyDied            += unit => OnEnemyDied?.Invoke(unit);
        _battleCore.OnBattleStarted        += players => OnBattleStarted?.Invoke(players);

        _playerUnitViews = new List<PlayerUnitView>();
        _enemyUnitViews  = new List<EnemyUnitView>();
    }

    private void Start() => SetUp();
    private void SetUp()
    {
        _playerInputHandler.Subscribe(this, _targetSelector);
        _battleUIController.Subscribe(this, _playerInputHandler);

        _targetSelector.OnTargetChanged += HandleTargetChanged;
    }

    private void OnDestroy()
    {
        _targetSelector.OnTargetChanged -= HandleTargetChanged;
    }

    /// <summary>
    /// 전투가 시작되면 한번만 호출되는 메소드 -> 플레이어 유닛과 적 유닛을 생성 / 전투에 참여하는 모든 유닛을 리스트에 추가 / 턴 시스템 초기화
    /// </summary>
    public void StartBattle(List<PlayerBattleUnit> players, List<EnemyBattleUnit> enemies)
    {
        foreach (var player in players)
        {
            PlayerUnitView view = _battleUnitLinker.GetPlayerUnitView(player);
            if (view != null)
            {
                _playerUnitViews.Add(view);
            }
        }

        foreach (var enemy in enemies)
        {
            EnemyUnitView view = _battleUnitLinker.GetEnemyUnitView(enemy);
            if (view != null)
            {
                _enemyUnitViews.Add(view);
            }
        }

        _battleCore.SetupBattle(players, enemies);

        _targetSelector.Initialize(_enemyUnitViews);

        _ = BattleLoop();
    }

    private async Awaitable BattleLoop()
    {
        BattleUnit currentUnit;

        while (!await CheckBattleEndAsync())
        {
            currentUnit = _battleCore.BeginUnitTurn();  // 현재 턴에서 행동할 유닛을 가져오고 OnTurnStart 발행

            if (currentUnit.IsPlayer)
            {
                await PlayerTurn(currentUnit as PlayerBattleUnit);
            }
            else
            {
                await EnemyTurn(currentUnit);
            }

            _battleCore.AdvanceTurn();
        }
    }

    private async Awaitable<bool> CheckBattleEndAsync()
    {
        BattleResult result = _battleCore.EvaluateBattleEnd();

        if (result == BattleResult.Ongoing) return false;

        if (result == BattleResult.PlayerLose)
        {
            foreach (var playerView in _playerUnitViews)
                await playerView.OnDeathAsync();
        }

        _battleCore.NotifyBattleEnd(result == BattleResult.PlayerWin);
        return true;
    }

    private async Awaitable EnemyTurn(BattleUnit currentUnit)
    {
        EnemyBattleUnit currentEnemy = currentUnit as EnemyBattleUnit;
        if (currentEnemy == null)
        {
            Debug.LogError("현재 유닛이 EnemyBattleUnit이 아님");
            return;
        }

        if (currentEnemy.SkipFirstAction)
        {
            currentEnemy.SkipFirstAction = false;
            return; // 로그라이크 "몬스터 행동불가" 디버프 — 이번 턴은 행동 없이 지나감
        }

        currentEnemy.ReduceCooldowns();

        EnemySkillData skill = currentEnemy.GetEnemySkill();

        // TODO#: 적의 행동을 결정하는 AI로직 필요 -> 어떤 스킬을 사용할지 결정
        if (skill != null)
        {
            EnemyUnitView enemyView = _enemyUnitViews.Find(v => v.LinkedUnit == currentEnemy);

            // 살아있는 파티원 중 첫 번째를 타겟으로 삼음 (단순 타겟팅 AI, 정교한 타겟 선택은 이후 과제)
            PlayerBattleUnit targetPlayer = _battleCore.GetFirstAlivePlayer();
            PlayerUnitView   playerView   = _playerUnitViews.Find(v => v.LinkedUnit == targetPlayer);

            if (enemyView != null && playerView != null)
            {
                await enemyView.PlayAttackAnimAsync(playerView.transform, async () =>
                {
                    int damage = _battleCore.CalculateAttackDamage(currentEnemy, targetPlayer, (int)skill.Power);
                    _battleCore.ApplyDamage(targetPlayer, damage); // 스킬 사용 시 타겟에게 데미지를 입히고 OnUnitDamaged 발행

                    await playerView.OnDamagedAsync(damage);
                });

                currentEnemy.SetCooldown(skill); // 스킬 사용 후 쿨 타임 적용
            }
        }
        else
        {
            Debug.Log("스킬을 가져올 수 없어서 비어있음");
        }
    }

    private async Awaitable PlayerTurn(PlayerBattleUnit player)
    {
        _actingPlayer = player;

        // 턴 시작 시 스태미나 회복
        _battleCore.RecoverPlayerStamina(player);

        _battleCore.NotifyPlayerActionComplete(player); // UI 업데이트 #TODO: 이름 조정필요해 보임 ActionComplete말고 UI업데이트? 하면 될듯?
        _battleCore.NotifyTurnStart(player);            // 스킬 버튼 다시 빌드

        if (!player.HasAnyUsableWeapon())
        {
            Debug.Log("사용 가능한 무기 없음 - 턴 스킵");
            return;
        }

        // 모든 몬스터 Idle ↔ Roar 시작
        StartEnemyRoarLoop();

        _playerActed = false;

        while (!_playerActed)
        {
            await Awaitable.NextFrameAsync();
        }
    }

    private void HandleTargetChanged(EnemyUnitView prevTarget, EnemyUnitView nextTarget)
    {
        prevTarget?.SetAsTarget(false);
        nextTarget?.SetAsTarget(true);
    }

    public EnemyUnitView GetEnemyUnitView(BattleUnit target)
    {
        return _enemyUnitViews.Find(v => v.LinkedUnit == target);
    }

    private void StartEnemyRoarLoop()
    {
        foreach (var enemyView in _enemyUnitViews)
        {
            EnemyAnimator enemyAnimator = enemyView.GetComponent<EnemyAnimator>();
            enemyAnimator?.StartIdleRoarLoop();
        }
    }

    private void StopEnemyRoarLoop()
    {
        foreach (var enemyView in _enemyUnitViews)
        {
            EnemyAnimator enemyAnimator = enemyView.GetComponent<EnemyAnimator>();
            enemyAnimator?.StopIdleRoarLoop();
        }
    }

    public void SetPlayerActed()
    {
        _playerActed = true;
        _battleCore.NotifyPlayerActionComplete(_actingPlayer);
    }

    public async void OnPlayerAction(IDamageable target, int weaponIndex)
    {
        // 플레이어가 행동을 시작하면 몬스터들의 Idle - Roar 루프를 멈춤
        StopEnemyRoarLoop();

        // 전달받은 타겟이 비어있으면 자동으로 살아있는 적 찾아서 타겟으로 설정
        if (target == null)
            target = _battleCore.GetFallbackTarget();

        // 다시 검사했는데 살아있는적이 없으면 리턴
        if (target == null)
        {
            Debug.LogError("타겟이 없습니다!");
            return;
        }

        PlayerBattleUnit player = _actingPlayer;
        if (!player.CanUseWeapon(weaponIndex))
        {
            Debug.Log("무기를 사용할 수 없습니다!");
            return;
        }

        // TODO#: 스킬의 데미지 계산 공식은 나중에 스킬 시스템이 완성되면 변경할 예정
        WeaponData weapon = player.Weapons[weaponIndex];

        int damage = _battleCore.CalculateAttackDamage(player, target as BattleUnit, weapon.Damage);

        PlayerUnitView playerView = _playerUnitViews.Find(v => v.LinkedUnit == player);
        EnemyUnitView enemyView = _enemyUnitViews.Find(v => v.LinkedUnit == target);

        PlayerAnimator playerAnimator = playerView?.GetComponent<PlayerAnimator>();

        var hitCallbackCompleted = new System.Threading.Tasks.TaskCompletionSource<bool>();

        if (playerAnimator != null)
        {
            playerAnimator.SetAttackHitCallback(async () =>
            {
                try
                {
                    player.UseWeapon(weaponIndex);
                    _battleCore.ApplyDamage(target, damage);

                    // Enemy Hit 애니메이션 + uGUI(적 체력바) 업데이트
                    if (enemyView != null)
                    {
                        await enemyView.OnDamagedAsync(damage);
                    }

                    if (target.IsDead)
                    {
                        if (enemyView != null)
                        {
                            enemyView.SetAsTarget(false);
                            _targetSelector.RemoveDeadTarget(enemyView);
                            await enemyView.OnDeathAsync();
                        }
                    }
                }
                finally
                {
                    hitCallbackCompleted.SetResult(true); // 콜백 완료 신호
                }
            });
        }

        // 공격 애니메이션 동작이 끝날때까지 대기
        await playerView.PlayAttackAnimAsync(weaponIndex, enemyView?.transform);

        playerAnimator?.SetAttackHitCallback(null);

        await hitCallbackCompleted.Task;

        if (target.IsDead)
        {
            EnemyBattleUnit deadUnit = target as EnemyBattleUnit;
            _battleCore.HandleEnemyDefeated(deadUnit);
        }

        _playerActed = true;

        _battleCore.NotifyPlayerActionComplete(player);
    }
}

}
