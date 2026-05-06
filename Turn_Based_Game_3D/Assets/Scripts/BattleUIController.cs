using System;
using UnityEngine;

public class BattleUIController : MonoBehaviour
{
    private BattleController _battleController;
    private PlayerInputHandler _playerInputHandler;

    public event Action<BattleUnit, PlayerSkillData> OnPlayerInput;

    //외부에서 호출할 
    public void Subscribe(BattleController battleController, PlayerInputHandler playerInputHandler)
    {
        _battleController = battleController;
        _playerInputHandler = playerInputHandler;

        _battleController.OnTurnStart += OnTurnStart;
        _battleController.OnUnitDamaged += HandleUnitDamaged;
        _battleController.OnBattleEnd += HandleBattleEnd;

        _playerInputHandler.OnPlayerInputReady += HandlePlayerInputReady;
    }

    private void HandlePlayerInputReady()
    {
        
    }
    private void HandleUnitDamaged(IDamageable target, int damage)
    {

    }

    private void HandleBattleEnd(bool result)
    {

    }

    private void Unsubscribe()
    {
        if (_battleController != null)
        {
            _battleController.OnTurnStart -= OnTurnStart;
            _battleController.OnBattleEnd -= HandleBattleEnd;
            _battleController.OnUnitDamaged -= HandleUnitDamaged;
        }
        if (_playerInputHandler != null)
        {
            _playerInputHandler.OnPlayerInputReady -= HandlePlayerInputReady;
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }


    private void OnTurnStart(BattleUnit unit) 
    {
        if (unit.IsPlayer)
        {
            // 카메라 위치를 플레이어 뒤쪽으로 이동
            // 스킬 버튼 활성화
            // 플레이어 입력을 기다림
        }
        else
        {
            // 스킬 버튼 비활성화
        }
    }
}
