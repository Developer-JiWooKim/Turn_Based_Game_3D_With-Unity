using UnityEngine;

namespace Assets.MyAssets.Scripts.Utility
{
    public class FadeScreenEffect : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeOutDuration = .5f;
        [SerializeField] private float _fadeInDuration = 1f;

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }

        public async Awaitable FadeOutAsync()
        {
            await FadeAsync(0f, 1f, _fadeOutDuration);
        }

        public async Awaitable FadeInAsync()
        {
            await FadeAsync(1f, 0f, _fadeInDuration);
        }

        private async Awaitable FadeAsync(float from, float to, float duration)
        {
            float elapsed = 0f; // 시간에 따른 투명도 값?, 초기값 0

            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = from;

            while (elapsed < duration) // 시간에 따라 변하는 투명도가 원하는 페이드 시간에 도달할때까지
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                _canvasGroup.alpha = Mathf.Lerp(from, to, t);
                await Awaitable.NextFrameAsync();
            }

            _canvasGroup.alpha = to;

            // fade효과가 끝나면 해당 UI 비활성화
            if (to == 0f)
            {
                _canvasGroup.blocksRaycasts = false;
            }
        }
    }
}
