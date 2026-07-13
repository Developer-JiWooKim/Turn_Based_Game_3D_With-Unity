using UnityEngine;

namespace Assets.MyAssets.Scripts.Scriptable
{

public enum SkillType
{
    Active,
    Passive
}

// [CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public abstract class SkillData : ScriptableObject
{
    [Header("Skill Information")]
    public SkillType skillType;
    public string SkillName;
    public string Description;

}

}
