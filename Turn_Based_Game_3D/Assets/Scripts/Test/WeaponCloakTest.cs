using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponCloakTest : MonoBehaviour
{
    [SerializeField] private WeaponCloakEffect _cloakEffect;

    private void Update()
    {
        if (Keyboard.current.uKey.wasPressedThisFrame)
            _ = _cloakEffect.UncloakAsync();

        if (Keyboard.current.cKey.wasPressedThisFrame)
            _ = _cloakEffect.CloakAsync();
    }
}