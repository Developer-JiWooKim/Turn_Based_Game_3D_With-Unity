using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;

    

    // TODO#: 나중에 DataManager로 분리 예정
    private WeaponData[] _selectedWeapons = new WeaponData[3];
    public WeaponData[] SelectedWeapons => _selectedWeapons;
    [SerializeField] private StageData[] _stageDatas;
    [SerializeField] private CharacterData _playerData;


    public void SelectWeapon(int slot, WeaponData weapon)
    {
        if (slot < 0 || slot >= 3) return;

        _selectedWeapons[slot] = weapon;
    }

    public void ClearSelectWeapons()
    {
        _selectedWeapons = new WeaponData[3];
    }

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
