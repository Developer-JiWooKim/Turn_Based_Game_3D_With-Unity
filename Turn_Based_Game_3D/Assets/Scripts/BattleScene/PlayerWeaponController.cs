using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Dictionary<WeaponType, WeaponCloakEffect> _weaponCloakEffects = new Dictionary<WeaponType, WeaponCloakEffect>();

    public void InitWeapons()
    {
        _weaponCloakEffects.Clear();

        WeaponCloakEffect[] effects = GetComponentsInChildren<WeaponCloakEffect>(true);

        foreach (var effect in effects)
        {
            foreach (WeaponType weaponType in System.Enum.GetValues(typeof(WeaponType)))
            {
                if (effect.gameObject.name.Contains(weaponType.ToString()))
                {
                    _weaponCloakEffects[weaponType] = effect;
                    break;
                }
            }
        }
    }

    public async Awaitable UncloakWeapon(WeaponType weaponType)
    {
        if (_weaponCloakEffects.TryGetValue(weaponType, out WeaponCloakEffect cloakEffect))
        {
            cloakEffect.gameObject.SetActive(true);
            await cloakEffect.UncloakAsync();
        }
            
    }

    public async Awaitable CloakWeapon(WeaponType weaponType)
    {
        if (_weaponCloakEffects.TryGetValue(weaponType, out WeaponCloakEffect cloakEffect))
        {
            await cloakEffect.CloakAsync();
            cloakEffect.gameObject.SetActive(false); // 비활성화
        }
    }
}
