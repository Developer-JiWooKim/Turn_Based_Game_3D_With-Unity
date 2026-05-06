using UnityEngine;

public class EnemyUnitView : UnitView
{
    protected override void OnAwake()
    {
        _unitAnimator = GetComponentInChildren<UnitAnimator>();
        _unitHUD = GetComponentInChildren<UnitHUD>();
    }

    private void Start()
    {
        BattleUnitManager.Instance.EnemyViewRegister(this);
    }

    protected override void PlayDeathAnim()
    {
        _unitAnimator?.PlayDeathAnim();
    }

    protected override void PlayHitAnim()
    {
        _unitAnimator?.PlayHitAnim();
    }
}
