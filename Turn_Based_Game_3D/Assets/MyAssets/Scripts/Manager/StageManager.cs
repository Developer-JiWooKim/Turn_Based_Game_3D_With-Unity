using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;

namespace Assets.MyAssets.Scripts.Manager
{
    public class StageManager : Singleton<StageManager>
    {
        [SerializeField] private StageData[] _stageDatas; // 1~n 스테이지 고정 데이터
        [SerializeField] private SpawnPatternPoolData _spawnPatternPool; // 고정 스테이지 이후 무한 진행용 패턴 풀

        private readonly System.Random _random = new System.Random();

        private int _currentStageLevel = 1;
        private StageData _resolvedStageData;

        private MetaProgressData _metaProgress;

        public int CurrentStageLevel => _currentStageLevel;

        public int BestStageReached => _metaProgress.BestStageReached;
        public int PermanentPoints => _metaProgress.PermanentPoints;

        public StageData CurrentStageData => _resolvedStageData;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Initialize()
        {
            _metaProgress = MetaProgressStorage.Load();
            _resolvedStageData = ResolveStageData();
        }

        public void NextStage()
        {
            bool isNewRecord = _currentStageLevel > _metaProgress.BestStageReached;

            if (isNewRecord)
            {
                _metaProgress.BestStageReached = _currentStageLevel;

                if (_currentStageLevel % 10 == 0)
                {
                    _metaProgress.PermanentPoints++;
                }

                MetaProgressStorage.Save(_metaProgress);
            }

            _currentStageLevel++;
            _resolvedStageData = ResolveStageData();
        }

        public void ResetStage()
        {
            _currentStageLevel = 1;
            _resolvedStageData = ResolveStageData();
        }

        // 고정 배치 스테이지를 넘어서면 스폰 패턴 풀에서 무작위로 골라 임시 StageData를 구성한다.
        private StageData ResolveStageData()
        {
            if (_currentStageLevel > 0 && _currentStageLevel <= _stageDatas.Length)
            {
                return _stageDatas[_currentStageLevel - 1];
            }

            bool isBossStage = _currentStageLevel % 5 == 0;
            var pool = isBossStage ? _spawnPatternPool.BossPatterns : _spawnPatternPool.NormalPatterns;

            if (pool == null || pool.Count == 0)
            {
                Debug.LogError("스폰 패턴 풀이 비어있습니다.");
                return null;
            }

            SpawnPattern chosen = pool[_random.Next(pool.Count)];

            StageData runtimeStage = ScriptableObject.CreateInstance<StageData>();
            runtimeStage.stageNumber = _currentStageLevel;
            runtimeStage.stageTitle = $"Stage {_currentStageLevel}";
            runtimeStage.enemySpawnDatas = chosen.EnemySpawnDatas;

            return runtimeStage;
        }
    }
}
