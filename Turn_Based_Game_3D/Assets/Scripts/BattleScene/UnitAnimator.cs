using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    protected Animator _animator;

    private void Awake() => Initialize();

    protected virtual void Initialize()
    {
        _animator = GetComponentInParent<Animator>();
        if (_animator == null)
        {
            Debug.Log($"{gameObject.name} 에 Animator 가 없습니다.");
        }
    }

    public virtual void PlayHitAnim()
    {
        Debug.Log($"{gameObject.name}의 히트 애니메이션.");
        _animator?.SetTrigger("Hit");
    }

    public virtual void PlayDeathAnim()
    {
        Debug.Log($"{gameObject.name} 이 죽는 애니메이션 재생.");
        _animator?.SetTrigger("Death");
    }
}
