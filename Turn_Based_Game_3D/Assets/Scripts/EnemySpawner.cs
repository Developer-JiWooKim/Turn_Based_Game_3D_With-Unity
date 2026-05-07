using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] 
    private Transform[] _spawnPoints;

    private List<EnemyData> _enemiesData;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _enemiesData = new List<EnemyData>();
    }

    public void SpawnEnemy(StageData stageData)
    {
        for (int i = 0; i < stageData.enemySpawnDatas.Count; i++)
        {
            if (i >= _spawnPoints.Length) return;

            EnemySpawnData spawnData = stageData.enemySpawnDatas[i];

            // TODO#: 지금은 스포너를 통해 몬스터를 Instantiate생성 삭제하겠지만, 나중에는 처음 게임을 실행할때 한번 모든 몬스터를 생성 후 리스트에 담아놓고 필요할때 가져다 쓰게끔(최적화)

            GameObject enemy = Instantiate(spawnData.enemyPrefab, _spawnPoints[i].position, _spawnPoints[i].rotation);
        }
    }

}
