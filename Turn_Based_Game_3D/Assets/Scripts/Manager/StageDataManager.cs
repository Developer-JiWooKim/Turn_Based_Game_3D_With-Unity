using UnityEngine;

public class StageDataManager : MonoBehaviour
{
    private static StageDataManager _instance;
    public static StageDataManager Instance => _instance;

    [SerializeField] private StageData[] _stageDatas;

    private int _currentStageIndex;

    public StageData CurrentStageData => _stageDatas[_currentStageIndex];

    public int CurrentStageIndex => _currentStageIndex;

    private void Awake() => Initialize();

    private void Initialize()
    {
        if (_instance == null)
        {
            _instance = this;
            _currentStageIndex = 0;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void NextStage()
    {
        _currentStageIndex++;
    }

    public void ResetStage()
    {
        _currentStageIndex = 0;
    }

    public bool IsLastStage()
    {
        return _currentStageIndex >= _stageDatas.Length;
    }
}