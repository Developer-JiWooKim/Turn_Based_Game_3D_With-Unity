using UnityEngine;

public class PlayerAnimator : UnitAnimator
{
    private int GetWeaponLayerIndex(WeaponType weaponType)
    {
        return _animator.GetLayerIndex($"{weaponType} Layer");
    }

    public override async void PlayAttackAnim(WeaponType weaponType)
    {
        int layerIndex = GetWeaponLayerIndex(weaponType);

        // 서서히 Sword Layer 활성화
        await FadeLayerWeight(layerIndex, 0f, 1f, 0.1f);

        _animator.SetTrigger($"{weaponType}Attack");

        await Awaitable.WaitForSecondsAsync(GetAnimationLength($"{weaponType}Attack"));

        // 서서히 Sword Layer 비활성화
        await FadeLayerWeight(layerIndex, 1f, 0f, 0.3f);
    }

    private async Awaitable FadeLayerWeight(int layerIndex, float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _animator.SetLayerWeight(layerIndex, Mathf.Lerp(from, to, t));
            await Awaitable.NextFrameAsync();
        }

        _animator.SetLayerWeight(layerIndex, to);
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
