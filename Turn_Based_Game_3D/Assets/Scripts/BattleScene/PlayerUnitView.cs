using UnityEngine;

public class PlayerUnitView : UnitView
{
    protected override void OnAwake()
    {
        _unitAnimator       = GetComponent<UnitAnimator>();
        _unitHUD            = GetComponentInChildren<UnitHUD>();
    }

    protected override void PlayDeathAnim()
    {
        _unitAnimator?.PlayDeathAnim();
    }

    protected override void PlayHitAnim()
    {
        _unitAnimator?.PlayHitAnim();
    }

    public async Awaitable PlayAttackAnimAsync(int weaponIndex)
    {
        PlayerBattleUnit player = _linkedUnit as PlayerBattleUnit;
        if (player == null) return;

        WeaponType weaponType = player.Weapons[weaponIndex].weaponType;
        await (_unitAnimator as PlayerAnimator)?.PlayAttackAnimAsync(weaponType);
    }
}
