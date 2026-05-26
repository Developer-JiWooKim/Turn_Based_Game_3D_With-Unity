using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private float      _toCameraPos = 2f;

    private Dictionary<WeaponType, WeaponCloakEffect> _weaponCloakEffects = new Dictionary<WeaponType, WeaponCloakEffect>();

    private HitEffectPool _hitEffectPool;

    private void Awake() => InitHitEffectPool();
    private void InitHitEffectPool()
    {
        if (_hitEffectPrefab != null)
        {
            _hitEffectPool = new HitEffectPool(_hitEffectPrefab, _toCameraPos);
        }
    }

    public void SpawnHitEffects(Vector3 position)
    {
        _hitEffectPool?.Spawn(position);
    }

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
