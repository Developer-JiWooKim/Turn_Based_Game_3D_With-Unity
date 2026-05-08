using UnityEngine;

public class PlayerUnitView : UnitView
{
    protected override void OnAwake()
    {
        _unitAnimator   = GetComponentInChildren<UnitAnimator>();
        _unitHUD        = GetComponentInChildren<UnitHUD>();
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
