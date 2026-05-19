using UnityEngine;

public class EnemyAnimator : UnitAnimator
{
    public async Awaitable PlayAttackAnimAsync()
    {
        if (_animator == null) return;

        _animator.SetTrigger("Attack");
        await Awaitable.WaitForSecondsAsync(GetAnimationLength("Attack"), destroyCancellationToken);
    }

    public override void PlayHitAnim()
    {
        _animator?.SetTrigger("Hit");
    }

    public override void PlayDeathAnim()
    {
        _animator?.SetTrigger("Death");
    }

    private float GetAnimationLength(string animName)
    {
        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animName)
                return clip.length;
        }
        return 1f;
    }
}