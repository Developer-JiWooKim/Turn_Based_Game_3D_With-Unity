using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _enemySpawnPoints;
    [SerializeField] private Transform[] _playerSpawnPoints;
    [SerializeField] private GameObject  _playerPrefab;


    // 풀: 프리팹별로 미리 생성한 오브젝트 보관
    private Dictionary<GameObject, List<GameObject>> _pool;
    private GameObject _playerInstance;



    private void Awake() => Initialize();

    private void Initialize()
    {
        _pool = new Dictionary<GameObject, List<GameObject>>();

        // 플레이어 생성 후 비활성화
        _playerInstance = Instantiate(_playerPrefab, _playerSpawnPoints[0].position, _playerSpawnPoints[0].rotation);
        _playerInstance.SetActive(false);
    }

    public void PreparePool(StageData stageData)
    {
        foreach(var spawnData in stageData.enemySpawnDatas)
        {
            if(!_pool.ContainsKey(spawnData.enemyPrefab))
            {
                _pool[spawnData.enemyPrefab] = new List<GameObject>();

                GameObject enemyObj = Instantiate(spawnData.enemyPrefab);
                enemyObj.SetActive(false);

                _pool[spawnData.enemyPrefab].Add(enemyObj);
            }
        }
    }

    public List<GameObject> SpawnEnemies(StageData stageData)
    {
        List<GameObject> spawnedUnits = new List<GameObject>();

        for (int i = 0; i < stageData.enemySpawnDatas.Count; i++)
        {
            if (i >= _enemySpawnPoints.Length)
            {
                Debug.LogError("_enemySpawnPoints의 수와 스폰할 몬스터의 데이터 수가 다름");
               
                break;
            }
            GameObject prefab = stageData.enemySpawnDatas[i].enemyPrefab;
            GameObject enemyObj = GetFromPool(prefab);

            enemyObj.transform.position = _enemySpawnPoints[i].position;
            enemyObj.transform.rotation = _enemySpawnPoints[i].rotation;
            enemyObj.SetActive(true);

            spawnedUnits.Add(enemyObj);
        }

        return spawnedUnits;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

    private GameObject GetFromPool(GameObject prefab)
    {
        if (_pool.ContainsKey(prefab))
        {
            foreach(var obj in _pool[prefab])
            {
                if (!obj.activeSelf)
                {
                    // View 초기화
                    EnemyUnitView view = obj.GetComponent<EnemyUnitView>();
                    view?.Reset();
                    return obj;
                }
            }
        }

        // 풀에 없으면 새로 생성
        Debug.LogWarning($"{prefab.name} 풀 부족 - 새로 생성");
        GameObject newObj = Instantiate(prefab);
        newObj.SetActive(false);

        if (!_pool.ContainsKey(prefab))
        {
            _pool[prefab] = new List<GameObject>();
        }

        _pool[prefab].Add(newObj);
        return newObj;
    }

    public GameObject SpawnPlayer()
    {
        _playerInstance.transform.position = _playerSpawnPoints[0].position;
        _playerInstance.transform.rotation = _playerSpawnPoints[0].rotation;
        _playerInstance.SetActive(true);

        return _playerInstance;
    }

    public void ReturnPlayerToPool()
    {
        _playerInstance.SetActive(false);
    }
}
