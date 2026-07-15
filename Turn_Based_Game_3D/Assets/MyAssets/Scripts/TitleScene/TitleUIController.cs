using UnityEngine;
using UnityEngine.UIElements;
using Assets.MyAssets.Scripts.Manager;

namespace Assets.MyAssets.Scripts.TitleScene
{

    public class TitleUIController : MonoBehaviour
    {
        private UIDocument _uiDocument;

        private Button _startBtn;
        private Button _optionBtn;
        private Button _quitBtn;

        private void Awake() => Initialize();
        private void Initialize()
        {
            _uiDocument = GetComponent<UIDocument>();
            VisualElement root = _uiDocument.rootVisualElement;
            if (root == null)
            {
                Debug.LogError("TitleUIController-Initialize(): VisualElement root is null");
                return;
            }

            _startBtn = root.Q<Button>("start-btn");
            _optionBtn = root.Q<Button>("option-btn");
            _quitBtn = root.Q<Button>("quit-btn");

            // Subscribe Button event
            _startBtn.clicked += OnStartButtonClicked;
            _optionBtn.clicked += OnOptionButtonClicked;
            _quitBtn.clicked += OnQuitButtonClicked;
        }

        private void OnDestroy()
        {
            // Unsubscribe Button event
            _startBtn.clicked -= OnStartButtonClicked;
            _optionBtn.clicked -= OnOptionButtonClicked;
            _quitBtn.clicked -= OnQuitButtonClicked;
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
