using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class HitEffectPool
{
    private readonly ObjectPool<GameObject> _pool;
    private readonly float                  _cameraOffset;

    public HitEffectPool(GameObject prefab, float cameraOffset = 2f, int defaultCapacity = 3)
    {
        _cameraOffset = cameraOffset;

        _pool = new ObjectPool<GameObject>(
            createFunc:       () => GameObject.Instantiate(prefab),
            actionOnGet:     obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => GameObject.Destroy(obj),
            defaultCapacity: defaultCapacity
        );
    }

    public async void Spawn(Vector3 position)
    {
        if (_pool == null) return;

        Vector3 dirToCamera = (Camera.main.transform.position - position).normalized;
        position += dirToCamera * _cameraOffset;

        GameObject effect = _pool.Get();
        effect.transform.position = position;

        while (effect.activeSelf)
        {
            await Awaitable.NextFrameAsync();
        }

        _pool.Release(effect);
    }
}
