using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.MyAssets.Scripts.Scriptable;
using Assets.MyAssets.Scripts.Manager;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class BattleUIController : MonoBehaviour
{
    private class PartyStatusRow
    {
        public VisualElement HpBar;
        public VisualElement MpBar;
        public Label         HpText;
        public Label         MpText;
    }

    private BattleController    _battleController;
    private PlayerInputHandler  _playerInputHandler;

    private UIDocument _uiDocument;

    private VisualElement _skillBar;
    private VisualElement _partyStatusContainer;
    private VisualElement _gameOverPanel;

    private Label _turnLabel;

    private Button _quitBtn;

    private readonly Dictionary<PlayerBattleUnit, PartyStatusRow> _partyStatusRows = new Dictionary<PlayerBattleUnit, PartyStatusRow>();

    private void Awake() => Initialize();

    private void Initialize()
    {
        _uiDocument = GetComponent<UIDocument>();
        var root = _uiDocument.rootVisualElement;

        _skillBar             = root.Q<VisualElement>("skill-bar");
        _partyStatusContainer = root.Q<VisualElement>("player-status");
        _gameOverPanel        = root.Q<VisualElement>("game-over-panel");

        _turnLabel  = root.Q<Label>("turn-label");
        _quitBtn    = root.Q<Button>("quit-btn");

        _turnLabel.text = $"Turn [ 1 ]";    // 턴 초기값

        _quitBtn.clicked += OnQuitButtonClicked;

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
        _battleController.OnBattleStarted        += HandleBattleStarted;
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
            _battleController.OnBattleStarted -= HandleBattleStarted;
        }
    }

    private void OnTitleButtonClicked()
    {
        // TODO#: 타이틀 씬으로 이동
        Debug.Log("타이틀로 이동");
        GameManager.Instance.LoadScene("TitleScene");
    }

    private void OnRetryButtonClicked()
    {
        StageManager.Instance.ResetStage();
        GameManager.Instance.LoadScene("BattleScene");
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

    private void HandleBattleStarted(List<PlayerBattleUnit> players)
    {
        _partyStatusContainer.Clear();
        _partyStatusRows.Clear();

        foreach (var player in players)
        {
            BuildPartyStatusRow(player);
            RefreshPlayerStatus(player);
        }
    }

    private void BuildPartyStatusRow(PlayerBattleUnit player)
    {
        var nameRow = new VisualElement();
        nameRow.AddToClassList("bar-row");
        var nameLabel = new Label(player.Name);
        nameLabel.AddToClassList("player-name");
        nameRow.Add(nameLabel);

        var hpRow = new VisualElement();
        hpRow.AddToClassList("bar-row");
        var hpLabel = new Label("HP");
        hpLabel.AddToClassList("bar-label");
        var hpBarBg = new VisualElement();
        hpBarBg.AddToClassList("bar-bg");
        var hpBar = new VisualElement();
        hpBar.AddToClassList("bar");
        hpBar.AddToClassList("hp-bar");
        hpBarBg.Add(hpBar);
        var hpText = new Label();
        hpText.AddToClassList("bar-text");
        hpRow.Add(hpLabel);
        hpRow.Add(hpBarBg);
        hpRow.Add(hpText);

        var mpRow = new VisualElement();
        mpRow.AddToClassList("bar-row");
        var mpLabel = new Label("ST");
        mpLabel.AddToClassList("bar-label");
        var mpBarBg = new VisualElement();
        mpBarBg.AddToClassList("bar-bg");
        var mpBar = new VisualElement();
        mpBar.AddToClassList("bar");
        mpBar.AddToClassList("mp-bar");
        mpBarBg.Add(mpBar);
        var mpText = new Label();
        mpText.AddToClassList("bar-text");
        mpRow.Add(mpLabel);
        mpRow.Add(mpBarBg);
        mpRow.Add(mpText);

        _partyStatusContainer.Add(nameRow);
        _partyStatusContainer.Add(hpRow);
        _partyStatusContainer.Add(mpRow);

        _partyStatusRows[player] = new PartyStatusRow
        {
            HpBar  = hpBar,
            MpBar  = mpBar,
            HpText = hpText,
            MpText = mpText,
        };
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
        if (!_partyStatusRows.TryGetValue(player, out PartyStatusRow row)) return;

        float hpRatio = (float)player.CurrentHp / player.MaxHp;
        float mpRatio = (float)player.CurrentStamina / player.MaxStamina;

        row.HpBar.style.width = Length.Percent(hpRatio * 100f);
        row.MpBar.style.width = Length.Percent(mpRatio * 100f);

        row.HpText.text = $"{player.CurrentHp}/{player.MaxHp}";
        row.MpText.text = $"{player.CurrentStamina}/{player.MaxStamina}";
    }

    private void HandleBattleEnd(bool result)
    {
        _skillBar.style.visibility = Visibility.Hidden;

        if (!result)
        {
            _gameOverPanel.style.display = DisplayStyle.Flex;
        }
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
