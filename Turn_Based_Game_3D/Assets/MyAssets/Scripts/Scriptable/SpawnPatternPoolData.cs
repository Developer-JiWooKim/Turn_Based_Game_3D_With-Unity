using System.Collections.Generic;
using UnityEngine;

namespace Assets.MyAssets.Scripts.Scriptable
{

[System.Serializable]
public class SpawnPattern
{
    public string PatternName;
    public List<EnemySpawnData> EnemySpawnDatas;
}

/// <summary>
/// 6스테이지 이후 무한 진행에서 무작위로 뽑아 쓰는 스폰 패턴 풀.
/// </summary>
[CreateAssetMenu(fileName = "SpawnPatternPoolData", menuName = "Scriptable Objects/SpawnPatternPoolData")]
public class SpawnPatternPoolData : ScriptableObject
{
    [Header("일반 스테이지용 패턴")]
    public List<SpawnPattern> NormalPatterns;

    [Header("보스 스테이지용 패턴 (5의 배수)")]
    public List<SpawnPattern> BossPatterns;
}

}
