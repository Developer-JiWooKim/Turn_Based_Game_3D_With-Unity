using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _enemySpawnPoints;
    [SerializeField] private Transform[] _playerSpawnPoints;
    [SerializeField] private GameObject  _playerPrefab;


    [Header("Enemy Pool")]
    [SerializeField] private EnemyData[]  _enemyPool;
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private int          _monsterPerStage = 3; // 스테이지 당 소환할 몬스터 수(현재 3고정)

    private void Awake() => Initialize();

    private void Initialize()
    {
        // 모든 몬스터 미리 생성 후 비활성화
        for (int i = 0; i < _enemyPrefabs.Length; i++)
        {
            
        }
    }

    public List<GameObject> SpawnEnemies()
    {
        List<GameObject> spawnedUnits = new List<GameObject>();

        for (int i = 0; i < _monsterPerStage; i++)
        {
            if (i >= _enemySpawnPoints.Length)
            {
                Debug.LogError("_enemySpawnPoints의 수와 스폰할 몬스터의 데이터 수가 다름");
               
                break;
            }
            int randomIndex = Random.Range(0, _enemyPool.Length);

            //TODO#: 생성할 필요가 있나? 이미 인스펙터 창에서 모든 몬스터 데이터를 넣어놨는데? 그냥 가져오면 되는거 아님?
            spawnedUnits.Add(Instantiate(
                _enemyPrefabs[randomIndex],
                _enemySpawnPoints[i].position,
                _enemySpawnPoints[i].rotation
            ));

            // 스테이지 레벨에 따른 스탯 스케일링 적용
            EnemyUnitView view = spawnedUnits[i].GetComponent<EnemyUnitView>();

            //TODO#: ??? 이 작업을 왜 여기에서? 배틀 씬 컨트롤러가 배틀씬 사전 세팅을 담당하고 있으면 여기가 아닌 배틀씬 컨트롤러에서 하는게 맞지 않음?
            //if (view != null)
                // view.SetEnemyData(ScaleEnemyData(_enemyPool[randomIndex]));
        }

        return spawnedUnits;
    }

    // TODO#: 플레이어가 무기를 전부 고르고 게임을 시작해서 배틀 씬이 로딩될때 확정된 플레이어 데이터를 받아서 필드에 플레이어 캐릭터를 스폰할 때 
    public GameObject SpawnPlayer()
    {
        if (_playerPrefab == null)
        {
            Debug.LogError("PlayerPrefab이 없습니다!");
            return null;
        }

        if (_playerSpawnPoints.Length == 0)
        {
            Debug.LogError("PlayerSpawnPoints가 없습니다!");
            return null;
        }

        // TODO#: 왜 플레이어를 새로 생성해서 리턴? 그냥 얘가 지금 갖고 있는 플레이어 프리팹을 리턴하면 되는거 아님?
        return Instantiate(_playerPrefab, _playerSpawnPoints[0].position, _playerSpawnPoints[0].rotation);
    }
}
