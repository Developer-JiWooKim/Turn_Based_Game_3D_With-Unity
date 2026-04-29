using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Win,
    Lose
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("참전 데이터")]
    public CharacterDataSO[] playerDataArray;  // 인스펙터에서 드래그
    public EnemyDataSO[] enemyDataArray;   // 인스펙터에서 드래그

    [Header("레벨")]
    public int playerLevel = 1;

    // 런타임 유닛
    public List<BattleUnit> PlayerUnits { get; private set; } = new();
    public List<BattleUnit> EnemyUnits { get; private set; } = new();

    public BattleState CurrentState { get; private set; }
    public BattleUnit CurrentUnit => _turnOrder.GetCurrentUnit();

    private TurnOrderSystem _turnOrder = new();

    // ── 이벤트 ──────────────────────────────────────────
    public event System.Action<BattleUnit> OnTurnStart;
    public event System.Action<BattleUnit, int> OnUnitDamaged;
    public event System.Action<BattleUnit, int> OnUnitHealed;
    public event System.Action<BattleUnit> OnUnitDied;
    public event System.Action OnBattleWin;
    public event System.Action OnBattleLose;

    // ────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        StartBattle();
    }

    // ── 전투 시작 ────────────────────────────────────────
    public void StartBattle()
    {
        // 유닛 생성
        PlayerUnits.Clear();
        EnemyUnits.Clear();

        foreach (var data in playerDataArray)
            PlayerUnits.Add(new BattleUnit(data, playerLevel));

        foreach (var data in enemyDataArray)
            EnemyUnits.Add(new BattleUnit(data));

        // 턴 순서 초기화
        var allUnits = new List<BattleUnit>();
        allUnits.AddRange(PlayerUnits);
        allUnits.AddRange(EnemyUnits);
        _turnOrder.Initialize(allUnits);

        CurrentState = BattleState.Start;
        StartCoroutine(BattleLoop());
    }

    // ── 메인 루프 ────────────────────────────────────────
    private IEnumerator BattleLoop()
    {
        yield return new WaitForSeconds(0.5f);

        while (CurrentState != BattleState.Win &&
               CurrentState != BattleState.Lose)
        {
            BattleUnit current = _turnOrder.GetCurrentUnit();
            OnTurnStart?.Invoke(current);

            if (current.IsPlayer)
            {
                CurrentState = BattleState.PlayerTurn;
                // 플레이어 입력 대기 (UI에서 ExecutePlayerAction 호출)
                yield return new WaitUntil(() => CurrentState != BattleState.PlayerTurn);
            }
            else
            {
                CurrentState = BattleState.EnemyTurn;
                yield return StartCoroutine(EnemyTurn(current));
            }

            // 죽은 유닛 정리
            _turnOrder.RemoveDeadUnits();

            // 승패 체크
            if (CheckBattleEnd()) yield break;

            // 다음 턴
            _turnOrder.AdvanceTurn();

            yield return new WaitForSeconds(0.3f);
        }
    }

    // ── 플레이어 액션 실행 (UI에서 호출) ────────────────
    public void ExecutePlayerAction(BattleUnit target, SkillDataSO skill = null)
    {
        BattleUnit actor = CurrentUnit;

        if (skill == null)
        {
            // 기본 공격
            int dmg = DamageCalculator.CalculatePhysicalDamage(actor, target);
            target.TakeDamage(dmg);
            OnUnitDamaged?.Invoke(target, dmg);
        }
        else
        {
            UseSkill(actor, target, skill);
        }

        if (target.IsDead) OnUnitDied?.Invoke(target);

        // 플레이어 턴 종료 신호
        CurrentState = BattleState.EnemyTurn;
    }

    // ── 적 AI 턴 ─────────────────────────────────────────
    private IEnumerator EnemyTurn(BattleUnit enemy)
    {
        yield return new WaitForSeconds(1f);

        // 살아있는 플레이어 중 랜덤 타겟
        List<BattleUnit> alive = PlayerUnits.FindAll(u => !u.IsDead);
        if (alive.Count == 0) yield break;

        BattleUnit target = alive[Random.Range(0, alive.Count)];
        int dmg = DamageCalculator.CalculatePhysicalDamage(enemy, target);
        target.TakeDamage(dmg);

        OnUnitDamaged?.Invoke(target, dmg);
        if (target.IsDead) OnUnitDied?.Invoke(target);
    }

    // ── 스킬 사용 ────────────────────────────────────────
    private void UseSkill(BattleUnit actor, BattleUnit target, SkillDataSO skill)
    {
        if (actor.Stats.currentMP < skill.mpCost)
        {
            Debug.Log("MP 부족!");
            return;
        }

        actor.Stats.currentMP -= skill.mpCost;

        switch (skill.skillType)
        {
            case SkillType.PhysicalAttack:
                int pdmg = DamageCalculator.CalculatePhysicalDamage(actor, target, skill.powerMultiplier);
                target.TakeDamage(pdmg);
                OnUnitDamaged?.Invoke(target, pdmg);
                break;

            case SkillType.MagicAttack:
                int mdmg = DamageCalculator.CalculateMagicDamage(actor, target, skill.powerMultiplier);
                target.TakeMagicDamage(mdmg);
                OnUnitDamaged?.Invoke(target, mdmg);
                break;

            case SkillType.Heal:
                int heal = DamageCalculator.CalculateHeal(actor, skill);
                target.Heal(heal);
                OnUnitHealed?.Invoke(target, heal);
                break;
        }
    }

    // ── 승패 판정 ────────────────────────────────────────
    private bool CheckBattleEnd()
    {
        bool allPlayerDead = PlayerUnits.TrueForAll(u => u.IsDead);
        bool allEnemyDead = EnemyUnits.TrueForAll(u => u.IsDead);

        if (allEnemyDead)
        {
            CurrentState = BattleState.Win;
            OnBattleWin?.Invoke();
            Debug.Log("전투 승리!");
            return true;
        }

        if (allPlayerDead)
        {
            CurrentState = BattleState.Lose;
            OnBattleLose?.Invoke();
            Debug.Log("전투 패배...");
            return true;
        }

        return false;
    }
}