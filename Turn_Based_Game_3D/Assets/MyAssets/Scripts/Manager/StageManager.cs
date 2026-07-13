using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;

namespace Assets.MyAssets.Scripts.Manager
{

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

    [Header("Challenge Mode - Stat Scaling")] // #TODO: 현재 기획이 안되어있어서 일단 하드코딩
    [SerializeField] private int _hpIncreasePerStage = 20;
    [SerializeField] private int _atkIncreasePerStage = 5;
    [SerializeField] private int _spdIncreasePerStage = 1;

    private int      _currentStageLevel = 1;
    private GameMode _currentGameMode   = GameMode.Normal;

    public int      CurrentStageLevel => _currentStageLevel;
    public GameMode CurrentGameMode   => _currentGameMode;

    // 노멀 모드에서 현재 스테이지 데이터
    public StageData CurrentStageData
    {
        // 인덱스 범위 초과 에러(IndexOutOfRangeException) 예방
        get
        {
            if (_currentGameMode == GameMode.Normal && _currentStageLevel > 0 && _currentStageLevel <= _stageDatas.Length)
            {
                return _stageDatas[_currentStageLevel - 1];
            }
            return null;
        }
    }
        

    public bool IsLastStage => _currentGameMode == GameMode.Normal && _currentStageLevel >= _stageDatas.Length;


    private void Awake() => Initialize();

    private void Initialize()
    {
        if (_instance != null && _instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }

        _instance = this;
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

}