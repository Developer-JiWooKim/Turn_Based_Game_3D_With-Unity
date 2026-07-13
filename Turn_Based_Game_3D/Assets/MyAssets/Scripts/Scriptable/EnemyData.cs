using System.Collections.Generic;
using UnityEngine;
using Assets.MyAssets.Scripts.Struct;

namespace Assets.MyAssets.Scripts.Scriptable
{

public enum EnemyType
{
    None = -999,
    Normal = 0,
    Elite,
    Boss
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Information")]
    public string EnemyName;
    public EnemyType enemyType;

    [Header("Stats")]
    public StatData enemyStat;

    [Header("Skills")]
    public List<EnemySkillData> ActiveSkills;
    public List<EnemySkillData> PassiveSkills;

    [Header("Reward")]
    public double Exp;
    public uint Gold;
}

}