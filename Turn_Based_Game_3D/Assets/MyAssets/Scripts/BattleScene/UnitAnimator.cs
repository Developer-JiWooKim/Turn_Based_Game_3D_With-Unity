using UnityEngine;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class UnitAnimator : MonoBehaviour
{
    protected Animator _animator;

    private void Awake() => Initialize();

    protected virtual void Initialize()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.Log($"{gameObject.name} 에 Animator 가 없습니다.");
        }
    }

    public virtual async Awaitable PlayDeathAnimAsync()
    {
        _animator?.SetTrigger("Death");
        await Awaitable.WaitForSecondsAsync(GetAnimationLength("Death"), destroyCancellationToken);
    }

    public virtual async Awaitable PlayHitAnimAsync()
    {
        _animator?.SetTrigger("Hit");
        await Awaitable.WaitForSecondsAsync(GetAnimationLength("Hit"), destroyCancellationToken);
    }

    protected float GetAnimationLength(string animName)
    {
        if (_animator == null) return 1f;
        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animName)
                return clip.length;
        }
        return 1f;
    }
}

}
