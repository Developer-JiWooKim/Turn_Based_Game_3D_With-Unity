using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _enemySpawnPoints;
    [SerializeField] private Transform[] _playerSpawnPoints;

    private List<EnemyData> _enemyDatas;    // TODO#: 오브젝트 풀링에 쓸 모든 적 데이터 리스트

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _enemyDatas = new List<EnemyData>();
    }

    public List<GameObject> SpawnEnemies(StageData stageData)
    {
        List<GameObject> spawnedUnits = new List<GameObject>();
        EnemySpawnData spawnData = null;

        for (int i = 0; i < stageData.enemySpawnDatas.Count; i++)
        {
            if (i >= _enemySpawnPoints.Length)
            {
                Debug.LogError("_enemySpawnPoints의 수와 스폰할 몬스터의 데이터 수가 다름");
               
                break;
            }

            spawnData = stageData.enemySpawnDatas[i];

            // TODO#: 오브젝트 풀링을 쓰면 만들어져있는 리스트에서 오브젝트를 가져옴
            spawnedUnits.Add(Instantiate(spawnData.enemyPrefab, _enemySpawnPoints[i].position, _enemySpawnPoints[i].rotation));
        }

        return spawnedUnits;
    }

    // TODO#: 플레이어가 무기를 전부 고르고 게임을 시작해서 배틀 씬이 로딩될때 확정된 플레이어 데이터를 받아서 필드에 플레이어 캐릭터를 스폰할 때 
    public void SpawnPlayer()
    {
        
    }
}
