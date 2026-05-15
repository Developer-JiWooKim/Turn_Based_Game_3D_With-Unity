using UnityEngine;

public enum GameMode
{
    Normal,
    Challenge
}

public class StageManager : MonoBehaviour
{
    private static StageManager _instance;
    public static StageManager Instance => _instance;

    [Header("Normal Mode")]
    [SerializeField] private StageData[] _stageDatas; // 1~n 스테이지 고정 데이터

    [Header("Challenge Mode - Stat Scaling")]
    [SerializeField] private int _hpIncreasePerStage = 20;
    [SerializeField] private int _atkIncreasePerStage = 5;
    [SerializeField] private int _spdIncreasePerStage = 1;

    private int      _currentStageLevel = 1;
    private GameMode _currentGameMode   = GameMode.Normal;

    public int      CurrentStageLevel => _currentStageLevel;
    public GameMode CurrentGameMode   => _currentGameMode;

    // 노멀 모드에서 현재 스테이지 데이터
    public StageData CurrentStageData =>
        _currentGameMode == GameMode.Normal ? _stageDatas[_currentStageLevel - 1] : null;

    public bool IsLastStage =>
        _currentGameMode == GameMode.Normal && _currentStageLevel >= _stageDatas.Length;


    private void Awake() => Initialize();

    private void Initialize()
    {
        if (_instance == null)
        {
            _instance = this;
            _currentStageLevel = 1;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetGameMode(GameMode mode)
    {
        _currentGameMode = mode;
        _currentStageLevel = 1;
    }

    public void NextStage() => _currentStageLevel++;

    public void ResetStage() => _currentStageLevel = 1;


    // 챌린지 모드 스탯 스케일링
    public int GetScaledHp(int baseHp)
        => baseHp + (_currentStageLevel - 1) * _hpIncreasePerStage;

    public int GetScaledAtk(int baseAtk)
        => baseAtk + (_currentStageLevel - 1) * _atkIncreasePerStage;

    public int GetScaledSpd(int baseSpd)
        => baseSpd + (_currentStageLevel - 1) * _spdIncreasePerStage;
}