using System.Collections.Generic;
using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;
using Assets.MyAssets.Scripts.Manager;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class BattleSceneController : MonoBehaviour
{
    [SerializeField] private BattleController _battleController;
    [SerializeField] private UnitSpawner      _unitSpawner; 
    [SerializeField] private BattleUnitLinker _battleUnitLinker;

    private void Start() => SetupBattle();
    private async void SetupBattle()
    {
        Debug.Log("StartBattle 호출됨");

        StageData  currentStageData  = StageManager.Instance.CurrentStageData;
        PlayerData playerData        = PlayerDataManager.Instance.PlayerData;

        // 플레이어 스폰
        GameObject playerObject = _unitSpawner.SpawnPlayer(); // TODO#: 플레이어가 늘어나면 SpawnPlayer()도 게임 오브젝트 리스트로 받아야됨

        PlayerUnitView playerUnitView = playerObject.GetComponent<PlayerUnitView>();

        WeaponData[] selectedWeapons = PlayerDataManager.Instance.SelectedWeapons;

        List<PlayerBattleUnit> players = new List<PlayerBattleUnit>();

        players.Add(new PlayerBattleUnit(playerData, selectedWeapons));

        // 사전 풀링 작업
        _unitSpawner.PreparePool(currentStageData);

        // 적 스폰
        List<GameObject> enemyObjects = _unitSpawner.SpawnEnemies(currentStageData);

        List<EnemyBattleUnit> enemies    = new List<EnemyBattleUnit>();
        List<EnemyUnitView>   enemyViews = new List<EnemyUnitView>();

        for (int i = 0; i < enemyObjects.Count; i++)
        {
            EnemyUnitView   enemyView = enemyObjects[i].GetComponent<EnemyUnitView>();
            EnemyBattleUnit enemyUnit = new EnemyBattleUnit(currentStageData.enemySpawnDatas[i].enemyData);

            _battleUnitLinker.RegisterEnemyView(enemyView);

            enemyViews.Add(enemyView);
            enemies.Add(enemyUnit);
        }

        _battleUnitLinker.RegisterPlayerView(playerUnitView);
        _battleUnitLinker.LinkUnits(players, enemies);
        _battleUnitLinker.SubscribeViews(_battleController);

        _battleController.OnBattleEnd += HandleBattleEnd;
        _battleController.OnEnemyDied += HandleEnemyDied;

        // 모든 몬스터 소환 애니메이션 완료까지 대기
        List<Awaitable> spawnTasks = new List<Awaitable>();
        foreach (var enemyView in enemyViews)
        {
            spawnTasks.Add(enemyView.PlaySpawnAnimAsync());
        }
        foreach (var task in spawnTasks)
        {
            await task;
        }

        _battleController.StartBattle(players, enemies);
    }

    private void HandleEnemyDied(BattleUnit target)
    {
        EnemyUnitView deadView = _battleController.GetEnemyUnitView(target);
        if (deadView != null)
        {
            _unitSpawner.ReturnToPool(deadView.gameObject);
        }
    }

    private void HandleBattleEnd(bool isWin)
    {
        if (isWin)
        {
            // 다음 스테이지 전 총알 리셋
            foreach (var player in _battleController.PlayerUnits)
                player.ResetAmmos();

            // 노멀 모드 스테이지 클리어 시 레벨업
            if (StageManager.Instance.CurrentGameMode == GameMode.Normal)
                PlayerDataManager.Instance.LevelUp();


            GameManager.Instance.NextStage();
        }
        else
        {
            // 게임 오버 시 초기화
            PlayerDataManager.Instance.ResetPlayerProgress();

            GameManager.Instance.GameOver();
        }
    }

    private void OnDestroy()
    {
        _battleController.OnBattleEnd -= HandleBattleEnd;
        _battleController.OnEnemyDied -= HandleEnemyDied;
    }
}

}
