using UnityEngine;

public enum CharacterType
{
    None = -999,
    Warrior = 0,
    Mage,
    Archer
}

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Character Information")]
    public string CharacterName;
    public CharacterType characterType;

    [Header("Stats")]
    public StatData characterStat;

    [Header("Skills")]
    public System.Collections.Generic.List<PlayerSkillData> Skills;

}
