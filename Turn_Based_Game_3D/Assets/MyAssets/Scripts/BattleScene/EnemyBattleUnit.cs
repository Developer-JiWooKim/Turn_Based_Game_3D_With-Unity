using System.Collections.Generic;
using System.Linq;
using Assets.MyAssets.Scripts.Scriptable;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class EnemyBattleUnit : BattleUnit
{
    // 각 스킬마다 고유의 쿨타임 존재, 스킬 사용 => 고유 쿨타임 만큼의 쿨타임 적용, 턴이 끝날 때마다 모든 스킬의 쿨타임 1 감소
    private Dictionary<EnemySkillData, int> _skillCooldowns;

    public Dictionary<EnemySkillData, int> SkillCooldowns => _skillCooldowns;

    private EnemyData enemyData;

    public EnemyData EnemyData => enemyData;

    // 로그라이크 "몬스터 행동불가" 디버프 적용 시 true — 다음 자신의 턴 1회를 그냥 흘려보낸다.
    public bool SkipFirstAction { get; set; }

    public EnemyBattleUnit(EnemyData data) : base(data)
    {
        _skillCooldowns = new Dictionary<EnemySkillData, int>();

        enemyData = data;

        foreach (var skill in data.ActiveSkills)
        {
            _skillCooldowns.Add(skill, 0); // 초기에는 모든 스킬의 현재 쿨타임이 0으로 설정되어 있다고 가정
        }
    }

    public EnemySkillData GetEnemySkill()
    {
        EnemySkillData result = null;

        foreach (var skill in _skillCooldowns.Keys)
        {
            if (!CanUseSkill(skill)) continue; //스킬 사용 여부 검사

            if (result == null || skill.Priority > result.Priority)
            {
                result = skill;
            }
        }

        return result;
    }

    public bool CanUseSkill(EnemySkillData skill)
    {
        return _skillCooldowns[skill] <= 0;
    }

    // 스킬 사용 시 해당 스킬이 가진 고유의 쿨타임 설정
    public void SetCooldown(EnemySkillData skill)
    {
        // 스킬 사용 시 해당 스킬이 가진 고유의 쿨타임 설정
        _skillCooldowns[skill] = (int)skill.CoolTime;
    }

    public void ReduceCooldowns(EnemySkillData skill)
    {
        _skillCooldowns[skill] = _skillCooldowns[skill] - 1 <= 0 ? 0 : _skillCooldowns[skill] - 1; // 턴이 끝날 때마다 모든 스킬의 쿨타임 1 감소, 최소값은 0
    }

    // 턴이 끝나면 모든 스킬의 쿨타임 1 감소할 때 호출될 메소드
    public void ReduceCooldowns()
    {
        List<EnemySkillData> skillData = _skillCooldowns.Keys.ToList();

        for (int i = 0; i < _skillCooldowns.Keys.Count; i++)
        {
            ReduceCooldowns(skillData[i]);
        }
    }
}

}
