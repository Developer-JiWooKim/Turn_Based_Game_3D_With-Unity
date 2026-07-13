using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.MyAssets.Scripts.Scriptable;
using Assets.MyAssets.Scripts.Manager;

namespace Assets.MyAssets.Scripts.BattleScene
{

/// <summary>
/// 스테이지 승리 후 로그라이크 선택지 3종을 제시하고, 파티원 영입 선택 시 파티가 꽉 찼다면
/// 교체 대상을 고르는 팝업까지 처리한다. BattleUIController와 같은 UIDocument를 사용하는 오버레이.
/// </summary>
public class RoguelikeChoiceUIController : MonoBehaviour
{
    private UIDocument _uiDocument;

    private VisualElement _choicePanel;
    private VisualElement _choiceCardContainer;

    private VisualElement _recruitReplacePanel;
    private VisualElement _replaceTargetContainer;
    private Button        _recruitCancelBtn;

    private Action _onProceed; // 선택/영입 흐름이 전부 끝난 뒤 호출할 콜백 (다음 스테이지 로드)
    private PlayerData _pendingRecruit;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        var root = _uiDocument.rootVisualElement;

        _choicePanel         = root.Q<VisualElement>("roguelike-choice-panel");
        _choiceCardContainer = root.Q<VisualElement>("choice-card-container");

        _recruitReplacePanel   = root.Q<VisualElement>("recruit-replace-panel");
        _replaceTargetContainer = root.Q<VisualElement>("replace-target-container");
        _recruitCancelBtn      = root.Q<Button>("recruit-cancel-btn");

        _recruitCancelBtn.clicked += OnRecruitCancelled;

        _choicePanel.style.display = DisplayStyle.None;
        _recruitReplacePanel.style.display = DisplayStyle.None;
    }

    public void ShowChoices(Action onProceed)
    {
        _onProceed = onProceed;

        List<RoguelikeChoiceData> choices = RoguelikeChoiceManager.Instance.DrawChoices(3);

        _choiceCardContainer.Clear();
        foreach (var choice in choices)
        {
            RoguelikeChoiceData captured = choice;
            var btn = new Button(() => OnChoiceSelected(captured));
            btn.text = $"{choice.ChoiceName}\n{choice.Description}";
            btn.AddToClassList("choice-card-btn");
            _choiceCardContainer.Add(btn);
        }

        _choicePanel.style.display = DisplayStyle.Flex;
    }

    private void OnChoiceSelected(RoguelikeChoiceData choice)
    {
        _choicePanel.style.display = DisplayStyle.None;

        if (choice.Category == RoguelikeChoiceCategory.RecruitAlly)
        {
            HandleRecruit();
            return;
        }

        RoguelikeChoiceManager.Instance.ApplyChoice(choice);
        _onProceed?.Invoke();
    }

    private void HandleRecruit()
    {
        _pendingRecruit = RoguelikeChoiceManager.Instance.DrawRandomCharacter();

        List<PartyMember> roster = PlayerDataManager.Instance.PartyRoster;

        if (roster.Count < 4)
        {
            PlayerDataManager.Instance.AddToRoster(_pendingRecruit);
            _onProceed?.Invoke();
            return;
        }

        // 파티가 꽉 찼으면 교체 대상 선택 팝업
        _replaceTargetContainer.Clear();
        foreach (var member in roster)
        {
            PartyMember captured = member;
            var btn = new Button(() => OnReplaceConfirmed(captured));
            btn.text = member.Data.PlayerName;
            btn.AddToClassList("choice-card-btn");
            _replaceTargetContainer.Add(btn);
        }

        _recruitReplacePanel.style.display = DisplayStyle.Flex;
    }

    private void OnReplaceConfirmed(PartyMember oldMember)
    {
        PlayerDataManager.Instance.ReplaceInRoster(oldMember, _pendingRecruit);
        _recruitReplacePanel.style.display = DisplayStyle.None;
        _onProceed?.Invoke();
    }

    private void OnRecruitCancelled()
    {
        if (_recruitReplacePanel.style.display != DisplayStyle.Flex) return;

        _recruitReplacePanel.style.display = DisplayStyle.None;
        _onProceed?.Invoke();
    }
}

}
