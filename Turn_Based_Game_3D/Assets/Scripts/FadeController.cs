using UnityEngine;
using UnityEngine.UIElements;

public class FadeController : MonoBehaviour
{
    private static FadeController _instance;

    public static FadeController Instance => _instance;


    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private float      _fadeOutDuration = 0.5f;
    [SerializeField] private float      _fadeInDuration = 1f;

    private VisualElement _fadePanel;

    private void Awake() => Initialize();

    private void Initialize()
    {
        if (Instance == null)
        {
            _instance = this;

            _fadePanel = _uiDocument.rootVisualElement.Q<VisualElement>("fade-panel");

            SetOpacity(0f);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void SetOpacity(float opacity)
    {
        // 패널의 opacity(투명도) 조절
        _fadePanel.style.opacity = opacity;

        // opacity가 0이면 입력 차단 안 되도록
        _fadePanel.pickingMode = opacity == 0f ? PickingMode.Ignore : PickingMode.Position;
    }

    public async Awaitable FadeOutAsync()
    {
        await FadeAsync(0f, 1f, _fadeOutDuration); // FadeAsync 함수가 끝날때까지 기다림? 페이드 아웃이니 원본화면에서 검은 화면으로 바뀔듯?
    }

    public async Awaitable FadeInAsync()
    {
        await FadeAsync(1f, 0f, _fadeInDuration); // FadeOut 반대
    }

    private async Awaitable FadeAsync(float from, float to, float duration)
    {
        _fadePanel.style.display = DisplayStyle.Flex; // 검은 배경 활성화

        float elapsed = 0f; // 시간에 따른 투명도 값?, 초기값 0

        SetOpacity(from); // 초기(시작할) 투명도 값 설정

        while (elapsed < duration) // 시간에 따라 변하는 투명도가 원하는 페이드 시간에 도달할때까지
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration); // clamp01? 
            SetOpacity(Mathf.Lerp(from, to, t)); 

            await Awaitable.NextFrameAsync(); // ??
         }

        SetOpacity(to);

        // fade효과가 끝나면 해당 UI 비활성화
        if (to == 0f)
        {
            _fadePanel.style.display = DisplayStyle.None;
        }
    }
}
