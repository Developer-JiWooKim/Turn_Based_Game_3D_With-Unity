using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;

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
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public async void LoadScene(string sceneName)
    {
        await FadeController.Instance.FadeOutAsync();
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"OnSceneLoaded: {scene.name}");

        if (FadeController.Instance == null) return;

        Debug.Log("FadeIn 시작");

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
        StageManager.Instance.NextStage();

        if (StageManager.Instance.CurrentGameMode == GameMode.Normal
        && StageManager.Instance.IsLastStage)
        {
            GameClear();
        }
        else
        {
            LoadScene("BattleScene");
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
        StageManager.Instance.ResetStage();
        LoadScene("BattleScene");
    }
}
