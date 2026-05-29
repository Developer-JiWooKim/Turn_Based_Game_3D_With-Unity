using System;
using DG.Tweening;
using UnityEngine;

public class PlayerAnimator : UnitAnimator
{
    [SerializeField] private float _weaponIdleTransitionDuration = 0.5f;  // Idle 모션으로 전환 속도
    [SerializeField] private float _moveSpeed                    = 5f;    // 이동 속도
    [SerializeField] private float _attackOffset                 = 5f;    // 타겟으로부터의 거리
    [SerializeField] private float _hitStopTimeScale             = 0.05f; // 히트스탑 시 타임스케일
    [SerializeField] private float _hitStopDuration              = 0.15f; // 히트스탑 지속 시간

    private PlayerWeaponController  _weaponController;
    private Transform               _currentTarget;
    private float                   _jumpDuration = 0.5f;
    private Func<Awaitable>         _onAttackHit;

    private System.Threading.Tasks.TaskCompletionSource<bool> _crossbowHitSource;

    protected override void Initialize()
    {
        base.Initialize();
        _weaponController = GetComponent<PlayerWeaponController>();
    }

    public async Awaitable PlayAttackAnimAsync(WeaponType weaponType, WeaponRangeType rangeType, Transform target = null)
    {
        if (_animator == null) return;
        if (target == null)
        {
            Debug.Log("PlayAttackAnimAsync에서 Target이 null");
        }

        _currentTarget = target;

        try
        {
            Vector3 originPosition = transform.position;
            Quaternion originRotation = transform.rotation;

            int layerIndex = GetWeaponLayerIndex(weaponType);

            //#TODO:총뿐만 아니라 크로스 보우도 애니메이션에 따라 무기 위치가 바뀔 수 있음
            if (weaponType == WeaponType.Gun)
            {
                // Idle 포즈 설정
                _weaponController.SetGunIdlePose();
            }
            else if (weaponType == WeaponType.Crossbow)
            {
                _weaponController.SetCrossbowIdlePose();
            }

            // 무기 언클로킹 그 무기에 맞는 애니메이션 레이어로 전환을 동시에
            Awaitable uncloakTask = _weaponController?.UncloakWeapon(weaponType);
            Awaitable fadeTask = FadeLayerWeight(layerIndex, 0f, 1f, _weaponIdleTransitionDuration);

            await uncloakTask;
            await fadeTask;

            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            await transform.DORotateQuaternion(targetRotation, 0.2f)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion();

            if (rangeType == WeaponRangeType.Melee)
                await PlayMeleeAttackAsync(weaponType, target, originPosition, originRotation);
            else
                await PlayRangedAttackAsync(weaponType, originRotation);

            Awaitable cloakTask = _weaponController?.CloakWeapon(weaponType);
            Awaitable fadeOutTask = FadeLayerWeight(layerIndex, 1f, 0f, _weaponIdleTransitionDuration);

            await cloakTask;
            await fadeOutTask;

        }
        catch (OperationCanceledException)
        {
            Debug.Log("PlayAttackAnim 중 문제 발생");
        }
    }

    // 근접 무기 공격 연출
    private async Awaitable PlayMeleeAttackAsync(WeaponType weaponType, Transform target, Vector3 originPosition, Quaternion originRotation)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - direction * _attackOffset;
        attackPosition.y = originPosition.y;

        float distance = Vector3.Distance(transform.position, attackPosition);
        float moveDuration = distance / _moveSpeed;
        
        // 근접무기는 해당 위치로 달려가는 애니메이션을 반드시 넣고 그 애니메이션은 Sprint로 할거임
        _animator.SetTrigger("Sprint");

        await transform.DOMove(attackPosition, moveDuration)
                .SetEase(Ease.InQuad)
                .AsyncWaitForCompletion();

        await Awaitable.WaitForSecondsAsync(GetAnimationLength($"{weaponType}Attack"), destroyCancellationToken);

        // 원래 위치로 복귀
        _animator.SetTrigger("Jump");
        await Awaitable.WaitForSecondsAsync(0.5f, destroyCancellationToken);

        transform.DOJump(originPosition, 1f, 1, _jumpDuration).SetEase(Ease.OutQuad);
        transform.DORotateQuaternion(originRotation, 0.2f).SetEase(Ease.OutQuad);

        await Awaitable.WaitForSecondsAsync(_jumpDuration, destroyCancellationToken);

        // WeaponIdle 복귀 후 대기
        _animator.SetTrigger($"{weaponType}Idle");
        await Awaitable.WaitForSecondsAsync(GetAnimationLength($"{weaponType}Idle"), destroyCancellationToken);
    }

    private async Awaitable PlayRangedAttackAsync(WeaponType weaponType, Quaternion originRotation)
    {
        float clipLength = GetAnimationLength($"{weaponType}Attack");

        if (weaponType == WeaponType.Gun)
        {
            _weaponController?.SetGunAttackPose();

            _animator.SetTrigger($"{weaponType}Attack");
            await Awaitable.WaitForSecondsAsync(clipLength, destroyCancellationToken);

            _weaponController?.SetGunIdlePose();
        }
        else if (weaponType == WeaponType.Crossbow)
        {
            // TODO#: 화살 오브젝트 구현 시
            // 1. CrossbowAttack 트리거로 애니메이션 재생
            // 2. 화살 오브젝트 발사
            // 3. 화살이 적에게 명중하는 시점에 OnAttackHit() 호출
            // 4. 명중 완료 신호 받을 때까지 await 대기 (TaskCompletionSource 방식 예정)

            _weaponController?.SetCrossbowAttackPose();
            _animator.SetTrigger($"{weaponType}Attack");

            await Awaitable.WaitForSecondsAsync(clipLength, destroyCancellationToken);

            // 명중 시점에 타격 판정
            OnAttackHit();
            _weaponController?.SetCrossbowIdlePose();
        }

        transform.DORotateQuaternion(originRotation, 0.2f).SetEase(Ease.OutQuad);
        await Awaitable.WaitForSecondsAsync(_weaponIdleTransitionDuration, destroyCancellationToken);
    }

    public void OnCrossbowFire()
    {
        if (_currentTarget == null) return;

        _crossbowHitSource = _weaponController?.FireCrossbowArrow(_currentTarget.position);
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

    public async void OnAttackHit()
    {
        if (_currentTarget != null)
        {
            _weaponController?.SpawnHitEffects(_currentTarget.position);
        }

        _ = HitStopAsync(); // 히트 스탑 연출(검이 몬스터에게 타격됐을 때 히트 스탑 연출)

        if(_onAttackHit != null)
        {
            await _onAttackHit();
        }
    }
    public void OnGunFire()
    {
        if (_currentTarget == null) return;

        _weaponController?.PlayGunFireEffect(_currentTarget.position);
    }

    private async Awaitable HitStopAsync()
    {
        Time.timeScale = _hitStopTimeScale;

        await Awaitable.WaitForSecondsAsync(_hitStopDuration * Time.timeScale, destroyCancellationToken);

        Time.timeScale = 1f;
    }

    public void SetAttackHitCallback(Func<Awaitable> callback)
    {
        _onAttackHit = callback;
    }
}
