using UnityEngine;

public class PlayerAnimator : UnitAnimator
{
    private PlayerWeaponController _weaponController;

    private float _weaponIdleTransitionDuration = 0.5f;

    protected override void Initialize()
    {
        base.Initialize();
        _weaponController = GetComponent<PlayerWeaponController>();
    }

    public override async Awaitable PlayAttackAnimAsync(WeaponType weaponType)
    {
        if (_animator == null) return; // null 체크 추가

        try
        {
            int layerIndex = GetWeaponLayerIndex(weaponType);

            Awaitable uncloakTask = _weaponController?.UncloakWeapon(weaponType);
            Awaitable fadeTask    = FadeLayerWeight(layerIndex, 0f, 1f, _weaponIdleTransitionDuration);

            await uncloakTask;
            await fadeTask;

            _animator.SetTrigger($"{weaponType}Attack");

            // 공격애니메이션 끝날때까지 대기, 비동기 작업중 오브젝트가 파괴되면 실행중인 비동기 작업 취소
            await Awaitable.WaitForSecondsAsync(GetAnimationLength($"{weaponType}Attack"), destroyCancellationToken);

            // SwordIdle 전환 대기
            await Awaitable.WaitForSecondsAsync(GetAnimationLength($"{weaponType}Idle"), destroyCancellationToken);

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

    private float GetAnimationLength(string animName)
    {
        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animName)
            {
                return clip.length;
            }
        }
        return 1f;
    }
}
