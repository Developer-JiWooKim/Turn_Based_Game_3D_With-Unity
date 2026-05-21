using System.Threading;
using DG.Tweening;
using UnityEngine;

public class EnemyAnimator : UnitAnimator
{
    [SerializeField] private float _moveSpeed    = 5f;
    [SerializeField] private float _attackOffset = 2.5f;

    private System.Func<Awaitable>  _onAttackHit;

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

        _animator.SetTrigger("Walk");

        // 타겟 앞으로 이동
        await transform.DOMove(attackPosition, moveDuration)
            .SetEase(Ease.InQuad)
            .AsyncWaitForCompletion();

        _animator.SetTrigger("Attack");
        await Awaitable.WaitForSecondsAsync(GetAnimationLength("Attack"), destroyCancellationToken);

        // Jump 애니메이션 + DOJump 동시에
        float jumpDistance = Vector3.Distance(transform.position, originPosition);
        float jumpDuration = 0.2f;

        _animator.SetTrigger("Jump");

        transform.DOJump(originPosition, 1f, 1, jumpDuration)
            .SetEase(Ease.OutQuad);

        transform.DORotateQuaternion(originRotation, jumpDuration)
            .SetEase(Ease.OutQuad);

        await Awaitable.WaitForSecondsAsync(GetAnimationLength("Landing"), destroyCancellationToken);
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

    public void StartIdleRoarLoop()
    {
        _animator?.SetBool("IsPlayerTurn", true);
    }

    public void StopIdleRoarLoop()
    {
        _animator?.SetBool("IsPlayerTurn", false);
    }

    public async Awaitable PlaySpawnAnimAsync()
    {
        if (_animator == null) return;

        await Awaitable.WaitForSecondsAsync(GetAnimationLength("Spawned"), destroyCancellationToken);
    }

}