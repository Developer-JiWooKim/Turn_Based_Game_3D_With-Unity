using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [SerializeField] private CharacterData  _playerData;   // 플레이어 캐릭터의 데이터
    [SerializeField] private EnemyData[]    _enemyDatas;   // 적 캐릭터들의 데이터 배열
    

    private UnitOrderBySpeedSystem  _unitOrderBySpeedSystem;    // 턴마다 유닛들의 행동 순서를 결정하는 시스템

    private BattleUnit              _playerUnit;                // 플레이어 캐릭터의 유닛
    private List<EnemyBattleUnit>   _enemyUnits;                // 적 캐릭터들의 유닛 리스트

    private List<BattleUnit>        _battleUnits;               // 현재 전투에 참여하는 모든 유닛을 저장하는 리스트


    private int   _turnCount;             // 현재 턴의 번호를 저장하는 변수
    private bool  _playerActed;           // 플레이어가 현재 턴에서 행동을 완료했는지 여부, true이면 플레이어가 행동을 마쳤음을 나타냄

    //TODO#: 현재는 OnTurnStart라고 이름을 정했지만, 자신의 차례가 되었을때 카메라 무빙, UI 업데이트 등등의 처리를 하기 위한 이벤트이므로 나중에 이름을 변경할 수도 있음
    public event Action<BattleUnit>         OnTurnStart;   // 턴이 시작될 때마다 호출되는 이벤트, 현재 턴에서 행동할 유닛을 인자로 전달
    public event Action<IDamageable, int>   OnUnitDamaged; // 유닛이 피해를 입었을 때 호출되는 이벤트, 피해를 입은 유닛과 입은 피해량을 인자로 전달
    public event Action<bool>               OnBattleEnd;   // 전투가 종료될 때 호출되는 이벤트, 플레이어가 승리했는지 여부를 인자로 전달

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _unitOrderBySpeedSystem = new UnitOrderBySpeedSystem();
        _battleUnits = new List<BattleUnit>();
        _enemyUnits = new List<EnemyBattleUnit>();
        _turnCount = 0;
    }

    // TODO#: 현재는 테스트를 위해 게임을 시작하자마자 전투가 시작되지만, 나중에는 타이틀 화면에서 플레이어가 캐릭터를 고르고 게임 시작 버튼을 누르면 시작하도록 변경
    private void Start()
    {
        StartBattle();
    }

    /// <summary>
    /// 전투가 시작되면 한번만 호출되는 메소드 -> 플레이어 유닛과 적 유닛을 생성 / 전투에 참여하는 모든 유닛을 리스트에 추가 / 턴 시스템 초기화
    /// </summary>
    public void StartBattle()
    {
        _playerUnit = new BattleUnit(_playerData);    // 플레이어 유닛 생성

        // 적 유닛 배열 초기화
        // TODO#: 몬스터 스포너를 새로 만들어서 턴이 시작되면 몬스터 스포너가 적 유닛을 생성하도록 변경
        foreach (var unit in _enemyDatas)
        {
            _enemyUnits.Add(new EnemyBattleUnit(unit));
        }

        _battleUnits.Add(_playerUnit);                      // 플레이어 유닛을 전투 유닛 리스트에 추가
        _battleUnits.AddRange(_enemyUnits);                 // 적 유닛들을 전투 유닛 리스트에 추가

        _unitOrderBySpeedSystem.OrderBySpeed(_battleUnits); // 유닛의 속도에 따라 정렬 및 턴 초기화

        StartCoroutine(BattleLoop(_battleUnits));
    }

    private IEnumerator BattleLoop(List<BattleUnit> battleUnits)
    {
        BattleUnit currentUnit;

        while (!CheckBattleEnd())
        {
            currentUnit = _unitOrderBySpeedSystem.GetCurrentUnit();  // 현재 턴에서 행동할 유닛을 가져옴
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
            }
        }
    }

    private bool CheckBattleEnd()
    {
        if (_playerUnit.IsDead || _enemyUnits.TrueForAll(u => u.IsDead))
        {
            OnBattleEnd?.Invoke(!_playerUnit.IsDead); // 플레이어가 죽었을때 전투 종료 이벤트 호출
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
            OnUnitDamaged?.Invoke(_playerUnit, (int)skill.Power);   // 이 이벤트에서 작동 시키면 될듯

            yield return new WaitForSeconds(2f);                    // 적이 플레이어를 타격하는 애니메이션 작동, 현재는 임시로 2초 대기

            
            // TODO#: 현재는 적 입장에서는 타겟이 플레이어 밖에 없으므로 _playerUnit의 TakeDamage를 쓰지만, 나중에 플레이어 측 유닛이 더 생기면 타겟을 정하는 로직 작성 필요            
            _playerUnit.TakeDamage((int)skill.Power);               // 스킬 사용 시 플레이어에게 스킬 데미지 만큼의 데미지를 입힘

            currentEnemy.SetCooldown(skill);                        // 스킬 사용 후 쿨 타임 적용
        }
        else
        {
            Debug.Log("스킬을 가져올 수 없어서 비어있음");
        }
    }

    private IEnumerator PlayerTurn()
    {
        _playerActed = false;                           // 플레이어가 행동을 완료했는지 여부 초기화

        yield return new WaitUntil(() => _playerActed); // 플레이어가 행동을 완료할 때까지 대기
    }

    public void OnPlayerAction(IDamageable target, PlayerSkillData skill)
    {
        if (skill == null)
        {
            Debug.LogError("PlayerSkillData is null. Cannot perform action.");
            return;
        }

        int damage = _playerUnit.Atk * (int)skill.Power; // TODO#: 스킬의 데미지 계산 공식은 나중에 스킬 시스템이 완성되면 변경할 예정

        _playerUnit.UseMp((int)skill.Cost); // 플레이어의 MP를 스킬의 비용만큼 감소

        target.TakeDamage(damage); // 타겟 유닛에게 데미지를 입힘

        OnUnitDamaged?.Invoke(target, damage); // 유닛이 피해를 입었을 때 이벤트 호출

        if (target.IsDead)
        {
            _enemyUnits.Remove(target as EnemyBattleUnit);
            _battleUnits.Remove(target as BattleUnit);
            _unitOrderBySpeedSystem.RemoveUnit(target as BattleUnit); // 유닛이 죽었을 때 턴 시스템에서 해당 유닛 제거
        }

        _playerActed = true; // 플레이어가 행동을 완료했음을 표시
    }
}