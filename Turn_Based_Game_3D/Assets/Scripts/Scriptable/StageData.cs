using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnData
{
    public EnemyData    enemyData;        
    public GameObject   enemyPrefab;     
}

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    [Header("Stage Information")]
    public int      stageNumber;
    public string   stageTitle;

    [Header("Enemy Spawn Data")]
    public List<EnemySpawnData> enemySpawnDatas;
}