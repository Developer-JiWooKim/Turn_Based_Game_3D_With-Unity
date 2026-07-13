using UnityEngine;

namespace Assets.MyAssets.Scripts.Scriptable
{

[CreateAssetMenu(fileName = "EnemySkillData", menuName = "Scriptable Objects/EnemySkillData")]
public class EnemySkillData : SkillData
{
    [Header("Skill Stats")]
    public uint Power;
    public uint CoolTime;
}

}
