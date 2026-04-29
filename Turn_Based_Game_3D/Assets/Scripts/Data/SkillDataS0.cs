using UnityEngine;

public enum SkillType
{
    PhysicalAttack,
    MagicAttack,
    Heal,
    Buff,
    Debuff,
    Revive
}

public enum SkillTarget
{
    SingleEnemy,
    AllEnemies,
    SingleAlly,
    AllAllies,
    Self
}

public enum StatusEffectType
{
    Poison,
    Burn,
    Paralysis,
    Sleep,
    Silence,
    AttackUp,
    DefenseUp,
    AttackDown,
    DefenseDown
}

[System.Serializable]
public class StatusEffectData
{
    public StatusEffectType effectType;
    [Range(0f, 1f)] public float applyChance;
    public int duration;
    public int value;
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "BattleGame/Skill Data")]
public class SkillDataSO : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName;
    [TextArea] public string description;
    public Sprite skillIcon;

    [Header("분류")]
    public SkillType skillType;
    public SkillTarget skillTarget;

    [Header("비용")]
    public int mpCost;

    [Header("데미지 / 회복")]
    public float powerMultiplier;
    public int flatValue;

    [Header("상태이상")]
    public StatusEffectData[] statusEffects;

    [Header("연출")]
    public string animationTrigger;
    public GameObject vfxPrefab;
}