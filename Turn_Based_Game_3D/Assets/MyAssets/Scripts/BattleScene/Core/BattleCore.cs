using System;
using System.Collections.Generic;
using Assets.MyAssets.Scripts.BattleScene;

namespace Assets.MyAssets.Scripts.BattleScene.Core
{

public enum BattleResult
{
    Ongoing,
    PlayerWin,
    PlayerLose
}

/// <summary>
/// 전투의 턴 진행/승패 판정/데미지 적용을 담당하는 순수 C# 로직. Unity API에 의존하지 않으며,
/// 연출(애니메이션 재생 타이밍 등)은 이 클래스를 호출하는 MonoBehaviour(BattleController)가 결정한다.
/// </summary>
public class BattleCore
{
    private readonly UnitOrderBySpeedSystem _turnOrder = new UnitOrderBySpeedSystem();
    private readonly Random _random = new Random();
    private readonly int _defenseConstant;

    private List<PlayerBattleUnit> _playerUnits;
    private List<EnemyBattleUnit>  _enemyUnits;
    private List<BattleUnit>       _battleUnits;

    private int _turnCount;

    public BattleCore(int defenseConstant = 100)
    {
        _defenseConstant = defenseConstant;
    }

    public int TurnCount => _turnCount;
    public List<PlayerBattleUnit> PlayerUnits => _playerUnits;
    public List<EnemyBattleUnit>  EnemyUnits  => _enemyUnits;
    public BattleUnit CurrentUnit => _turnOrder.CurrentUnit;

    public event Action<BattleUnit>       OnTurnStart;
    public event Action<IDamageable, int> OnUnitDamaged;
    public event Action<bool>             OnBattleEnd;
    public event Action<PlayerBattleUnit> OnPlayerActionComplete;
    public event Action<int>              OnTurnChanged;
    public event Action<BattleUnit>       OnEnemyDied;

    public void SetupBattle(List<PlayerBattleUnit> players, List<EnemyBattleUnit> enemies)
    {
        _playerUnits = players;
        _enemyUnits  = enemies;

        _battleUnits = new List<BattleUnit>();
        _battleUnits.AddRange(_playerUnits);
        _battleUnits.AddRange(_enemyUnits);

        _turnCount = 1;

        _turnOrder.OrderBySpeed(_battleUnits);

        OnTurnChanged?.Invoke(_turnCount);
    }

    /// <summary>
    /// 현재 턴에서 행동할 유닛을 조회하고 OnTurnStart를 발행한다.
    /// </summary>
    public BattleUnit BeginUnitTurn()
    {
        BattleUnit unit = _turnOrder.GetCurrentUnit();
        OnTurnStart?.Invoke(unit);
        return unit;
    }

    /// <summary>
    /// 플레이어 턴 중간에 스킬 버튼을 다시 빌드시키기 위해 OnTurnStart를 재발행할 때 사용.
    /// </summary>
    public void NotifyTurnStart(BattleUnit unit)
    {
        OnTurnStart?.Invoke(unit);
    }

    /// <summary>
    /// 다음 유닛으로 턴을 넘긴다. 모든 유닛이 행동을 마쳤다면 턴 카운트를 올리고 속도순으로 재정렬한다.
    /// </summary>
    public void AdvanceTurn()
    {
        if (!_turnOrder.NextUnit())
        {
            _turnCount++;
            _turnOrder.OrderBySpeed(_battleUnits);
            OnTurnChanged?.Invoke(_turnCount);
        }
    }

    /// <summary>
    /// 승패를 판정만 한다 (이벤트 발행 없음). 패배 시 사망 연출을 먼저 재생해야 하므로
    /// 판정과 OnBattleEnd 발행 시점을 분리해뒀다.
    /// </summary>
    public BattleResult EvaluateBattleEnd()
    {
        bool playerAllDead = _playerUnits.TrueForAll(u => u.IsDead);
        bool enemyAllDead  = _enemyUnits.TrueForAll(u => u.IsDead);

        if (playerAllDead) return BattleResult.PlayerLose;
        if (enemyAllDead)  return BattleResult.PlayerWin;
        return BattleResult.Ongoing;
    }

    public void NotifyBattleEnd(bool isWin)
    {
        OnBattleEnd?.Invoke(isWin);
    }

    public void RecoverPlayerStamina(PlayerBattleUnit player)
    {
        player.RecoverStaminaPerTurn(player.PlayerData.playerStat.StaminaRecovery);
    }

    public void NotifyPlayerActionComplete(PlayerBattleUnit player)
    {
        OnPlayerActionComplete?.Invoke(player);
    }

    /// <summary>
    /// 타겟이 지정되지 않았을 때 자동으로 선택할 살아있는 첫 번째 적을 반환한다.
    /// </summary>
    public EnemyBattleUnit GetFallbackTarget()
    {
        return _enemyUnits.Find(u => !u.IsDead);
    }

    /// <summary>
    /// 최종 데미지 = (ATK + bonusPower) x (DefenseConstant / (DefenseConstant + DEF)), 크리티컬 시 CritDamageBonus만큼 추가.
    /// bonusPower는 플레이어는 weapon.Damage, 적은 skill.Power를 ATK에 더하는 값으로 사용한다.
    /// </summary>
    public int CalculateAttackDamage(BattleUnit attacker, BattleUnit target, int bonusPower)
    {
        int rawAttack = attacker.Atk + bonusPower;
        double ratio = _defenseConstant / (double)(_defenseConstant + target.Def);
        int damage = (int)Math.Round(rawAttack * ratio, MidpointRounding.AwayFromZero);

        int clampedCritRate = Math.Clamp(attacker.CritRate, 0, 100);
        bool isCrit = _random.Next(0, 100) < clampedCritRate;

        if (isCrit)
        {
            damage = (int)Math.Round(damage * (100 + attacker.CritDamageBonus) / 100.0, MidpointRounding.AwayFromZero);
        }

        return damage;
    }

    public void ApplyDamage(IDamageable target, int damage)
    {
        target.TakeDamage(damage);
        OnUnitDamaged?.Invoke(target, damage);
    }

    public void HandleEnemyDefeated(EnemyBattleUnit enemy)
    {
        _enemyUnits.Remove(enemy);
        _battleUnits.Remove(enemy);
        _turnOrder.RemoveUnit(enemy);
        OnEnemyDied?.Invoke(enemy);
    }
}

}
