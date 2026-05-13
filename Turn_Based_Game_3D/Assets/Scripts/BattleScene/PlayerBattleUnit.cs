using System.Collections.Generic;
public class PlayerBattleUnit : BattleUnit
{
    private CharacterData _playerData;
    public CharacterData  PlayerData => _playerData;

    public List<PlayerSkillData> Skills { get; private set; }
    
    public PlayerBattleUnit(CharacterData data) : base(data)
    {
        _playerData = data;

        Skills = data.Skills;
    }

    // 무기 선택 씬에서 선택한 무기 스킬로 교체
    public PlayerBattleUnit(CharacterData data, List<PlayerSkillData> weaponSkills) : base(data)
    {
        _playerData = data;
        Skills = weaponSkills;
    }
}
