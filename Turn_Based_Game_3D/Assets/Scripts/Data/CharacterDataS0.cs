using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "BattleGame/Character Data")]
public class CharacterDataSO : ScriptableObject
{
    [Header("기본 정보")]
    public string characterName;
    public Sprite portrait;
    public GameObject battlePrefab;

    [Header("기본 스탯 (레벨 1)")]
    public CharacterStats baseStats;

    [Header("레벨당 증가량")]
    public CharacterStats growthPerLevel;

    public CharacterStats GetStatsAtLevel(int level)
    {
        var s = new CharacterStats();
        s.maxHP = baseStats.maxHP + growthPerLevel.maxHP * (level - 1);
        s.maxMP = baseStats.maxMP + growthPerLevel.maxMP * (level - 1);
        s.attack = baseStats.attack + growthPerLevel.attack * (level - 1);
        s.defense = baseStats.defense + growthPerLevel.defense * (level - 1);
        s.magicAttack = baseStats.magicAttack + growthPerLevel.magicAttack * (level - 1);
        s.magicDefense = baseStats.magicDefense + growthPerLevel.magicDefense * (level - 1);
        s.speed = baseStats.speed + growthPerLevel.speed * (level - 1);
        s.luck = baseStats.luck + growthPerLevel.luck * (level - 1);
        s.currentHP = s.maxHP;
        s.currentMP = s.maxMP;
        return s;
    }
}