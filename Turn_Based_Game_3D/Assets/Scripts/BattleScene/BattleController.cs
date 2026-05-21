using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler _playerInputHandler;    // 플레이어 입력 핸들러
    [SerializeField] private BattleUIController _battleUIController;    // 전투 UI 컨트롤러
    [SerializeField] private TargetSelector     _targetSelector;        // 플레이어가 현재 선택한 타겟을 알려주는 Selector
    [SerializeField] private BattleUnitLinker   _battleUnitLinker;      // 전투 유닛들의 뷰와 해당 이벤트를 구독시켜주는 링커
    [SerializeField] private UnitSpawner        _unitSpawner;           // 죽은 유닛 풀에 반납하기 위해 스포너 참조

    private UnitOrderBySpeedSystem  _unitOrderBySpeedSystem;    // 턴마다 유닛들의 행동 순서를 결정하는 시스템
    
    private List<PlayerBattleUnit>  _playerUnits;               // 플레이어 캐릭터의 유닛
    private List<EnemyBattleUnit>   _enemyUnits;                // 적 캐릭터들의 유닛 리스트
    private List<BattleUnit>        _battleUnits;               // 현재 전투에 참여하는 모든 유닛을 저장하는 리스트

    private List<PlayerUnitView>    _playerUnitViews;           // 플레이어 캐릭터 유닛 뷰 리스트
    private List<EnemyUnitView>     _enemyUnitViews;            // 적 캐릭터 유닛 뷰 리스트

    private int   _turnCount;             // 현재 턴의 번호를 저장하는 변수
    private bool  _playerActed;           // 플레이어가 현재 턴에서 행동을 완료했는지 여부, true이면 플레이어가 행동을 마쳤음을 나타냄

    public int TurnCount => _turnCount;
    public List<PlayerBattleUnit> PlayerUnits => _playerUnits;

    //TODO#: 현재는 OnTurnStart라고 이름을 정했지만, 자신의 차례가 되었을때 카메라 무빙, UI 업데이트 등등의 처리를 하기 위한 이벤트이므로 나중에 이름을 변경할 수도 있음
    public event Action<BattleUnit>         OnTurnStart;            // 턴이 시작될 때마다 호출되는 이벤트, 현재 턴에서 행동할 유닛을 인자로 전달
    public event Action<IDamageable, int>   OnUnitDamaged;          // 유닛이 피해를 입었을 때 호출되는 이벤트, 피해를 입은 유닛과 입은 피해량을 인자로 전달
    public event Action<bool>               OnBattleEnd;            // 전투가 종료될 때 호출되는 이벤트, 플레이어가 승리했는지 여부를 인자로 전달
    public event Action<PlayerBattleUnit>   OnPlayerActionComplete; // 플레이어의 행동이 종료되고 처리할 이벤트(MP갱신 등등)
    public event Action<int>                OnTurnChanged;          // 턴이 변경될 때 처리할 이벤트(턴 UI 업데이트, 유닛 행동 순서 갱신)
    public event Action<BattleUnit>         OnEnemyDied;            // 적이 죽었을 때 처리할 이벤트

    private void Awake() => Initialize();
    private void Initialize()
    {
        _unitOrderBySpeedSystem = new UnitOrderBySpeedSystem();

        _battleUnits = new List<BattleUnit>();
        _playerUnits = new List<PlayerBattleUnit>();
        _enemyUnits  = new List<EnemyBattleUnit>();

        _playerUnitViews = new List<PlayerUnitView>();
        _enemyUnitViews  = new List<EnemyUnitView>();

        _turnCount = 1;
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
        _playerUnits = players;
        _enemyUnits  = enemies;

        _battleUnits.AddRange(_playerUnits);
        _battleUnits.AddRange(_enemyUnits);

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

        

        _unitOrderBySpeedSystem.OrderBySpeed(_battleUnits);
        
        OnTurnChanged?.Invoke(_turnCount);

        _targetSelector.Initialize(_enemyUnitViews);

        _ = BattleLoop(_battleUnits);
    }

    private async Awaitable BattleLoop(List<BattleUnit> battleUnits)
    {
        BattleUnit currentUnit;
        
        while (!await CheckBattleEndAsync())
        {
            currentUnit = _unitOrderBySpeedSystem.GetCurrentUnit();  // 현재 턴에서 행동할 유닛을 가져옴

            OnTurnStart?.Invoke(currentUnit);                        // 자신의 차례가 시작될 때마다 이벤트 호출

            if (currentUnit.IsPlayer)
            {
                await PlayerTurn();
            }
            else
            {
                await EnemyTurn(currentUnit);
            }

            if (!_unitOrderBySpeedSystem.NextUnit())
            {
                _turnCount++;

                _unitOrderBySpeedSystem.OrderBySpeed(battleUnits);   // 다음 턴을 위해 유닛의 속도에 따라 재정렬

                OnTurnChanged?.Invoke(TurnCount);
            }
        }
    }

    private async Awaitable<bool> CheckBattleEndAsync()
    {
        bool playerAllDead = _playerUnits.TrueForAll(u => u.IsDead);
        bool enemyAllDead = _enemyUnits.TrueForAll(u => u.IsDead);

        if (playerAllDead)
        {
            foreach (var playerView in _playerUnitViews)
                await playerView.OnDeathAsync();

            OnBattleEnd?.Invoke(false);
            return true;
        }

        if (enemyAllDead)
        {
            OnBattleEnd?.Invoke(true);
            return true;
        }

        return false;
    }

    private async Awaitable EnemyTurn(BattleUnit currentUnit)
    { 
        EnemyBattleUnit currentEnemy = currentUnit as EnemyBattleUnit;
        if (currentEnemy == null)
        {
            Debug.LogError("현재 유닛이 EnemyBattleUnit이 아님");
            return;
        }

        currentEnemy.ReduceCooldowns();

        EnemySkillData skill = currentEnemy.GetEnemySkill();

        // TODO#: 적의 행동을 결정하는 AI로직 필요 -> 어떤 스킬을 사용할지 결정
        if (skill != null)
        {
            EnemyUnitView enemyView = _enemyUnitViews.Find(v => v.LinkedUnit == currentEnemy);
            // TODO#: 현재는 적 입장에서는 타겟이 플레이어 한명 밖에 없으므로 _playerUnitViews[0]에서 찾지만, 나중에 플레이어 측이 늘어나면 위 방법처럼 찾는 로직 필요
            PlayerUnitView playerView = _playerUnitViews[0];

            if (enemyView != null)
            {
                await enemyView.PlayAttackAnimAsync(playerView.transform, async () =>
                {
                    
                    _playerUnits[0].TakeDamage((int)skill.Power); // 스킬 사용 시 타겟에게 스킬 데미지 만큼의 데미지를 입힘

                    OnUnitDamaged?.Invoke(_playerUnits[0], (int)skill.Power); // 유닛이 데미지를 입으면 관련 행동 실행(UI업데이트)

                    if (playerView != null)
                    {
                        await playerView.OnDamagedAsync((int)skill.Power);
                    }
                });

                currentEnemy.SetCooldown(skill); // 스킬 사용 후 쿨 타임 적용
            }           
        }
        else
        {
            Debug.Log("스킬을 가져올 수 없어서 비어있음");
        }
    }

    private async Awaitable PlayerTurn()
    {
        // #TODO: 플레이어 유닛 늘어나면 현재 턴에 해당하는 유닛을 찾는 로직 넣어야됨
        PlayerBattleUnit player = _playerUnits[0];

        // 턴 시작 시 스태미나 회복
        player.RecoverStaminaPerTurn(player.PlayerData.playerStat.StaminaRecovery);

        OnPlayerActionComplete?.Invoke(player); // UI 업데이트 #TODO: 이름 조정필요해 보임 ActionComplete말고 UI업데이트? 하면 될듯?
        OnTurnStart?.Invoke(player);            // 스킬 버튼 다시 빌드

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
        float startTime = Time.time; // 모든 몬스터 동일한 시작 시간
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
        OnPlayerActionComplete?.Invoke(_playerUnits[0]);
    }

    public async void OnPlayerAction(IDamageable target, int weaponIndex)
    {
        // 플레이어가 행동을 시작하면 몬스터들의 Idle - Roar 루프를 멈춤
        StopEnemyRoarLoop();

        // 전달받은 타겟이 비어있으면 자동으로 살아있는 적 찾아서 타겟으로 설정
        if (target == null)
            target = _enemyUnits.Find(u => !u.IsDead);

        // 다시 검사했는데 살아있는적이 없으면 리턴
        if (target == null)
        {
            Debug.LogError("타겟이 없습니다!");
            return;
        }

        PlayerBattleUnit player = _playerUnits[0];
        if (!player.CanUseWeapon(weaponIndex))
        {
            Debug.Log("무기를 사용할 수 없습니다!");
            return;
        }

        // TODO#: 스킬의 데미지 계산 공식은 나중에 스킬 시스템이 완성되면 변경할 예정
        // TODO#: 현재는 플레이어가 한명이므로 무조건 리스트 0번 자리에 있지만 늘어나면
        //        현재 행동하는 플레이어를 찾아 얻어오는 식으로 새로 짜야됨
        WeaponData weapon = player.Weapons[weaponIndex];

        int damage = _playerUnits[0].Atk + weapon.Damage;

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
                    target.TakeDamage(damage);

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
            _enemyUnits.Remove(deadUnit);
            _battleUnits.Remove(deadUnit);
            _unitOrderBySpeedSystem.RemoveUnit(deadUnit);
            OnEnemyDied?.Invoke(deadUnit);
        }

        _playerActed = true;

        OnPlayerActionComplete?.Invoke(player);
    }
}