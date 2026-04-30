using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSkillData", menuName = "Scriptable Objects/PlayerSkillData")]
public class PlayerSkillData : SkillData
{
    [Header("Skill Stats")]
    public uint Power;
    public uint Cost;
}
