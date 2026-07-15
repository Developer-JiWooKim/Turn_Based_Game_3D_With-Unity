using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.MyAssets.Scripts.Scriptable;

namespace Assets.MyAssets.Scripts.SettingScene
{
    public class CharacterSelectUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        private VisualElement _root;
        private VisualElement _previewArea;
        private VisualElement _characterGrid;

        private Label _characterName;
        private Label _characterHp;
        private Label _characterAtk;
        private Label _characterDef;
        private Label _characterSpd;

        private Button _backBtn;
        private Button _startBtn;

        private readonly Dictionary<PlayerData, Button> _characterButtons = new Dictionary<PlayerData, Button>();

        public event Action OnBackClicked;
        public event Action OnSelectClicked;
        public event Action<PlayerData> OnCharacterClicked;

        private void Awake()
        {
            VisualElement root = _uiDocument.rootVisualElement;
            if (root == null) { Debug.LogError("CharacterSelectUI-Awake(): VisualElement root is null"); return; }

            _root = root.Q<VisualElement>("character-select-area");
            _characterGrid = root.Q<VisualElement>("character-grid");
            _previewArea = root.Q<VisualElement>("preview-area");

            _characterName = root.Q<Label>("character-name");
            _characterHp = root.Q<Label>("character-hp");
            _characterAtk = root.Q<Label>("character-atk");
            _characterDef = root.Q<Label>("character-def");
            _characterSpd = root.Q<Label>("character-spd");

            _backBtn = root.Q<Button>("back-btn");
            _startBtn = root.Q<Button>("start-btn");

            _backBtn.clicked += () => OnBackClicked?.Invoke();
            _startBtn.clicked += () => OnSelectClicked?.Invoke();
        }

        public void Show() => _root.style.display = DisplayStyle.Flex;
        public void Hide() => _root.style.display = DisplayStyle.None;

        public void SetPreviewTexture(RenderTexture texture)
        {
            _previewArea.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(texture));
        }

        public void RegisterPreviewDragCallbacks(Action onDragStart, Action onDragEnd)
        {
            _previewArea.RegisterCallback<MouseDownEvent>(evt => onDragStart?.Invoke());
            _previewArea.RegisterCallback<MouseUpEvent>(evt => onDragEnd?.Invoke());
            _previewArea.RegisterCallback<MouseLeaveEvent>(evt => onDragEnd?.Invoke());
        }

        public void BuildCharacterGrid(IEnumerable<PlayerData> characters)
        {
            _characterGrid.Clear();
            _characterButtons.Clear();

            foreach (var character in characters)
            {
                PlayerData captured = character;
                var btn = new Button(() => OnCharacterClicked?.Invoke(captured));
                btn.AddToClassList("character-grid-btn");

                Sprite icon = character.RepresentativeWeapon != null ? character.RepresentativeWeapon.WeaponIcon : null;
                if (icon != null)
                    btn.style.backgroundImage = new StyleBackground(icon);
                else
                    btn.text = character.PlayerName;

                _characterButtons[character] = btn;
                _characterGrid.Add(btn);
            }
        }

        public void SetSelectedHighlight(PlayerData previous, PlayerData current)
        {
            if (previous != null && _characterButtons.TryGetValue(previous, out Button previousBtn))
                previousBtn.RemoveFromClassList("character-grid-btn-selected");

            if (current != null && _characterButtons.TryGetValue(current, out Button currentBtn))
                currentBtn.AddToClassList("character-grid-btn-selected");
        }

        public void SetCharacterInfo(string name, int hp, int atk, int def, int spd)
        {
            _characterName.text = name;
            _characterHp.text = hp.ToString();
            _characterAtk.text = atk.ToString();
            _characterDef.text = def.ToString();
            _characterSpd.text = spd.ToString();
        }

        public void SetSelectEnabled(bool enabled) => _startBtn.SetEnabled(enabled);
    }

}
