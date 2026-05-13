using System.Collections.Generic;
using System.Data;
using System.Linq;

public class EnemyBattleUnit : BattleUnit
{
    // 각 스킬마다 고유의 쿨타임 존재, 스킬 사용 => 고유 쿨타임 만큼의 쿨타임 적용, 턴이 끝날 때마다 모든 스킬의 쿨타임 1 감소
    private Dictionary<EnemySkillData, int> _skillCooldowns;

    public Dictionary<EnemySkillData, int> SkillCooldowns => _skillCooldowns;

    private EnemyData enemyData;

    public EnemyData EnemyData => enemyData;

    public EnemyBattleUnit(EnemyData data) : base(data)
    {
        _skillCooldowns = new Dictionary<EnemySkillData, int>();

        enemyData = data;

        foreach (var skill in data.Skills)
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

            result = skill;

            //TODO#: 현재는 우선순위에 따라 스킬을 사용하도록하지 않고 사용할 수 있는 스킬만 반환하도록 되어있음, 나중에 우선순위 로직 구현해서 여기에 추가 예정
            //if (skill.Priority > result.Priority)
            //{
            //    result = skill;
            //}
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
