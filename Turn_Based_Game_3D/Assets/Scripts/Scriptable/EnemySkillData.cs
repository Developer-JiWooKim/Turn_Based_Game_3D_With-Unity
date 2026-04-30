using UnityEngine;

[CreateAssetMenu(fileName = "EnemySkillData", menuName = "Scriptable Objects/EnemySkillData")]
public class EnemySkillData : SkillData
{
    [Header("Skill Stats")]
    public uint Power;
    public uint CoolTime;
}
