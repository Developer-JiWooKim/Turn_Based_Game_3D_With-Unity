using System.Collections.Generic;
public class PlayerBattleUnit : BattleUnit
{
    public List<PlayerSkillData> Skills { get; private set; }

    public PlayerBattleUnit(CharacterData data) : base(data)
    {
        Skills = data.Skills;
    }
}
