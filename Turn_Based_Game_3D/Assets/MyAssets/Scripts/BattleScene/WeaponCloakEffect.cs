using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class WeaponCloakEffect : MonoBehaviour
{
    [SerializeField] private float _uncloakDuration = 1f; // 클로킹 해제 시간
    [SerializeField] private float _cloakDuration   = 1f; // 클로킹 시간

    [SerializeField] private Material _cloakMaterial; // 클로킹 머테리얼

    private Renderer[] _renderers;

    private Material[][] _originalMaterials; // 원본 머테리얼 저장

    private static readonly int CloakAmount     = Shader.PropertyToID("_CloakAmount");
    private static readonly int FlashIntensity  = Shader.PropertyToID("_FlashIntensity");

    public WeaponType WeaponType { get; private set; }


    private void Awake() => CacheRenderers();
    private void CacheRenderers()
    {
        _renderers = GetComponentsInChildren<Renderer>();

        // 원본 머테리얼 저장
        _originalMaterials = new Material[_renderers.Length][];
        for (int i = 0; i < _renderers.Length; i++)
            _originalMaterials[i] = _renderers[i].materials;
    }
    public void Initialize(WeaponType weaponType)
    {
        WeaponType = weaponType;
    }

    private async Awaitable FlashAsync()
    {
        float duration = 0.1f;
        int flashCount = 3; // 번쩍 횟수

        for (int i = 0; i < flashCount; i++)
        {
            SetFlashIntensity(3f); // 번쩍
            await Awaitable.WaitForSecondsAsync(duration);
            SetFlashIntensity(0f); // 꺼짐
            await Awaitable.WaitForSecondsAsync(duration);
        }
    }

    private void SetFlashIntensity(float value)
    {
        foreach (var renderer in _renderers)
            foreach (var mat in renderer.materials)
                mat.SetFloat(FlashIntensity, value);
    }

    public async Awaitable UncloakAsync()
    {
        // 클로킹 머테리얼로 교체
        SetCloakMaterials();
        SetCloakAmount(0f);

        _ = FlashAsync(); // 번쩍 효과 동시 실행

        await AnimateCloakAsync(0f, 1f, _uncloakDuration);

        // 원본 머테리얼로 교체
        SetOriginalMaterials();
    }

    public async Awaitable CloakAsync()
    {
        // 클로킹 머테리얼로 교체
        SetCloakMaterials();
        SetCloakAmount(1f);

        _ = FlashAsync(); // 번쩍 효과 동시 실행
        await AnimateCloakAsync(1f, 0f, _cloakDuration);
    }

    private void SetCloakMaterials()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            Material[] original = _originalMaterials[i];
            Material[] combined = new Material[original.Length + 1];

            for (int j = 0; j < original.Length; j++)
                combined[j] = original[j];

            combined[original.Length] = _cloakMaterial; // 클로킹 머테리얼 추가

            _renderers[i].materials = combined;
        }
    }

    private void SetOriginalMaterials()
    {
        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].materials = _originalMaterials[i];
    }

    private async Awaitable AnimateCloakAsync(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetCloakAmount(Mathf.Lerp(from, to, t));
            await Awaitable.NextFrameAsync();
        }

        SetCloakAmount(to);
    }

    private void SetCloakAmount(float value)
    {
        foreach (var renderer in _renderers)
            foreach (var mat in renderer.materials)
                mat.SetFloat(CloakAmount, value);
    }

    public void CloakInstant()
    {
        foreach (var renderer in _renderers)
            renderer.enabled = false;
    }
}

}
