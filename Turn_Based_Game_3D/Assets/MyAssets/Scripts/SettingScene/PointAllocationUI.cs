using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.MyAssets.Scripts.Scriptable;

namespace Assets.MyAssets.Scripts.SettingScene
{

    /// <summary>
    /// 성향 포인트 배분 패널의 UI Toolkit 바인딩 전담 - 카테고리 목록 렌더링, 클릭을 도메인 이벤트로 재발행.
    /// StageManager 등 매니저 호출은 전혀 하지 않고, 표시할 값은 전부 PointAllocationUIController가 넘겨준다.
    /// </summary>
    public class PointAllocationUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        private VisualElement _root;
        private VisualElement _categoryList;

        private Label _remainingPoints;
        private Button _respecAllBtn;
        private Button _battleStartBtn;

        private readonly Dictionary<RoguelikeChoiceCategory, Label> _categoryInvestedLabels = new Dictionary<RoguelikeChoiceCategory, Label>();

        public event Action OnRespecAllClicked;
        public event Action OnBattleStartClicked;
        public event Action<RoguelikeChoiceCategory> OnInvestClicked;
        public event Action<RoguelikeChoiceCategory> OnWithdrawClicked;

        private void Awake()
        {
            var root = _uiDocument.rootVisualElement;

            _root = root.Q<VisualElement>("weight-allocation-panel");
            _categoryList = root.Q<VisualElement>("category-list");

            _remainingPoints = root.Q<Label>("remaining-points");
            _respecAllBtn    = root.Q<Button>("respec-all-btn");
            _battleStartBtn  = root.Q<Button>("battle-start-btn");

            _respecAllBtn.clicked   += () => OnRespecAllClicked?.Invoke();
            _battleStartBtn.clicked += () => OnBattleStartClicked?.Invoke();
        }

        public void Show() => _root.style.display = DisplayStyle.Flex;
        public void Hide() => _root.style.display = DisplayStyle.None;

        public void BuildCategoryRows(IReadOnlyDictionary<RoguelikeChoiceCategory, int> investedByCategory)
        {
            _categoryList.Clear();
            _categoryInvestedLabels.Clear();

            foreach (var pair in investedByCategory)
            {
                RoguelikeChoiceCategory captured = pair.Key;

                var row = new VisualElement();
                row.AddToClassList("category-row");

                var nameLabel = new Label(captured.ToString());
                nameLabel.AddToClassList("category-name");

                var minusBtn = new Button(() => OnWithdrawClicked?.Invoke(captured));
                minusBtn.text = "-";
                minusBtn.AddToClassList("category-btn");

                var investedLabel = new Label(pair.Value.ToString());
                investedLabel.AddToClassList("category-invested");

                var plusBtn = new Button(() => OnInvestClicked?.Invoke(captured));
                plusBtn.text = "+";
                plusBtn.AddToClassList("category-btn");

                row.Add(nameLabel);
                row.Add(minusBtn);
                row.Add(investedLabel);
                row.Add(plusBtn);

                _categoryList.Add(row);
                _categoryInvestedLabels[captured] = investedLabel;
            }
        }

        public void SetCategoryInvested(RoguelikeChoiceCategory category, int value)
        {
            _categoryInvestedLabels[category].text = value.ToString();
        }

        public void SetRemainingPoints(int value) => _remainingPoints.text = value.ToString();
    }

}
