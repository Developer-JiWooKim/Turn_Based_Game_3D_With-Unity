using UnityEngine;
using UnityEngine.UIElements;
using Assets.MyAssets.Scripts.Scriptable;
using Assets.MyAssets.Scripts.Manager;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class BattleUIController : MonoBehaviour
{
    private BattleController    _battleController;
    private PlayerInputHandler  _playerInputHandler;

    private UIDocument _uiDocument;

    private VisualElement _skillBar;
    private VisualElement _hpBar;
    private VisualElement _mpBar;
    private VisualElement _gameClearPanel;
    private VisualElement _gameOverPanel;
    private VisualElement _expBar;

    private Label _levelText;
    private Label _hpText;
    private Label _mpText;
    private Label _turnLabel;

    private Button _quitBtn;

    private void Awake() => Initialize();

    private void Initialize()
    {
        _uiDocument = GetComponent<UIDocument>();
        var root = _uiDocument.rootVisualElement;

        _skillBar       = root.Q<VisualElement>("skill-bar");
        _hpBar          = root.Q<VisualElement>("hp-bar");
        _mpBar          = root.Q<VisualElement>("mp-bar");
        _gameClearPanel = root.Q<VisualElement>("game-clear-panel");
        _gameOverPanel  = root.Q<VisualElement>("game-over-panel");
        _expBar         = root.Q<VisualElement>("exp-bar");

        _levelText  = root.Q<Label>("level-text");
        _hpText     = root.Q<Label>("hp-text");
        _mpText     = root.Q<Label>("mp-text");
        _turnLabel  = root.Q<Label>("turn-label");
        _quitBtn    = root.Q<Button>("quit-btn");        

        _turnLabel.text = $"Turn [ 1 ]";    // 턴 초기값

        _quitBtn.clicked += OnQuitButtonClicked;

        root.Q<Button>("title-btn-clear").clicked    += OnTitleButtonClicked;
        root.Q<Button>("title-btn-gameover").clicked += OnTitleButtonClicked;
        root.Q<Button>("retry-btn").clicked          += OnRetryButtonClicked;

        _skillBar.style.visibility = Visibility.Hidden;

        // 초기 레벨 표시
        UpdateLevelUI();
    }

    private void UpdateLevelUI()
    {
        int level = PlayerDataManager.Instance.CurrentLevel;
        if (_levelText != null)
            _levelText.text = $"Lv. {level}";

        // 노멀 모드는 항상 꽉 찬 상태 (스테이지 클리어 = 1레벨업)
        if (_expBar != null)
            _expBar.style.width = Length.Percent(100f);
    }

    private void HandleLevelUp(int newLevel)
    {
        UpdateLevelUI();
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

        PlayerDataManager.Instance.OnLevelUp += HandleLevelUp;
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

            GameManager.Instance.OnGameOver  -= HandleGameOver;
            GameManager.Instance.OnGameClear -= HandleGameClear;

            PlayerDataManager.Instance.OnLevelUp -= HandleLevelUp;
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
        Debug.Log($"RefreshPlayerStatus - HP: {player.CurrentHp}/{player.MaxHp}, ST: {player.CurrentStamina}/{player.MaxStamina}");

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

            var btn = new Button(() => OnWeaponButtonClicked(capturedIndex));
            btn.AddToClassList("skill-btn");

            // 아이콘 설정
            if (weapon.WeaponIcon != null)
                btn.style.backgroundImage = new StyleBackground(weapon.WeaponIcon);


            // 마우스 오버 시 무기 이름 표시
            btn.RegisterCallback<MouseEnterEvent>(evt =>
            {
                btn.text = weapon.WeaponName;
                btn.style.unityBackgroundImageTintColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            });

            btn.RegisterCallback<MouseLeaveEvent>(evt =>
            {
                btn.text = "";
                btn.style.unityBackgroundImageTintColor = new Color(1f, 1f, 1f, 1f);
            });

            // 사용 불가능한 무기는 비활성화
            btn.SetEnabled(player.CanUseWeapon(capturedIndex));
            _skillBar.Add(btn);
        }
    }

    private void OnWeaponButtonClicked(int weaponIndex)
    {
        _playerInputHandler.NotifyPlayerActed(weaponIndex);
        _skillBar.style.visibility = Visibility.Hidden;
    }
}

}
