using System.Collections.Generic;
using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private float      _toCameraPos = 2f;

    //석궁 애니메이션에 맞춘 총 위치
    private Vector3 _crossbowIdlePosition = new Vector3(0.272f, 0.138f, -0.003f);
    private Vector3 _crossbowIdleRotation = new Vector3(-22.356f, 97.813f, 0.4f);
    private Vector3 _crossbowAttackPosition = new Vector3(0.304f, 0.1f, 0.063f);
    private Vector3 _crossbowAttackRotation = new Vector3(-10.231f, 79.644f, 18.365f);

    private Dictionary<WeaponType, (Transform rightHand, Transform leftHand)> _weaponIKPoints = new();

    private Dictionary<WeaponType, WeaponCloakEffect> _weaponCloakEffects = new Dictionary<WeaponType, WeaponCloakEffect>();

    private HitEffectPool               _hitEffectPool;
    private GunEffectController         _gunEffectController;
    private CrossbowEffectController    _crossbowEffectController;

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

        if (_weaponCloakEffects.TryGetValue(WeaponType.Crossbow, out WeaponCloakEffect crossbowEffect))
        {
            _crossbowEffectController = crossbowEffect.GetComponentInChildren<CrossbowEffectController>(true);
        }

        foreach (var kvp in _weaponCloakEffects)
        {
            Transform rightHand = null;
            Transform leftHand = null;

            foreach (Transform t in kvp.Value.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == "RightHandIKPoint") rightHand = t;
                if (t.name == "LeftHandIKPoint") leftHand = t;
            }

            _weaponIKPoints[kvp.Key] = (rightHand, leftHand);
        }
    }

    public (Transform rightHand, Transform leftHand) GetWeaponIKPoints(WeaponType weaponType)
    {
        if (_weaponIKPoints.TryGetValue(weaponType, out var points))
            return points;
        return (null, null);
    }

    public System.Threading.Tasks.TaskCompletionSource<bool> FireCrossbowArrow(Vector3 targetPosition)
    {
        return _crossbowEffectController?.FireArrow(targetPosition);
    }

    public void PlayGunFireEffect(Vector3 targetPosition)
    {
        _gunEffectController?.PlayFireEffect(targetPosition);
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

    public void SetCrossbowIdlePose()
    {
        if (!_weaponCloakEffects.TryGetValue(WeaponType.Crossbow, out WeaponCloakEffect cloakEffect)) return;

        cloakEffect.transform.localPosition = _crossbowIdlePosition;
        cloakEffect.transform.localRotation = Quaternion.Euler(_crossbowIdleRotation);
    }

    public void SetCrossbowAttackPose()
    {
        if (!_weaponCloakEffects.TryGetValue(WeaponType.Crossbow, out WeaponCloakEffect cloakEffect)) return;

        cloakEffect.transform.localPosition = _crossbowAttackPosition;
        cloakEffect.transform.localRotation = Quaternion.Euler(_crossbowAttackRotation);
    }
}

}
