using System.Collections.Generic;
using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;
using Assets.MyAssets.Scripts.Struct;
using Assets.MyAssets.Scripts.Manager;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class BattleSceneController : MonoBehaviour
{
    [SerializeField] private BattleController _battleController;
    [SerializeField] private UnitSpawner      _unitSpawner;
    [SerializeField] private BattleUnitLinker _battleUnitLinker;
    [SerializeField] private RoguelikeChoiceUIController _roguelikeChoiceUIController; // 스테이지 승리 후 로그라이크 선택지 팝업

    private void Start() => SetupBattle();
    private async void SetupBattle()
    {
        Debug.Log("StartBattle 호출됨");

        StageData         currentStageData = StageManager.Instance.CurrentStageData;
        List<PartyMember> partyRoster      = PlayerDataManager.Instance.PartyRoster;
        WeaponData[]       selectedWeapons  = PlayerDataManager.Instance.SelectedWeapons;

        // 파티 시너지 계산을 위해 캐릭터 유형별 인원 수를 미리 집계
        Dictionary<CharacterType, int> typeCounts = new Dictionary<CharacterType, int>();
        foreach (var member in partyRoster)
        {
            typeCounts.TryGetValue(member.Data.Character, out int count);
            typeCounts[member.Data.Character] = count + 1;
        }

        // 파티 스폰
        List<GameObject> playerObjects = _unitSpawner.SpawnParty(partyRoster.Count);

        List<PlayerBattleUnit> players = new List<PlayerBattleUnit>();

        for (int i = 0; i < playerObjects.Count; i++)
        {
            PlayerUnitView   playerUnitView = playerObjects[i].GetComponent<PlayerUnitView>();
            PartyMember      member         = partyRoster[i];
            PlayerBattleUnit playerUnit     = new PlayerBattleUnit(member, selectedWeapons);

            // 런 로그라이크 버프 적용
            playerUnit.ApplyStatBonus(RunStateManager.Instance.RunBuffs);

            // 파티 시너지 적용 (동일 캐릭터가 RequiredCount 이상일 때 해당 캐릭터 본인에게만)
            SynergyEntry synergy = RoguelikeChoiceManager.Instance.SynergyPool.Entries.Find(e => e.Character == member.Data.Character);
            if (synergy != null && typeCounts[member.Data.Character] >= synergy.RequiredCount)
            {
                playerUnit.ApplyStatBonus(synergy.Bonus);
            }

            // 이전 스테이지에서 이어받은 HP로 최종 덮어쓰기 (버프 적용 이후에 해야 늘어난 최대체력 기준으로 정확히 클램프됨)
            playerUnit.SetCurrentHp(member.CurrentHp);

            _battleUnitLinker.RegisterPlayerView(playerUnitView);
            players.Add(playerUnit);
        }

        // 사전 풀링 작업
        _unitSpawner.PreparePool(currentStageData);

        // 적 스폰
        List<GameObject> enemyObjects = _unitSpawner.SpawnEnemies(currentStageData);

        List<EnemyBattleUnit> enemies    = new List<EnemyBattleUnit>();
        List<EnemyUnitView>   enemyViews = new List<EnemyUnitView>();

        // 로그라이크 몬스터 디버프 예약을 1회만 소비 (스테이지의 모든 적에게 동일 적용)
        bool stunNext      = RunStateManager.Instance.ConsumeEnemyStun();
        int  hpDownPercent  = RunStateManager.Instance.ConsumeEnemyHpDownPercent();
        int  atkDownPercent = RunStateManager.Instance.ConsumeEnemyAtkDownPercent();

        for (int i = 0; i < enemyObjects.Count; i++)
        {
            EnemyUnitView   enemyView = enemyObjects[i].GetComponent<EnemyUnitView>();
            EnemyBattleUnit enemyUnit = new EnemyBattleUnit(currentStageData.enemySpawnDatas[i].enemyData);

            if (stunNext)
            {
                enemyUnit.SkipFirstAction = true;
            }

            if (hpDownPercent > 0 || atkDownPercent > 0)
            {
                StatData debuff = default;
                debuff.Hp = -(enemyUnit.MaxHp * hpDownPercent / 100);
                debuff.AttackPower = -(enemyUnit.Atk * atkDownPercent / 100);
                enemyUnit.ApplyStatBonus(debuff);
            }

            _battleUnitLinker.RegisterEnemyView(enemyView);

            enemyViews.Add(enemyView);
            enemies.Add(enemyUnit);
        }

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
            // 사망한 파티원은 영구 추방, 생존자는 다음 스테이지 전 총알 리셋 + HP 이어받기 기록
            foreach (var player in _battleController.PlayerUnits)
            {
                if (player.IsDead)
                {
                    PlayerDataManager.Instance.RemoveFromRoster(player.PartyMember);
                }
                else
                {
                    player.ResetAmmos();
                    PlayerDataManager.Instance.SetCurrentHp(player.PartyMember, player.CurrentHp);
                }
            }

            // 로그라이크 선택지 팝업 → 선택 완료(영입 교체 흐름 포함) 후 다음 스테이지로 이동
            _roguelikeChoiceUIController.ShowChoices(() =>
            {
                StageManager.Instance.NextStage();
                GameManager.Instance.LoadScene("BattleScene");
            });
        }
        else
        {
            // 전멸 시 다음 런을 위해 로스터/런 상태 복원 (게임 오버 UI는 BattleUIController가 OnBattleEnd로 직접 처리)
            PlayerDataManager.Instance.ResetRoster();
            RunStateManager.Instance.ResetRun();
        }
    }

    private void OnDestroy()
    {
        _battleController.OnBattleEnd -= HandleBattleEnd;
        _battleController.OnEnemyDied -= HandleEnemyDied;
    }
}

}
