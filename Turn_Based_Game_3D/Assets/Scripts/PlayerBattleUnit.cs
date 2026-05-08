using System.Collections.Generic;
public class PlayerBattleUnit : BattleUnit
{
    private CharacterData _playerData;
    public CharacterData PlayerData => _playerData;

    public List<PlayerSkillData> Skills { get; private set; }
    
    public PlayerBattleUnit(CharacterData data) : base(data)
    {
        _playerData = data;

        Skills = data.Skills;
    }
}
