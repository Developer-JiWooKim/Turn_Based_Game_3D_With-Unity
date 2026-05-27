using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private float      _toCameraPos = 2f;

    // 총 애니메이션에 맞춘 총 위치
    private Vector3 _gunIdlePosition = new Vector3(0.021f, 0.0117f, -0.0072f);
    private Vector3 _gunIdleRotation = new Vector3(-27.923f, 91.552f, 17.673f);
    private Vector3 _gunAttackPosition = new Vector3(-0.013f, 0.007f, -0.027f);
    private Vector3 _gunAttackRotation = new Vector3(-7.931f, 78.542f, 24.135f);


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
            WeaponType type = effect.WeaponType;
            if (!_weaponCloakEffects.ContainsKey(type))
            {
                _weaponCloakEffects[type] = effect;
            }
            else
            {
                Debug.LogWarning($"WeaponType {type}이 중복 등록됨: {effect.gameObject.name}");
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
