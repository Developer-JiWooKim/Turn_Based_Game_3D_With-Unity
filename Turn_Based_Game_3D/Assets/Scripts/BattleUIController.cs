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
    private Label _turnLabel;
    private Button _quitBtn;

    private VisualElement _gameClearPanel;
    private VisualElement _gameOverPanel;


    public event Action<BattleUnit, PlayerSkillData> OnPlayerInput;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _uiDocument = GetComponent<UIDocument>();
        var root = _uiDocument.rootVisualElement;

        _skillBar   = root.Q<VisualElement>("skill-bar");
        _hpBar      = root.Q<VisualElement>("hp-bar");
        _mpBar      = root.Q<VisualElement>("mp-bar");
        _hpText     = root.Q<Label>("hp-text");
        _mpText     = root.Q<Label>("mp-text");
        _turnLabel  = root.Q<Label>("turn-label");
        _quitBtn    = root.Q<Button>("quit-btn");

        _gameClearPanel = root.Q<VisualElement>("game-clear-panel");
        _gameOverPanel  = root.Q<VisualElement>("game-over-panel");

        _turnLabel.text = $"Turn [ 1 ]";    // 턴 초기값

        _quitBtn.clicked += OnQuitButtonClicked;

        root.Q<Button>("title-btn-clear").clicked    += OnTitleButtonClicked;
        root.Q<Button>("title-btn-gameover").clicked += OnTitleButtonClicked;
        root.Q<Button>("retry-btn").clicked          += OnRetryButtonClicked;

        _skillBar.style.visibility = Visibility.Hidden;
    }

    public void Subscribe(BattleController battleController, PlayerInputHandler playerInputHandler)
    {
        Debug.Log($"Subscribe 호출됨 - playerInputHandler: {playerInputHandler}");

        _battleController = battleController;
        _playerInputHandler = playerInputHandler;

        _battleController.OnTurnStart            += OnTurnStart;
        _battleController.OnUnitDamaged          += HandleUnitDamaged;
        _battleController.OnBattleEnd            += HandleBattleEnd;
        _battleController.OnPlayerActionComplete += HandlePlayerActionComplete;
        _battleController.OnTurnChanged          += HangleTurnChanged;

        GameManager.Instance.OnGameClear += HandleGameClear;
        GameManager.Instance.OnGameOver  += HandleGameOver;
    }

    private void HandleGameOver()
    {
        _gameOverPanel.style.display = DisplayStyle.Flex;
        _skillBar.style.visibility = Visibility.Hidden;
    }

    private void HandleGameClear()
    {
        _gameClearPanel.style.display = DisplayStyle.Flex;
        _skillBar.style.visibility = Visibility.Hidden;
    }

    private void OnTitleButtonClicked()
    {
        // TODO#: 타이틀 씬으로 이동
        Debug.Log("타이틀로 이동");
        GameManager.Instance.LoadScene("TitleScene");
    }

    private void OnRetryButtonClicked()
    {
        // TODO#: 재도전
        Debug.Log("재도전");
        GameManager.Instance.ResetStage();
    }

    private void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 실행 중지
#else
        Application.Quit(); // 빌드에서 종료
#endif
    }

    private void HangleTurnChanged(int currentTurn)
    {
        _turnLabel.text = $"Turn [ {currentTurn} ]";
    }

    private void HandlePlayerActionComplete(PlayerBattleUnit player)
    {
        RefreshPlayerStatus(player);
    }

    private void HandleUnitDamaged(IDamageable target, int damage)
    {
        PlayerBattleUnit player = target as PlayerBattleUnit;
        if (player != null)
        {
            RefreshPlayerStatus(player);
        }
    }

    private void RefreshPlayerStatus(PlayerBattleUnit player)
    {
        float hpRatio = (float)player.CurrentHp / player.MaxHp;
        float mpRatio = (float)player.CurrentMp / player.MaxMp;

        _hpBar.style.width = Length.Percent(hpRatio * 100f);
        _mpBar.style.width = Length.Percent(mpRatio * 100f);

        _hpText.text = $"{player.CurrentHp}/{player.MaxHp}";
        _mpText.text = $"{player.CurrentMp}/{player.MaxMp}";
    }


    private void HandleBattleEnd(bool result)
    {
        _skillBar.style.visibility = Visibility.Hidden;
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
            _skillBar.style.visibility = Visibility.Hidden;
        }
    }
    private void BuildSkillButtons(List<PlayerSkillData> skills)
    {
        _skillBar.Clear();
        _skillBar.style.visibility = Visibility.Visible;

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
        // TODO#: 나중에 타겟 선택 UI 추가 예정
        // 지금은 임시로 첫 번째 살아있는 적 자동 타겟
        _playerInputHandler.NotifyPlayerActed(skill);
        _skillBar.style.visibility = Visibility.Hidden;
    }

    private void Unsubscribe()
    {
        if (_battleController != null)
        {
            _battleController.OnTurnStart               -= OnTurnStart;
            _battleController.OnBattleEnd               -= HandleBattleEnd;
            _battleController.OnUnitDamaged             -= HandleUnitDamaged;
            _battleController.OnPlayerActionComplete    -= HandlePlayerActionComplete;
            _battleController.OnTurnChanged             -= HangleTurnChanged;


            GameManager.Instance.OnGameOver -= HandleGameOver;
            GameManager.Instance.OnGameOver -= HandleGameClear;
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}
