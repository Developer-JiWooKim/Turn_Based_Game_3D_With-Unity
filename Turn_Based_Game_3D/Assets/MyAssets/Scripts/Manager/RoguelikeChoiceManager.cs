using System.Collections.Generic;
using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;

namespace Assets.MyAssets.Scripts.Manager
{

    /// <summary>
    /// 로그라이크 선택지 추첨/적용 및 파티원 영입 후보 추첨을 전담한다.
    /// </summary>
    public class RoguelikeChoiceManager : Singleton<RoguelikeChoiceManager>
    {
        [SerializeField] private RoguelikeChoicePoolData _choicePool;
        [SerializeField] private PartySynergyPoolData     _synergyPool;
        [SerializeField] private List<PlayerData>         _allCharacterPool; // 영입 후보 6종

        private readonly System.Random _random = new System.Random();

        public PartySynergyPoolData SynergyPool => _synergyPool;

        /// <summary>
        /// BaseWeight + 성향 포인트 투자치 기반 가중치 비복원 추출로 count개의 선택지를 뽑는다.
        /// </summary>
        public List<RoguelikeChoiceData> DrawChoices(int count = 3)
        {
            List<RoguelikeChoiceData> pool = new List<RoguelikeChoiceData>(_choicePool.AllChoices);
            List<RoguelikeChoiceData> result = new List<RoguelikeChoiceData>();

            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                float totalWeight = 0f;
                foreach (var choice in pool) totalWeight += EffectiveWeight(choice);

                double roll = _random.NextDouble() * totalWeight;
                float cumulative = 0f;
                RoguelikeChoiceData picked = pool[pool.Count - 1];

                foreach (var choice in pool)
                {
                    cumulative += EffectiveWeight(choice);
                    if (roll < cumulative)
                    {
                        picked = choice;
                        break;
                    }
                }

                result.Add(picked);
                pool.Remove(picked);
            }

            return result;
        }

        private float EffectiveWeight(RoguelikeChoiceData choice)
        {
            int invested = StageManager.Instance.GetCategoryInvestedPoints(choice.Category);
            return choice.BaseWeight + invested * _choicePool.WeightPerInvestedPoint;
        }

        public PlayerData DrawRandomCharacter()
        {
            return _allCharacterPool[_random.Next(_allCharacterPool.Count)];
        }

        /// <summary>
        /// 선택지 효과를 적용한다. RecruitAlly는 파티 여유/교체 여부 판단이 필요해 UI 레이어(RoguelikeChoiceUIController)가 별도로 처리한다.
        /// </summary>
        public void ApplyChoice(RoguelikeChoiceData choice)
        {
            switch (choice.Category)
            {
                case RoguelikeChoiceCategory.AtkUp:
                case RoguelikeChoiceCategory.SpdUp:
                case RoguelikeChoiceCategory.DefensiveUp:
                case RoguelikeChoiceCategory.CritUp:
                    RunStateManager.Instance.AddRunBuff(choice.Bonus);
                    break;

                case RoguelikeChoiceCategory.Heal:
                    PlayerDataManager.Instance.HealAllByPercent(choice.HealPercent);
                    break;

                case RoguelikeChoiceCategory.EnemyStunNextStage:
                    RunStateManager.Instance.QueueEnemyStunNextStage();
                    break;

                case RoguelikeChoiceCategory.EnemyHpDownNextStage:
                    RunStateManager.Instance.QueueEnemyHpDownNextStage(choice.EnemyHpDownPercent);
                    break;

                case RoguelikeChoiceCategory.EnemyAtkDownNextStage:
                    RunStateManager.Instance.QueueEnemyAtkDownNextStage(choice.EnemyAtkDownPercent);
                    break;

                case RoguelikeChoiceCategory.RecruitAlly:
                    Debug.LogWarning("RecruitAlly는 RoguelikeChoiceUIController가 처리해야 합니다.");
                    break;
            }
        }
    }

}
