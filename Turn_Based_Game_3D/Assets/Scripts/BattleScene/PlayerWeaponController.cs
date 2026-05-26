using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private float      _toCameraPos = 2f;

    // 총 애니메이션에 맞춘 총 위치
    [SerializeField] private Vector3 _gunIdlePosition = new Vector3(-0.01821823f, 0.0319531f, -0.02460939f);
    [SerializeField] private Vector3 _gunIdleRotation = new Vector3(-11.144f, 87.692f, 14.683f);
    [SerializeField] private Vector3 _gunAttackPosition = new Vector3(-0.0246552f, 0.02883693f, -0.0247517f);
    [SerializeField] private Vector3 _gunAttackRotation = new Vector3(3.97f, 79.856f, 13.905f);


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

    public void SetGunIdlePose()
    {
        if (!_weaponCloakEffects.TryGetValue(WeaponType.Gun, out WeaponCloakEffect cloakEffect)) return;

        cloakEffect.transform.localPosition = _gunIdlePosition;
        cloakEffect.transform.localRotation = Quaternion.Euler(_gunIdleRotation);
    }

    public void SetGunAttackPose()
    {
        if (!_weaponCloakEffects.TryGetValue(WeaponType.Gun, out WeaponCloakEffect cloakEffect)) return;

        cloakEffect.transform.localPosition = _gunAttackPosition;
        cloakEffect.transform.localRotation = Quaternion.Euler(_gunAttackRotation);
    }

}
