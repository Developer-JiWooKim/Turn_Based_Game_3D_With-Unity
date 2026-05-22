using System;
using System.Collections.Generic;
using System.Linq;
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
            if (!_pool.ContainsKey(spawnData.enemyPrefab))
            {
                _pool[spawnData.enemyPrefab] = new List<GameObject>();

            }
            int needed = stageData.enemySpawnDatas.Count(s => s.enemyPrefab == spawnData.enemyPrefab);
            while (_pool[spawnData.enemyPrefab].Count < needed)
            {
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

            // 플레이어 캐릭터 위치에서 스폰할 몬스터의 위치를 빼면 방향 벡터 나옴
            Vector3 direction = _playerSpawnPoints[0].position - _enemySpawnPoints[i].position;

            direction.y = 0f; // 수평 회전만 적용 위해 y축 0
            enemyObj.transform.rotation = Quaternion.LookRotation(direction); // 몬스터가 플레이어 캐릭터를 바라보게 회전
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
                    obj.GetComponent<EnemyUnitView>()?.Reset();
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

        // 새로 생성된 오브젝트도 초기화
        newObj.GetComponent<EnemyUnitView>()?.Reset();

        return newObj;
    }

    public GameObject SpawnPlayer()
    {
        _playerInstance.transform.position = _playerSpawnPoints[0].position;
        _playerInstance.transform.rotation = _playerSpawnPoints[0].rotation;
        _playerInstance.SetActive(true);

        // 선택한 무기 스폰
        SpawnWeapons();

        return _playerInstance;
    }

    private void SpawnWeapons()
    {
        Transform[] allTransforms = _playerInstance.GetComponentsInChildren<Transform>();
        Transform socket = null;

        foreach (var t in allTransforms)
        {
            if (t.name == "WeaponSocket")
            {
                socket = t;
                break;
            }
        }

        if (socket == null)
        {
            Debug.LogError("WeaponSocket을 찾을 수 없습니다!");
            return;
        }

        // 선택한 무기들 스폰
        WeaponData[] selectedWeapons = PlayerDataManager.Instance.SelectedWeapons;
        foreach (var weaponData in selectedWeapons)
        {
            if (weaponData == null || weaponData.WeaponPrefab == null) continue;
            if (weaponData.weaponType != WeaponType.Sword) continue; // 검만 스폰 #TODO: 임시, 나중에 각각의 무기 위치 잡은 뒤 지울거임

            //TODO#: 무기 데이터 자체에 자신이 위치할 포지션을 갖도록하는게 좋아보임
            GameObject weaponObj = Instantiate(weaponData.WeaponPrefab, socket);
            weaponObj.transform.localPosition = Vector3.zero;
            weaponObj.transform.localRotation = Quaternion.identity;
            weaponObj.SetActive(false);
        }

        // 무기 스폰 후 PlayerWeaponController 초기화
        PlayerWeaponController weaponController = _playerInstance.GetComponentInChildren<PlayerWeaponController>();
        weaponController?.InitWeapons();
    }

    public void ReturnPlayerToPool()
    {
        _playerInstance.SetActive(false);
    }
}
