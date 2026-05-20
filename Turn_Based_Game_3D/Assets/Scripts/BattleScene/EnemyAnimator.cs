using UnityEngine;

public class EnemyAnimator : UnitAnimator
{
    public async Awaitable PlayAttackAnimAsync()
    {
        if (_animator == null) return;

        _animator.SetTrigger("Attack");
        await Awaitable.WaitForSecondsAsync(GetAnimationLength("Attack"), destroyCancellationToken);
    }
}