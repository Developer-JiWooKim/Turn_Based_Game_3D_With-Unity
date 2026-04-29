using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    [Header("HUD")]
    public List<BattleHUD> playerHUDs;  // 플레이어 수만큼
    public List<BattleHUD> enemyHUDs;   // 적 수만큼

    [Header("액션 메뉴")]
    public ActionMenuUI actionMenu;

    [Header("전투 결과")]
    public GameObject winPanel;
    public GameObject losePanel;

    private void Start()
    {
        InitHUDs();
        SubscribeEvents();

        winPanel.SetActive(false);
        losePanel.SetActive(false);
        actionMenu.gameObject.SetActive(false);
    }

    private void InitHUDs()
    {
        var players = BattleManager.Instance.PlayerUnits;
        var enemies = BattleManager.Instance.EnemyUnits;

        for (int i = 0; i < playerHUDs.Count; i++)
            if (i < players.Count)
                playerHUDs[i].Initialize(players[i]);

        for (int i = 0; i < enemyHUDs.Count; i++)
            if (i < enemies.Count)
                enemyHUDs[i].Initialize(enemies[i]);
    }

    private void SubscribeEvents()
    {
        BattleManager.Instance.OnUnitDamaged += HandleDamaged;
        BattleManager.Instance.OnUnitHealed += HandleHealed;
        BattleManager.Instance.OnBattleWin += HandleWin;
        BattleManager.Instance.OnBattleLose += HandleLose;
    }

    private void OnDestroy()
    {
        if (BattleManager.Instance == null) return;
        BattleManager.Instance.OnUnitDamaged -= HandleDamaged;
        BattleManager.Instance.OnUnitHealed -= HandleHealed;
        BattleManager.Instance.OnBattleWin -= HandleWin;
        BattleManager.Instance.OnBattleLose -= HandleLose;
    }

    // ── 이벤트 처리 ──────────────────────────────────────
    private void HandleDamaged(BattleUnit unit, int amount)
    {
        RefreshHUD(unit);
    }

    private void HandleHealed(BattleUnit unit, int amount)
    {
        RefreshHUD(unit);
    }

    private void HandleWin()
    {
        winPanel.SetActive(true);
    }

    private void HandleLose()
    {
        losePanel.SetActive(true);
    }

    // ── HUD 갱신 ─────────────────────────────────────────
    private void RefreshHUD(BattleUnit unit)
    {
        foreach (var hud in playerHUDs)
            if (hud != null) hud.Refresh();

        foreach (var hud in enemyHUDs)
            if (hud != null) hud.Refresh();
    }
}