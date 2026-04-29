using UnityEngine;

public enum EnemyType { Normal, Elite, Boss }

public enum EnemyActionType { Attack, Skill, Buff, Defend }

[CreateAssetMenu(fileName = "NewEnemy", menuName = "BattleGame/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyName;
    public EnemyType enemyType;
    public Sprite battleSprite;

    [Header("스탯")]
    public CharacterStats stats;

    [Header("보상")]
    public int expReward;
    public int goldReward;
}