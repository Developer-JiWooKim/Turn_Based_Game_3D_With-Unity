using UnityEngine;

public class BattleUnit
{
    public string UnitName { get; private set; }
    public CharacterStats Stats { get; private set; }
    public bool IsPlayer { get; private set; }

    public bool IsDead => Stats.currentHP <= 0;

    // 플레이어 캐릭터로 생성
    public BattleUnit(CharacterDataSO data, int level)
    {
        IsPlayer = true;
        UnitName = data.characterName;
        Stats = data.GetStatsAtLevel(level);
    }

    // 적으로 생성
    public BattleUnit(EnemyDataSO data)
    {
        IsPlayer = false;
        UnitName = data.enemyName;
        Stats = new CharacterStats
        {
            maxHP = data.stats.maxHP,
            maxMP = data.stats.maxMP,
            attack = data.stats.attack,
            defense = data.stats.defense,
            magicAttack = data.stats.magicAttack,
            magicDefense = data.stats.magicDefense,
            speed = data.stats.speed,
            luck = data.stats.luck,
            currentHP = data.stats.maxHP,
            currentMP = data.stats.maxMP
        };
    }

    public void TakeDamage(int damage)
    {
        int actual = Mathf.Max(1, damage - Stats.defense);
        Stats.currentHP = Mathf.Max(0, Stats.currentHP - actual);
    }

    public void TakeMagicDamage(int damage)
    {
        int actual = Mathf.Max(1, damage - Stats.magicDefense);
        Stats.currentHP = Mathf.Max(0, Stats.currentHP - actual);
    }

    public void Heal(int amount)
    {
        Stats.currentHP = Mathf.Min(Stats.maxHP, Stats.currentHP + amount);
    }

    public void RestoreMP(int amount)
    {
        Stats.currentMP = Mathf.Min(Stats.maxMP, Stats.currentMP + amount);
    }
}