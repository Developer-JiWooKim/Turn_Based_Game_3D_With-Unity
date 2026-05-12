using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler _playerInputHandler;    // 플레이어 입력 핸들러
    [SerializeField] private BattleUIController _battleUIController;    // 전투 UI 컨트롤러
    [SerializeField] private TargetSelector     _targetSelector;        // 플레이어가 현재 선택한 타겟을 알려주는 Selector

    private UnitOrderBySpeedSystem  _unitOrderBySpeedSystem;    // 턴마다 유닛들의 행동 순서를 결정하는 시스템
    

    private List<PlayerBattleUnit>  _playerUnits;               // 플레이어 캐릭터의 유닛
    private List<EnemyBattleUnit>   _enemyUnits;                // 적 캐릭터들의 유닛 리스트
    private List<BattleUnit>        _battleUnits;               // 현재 전투에 참여하는 모든 유닛을 저장하는 리스트

    private int   _turnCount;             // 현재 턴의 번호를 저장하는 변수
    private bool  _playerActed;           // 플레이어가 현재 턴에서 행동을 완료했는지 여부, true이면 플레이어가 행동을 마쳤음을 나타냄

    public int TurnCount => _turnCount;

    //TODO#: 현재는 OnTurnStart라고 이름을 정했지만, 자신의 차례가 되었을때 카메라 무빙, UI 업데이트 등등의 처리를 하기 위한 이벤트이므로 나중에 이름을 변경할 수도 있음
    public event Action<BattleUnit>         OnTurnStart;            // 턴이 시작될 때마다 호출되는 이벤트, 현재 턴에서 행동할 유닛을 인자로 전달
    public event Action<IDamageable, int>   OnUnitDamaged;          // 유닛이 피해를 입었을 때 호출되는 이벤트, 피해를 입은 유닛과 입은 피해량을 인자로 전달
    public event Action<bool>               OnBattleEnd;            // 전투가 종료될 때 호출되는 이벤트, 플레이어가 승리했는지 여부를 인자로 전달
    public event Action<PlayerBattleUnit>   OnPlayerActionComplete; // 플레이어의 행동이 종료되고 처리할 이벤트(MP갱신 등등)
    public event Action<int>                OnTurnChanged;
    public event Action<BattleUnit>         OnEnemyDied;

    private void Awake() => Initialize();
    private void Start() => SetUp();
    private void Initialize()
    {
        _unitOrderBySpeedSystem = new UnitOrderBySpeedSystem();

        _battleUnits = new List<BattleUnit>();
        _playerUnits = new List<PlayerBattleUnit>();
        _enemyUnits  = new List<EnemyBattleUnit>();

        _turnCount = 1;
    }

    private void SetUp()
    {
        _playerInputHandler.Subscribe(this, _targetSelector);
        _battleUIController.Subscribe(this, _playerInputHandler);
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

        _unitOrderBySpeedSystem.OrderBySpeed(_battleUnits);

        OnTurnChanged?.Invoke(_turnCount);

        StartCoroutine(BattleLoop(_battleUnits));
    }

    private IEnumerator BattleLoop(List<BattleUnit> battleUnits)
    {
        // TODO#: 지울예정
        Debug.Log("BattleLoop 시작");
        BattleUnit currentUnit;
        
        while (!CheckBattleEnd())
        {
            currentUnit = _unitOrderBySpeedSystem.GetCurrentUnit();  // 현재 턴에서 행동할 유닛을 가져옴
            // TODO#: 지울예정
            Debug.Log($"{currentUnit.Name}의 턴");
            OnTurnStart?.Invoke(currentUnit);                        // 자신의 차례가 시작될 때마다 이벤트 호출

            if (currentUnit.IsPlayer)
            {
                yield return StartCoroutine(PlayerTurn());           // 플레이어의 행동을 처리하는 코루틴 시작
            }
            else
            {
                yield return StartCoroutine(EnemyTurn(currentUnit)); // 적의 행동을 처리하는 코루틴 시작
            }

            if (!_unitOrderBySpeedSystem.NextUnit())
            {
                _turnCount++;
                _unitOrderBySpeedSystem.OrderBySpeed(battleUnits);   // 다음 턴을 위해 유닛의 속도에 따라 재정렬
                OnTurnChanged?.Invoke(TurnCount);
            }
        }
    }

    private bool CheckBattleEnd()
    {
        if (_playerUnits.TrueForAll(u => u.IsDead) || _enemyUnits.TrueForAll(u => u.IsDead))
        {
            OnBattleEnd?.Invoke(!_playerUnits.TrueForAll(u => u.IsDead)); // 플레이어가 죽었을때 전투 종료 이벤트 호출
            return true;                
        }

        return false;
    }

    private IEnumerator EnemyTurn(BattleUnit currentUnit)
    { 
        EnemyBattleUnit currentEnemy = currentUnit as EnemyBattleUnit;
        if (currentEnemy == null)
        {
            Debug.LogError("currentUnit is not EnemyBattleUnit");
            yield break;
        }

        currentEnemy.ReduceCooldowns();

        EnemySkillData skill = currentEnemy.GetEnemySkill();

        // TODO#: 적의 행동을 결정하는 AI로직 필요 -> 어떤 스킬을 사용할지 결정
        if (skill != null)
        {
            // TODO#: 스킬 사용 시 고유의 애니메이션, 이펙트, 카메라 무빙 등 작동하는 이벤트
            OnUnitDamaged?.Invoke(_playerUnits[0], (int)skill.Power); // 이 이벤트에서 작동 시키면 될듯

            yield return new WaitForSeconds(2f);                      // 적이 플레이어를 타격하는 애니메이션 작동, 현재는 임시로 2초 대기


            // TODO#: 현재는 적 입장에서는 타겟이 플레이어 밖에 없으므로 _playerUnit의 TakeDamage를 쓰지만, 나중에 플레이어 측 유닛이 더 생기면 타겟을 정하는 로직 작성 필요            
            _playerUnits[0].TakeDamage((int)skill.Power);             // 스킬 사용 시 플레이어에게 스킬 데미지 만큼의 데미지를 입힘

            currentEnemy.SetCooldown(skill);                          // 스킬 사용 후 쿨 타임 적용
        }
        else
        {
            Debug.Log("스킬을 가져올 수 없어서 비어있음");
        }
    }

    private IEnumerator PlayerTurn()
    {
        _playerActed = false;                           // 플레이어가 행동을 완료했는지 여부 초기화
        Debug.Log("PlayerTurn 시작 - 입력 대기");
        yield return new WaitUntil(() => _playerActed); // 플레이어가 행동을 완료할 때까지 대기
        Debug.Log("PlayerTurn 종료 - 행동 완료");
    }

    public void OnPlayerAction(IDamageable target, PlayerSkillData skill)
    {
        // TODO#: 수정 예정, 타겟이 null이면 첫 번째 살아있는 적 자동 선택
        if (target == null)
            target = _enemyUnits.Find(u => !u.IsDead);

        if (target == null)
        {
            Debug.LogError("타겟이 없습니다!");
            return;
        }

        if (skill == null)
        {
            Debug.LogError("PlayerSkillData is null. Cannot perform action.");
            return;
        }

        Debug.Log($"OnPlayerAction 호출 - skill: {skill.SkillName}");
        // TODO#: 스킬의 데미지 계산 공식은 나중에 스킬 시스템이 완성되면 변경할 예정
        // TODO#: 현재는 플레이어가 한명이므로 무조건 리스트 0번 자리에 있지만 늘어나면 현재 행동하는 플레이어를 찾아 얻어오는 식으로 새로 짜야됨
        int damage = _playerUnits[0].Atk * (int)skill.Power; 
        _playerUnits[0].UseMp((int)skill.Cost); // 플레이어의 MP를 스킬의 비용만큼 감소

        target.TakeDamage(damage); // 타겟 유닛에게 데미지를 입힘

        OnUnitDamaged?.Invoke(target, damage); // 유닛이 피해를 입었을 때 이벤트 호출

        if (target.IsDead)
        {
            EnemyBattleUnit deadUnit = target as EnemyBattleUnit;

            _enemyUnits.Remove(deadUnit);
            _battleUnits.Remove(deadUnit);

            _unitOrderBySpeedSystem.RemoveUnit(deadUnit); // 유닛이 죽었을 때 턴 시스템에서 해당 유닛 제거

            OnEnemyDied?.Invoke(deadUnit);
        }

        _playerActed = true; // 플레이어가 행동을 완료했음을 표시

        OnPlayerActionComplete?.Invoke(_playerUnits[0]);
    }
}