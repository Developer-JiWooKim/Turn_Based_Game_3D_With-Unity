using System.Collections.Generic;
using UnityEngine;

public class BattleUnitLinker : MonoBehaviour
{
    private List<PlayerUnitView> _playerViews = new List<PlayerUnitView>();
    private List<EnemyUnitView>  _enemyViews  = new List<EnemyUnitView>();

    public void RegisterPlayerView(PlayerUnitView playerView)
    {
        _playerViews.Add(playerView);
    }

    public void RegisterEnemyView(EnemyUnitView enemyView)
    {
        _enemyViews.Add(enemyView);
    }

    public void LinkUnits(List<PlayerBattleUnit> players, List<EnemyBattleUnit> enemies)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (i >= _playerViews.Count)
            {
                Debug.Log("_playerViews의 숫자와 플레이어 배틀 유닛의 숫자가 다름");
                break;
            }
            _playerViews[i].UnitLink(players[i]);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (i >= _enemyViews.Count)
            {
                Debug.Log("_enemyViews의 숫자와 적 배틀 유닛의 숫자가 다름");
                break;
            }
            _enemyViews[i].UnitLink(enemies[i]);
        }
    }

    public void SubscribeViews(BattleController battleController)
    {
        foreach (var view in _playerViews)
            view.Subscribe(battleController);

        foreach (var view in _enemyViews)
            view.Subscribe(battleController);
    }
}
