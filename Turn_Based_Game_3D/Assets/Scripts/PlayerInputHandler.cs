using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private BattleController _battleController;

    public event Action OnPlayerInputReady;

    public void Subscribe(BattleController battleController)
    {
        _battleController = battleController;

        _battleController.OnTurnStart += HandleTurnStart;
    }

    private void Unsubscribe()
    {
        if (_battleController != null)
        {
            _battleController.OnTurnStart -= HandleTurnStart;
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void HandleTurnStart(BattleUnit unit)
    {
        if (unit.IsPlayer)
        {
            OnPlayerInputReady?.Invoke(); // 스킬 버튼 활성화
        }
        else
        {
           // 스킬 버튼 비활성화
        }
    }
}
