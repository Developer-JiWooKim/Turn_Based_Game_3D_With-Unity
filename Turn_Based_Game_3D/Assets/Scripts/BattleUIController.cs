using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleUIController : MonoBehaviour
{
    private BattleController    _battleController;
    private PlayerInputHandler  _playerInputHandler;

    // UIToolKit 관련, 이 부분은 잘 몰라서 AI에게 맡김
    private UIDocument _uiDocument;
    private VisualElement _skillBar;
    private VisualElement _hpBar;
    private VisualElement _mpBar;
    private Label _hpText;
    private Label _mpText;

    public event Action<BattleUnit, PlayerSkillData> OnPlayerInput;

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        var root = _uiDocument.rootVisualElement;

        _skillBar   = root.Q<VisualElement>("skill-bar");
        _hpBar      = root.Q<VisualElement>("hp-bar");
        _mpBar      = root.Q<VisualElement>("mp-bar");
        _hpText     = root.Q<Label>("hp-text");
        _mpText     = root.Q<Label>("mp-text");

        _skillBar.style.display = DisplayStyle.None;
    }

    public void Subscribe(BattleController battleController, PlayerInputHandler playerInputHandler)
    {
        Debug.Log($"Subscribe 호출됨 - playerInputHandler: {playerInputHandler}");
        
        _battleController   = battleController;
        _playerInputHandler = playerInputHandler;

        _battleController.OnTurnStart   += OnTurnStart;
        _battleController.OnUnitDamaged += HandleUnitDamaged;
        _battleController.OnBattleEnd   += HandleBattleEnd;
    }

    private void HandleUnitDamaged(IDamageable target, int damage)
    {
        // TODO#: 플레이어 HP/MP 갱신 예정
    }

    private void HandleBattleEnd(bool result)
    {
        _skillBar.style.display = DisplayStyle.None;
        Debug.Log(result ? "승리!" : "패배...");
    }

    private void OnTurnStart(BattleUnit unit) 
    {
        if (unit is PlayerBattleUnit player)
        {
            // 카메라 위치를 플레이어 뒤쪽으로 이동
            // 스킬 버튼 활성화
            // 플레이어 입력을 기다림
            BuildSkillButtons(player.Skills);
        }
        else
        {
            // 스킬 버튼 비활성화
            _skillBar.style.display = DisplayStyle.None;
        }
    }
    private void BuildSkillButtons(List<PlayerSkillData> skills)
    {

        Debug.Log($"skills: {skills}");          // null 인지 확인
        Debug.Log($"skills count: {skills?.Count}"); // 몇 개인지 확인
        Debug.Log($"_skillBar: {_skillBar}");    // skillBar null 인지 확인

        _skillBar.Clear();
        _skillBar.style.display = DisplayStyle.Flex;

        foreach (var skill in skills)
        {
            PlayerSkillData captured = skill;
            var btn = new Button(() => OnSkillButtonClicked(captured))
            {
                text = skill.SkillName
            };
            btn.AddToClassList("skill-btn");
            _skillBar.Add(btn);
        }
    }

    private void OnSkillButtonClicked(PlayerSkillData skill)
    {
        Debug.Log($"버튼 클릭됨! skill: {skill.SkillName}");
        Debug.Log($"_playerInputHandler: {_playerInputHandler}");


        // TODO#: 나중에 타겟 선택 UI 추가 예정
        // 지금은 임시로 첫 번째 살아있는 적 자동 타겟
        _playerInputHandler.NotifyPlayerActed(skill);
        _skillBar.style.display = DisplayStyle.None;
    }

    private void Unsubscribe()
    {
        if (_battleController != null)
        {
            _battleController.OnTurnStart -= OnTurnStart;
            _battleController.OnBattleEnd -= HandleBattleEnd;
            _battleController.OnUnitDamaged -= HandleUnitDamaged;
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}
