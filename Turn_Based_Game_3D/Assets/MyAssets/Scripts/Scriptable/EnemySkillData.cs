using UnityEngine;

namespace Assets.MyAssets.Scripts.Scriptable
{

[CreateAssetMenu(fileName = "EnemySkillData", menuName = "Scriptable Objects/EnemySkillData")]
public class EnemySkillData : SkillData
{
    [Header("Skill Stats")]
    public uint Power;
    public uint CoolTime;
    public int Priority; // 여러 스킬이 동시에 사용 가능할 때 더 높은 값을 우선 사용
}

}
