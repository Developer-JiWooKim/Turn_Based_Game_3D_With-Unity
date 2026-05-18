using UnityEngine;

public class EnemyAnimator : UnitAnimator
{
    public override void PlayAttackAnim(WeaponType weapon)
    {
        _animator?.SetTrigger("Attack");
    }
}