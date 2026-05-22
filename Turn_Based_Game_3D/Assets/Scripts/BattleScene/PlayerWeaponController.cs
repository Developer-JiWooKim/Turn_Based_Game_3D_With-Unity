using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject _attackEffectPrefab;
    [SerializeField] private int        _effectPoolSize = 2;


    [SerializeField] private float        _toCameraPos = 0.5f;

    private Queue<GameObject> _effectPool = new Queue<GameObject>();

    private Dictionary<WeaponType, WeaponCloakEffect> _weaponCloakEffects = new Dictionary<WeaponType, WeaponCloakEffect>();


    private void Awake() => InitHitEffectPool();
    private void InitHitEffectPool()
    {
        if (_attackEffectPrefab == null) return;

        for (int i = 0; i < _effectPoolSize; i++)
        {
            GameObject obj = Instantiate(_attackEffectPrefab);
            obj.SetActive(false);
            _effectPool.Enqueue(obj);
        }
    }

    public void SpawnAttackEffect(Vector3 position)
    {
        if (_effectPool.Count == 0) return;

        // 카메라 방향으로 약간 앞으로
        Vector3 dirToCamera = (Camera.main.transform.position - position).normalized;
        position += dirToCamera * _toCameraPos;

        GameObject effect = _effectPool.Dequeue();
        effect.transform.position = position;
        effect.SetActive(true);

        _ = ReturnEffectToPool(effect);
    }

    private async Awaitable ReturnEffectToPool(GameObject effect)
    {
        while (effect.activeSelf)
        {
            await Awaitable.NextFrameAsync(destroyCancellationToken);
        }

        _effectPool.Enqueue(effect);
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
