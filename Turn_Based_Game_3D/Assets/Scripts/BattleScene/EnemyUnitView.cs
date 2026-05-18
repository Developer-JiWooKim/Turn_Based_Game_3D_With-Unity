using UnityEngine;

public class EnemyUnitView : UnitView
{
    // TargetOutline 효과를 주기 위한 레이어
    private int _enemyLayer;
    private int _targetEnemyLayer;

    protected override void OnAwake()
    {
        _unitAnimator = GetComponentInChildren<UnitAnimator>();
        _unitHUD      = GetComponentInChildren<UnitHUD>();

        _enemyLayer       = LayerMask.NameToLayer("Enemy");
        _targetEnemyLayer = LayerMask.NameToLayer("TargetEnemy");
    }

    protected override void PlayDeathAnim()
    {
        _unitAnimator?.PlayDeathAnim();
    }

    protected override void PlayHitAnim()
    {
        _unitAnimator?.PlayHitAnim();
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

    public override void Reset()
    {
        base.Reset();

        // 아웃라인 초기화 (Enemy 레이어로 복구)
        SetLayerRecursively(gameObject, _enemyLayer);
    }

    protected override void PlayAttackAnim(WeaponType weaponType)
    {
        //TODO#: 몬스터에 맞게 수정 필요
    }
}
