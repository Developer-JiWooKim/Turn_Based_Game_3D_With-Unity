using UnityEngine;
using UnityEngine.UIElements;

public class TitleSceneController : MonoBehaviour
{
    private UIDocument _uiDocument;

    private Button _startBtn;
    private Button _optionBtn;
    private Button _quitBtn;

    private void Awake() => Initialize();

    private void Initialize()
    {
        _uiDocument = GetComponent<UIDocument>();

        var root = _uiDocument.rootVisualElement;

        _startBtn  = root.Q<Button>("start-btn");
        _optionBtn = root.Q<Button>("option-btn");
        _quitBtn   = root.Q<Button>("quit-btn");

        _startBtn.clicked  += OnStartButtonClicked;
        _optionBtn.clicked += OnOptionButtonClicked;
        _quitBtn.clicked   += OnQuitButtonClicked;
    }



    private void OnDestroy()
    {
        _startBtn.clicked -= OnStartButtonClicked;
        _optionBtn.clicked -= OnOptionButtonClicked;
        _quitBtn.clicked -= OnQuitButtonClicked;
    }


    private void OnStartButtonClicked()
    {
        // TODO#: 나중에 무기 선택 씬으로 변경
        GameManager.Instance.LoadScene("BattleScene");
    }

    private void OnOptionButtonClicked()
    {
        // TODO#: 옵션 씬 또는 패널 구현 예정
        Debug.Log("옵션");
    }

    private void OnQuitButtonClicked()
    {
        _uiDocument.enabled = false;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}