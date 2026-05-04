using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private BattleController _battleController;

    public static event Action OnPlayerInputReady;

    private void OnEnable()
    {
        _battleController.OnTurnStart += HandleTurnStart;


    }

    private void OnDisable()
    {
        _battleController.OnTurnStart -= HandleTurnStart;
    }

    private void HandleTurnStart(BattleUnit unit)
    {
        if (unit.IsPlayer)
        {
            OnPlayerInputReady?.Invoke(); // 스킬 버튼 활성화
        }
    }

    


}
