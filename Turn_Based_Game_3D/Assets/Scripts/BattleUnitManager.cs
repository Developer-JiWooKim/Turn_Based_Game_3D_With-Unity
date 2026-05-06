using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager : MonoBehaviour
{
    private static BattleUnitManager _instance;
    public static BattleUnitManager Instance
    {
        get 
        {
            if (_instance == null)
            {
                return null;
            }
            return _instance;
        }
    }


    private List<PlayerUnitView> _playerViews;
    private List<EnemyUnitView> _enemyViews;
    private BattleController _battleController;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            _playerViews = new List<PlayerUnitView>();
            _enemyViews = new List<EnemyUnitView>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayerViewRegister(PlayerUnitView playerView)
    {
        _playerViews.Add(playerView);
    }

    public void EnemyViewRegister(EnemyUnitView enemyView)
    {
        _enemyViews.Add(enemyView);
    }

    public void LinkUnit(List<PlayerBattleUnit> players, List<EnemyBattleUnit> enemies)
    {
        for (int i = 0; i < players.Count; i++)
        {
            _playerViews[i].UnitLink(players[i]);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            _enemyViews[i].UnitLink(enemies[i]);
        }
    }

    public void Subscribe(BattleController battleController)
    {
        _battleController = battleController;

        _battleController.OnUnitDamaged += OnUnitDamaged;
        _battleController.OnBattleEnd += OnBattleEnd;
    }

    private void OnBattleEnd(bool result)
    {

    }

    private void OnUnitDamaged(IDamageable unit, int damage)
    {
        foreach (var view in _playerViews)
        {
            if (view.LinkedUnit == unit as BattleUnit)
            {
                view.OnDamaged(damage);
                return;
            }
        }

        foreach (var view in _enemyViews)
        {
            if (view.LinkedUnit == unit as BattleUnit)
            {
                view.OnDamaged(damage);
                return;
            }
        }
    }

    private void UnSubscribe()
    {
        if (_battleController != null)
        {
            _battleController.OnUnitDamaged -= OnUnitDamaged;
            _battleController.OnBattleEnd -= OnBattleEnd;
        }
       
    }
    private void OnDestroy()
    {
        UnSubscribe();
    }
}
