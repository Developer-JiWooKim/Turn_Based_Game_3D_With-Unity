using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;

    [SerializeField] private StageData[]    _stageDatas;
    [SerializeField] private CharacterData  _playerData;

    private int _currentStageIndex;

    public CharacterData PlayerData => _playerData;
    public StageData CurrentStageData => _stageDatas[_currentStageIndex];

    public event Action OnGameClear;
    public event Action OnGameOver;

    private void Awake() => Initialize();

    private void Initialize()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            _currentStageIndex = 0;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public async void LoadScene(string sceneName)
    {
        Debug.Log("페이드 아웃 시작");
        await FadeController.Instance.FadeOutAsync();
        Debug.Log("페이드 아웃 완료 - 씬 전환");
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (FadeController.Instance == null) return;

        if (scene.name == "TitleScene") return;

        _ = FadeInAfterSceneLoaded();
    }

    private async Awaitable FadeInAfterSceneLoaded()
    {
        await Awaitable.NextFrameAsync();

        await FadeController.Instance.FadeInAsync();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void NextStage()
    {
        _currentStageIndex++;

        if (_currentStageIndex < _stageDatas.Length)
        {
            LoadScene("BattleScene");
        }
        else
        {
            GameClear();
        }
    }

    private void GameClear()
    {
        OnGameClear?.Invoke();
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
    }

    public void ResetStage()
    {
        _currentStageIndex = 0;

        LoadScene("BattleScene");
    }
}
