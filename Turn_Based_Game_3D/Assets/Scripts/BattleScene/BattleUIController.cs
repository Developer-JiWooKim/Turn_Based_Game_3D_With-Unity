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


    // public event Action<BattleUnit, PlayerSkillData> OnPlayerInput;

    private void Awake() => Initialize();

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
        _battleController   = battleController;
        _playerInputHandler = playerInputHandler;

        _battleController.OnTurnStart            += OnTurnStart;
        _battleController.OnUnitDamaged          += HandleUnitDamaged;
        _battleController.OnBattleEnd            += HandleBattleEnd;
        _battleController.OnPlayerActionComplete += HandlePlayerActionComplete;
        _battleController.OnTurnChanged          += HangleTurnChanged;

         GameManager.Instance.OnGameClear += HandleGameClear;
         GameManager.Instance.OnGameOver  += HandleGameOver;
    }

    private void OnDestroy() => Unsubscribe();
    private void Unsubscribe()
    {
        if (_battleController != null)
        {
            _battleController.OnTurnStart -= OnTurnStart;
            _battleController.OnBattleEnd -= HandleBattleEnd;
            _battleController.OnUnitDamaged -= HandleUnitDamaged;
            _battleController.OnPlayerActionComplete -= HandlePlayerActionComplete;
            _battleController.OnTurnChanged -= HangleTurnChanged;

            GameManager.Instance.OnGameOver -= HandleGameOver;
            GameManager.Instance.OnGameOver -= HandleGameClear;
        }
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
        float mpRatio = (float)player.CurrentStamina / player.MaxStamina;

        _hpBar.style.width = Length.Percent(hpRatio * 100f);
        _mpBar.style.width = Length.Percent(mpRatio * 100f);

        _hpText.text = $"{player.CurrentHp}/{player.MaxHp}";
        _mpText.text = $"{player.CurrentStamina}/{player.MaxStamina}";
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
            // 스킬 버튼 활성화, 플레이어 입력을 기다림
            BuildWeaponButtons(player);
        }
        else
        {
            // 스킬 버튼 비활성화
            _skillBar.style.visibility = Visibility.Hidden;
        }
    }

    private void BuildWeaponButtons(PlayerBattleUnit player)
    {
        _skillBar.Clear();
        _skillBar.style.visibility = Visibility.Visible;

        for (int i = 0; i < player.Weapons.Length; i++)
        {
            WeaponData weapon = player.Weapons[i];
            int capturedIndex = i;

            var btn = new Button(() => OnWeaponButtonClicked(capturedIndex))
            {
                text = weapon.WeaponName
            };

            // 사용 불가능한 무기는 비활성화
            btn.SetEnabled(player.CanUseWeapon(capturedIndex));

            btn.AddToClassList("skill-btn");
            _skillBar.Add(btn);
        }
    }

    private void OnWeaponButtonClicked(int weaponIndex)
    {
        _playerInputHandler.NotifyPlayerActed(weaponIndex);
        _skillBar.style.visibility = Visibility.Hidden;
    }
}
