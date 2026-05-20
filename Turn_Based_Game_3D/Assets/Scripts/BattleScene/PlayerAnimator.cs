using DG.Tweening;
using UnityEngine;

public class PlayerAnimator : UnitAnimator
{
    private PlayerWeaponController _weaponController;

    [SerializeField] private float _weaponIdleTransitionDuration = 0.5f; // Idle 모션으로 전환 속도

    [SerializeField] private float _moveSpeed                    = 5f;   // 이동 속도
    [SerializeField] private float _attackOffset                 = 5f; // 타겟으로부터의 거리


    protected override void Initialize()
    {
        base.Initialize();
        _weaponController = GetComponent<PlayerWeaponController>();
    }

    public async Awaitable PlayAttackAnimAsync(WeaponType weaponType, Transform target = null)
    {
        if (_animator == null) return;
        if (target == null)
        {
            Debug.Log("PlayAttackAnimAsync에서 Target이 null");
        }

        try
        {
            Vector3    originPosition = transform.position;
            Quaternion originRotation = transform.rotation;
            

            int layerIndex = GetWeaponLayerIndex(weaponType);

            // 무기 언클로킹 그 무기에 맞는 애니메이션 레이어로 전환을 동시에
            Awaitable uncloakTask = _weaponController?.UncloakWeapon(weaponType);
            Awaitable fadeTask = FadeLayerWeight(layerIndex, 0f, 1f, _weaponIdleTransitionDuration);

            await uncloakTask;
            await fadeTask;

            // 무기가 검이면 타겟 앞으로 이동 
            // TODO#: 현재는 근접무기가 검뿐이라 조건을 이렇게 했지만, 나중에 무기가 늘어나면 무기 타입에 근접, 원거리 타입 새로 넣어서 이걸로 이동 여부 정해야 됨
            if (weaponType == WeaponType.Sword && target != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                await transform.DORotateQuaternion(targetRotation, 0.2f)
                    .SetEase(Ease.OutQuad)
                    .AsyncWaitForCompletion();

                Vector3 attackPosition = target.position - direction * _attackOffset;
                attackPosition.y = originPosition.y;

                float distance = Vector3.Distance(transform.position, attackPosition);
                float moveDuration = distance / _moveSpeed;

                await transform.DOMove(attackPosition, moveDuration)
                    .SetEase(Ease.InQuad)
                    .AsyncWaitForCompletion();
            }

            _animator.SetTrigger($"{weaponType}Attack");

            // 공격애니메이션 끝날때까지 대기, 비동기 작업중 오브젝트가 파괴되면 실행중인 비동기 작업 취소
            await Awaitable.WaitForSecondsAsync(GetAnimationLength($"{weaponType}Attack"), destroyCancellationToken);

            // SwordIdle 전환 대기
            await Awaitable.WaitForSecondsAsync(GetAnimationLength($"{weaponType}Idle"), destroyCancellationToken);

            // 원래 위치로 복귀
            if (weaponType == WeaponType.Sword && target != null)
            {
                float distance = Vector3.Distance(transform.position, originPosition);
                float moveDuration = distance / _moveSpeed;

                await transform.DOMove(originPosition, moveDuration)
                    .SetEase(Ease.OutQuad)
                    .AsyncWaitForCompletion();

                await transform.DORotateQuaternion(originRotation, 0.2f)
                    .SetEase(Ease.OutQuad)
                    .AsyncWaitForCompletion();
            }

            // 무기 클로킹 완료까지 대기
            Awaitable cloakTask = _weaponController?.CloakWeapon(weaponType);
            fadeTask = FadeLayerWeight(layerIndex, 1f, 0f, _weaponIdleTransitionDuration); // 서서히 해당 무기 Layer 비활성화

            await cloakTask;
            await fadeTask;

        } catch (System.OperationCanceledException)
        {
            Debug.Log("PlayAttackAnim 중 문제 발생");
        }
    }

    private async Awaitable FadeLayerWeight(int layerIndex, float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _animator.SetLayerWeight(layerIndex, Mathf.Lerp(from, to, t));

            // 비동기 작업중 오브젝트가 파괴되면 실행중인 await 중인 작업 취소
            await Awaitable.NextFrameAsync(destroyCancellationToken);
        }

        _animator.SetLayerWeight(layerIndex, to);
    }

    private int GetWeaponLayerIndex(WeaponType weaponType)
    {
        return _animator.GetLayerIndex($"{weaponType} Layer");
    }
}
