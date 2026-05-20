using System.Collections.Generic;
using UnityEngine;

public class BattleSceneController : MonoBehaviour
{
    [SerializeField] private BattleController _battleController;
    [SerializeField] private UnitSpawner      _unitSpawner; 
    [SerializeField] private BattleUnitLinker _battleUnitLinker;
    [SerializeField] private TargetSelector   _targetSelector;

    private void Start() => SetupBattle();
    private void SetupBattle()
    {
        StageData  currentStageData  = StageManager.Instance.CurrentStageData;
        PlayerData playerData        = PlayerDataManager.Instance.PlayerData;

        // 플레이어 스폰
        GameObject playerObject = _unitSpawner.SpawnPlayer();
        PlayerUnitView playerUnitView = playerObject.GetComponent<PlayerUnitView>();

        List<PlayerBattleUnit> players = new List<PlayerBattleUnit>();

        WeaponData[] selectedWeapons = PlayerDataManager.Instance.SelectedWeapons;

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

        Subscribe();

        _targetSelector.OnTargetChanged += HandleTargetChanged;
        _targetSelector.Initialize(enemyViews);

        _battleController.StartBattle(players, enemies);
    }
    private void Subscribe()
    {
        _battleController.OnBattleEnd += HandleBattleEnd;
        _battleController.OnEnemyDied += HandleEnemyDied;
    }

    private void HandleEnemyDied(BattleUnit target)
    {
        EnemyUnitView deadView = _targetSelector.EnemyViews.Find(v => v.LinkedUnit == target);
        
        if (deadView != null)
        {
            deadView.SetAsTarget(false);

            _targetSelector.RemoveDeadTarget(deadView);

            _unitSpawner.ReturnToPool(deadView.gameObject); // 풀 반납
        }
    }

    private void HandleTargetChanged(EnemyUnitView prevTarget, EnemyUnitView nextTarget)
    {
        prevTarget?.SetAsTarget(false);
        nextTarget?.SetAsTarget(true);
    }

    private void HandleBattleEnd(bool isWin)
    {
        if (isWin)
        {
            // 다음 스테이지 전 총알 리셋
            // TODO#: 나중에 플레이어 유닛 리스트로 교체
            foreach (var player in _battleController.PlayerUnits)
                player.ResetAmmos();
            GameManager.Instance.NextStage();
        }
        else
        {
            GameManager.Instance.GameOver();
        }
    }

    private void OnDestroy() => UnSubscribe();
    private void UnSubscribe()
    {
        _battleController.OnBattleEnd -= HandleBattleEnd;
        _battleController.OnEnemyDied -= HandleEnemyDied;
    }
}
