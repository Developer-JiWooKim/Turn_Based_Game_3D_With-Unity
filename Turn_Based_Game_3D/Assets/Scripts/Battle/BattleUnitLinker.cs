using System.Collections.Generic;
using UnityEngine;

public class BattleUnitLinker : MonoBehaviour
{
    [Header("씬의 UnitView 오브젝트들")]
    public UnitView[] playerViews;   // 씬에 배치된 플레이어 오브젝트
    public UnitView[] enemyViews;    // 씬에 배치된 적 오브젝트

    private void Start()
    {
        LinkUnits();
    }

    private void LinkUnits()
    {
        List<BattleUnit> players = BattleManager.Instance.PlayerUnits;
        List<BattleUnit> enemies = BattleManager.Instance.EnemyUnits;

        // 플레이어 연결
        for (int i = 0; i < playerViews.Length; i++)
        {
            if (i < players.Count)
                playerViews[i].Initialize(players[i]);
        }

        // 적 연결
        for (int i = 0; i < enemyViews.Length; i++)
        {
            if (i < enemies.Count)
                enemyViews[i].Initialize(enemies[i]);
        }

        // 이벤트 구독
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        BattleManager.Instance.OnUnitDamaged += HandleDamaged;
        BattleManager.Instance.OnUnitHealed += HandleHealed;
        BattleManager.Instance.OnUnitDied += HandleDied;
        BattleManager.Instance.OnTurnStart += HandleTurnStart;
    }

    private void OnDestroy()
    {
        if (BattleManager.Instance == null) return;
        BattleManager.Instance.OnUnitDamaged -= HandleDamaged;
        BattleManager.Instance.OnUnitHealed -= HandleHealed;
        BattleManager.Instance.OnUnitDied -= HandleDied;
        BattleManager.Instance.OnTurnStart -= HandleTurnStart;
    }

    // ── 이벤트 처리 ──────────────────────────────────────
    private void HandleDamaged(BattleUnit unit, int amount)
    {
        UnitView view = FindView(unit);
        view?.PlayHitAnim();
        Debug.Log($"{unit.UnitName} 이(가) {amount} 데미지를 받았습니다.");
    }

    private void HandleHealed(BattleUnit unit, int amount)
    {
        Debug.Log($"{unit.UnitName} 이(가) {amount} 회복했습니다.");
    }

    private void HandleDied(BattleUnit unit)
    {
        UnitView view = FindView(unit);
        view?.PlayDeathAnim();
        Debug.Log($"{unit.UnitName} 사망");
    }

    private void HandleTurnStart(BattleUnit unit)
    {
        Debug.Log($"{unit.UnitName} 의 턴");
    }

    // ── UnitView 탐색 ─────────────────────────────────────
    private UnitView FindView(BattleUnit unit)
    {
        foreach (var v in playerViews)
            if (v.LinkedUnit == unit) return v;

        foreach (var v in enemyViews)
            if (v.LinkedUnit == unit) return v;

        return null;
    }
}