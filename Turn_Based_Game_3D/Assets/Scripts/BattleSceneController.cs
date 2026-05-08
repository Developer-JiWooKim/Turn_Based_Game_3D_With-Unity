using System.Collections.Generic;
using UnityEngine;

public class BattleSceneController : MonoBehaviour
{
    [SerializeField] private BattleController _battleController;
    [SerializeField] private EnemySpawner     _enemySpawner;
    [SerializeField] private BattleUnitLinker _battleUnitLinker;
    [SerializeField] private StageData        _stageData;   // TODO#: 나중에 GameManager에서 받아오기
    [SerializeField] private CharacterData    _playerData;  // TODO#: 나중에 GameManager에서 받아오기

    [SerializeField] private PlayerUnitView   _playerUnitView; // TODO#: 테스트용으로 일단 인스펙터에서 플레이어 프리팹 가져오기

    private void Start() => SetUpBattle();

    private void SetUpBattle()
    {
        // 1. 적 스폰
        List<GameObject> enemyObjects = _enemySpawner.SpawnEnemies(_stageData);
        List<EnemyBattleUnit> enemies = new List<EnemyBattleUnit>();

        for (int i = 0; i < enemyObjects.Count; i++)
        {
            EnemyUnitView enemyView = enemyObjects[i].GetComponent<EnemyUnitView>();
            EnemyBattleUnit enemyUnit = new EnemyBattleUnit(_stageData.enemySpawnDatas[i].enemyData);

            _battleUnitLinker.RegisterEnemyView(enemyView);
            enemies.Add(enemyUnit);
        }

        // 2. 플레이어 유닛 생성
        // TODO#: 플레이어 스폰 역시 EnemySpawner 이름을 UnitSpawner로 바꾸고 여기서 생성하고 배치하는걸로 바꾸는게 좋을듯
        List<PlayerBattleUnit> players = new List<PlayerBattleUnit>();
        players.Add(new PlayerBattleUnit(_playerData));
        _battleUnitLinker.RegisterPlayerView(_playerUnitView);

        _battleUnitLinker.LinkUnits(players, enemies);
        _battleUnitLinker.SubscribeViews(_battleController);

        _battleController.OnBattleEnd += HandleBattleEnd;

        _battleController.StartBattle(players, enemies);
    }

    private void HandleBattleEnd(bool isWin)
    {
        if (isWin)
        {
            GameManager.Instance.NextStage();
        }
        else
        {
            GameManager.Instance.GameOver();
        }
    }

    private void OnDestroy()
    {
        _battleController.OnBattleEnd -= HandleBattleEnd;
    }
}
