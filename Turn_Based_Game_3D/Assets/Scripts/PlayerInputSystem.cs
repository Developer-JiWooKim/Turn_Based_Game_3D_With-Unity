using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    public event Action OnNextInput;
    public event Action OnPrevInput;
    public event Action<Vector2> OnSelectTargetInput; // 화면상 마우스 위치 전달

    public void OnNext(InputAction.CallbackContext context)
    {
        if (context.performed) OnNextInput?.Invoke();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (context.performed) OnPrevInput?.Invoke();
    }

    public void OnSelectTarget(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            OnSelectTargetInput?.Invoke(mousePos);
        }
    }
}
