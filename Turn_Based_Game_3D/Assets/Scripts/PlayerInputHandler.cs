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
        // TODO#: 지울예정
        Debug.Log($"HandleTurnStart 호출됨: {unit.Name} IsPlayer: {unit.IsPlayer}");
        
        if (unit is PlayerBattleUnit)
        {
            OnPlayerInputReady?.Invoke();
        }
    }

    /// <summary>
    /// 플레이어가 사용할 스킬 버튼을 누르면 대상과 사용한 스킬을 알림
    /// </summary>
    public void NotifyPlayerActed(PlayerSkillData skill)
    {
        // BattleController 에서 첫 번째 살아있는 적 찾기
        _battleController.OnPlayerAction(null, skill); // BattleController가 타겟 찾음
    }
}
