using UnityEngine;

public static class DamageCalculator
{
    // 물리 데미지
    public static int CalculatePhysicalDamage(BattleUnit attacker, BattleUnit target, float multiplier = 1f)
    {
        float raw = attacker.Stats.attack * multiplier;
        float reduced = Mathf.Max(1f, raw - target.Stats.defense);

        // 크리티컬 판정 (luck 기반)
        bool isCritical = IsCritical(attacker.Stats.luck);
        if (isCritical) reduced *= 1.5f;

        return Mathf.RoundToInt(reduced);
    }

    // 마법 데미지
    public static int CalculateMagicDamage(BattleUnit attacker, BattleUnit target, float multiplier = 1f)
    {
        float raw = attacker.Stats.magicAttack * multiplier;
        float reduced = Mathf.Max(1f, raw - target.Stats.magicDefense);

        bool isCritical = IsCritical(attacker.Stats.luck);
        if (isCritical) reduced *= 1.5f;

        return Mathf.RoundToInt(reduced);
    }

    // 회복량 계산
    public static int CalculateHeal(BattleUnit caster, SkillDataSO skill)
    {
        if (skill.flatValue > 0)
            return skill.flatValue;

        return Mathf.RoundToInt(caster.Stats.magicAttack * skill.powerMultiplier);
    }

    // 크리티컬 판정 (luck이 높을수록 확률 증가, 최대 30%)
    private static bool IsCritical(int luck)
    {
        float critChance = Mathf.Min(luck * 0.3f / 100f, 0.30f);
        return Random.value < critChance;
    }
}