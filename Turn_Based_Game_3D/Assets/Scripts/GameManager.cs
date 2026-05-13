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

    private void Start()
    {
        // TODO#: 나중에 타이틀 씬에서 시작하도록 변경
        // 임시 테스트용 바로 배틀씬 로드


        Debug.Log("GameManager Start 호출");

        //LoadScene("BattleScene");
    }

    public void LoadScene(string sceneName)
    {
        // await FadeController.Instance.FadeOutAsync();
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //if (FadeController.Instance == null) return;
        //_ = FadeInAfterSceneLoaded();
    }

    private async Awaitable FadeInAfterSceneLoaded()
    {
        // 몇 프레임 기다려보기
        for (int i = 0; i < 5; i++)
        {
            await Awaitable.NextFrameAsync();
        }
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
