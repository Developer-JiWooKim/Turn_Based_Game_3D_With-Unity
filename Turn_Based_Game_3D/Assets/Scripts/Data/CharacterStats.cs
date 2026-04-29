using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    [Header("기본 스탯")]
    public int maxHP;
    public int maxMP;
    public int attack;          // 물리 공격력
    public int defense;         // 물리 방어력
    public int magicAttack;     // 마법 공격력
    public int magicDefense;    // 마법 방어력
    public int speed;           // 행동 순서 결정
    public int luck;            // 크리티컬/회피 영향

    [HideInInspector]
    public int currentHP;
    [HideInInspector]
    public int currentMP;

    public bool IsDead => currentHP <= 0;
}
