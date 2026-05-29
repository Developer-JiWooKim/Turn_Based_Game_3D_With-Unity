using UnityEngine;
using UnityEngine.Pool;

public class CrossbowEffectController : MonoBehaviour
{
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private Transform  _muzzlePoint;
    [SerializeField] private int        _poolDefaultCapacity = 3;

    private ObjectPool<ArrowProjectile> _arrowPool;

    private void Awake() => Initialize();
    private void Initialize()
    {
        _arrowPool = new ObjectPool<ArrowProjectile>(
            createFunc: () => Instantiate(_arrowPrefab).GetComponent<ArrowProjectile>(),
            actionOnGet: arrow => arrow.gameObject.SetActive(true),
            actionOnRelease: arrow => arrow.gameObject.SetActive(false),
            actionOnDestroy: arrow => Destroy(arrow.gameObject),
            defaultCapacity: _poolDefaultCapacity
        );
    }

    public System.Threading.Tasks.TaskCompletionSource<bool> FireArrow(Vector3 targetPosition)
    {
        System.Threading.Tasks.TaskCompletionSource<bool> hitCompletionSource = new System.Threading.Tasks.TaskCompletionSource<bool>();

        ArrowProjectile arrow = _arrowPool.Get();
        arrow.transform.position = _muzzlePoint.position;
        arrow.transform.rotation = Quaternion.identity;
        arrow.Launch(targetPosition, hitCompletionSource, () => _arrowPool.Release(arrow));

        return hitCompletionSource;
    }
}
