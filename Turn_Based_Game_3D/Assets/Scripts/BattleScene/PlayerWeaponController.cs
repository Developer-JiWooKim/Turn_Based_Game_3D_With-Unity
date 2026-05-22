using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private float      _toCameraPos = 2f;

    private Dictionary<WeaponType, WeaponCloakEffect> _weaponCloakEffects = new Dictionary<WeaponType, WeaponCloakEffect>();

    private ObjectPool<GameObject> _hitEffectPool;

    private void Awake() => InitHitEffectPool();
    private void InitHitEffectPool()
    {
        if (_hitEffectPrefab == null) return;

        _hitEffectPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(_hitEffectPrefab),
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            defaultCapacity: 3
        );
    }

    public void SpawnHitEffect(Vector3 position)
    {
        if (_hitEffectPool == null) return;

        // 카메라 방향으로 약간 앞으로
        Vector3 dirToCamera = (Camera.main.transform.position - position).normalized;
        position += dirToCamera * _toCameraPos;

        GameObject effect = _hitEffectPool.Get();
        effect.transform.position = position;

        _ = ReturnEffectToPool(effect);
    }

    private async Awaitable ReturnEffectToPool(GameObject effect)
    {
        while (effect.activeSelf)
        {
            await Awaitable.NextFrameAsync(destroyCancellationToken);
        }

        _hitEffectPool.Release(effect);
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
