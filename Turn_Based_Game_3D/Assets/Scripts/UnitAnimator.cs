using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInParent<Animator>();
        if ( _animator != null )
        {
            Debug.Log($"{gameObject.name} 에 Animator 가 없습니다.");
        }
    }

    public void PlayHitAnim()
    {
        _animator.SetTrigger("Hit");
    }

    public void PlayDeathAnim()
    {
        _animator.SetTrigger("Death");
    }

    public void PlaySkillAnim(string skillName)
    {
        _animator.SetTrigger(skillName);
    }
}
