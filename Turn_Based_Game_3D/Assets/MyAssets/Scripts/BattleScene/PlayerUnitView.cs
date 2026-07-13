using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class PlayerUnitView : UnitView
{
    protected override void OnAwake()
    {
        _unitAnimator = GetComponent<UnitAnimator>();
        _unitHUD      = GetComponentInChildren<UnitHUD>();
    }

    public async Awaitable PlayAttackAnimAsync(int weaponIndex, Transform target = null)
    {
        PlayerBattleUnit player = _linkedUnit as PlayerBattleUnit;
        if (player == null) return;

        WeaponData weapon = player.Weapons[weaponIndex];
        // WeaponType weaponType = player.Weapons[weaponIndex].weaponType; TODO#: 지울예정
        //await (_unitAnimator as PlayerAnimator)?.PlayAttackAnimAsync(weaponType, target);
        await (_unitAnimator as PlayerAnimator)?.PlayAttackAnimAsync(weapon.weaponType, weapon.rangeType, target);
    }
}

}
