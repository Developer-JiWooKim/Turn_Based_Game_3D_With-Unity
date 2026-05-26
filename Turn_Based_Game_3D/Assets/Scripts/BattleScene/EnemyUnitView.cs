using UnityEngine;
using UnityEngine.Pool;

public class EnemyUnitView : UnitView
{
    [SerializeField] private GameObject _hitEffectPrefab;
    [SerializeField] private float      _toCameraPos = 2f;

    private HitEffectPool _hitEffectPool;

    // TargetOutline 효과를 주기 위한 레이어
    private int _enemyLayer;
    private int _targetEnemyLayer;

    protected override void OnAwake()
    {
        _unitAnimator = GetComponent<UnitAnimator>();
        _unitHUD      = GetComponentInChildren<UnitHUD>();

        _enemyLayer       = LayerMask.NameToLayer("Enemy");
        _targetEnemyLayer = LayerMask.NameToLayer("TargetEnemy");

        InitHitEffectPool();
    }

    private void InitHitEffectPool()
    {
        if (_hitEffectPrefab == null) return;

        _hitEffectPool = new HitEffectPool(_hitEffectPrefab, _toCameraPos);
    }

    public void SpawnHitEffect(Vector3 position)
    {
        _hitEffectPool?.Spawn(position);
    }

    public void SetAsTarget(bool isTarget)
    {
        SetLayerRecursively(gameObject, isTarget ? _targetEnemyLayer : _enemyLayer);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    public async Awaitable PlayAttackAnimAsync(Transform target, System.Func<Awaitable> onHitCallback)
    {
        EnemyAnimator enemyAnimator = _unitAnimator as EnemyAnimator;
        if (enemyAnimator != null)
        {
            enemyAnimator.SetAttackHitCallback(onHitCallback);

            await enemyAnimator.PlayAttackAnimAsync(target);

            enemyAnimator.SetAttackHitCallback(null);
        }
    }

    public override void Reset()
    {
        base.Reset();

        // 아웃라인 초기화 (Enemy 레이어로 복구)
        SetLayerRecursively(gameObject, _enemyLayer);
    }

    public async Awaitable PlaySpawnAnimAsync()
    {
        EnemyAnimator enemyAnimator = _unitAnimator as EnemyAnimator;
        if (enemyAnimator != null)
        {
            await enemyAnimator.PlaySpawnAnimAsync();
        }            
    }
}
