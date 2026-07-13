using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class TargetSelector : MonoBehaviour
{
    private List<EnemyUnitView> _enemyViews;

    private int _currentTargetIndex;


    public List<EnemyUnitView> EnemyViews => _enemyViews;

    public EnemyUnitView CurrentTarget => _enemyViews.Count > 0 ? _enemyViews[_currentTargetIndex] : null;

    public event Action<EnemyUnitView, EnemyUnitView> OnTargetChanged; // 이전 타겟, 새 타겟


    public void Initialize(List<EnemyUnitView> enemyViews)
    {
        _enemyViews = new List<EnemyUnitView>();

        _enemyViews = enemyViews
                        .Where(view => !view.LinkedUnit.IsDead)
                        .OrderByDescending(view => view.transform.position.x)
                        .ToList();

        _currentTargetIndex = 0;

        OnTargetChanged?.Invoke(null, CurrentTarget);
    }

    public void SelectTarget(int index)
    {
        if (_enemyViews.Count == 0) return;

        EnemyUnitView prevTarget = CurrentTarget;

        _currentTargetIndex = Mathf.Clamp(index, 0, _enemyViews.Count - 1);

        OnTargetChanged?.Invoke(prevTarget, CurrentTarget);
    }

    public void SelectNext()
    {
        // 리스트 끝에 도달시 처음으로 순환
        int nextIndex = (_currentTargetIndex - 1 + _enemyViews.Count) % _enemyViews.Count;

        SelectTarget(nextIndex);
    }

    public void SelectPrev()
    {
        // 리스트 처음일때 옆으로 이동시 마지막으로 순환
        int prevIndex = (_currentTargetIndex + 1) % _enemyViews.Count;

        SelectTarget(prevIndex);
    }

    public void RemoveDeadTarget(EnemyUnitView deadUnit)
    {
        _enemyViews.Remove(deadUnit);

        if (_enemyViews.Count == 0) return;

        _currentTargetIndex = Mathf.Clamp(_currentTargetIndex, 0, _enemyViews.Count - 1);

        OnTargetChanged?.Invoke(null, CurrentTarget);
    }
}

}