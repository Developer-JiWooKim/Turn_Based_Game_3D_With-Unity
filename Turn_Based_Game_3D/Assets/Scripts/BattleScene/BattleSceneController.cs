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
        StageData currentStageData = StageDataManager.Instance.CurrentStageData;
        PlayerData playerData = PlayerDataManager.Instance.PlayerData;

        // 적 스폰
        List<GameObject> enemyObjects = _unitSpawner.SpawnEnemies(currentStageData);

        List<EnemyBattleUnit> enemies = new List<EnemyBattleUnit>();
        List<EnemyUnitView> enemyViews = new List<EnemyUnitView>();

        for (int i = 0; i < enemyObjects.Count; i++)
        {
            EnemyUnitView enemyView = enemyObjects[i].GetComponent<EnemyUnitView>();
            EnemyBattleUnit enemyUnit = new EnemyBattleUnit(currentStageData.enemySpawnDatas[i].enemyData);

            _battleUnitLinker.RegisterEnemyView(enemyView);

            enemyViews.Add(enemyView);
            enemies.Add(enemyUnit);
        }

        // 플레이어 유닛 생성
        // TODO#: 플레이어 스폰 역시 EnemySpawner 이름을 UnitSpawner로 바꾸고 여기서 생성하고 배치하는걸로 바꾸는게 좋을듯
        GameObject playerObject = _unitSpawner.SpawnPlayer();
        PlayerUnitView playerUnitView = playerObject.GetComponent<PlayerUnitView>();

        List<PlayerBattleUnit> players = new List<PlayerBattleUnit>();

        // GameManager에서 선택한 무기 스킬 가져오기
        WeaponData[] selectedWeapons = PlayerDataManager.Instance.SelectedWeapons;

        players.Add(new PlayerBattleUnit(playerData, selectedWeapons));

        _battleUnitLinker.RegisterPlayerView(playerUnitView);

        _battleUnitLinker.LinkUnits(players, enemies);
        _battleUnitLinker.SubscribeViews(_battleController);

        _battleController.OnBattleEnd   += HandleBattleEnd;
        _battleController.OnEnemyDied   += HandleEnemyDied;

        _targetSelector.OnTargetChanged += HandleTargetChanged;
        _targetSelector.Initialize(enemyViews);

        _battleController.StartBattle(players, enemies);
    }

    private void HandleEnemyDied(BattleUnit target)
    {
        EnemyUnitView deadView = _targetSelector.EnemyViews.Find(v => v.LinkedUnit == target);

        if (deadView != null)
        {
            deadView.SetAsTarget(false);

            deadView.OnDeath();

            _targetSelector.RemoveDeadTarget(deadView);
        }
    }

    private void HandleTargetChanged(EnemyUnitView prevTarget, EnemyUnitView nextTarget)
    {
        // TODO#: 질문 => 왜 true false?
        prevTarget?.SetAsTarget(false);
        nextTarget?.SetAsTarget(true);
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
        _battleController.OnEnemyDied -= HandleEnemyDied;
    }
}
