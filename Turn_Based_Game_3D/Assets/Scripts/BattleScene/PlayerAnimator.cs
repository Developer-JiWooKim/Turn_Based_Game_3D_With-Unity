using UnityEngine;

public class PlayerAnimator : UnitAnimator
{

    public override void PlayAttackAnim(WeaponType weaponType)
    {
        Debug.Log($"PlayAttackAnim 호출됨 - WeaponType: {weaponType}");
        switch (weaponType)
        {
            case WeaponType.None:
                break;
            case WeaponType.Gun:
                _animator?.SetTrigger("GunAttack");
                break;
            case WeaponType.Sword:
                Debug.Log("SwordAttack 트리거 실행");
                _animator?.SetTrigger("SwordAttack");
                break;
            case WeaponType.Crossbow:
                _animator?.SetTrigger("CrossbowAttack");
                break;
            case WeaponType.Drone:
                _animator?.SetTrigger("DroneAttack");
                break;
        }
    }
}
