using UnityEngine;
using Assets.MyAssets.Scripts.Manager;

namespace Assets.MyAssets.Scripts.TitleScene
{
    public class TitleUIController : MonoBehaviour
    {
        [SerializeField] private TitleUI _titleUI;

        private void Start() => Initialize();
        private void Initialize()
        {
            if (_titleUI == null)
            {
                Debug.Log("TitleUIController-Initialize(): _titleUI is null");
                return;
            }

            _titleUI.OnStartClicked += OnStartButtonClicked;
            _titleUI.OnOptionClicked += OnOptionButtonClicked;
            _titleUI.OnQuitClicked += OnQuitButtonClicked;
        }

        private void OnDestroy()
        {
            if (_titleUI == null) return;

            _titleUI.OnStartClicked -= OnStartButtonClicked;
            _titleUI.OnOptionClicked -= OnOptionButtonClicked;
            _titleUI.OnQuitClicked -= OnQuitButtonClicked;
        }

        private void OnStartButtonClicked()
        {
            GameManager.Instance.LoadScene("SettingScene");
        }

        private void OnOptionButtonClicked()
        {
            Debug.Log("옵션");
        }

        private void OnQuitButtonClicked()
        {
            GameManager.Instance.GameQuit();
        }
    }
}
