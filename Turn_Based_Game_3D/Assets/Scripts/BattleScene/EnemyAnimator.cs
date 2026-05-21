using System.Threading;
using DG.Tweening;
using UnityEngine;

public class EnemyAnimator : UnitAnimator
{
    [SerializeField] private float _moveSpeed    = 5f;
    [SerializeField] private float _attackOffset = 2.5f;

    private CancellationTokenSource _roarLoopCts;
    private System.Func<Awaitable>  _onAttackHit;

    bool _isRoaring = false;

    public async Awaitable PlayAttackAnimAsync(Transform target)
    {
        if (_animator == null) return;

        Vector3    originPosition = transform.position;
        Quaternion originRotation = transform.rotation;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 타겟 방향으로 회전
        await transform.DORotateQuaternion(targetRotation, 0.2f)
            .SetEase(Ease.OutQuad)
            .AsyncWaitForCompletion();

        Vector3 attackPosition = target.position - direction * _attackOffset;
        attackPosition.y = originPosition.y;

        float distance = Vector3.Distance(transform.position, attackPosition);
        float moveDuration = distance / _moveSpeed;

        // 타겟 앞으로 이동
        await transform.DOMove(attackPosition, moveDuration)
            .SetEase(Ease.InQuad)
            .AsyncWaitForCompletion();

        _animator.SetTrigger("Attack");
        await Awaitable.WaitForSecondsAsync(GetAnimationLength("Attack"), destroyCancellationToken);

        distance = Vector3.Distance(transform.position, originPosition);
        moveDuration = distance / _moveSpeed;

        //원래 위치로 복귀
        await transform.DOMove(originPosition, moveDuration)
            .SetEase(Ease.OutQuad)
            .AsyncWaitForCompletion();

        // 원래 회전값으로 복귀
        await transform.DORotateQuaternion(originRotation, 0.2f)
            .SetEase(Ease.OutQuad)
            .AsyncWaitForCompletion();
    }

    public void SetAttackHitCallback(System.Func<Awaitable> callback)
    {
        _onAttackHit = callback;
    }

    public async void OnAttackHit()
    {
        if (_onAttackHit != null)
            await _onAttackHit();
    }

    public async void StartIdleRoarLoop()
    {
        _isRoaring = true;

        // 부드럽게 Idle 처음부터 시작
        _animator?.CrossFade("Idle", 0.1f, 0, 0f);

        if (!_isRoaring) return;

        while (_isRoaring)
        {
            await Awaitable.WaitForSecondsAsync(GetAnimationLength("Idle"), destroyCancellationToken);
            if (!_isRoaring) break;

            _animator.SetTrigger("Roar");
            await Awaitable.WaitForSecondsAsync(GetAnimationLength("Roar"), destroyCancellationToken);
            if (!_isRoaring) break;
        }
    }

    public void StopIdleRoarLoop()
    {
        _isRoaring = false;
        _animator?.CrossFade("Idle", 0.1f); // 0.2초 동안 부드럽게 Idle로 전환
    }
}