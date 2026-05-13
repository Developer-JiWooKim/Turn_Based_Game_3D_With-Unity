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
        Debug.Log($"{gameObject.name}의 공격 애니메이션.");
        _animator?.SetTrigger("Hit");
    }

    public void PlayDeathAnim()
    {
        Debug.Log($"{gameObject.name} 이 죽는 애니메이션 재생.");
        _animator?.SetTrigger("Death");
    }

    public void PlaySkillAnim(string skillName)
    {
        Debug.Log($"{skillName} 공격 애니메이션 실행");
        _animator?.SetTrigger(skillName);
    }
}
