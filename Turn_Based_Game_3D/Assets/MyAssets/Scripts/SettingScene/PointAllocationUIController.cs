using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;
using Assets.MyAssets.Scripts.Manager;

namespace Assets.MyAssets.Scripts.SettingScene
{

    /// <summary>
    /// 성향 포인트 배분 패널 동작 전담 - 로그라이크 선택지 9종 카테고리별 투자/리스펙, 최종 전투 시작.
    /// UI Toolkit 요소는 전혀 모르고 PointAllocationUI가 발행하는 도메인 이벤트만 다룬다.
    /// </summary>
    public class PointAllocationUIController : MonoBehaviour
    {
        [SerializeField] private PointAllocationUI _ui;

        private PlayerData _selectedCharacter;

        private void Awake()
        {
            _ui.OnInvestClicked    += OnInvestButtonClicked;
            _ui.OnWithdrawClicked  += OnWithdrawButtonClicked;
            _ui.OnRespecAllClicked += OnRespecAllButtonClicked;
            _ui.OnBattleStartClicked += OnBattleStartButtonClicked;
        }

        private void OnDestroy()
        {
            _ui.OnInvestClicked    -= OnInvestButtonClicked;
            _ui.OnWithdrawClicked  -= OnWithdrawButtonClicked;
            _ui.OnRespecAllClicked -= OnRespecAllButtonClicked;
            _ui.OnBattleStartClicked -= OnBattleStartButtonClicked;
        }

        public void Show(PlayerData selectedCharacter)
        {
            _selectedCharacter = selectedCharacter;
            _ui.Show();
            RefreshAll();
        }

        public void Hide() => _ui.Hide();

        private void RefreshAll()
        {
            var invested = new Dictionary<RoguelikeChoiceCategory, int>();
            foreach (RoguelikeChoiceCategory category in Enum.GetValues(typeof(RoguelikeChoiceCategory)))
                invested[category] = StageManager.Instance.GetCategoryInvestedPoints(category);

            _ui.BuildCategoryRows(invested);
            RefreshRemainingPoints();
        }

        private void OnInvestButtonClicked(RoguelikeChoiceCategory category)
        {
            if (!StageManager.Instance.TryInvestPoint(category)) return;

            _ui.SetCategoryInvested(category, StageManager.Instance.GetCategoryInvestedPoints(category));
            RefreshRemainingPoints();
        }

        private void OnWithdrawButtonClicked(RoguelikeChoiceCategory category)
        {
            if (!StageManager.Instance.TryWithdrawPoint(category)) return;

            _ui.SetCategoryInvested(category, StageManager.Instance.GetCategoryInvestedPoints(category));
            RefreshRemainingPoints();
        }

        private void OnRespecAllButtonClicked()
        {
            foreach (RoguelikeChoiceCategory category in Enum.GetValues(typeof(RoguelikeChoiceCategory)))
            {
                StageManager.Instance.RespecCategory(category);
                _ui.SetCategoryInvested(category, 0);
            }

            RefreshRemainingPoints();
        }

        private void RefreshRemainingPoints()
        {
            _ui.SetRemainingPoints(StageManager.Instance.PermanentPoints);
        }

        private void OnBattleStartButtonClicked()
        {
            PlayerDataManager.Instance.SetStartingCharacter(_selectedCharacter);
            GameManager.Instance.LoadScene("BattleScene");
        }
    }

}
