using UnityEngine;
using UnityEngine.UIElements;

public class TitleSceneController : MonoBehaviour
{
    private UIDocument _uiDocument;

    private Button _startBtn;
    private Button _optionBtn;
    private Button _quitBtn;

    private VisualElement _modeSelectPanel;
    private Button _normalBtn;
    private Button _challengeBtn;
    private Button _modeBackBtn;

    private void Awake() => Initialize();
    private void Initialize()
    {
        _uiDocument = GetComponent<UIDocument>();
        var root = _uiDocument.rootVisualElement;

        _startBtn = root.Q<Button>("start-btn");
        _optionBtn = root.Q<Button>("option-btn");
        _quitBtn = root.Q<Button>("quit-btn");

        _startBtn.clicked += OnStartButtonClicked;
        _optionBtn.clicked += OnOptionButtonClicked;
        _quitBtn.clicked += OnQuitButtonClicked;

        _modeSelectPanel = root.Q<VisualElement>("mode-select-panel");
        _normalBtn = root.Q<Button>("normal-btn");
        _challengeBtn = root.Q<Button>("challenge-btn");
        _modeBackBtn = root.Q<Button>("mode-back-btn");

        _normalBtn.clicked += OnNormalButtonClicked;
        _challengeBtn.clicked += OnChallengeButtonClicked;
        _modeBackBtn.clicked += OnModeBackButtonClicked;
    }

    private void OnDestroy() => UnSubscribeButtonEvent();
    private void UnSubscribeButtonEvent()
    {
        _startBtn.clicked -= OnStartButtonClicked;
        _optionBtn.clicked -= OnOptionButtonClicked;
        _quitBtn.clicked -= OnQuitButtonClicked;

        _normalBtn.clicked -= OnNormalButtonClicked;
        _challengeBtn.clicked -= OnChallengeButtonClicked;
        _modeBackBtn.clicked -= OnModeBackButtonClicked;
    }


    private void OnStartButtonClicked()
    {
        _modeSelectPanel.style.display = DisplayStyle.Flex;
    }
    private void OnNormalButtonClicked()
    {
        StageManager.Instance.SetGameMode(GameMode.Normal);
        GameManager.Instance.LoadScene("WeaponSelectScene");
    }

    private void OnChallengeButtonClicked()
    {
        StageManager.Instance.SetGameMode(GameMode.Challenge);
        GameManager.Instance.LoadScene("WeaponSelectScene");
    }

    private void OnModeBackButtonClicked()
    {
        _modeSelectPanel.style.display = DisplayStyle.None;
    }

    private void OnOptionButtonClicked()
    {
        Debug.Log("옵션");
    }

    private void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}