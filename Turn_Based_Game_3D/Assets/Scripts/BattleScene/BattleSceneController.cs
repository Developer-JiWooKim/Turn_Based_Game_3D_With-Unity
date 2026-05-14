using System.Collections.Generic;
using UnityEngine;

public class BattleSceneController : MonoBehaviour
{
    [SerializeField] private BattleController _battleController;
    [SerializeField] private UnitSpawner      _unitSpawner; 
    [SerializeField] private BattleUnitLinker _battleUnitLinker;
    [SerializeField] private TargetSelector   _targetSelector;
    [SerializeField] private StageData        _stageData;   // TODO#: 나중에 GameManager에서 받아오기
    [SerializeField] private CharacterData    _playerData;  // TODO#: 나중에 GameManager에서 받아오기
    


    // [SerializeField] private PlayerUnitView   _playerUnitView; // TODO#: 테스트용으로 일단 인스펙터에서 플레이어 프리팹 가져오기

    private void Start() => SetupBattle();

    private void SetupBattle()
    {
        // 적 스폰
        List<GameObject> enemyObjects = _unitSpawner.SpawnEnemies(_stageData);
        List<EnemyBattleUnit> enemies = new List<EnemyBattleUnit>();
        List<EnemyUnitView> enemyViews = new List<EnemyUnitView>();

        for (int i = 0; i < enemyObjects.Count; i++)
        {
            EnemyUnitView enemyView = enemyObjects[i].GetComponent<EnemyUnitView>();
            EnemyBattleUnit enemyUnit = new EnemyBattleUnit(_stageData.enemySpawnDatas[i].enemyData);

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
        WeaponData[] selectedWeapons = GameManager.Instance.SelectedWeapons;
        List<PlayerSkillData> weaponSkills = new List<PlayerSkillData>();

        foreach(var weapon in selectedWeapons)
        {
            if (weapon != null && weapon.skill != null)
                weaponSkills.Add(weapon.skill);
        }

        // 무기 스킬이 있으면 무기 스킬로, 없으면 기본 캐릭터 스킬로
        if (weaponSkills.Count > 0)
            players.Add(new PlayerBattleUnit(_playerData, weaponSkills));
        else
            players.Add(new PlayerBattleUnit(_playerData));

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
        prevTarget?.SetAsTarget(false);  // 왜 false?
        nextTarget?.SetAsTarget(true);  // 왜 true?
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
