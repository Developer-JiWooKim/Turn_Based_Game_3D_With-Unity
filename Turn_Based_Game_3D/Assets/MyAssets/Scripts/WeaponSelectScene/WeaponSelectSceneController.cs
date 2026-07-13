using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Assets.MyAssets.Scripts.Scriptable;
using Assets.MyAssets.Scripts.Manager;

namespace Assets.MyAssets.Scripts.WeaponSelectScene
{

    public class WeaponSelectSceneController : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private WeaponData[] _weaponDatas;
        [SerializeField] private Transform _previewPoint;
        [SerializeField] private RenderTexture _previewRenderTexture;
        [SerializeField] private WeaponPreviewRotator _previewRotator;

        private VisualElement _previewArea;
        private VisualElement _weaponGrid;
        private VisualElement[] _selectedSlots = new VisualElement[3];

        private Label _weaponName;
        private Label _weaponAtk;
        private Label _weaponSpd;
        private Label _weaponDesc;

        private Button _backBtn;
        private Button _startBtn;
        private Button _selectBtn;
        private Button _resetBtn;

        private GameObject _currentPreviewObj;

        private WeaponData _currentWeapon; // 현재 하이라이트된 무기 데이터
        private WeaponData[] _selectedWeapons = new WeaponData[3];

        private Dictionary<WeaponData, GameObject> _previewPool = new Dictionary<WeaponData, GameObject>();
        private Dictionary<WeaponData, Button> _weaponButtons = new Dictionary<WeaponData, Button>();
        private void OnDestroy() => UnSubscribeButtonEvent();
        private void UnSubscribeButtonEvent()
        {
            _backBtn.clicked -= OnBackButtonClicked;
            _startBtn.clicked -= OnStartButtonClicked;
            _selectBtn.clicked -= OnSelectButtonClicked;
            _resetBtn.clicked -= OnResetButtonClicked;
        }

        private void Awake() => Initialize();
        private void Initialize()
        {
            var root = _uiDocument.rootVisualElement;

            _weaponGrid = root.Q<VisualElement>("weapon-grid");
            _previewArea = root.Q<VisualElement>("preview-area");

            _weaponName = root.Q<Label>("weapon-name");
            _weaponAtk = root.Q<Label>("weapon-atk");
            _weaponSpd = root.Q<Label>("weapon-speed");
            _weaponDesc = root.Q<Label>("weapon-desc");

            _backBtn = root.Q<Button>("back-btn");
            _startBtn = root.Q<Button>("start-btn");
            _selectBtn = root.Q<Button>("select-btn");
            _resetBtn = root.Q<Button>("reset-btn");

            _selectedSlots[0] = root.Q<VisualElement>("selected-slot-0");
            _selectedSlots[1] = root.Q<VisualElement>("selected-slot-1");
            _selectedSlots[2] = root.Q<VisualElement>("selected-slot-2");

            // Render Texture 프리뷰 설정
            _previewArea.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(_previewRenderTexture));

            _previewArea.RegisterCallback<MouseDownEvent>(evt => _previewRotator?.OnDragStart());
            _previewArea.RegisterCallback<MouseUpEvent>(evt => _previewRotator?.OnDragEnd());
            _previewArea.RegisterCallback<MouseLeaveEvent>(evt => _previewRotator?.OnDragEnd());

            subscribeButtonEvent();

            for (int i = 0; i < _selectedSlots.Length; i++)
            {
                int captured = i;
                _selectedSlots[i].RegisterCallback<ClickEvent>(evt => OnSelectedSlotClicked(captured));
            }

            InitPreviewPool();
            BuildWeaponGrid();
            UpdateStartButton();
        }
        private void subscribeButtonEvent()
        {
            _backBtn.clicked += OnBackButtonClicked;
            _startBtn.clicked += OnStartButtonClicked;
            _selectBtn.clicked += OnSelectButtonClicked;
            _resetBtn.clicked += OnResetButtonClicked;
        }

        private void OnResetButtonClicked()
        {
            for (int i = 0; i < _selectedWeapons.Length; i++)
            {
                _selectedWeapons[i] = null;
                _selectedSlots[i].style.backgroundImage = null;
                _selectedSlots[i].RemoveFromClassList("selected-slot-filled");
            }
            UpdateStartButton();
        }

        private void InitPreviewPool()
        {
            foreach (var weapon in _weaponDatas)
            {
                if (weapon.WeaponPrefab == null) continue;

                GameObject obj = Instantiate(weapon.WeaponPrefab,
                    _previewPoint.position + weapon.WeaponPrefab.GetComponent<Transform>().position,
                    _previewPoint.rotation, _previewPoint);

                int layer = LayerMask.NameToLayer("WeaponPreview");
                foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
                    t.gameObject.layer = layer;

                obj.SetActive(false);
                _previewPool[weapon] = obj;
            }
        }

        private void BuildWeaponGrid()
        {
            _weaponGrid.Clear();
            _weaponButtons.Clear();

            foreach (var weapon in _weaponDatas)
            {
                WeaponData captured = weapon;
                var btn = new Button(() => OnWeaponButtonClicked(captured));
                btn.AddToClassList("weapon-grid-btn");

                // 아이콘이 있으면 표시, 없으면 이름 표시
                if (weapon.WeaponIcon != null)
                    btn.style.backgroundImage = new StyleBackground(weapon.WeaponIcon);
                else
                    btn.text = weapon.WeaponName;

                _weaponButtons[weapon] = btn;
                _weaponGrid.Add(btn);
            }
        }

        private void OnWeaponButtonClicked(WeaponData weapon)
        {
            // 같은 무기 다시 클릭 시 슬롯에 추가
            if (_currentWeapon == weapon)
            {
                AddToSelectedSlot(weapon);
                return;
            }

            // 이전 버튼 하이라이트 제거
            if (_currentWeapon != null && _weaponButtons.ContainsKey(_currentWeapon))
                _weaponButtons[_currentWeapon].RemoveFromClassList("weapon-grid-btn-selected");

            _currentWeapon = weapon;

            // 현재 버튼 하이라이트
            _weaponButtons[weapon].AddToClassList("weapon-grid-btn-selected");

            UpdateWeaponInfo(weapon);
            UpdatePreview(weapon);
        }

        private void OnSelectButtonClicked()
        {
            if (_currentWeapon == null) return;
            AddToSelectedSlot(_currentWeapon);
        }

        private void UpdateWeaponInfo(WeaponData weapon)
        {
            _weaponName.text = weapon.WeaponName;
            _weaponAtk.text = weapon.bonusStat.AttackPower.ToString();
            _weaponSpd.text = weapon.bonusStat.Speed.ToString();
            _weaponDesc.text = weapon.Description;
        }

        private void UpdatePreview(WeaponData weapon)
        {
            // 현재 활성화된 프리뷰 비활성화
            if (_currentPreviewObj != null)
                _currentPreviewObj.SetActive(false);

            if (weapon.WeaponPrefab == null || !_previewPool.ContainsKey(weapon)) return;

            _currentPreviewObj = _previewPool[weapon];
            _previewPoint.rotation = new Quaternion(0, 0, 0, 0);

            _currentPreviewObj.SetActive(true);
        }

        private void AddToSelectedSlot(WeaponData weapon)
        {
            // 이미 선택된 무기인지 확인
            for (int i = 0; i < _selectedWeapons.Length; i++)
            {
                if (_selectedWeapons[i] == weapon)
                {
                    Debug.Log("이미 선택된 무기입니다!");
                    return;
                }
            }

            // 빈 슬롯 찾아서 추가
            for (int i = 0; i < _selectedWeapons.Length; i++)
            {
                if (_selectedWeapons[i] == null)
                {
                    _selectedWeapons[i] = weapon;
                    _selectedSlots[i].AddToClassList("selected-slot-filled");

                    if (weapon.WeaponIcon != null)
                        _selectedSlots[i].style.backgroundImage = new StyleBackground(weapon.WeaponIcon);
                    else
                        _selectedSlots[i].style.backgroundImage = null;

                    UpdateStartButton();
                    return;
                }
            }

            Debug.Log("슬롯이 꽉 찼습니다!");
        }

        private void OnSelectedSlotClicked(int slotIndex)
        {
            if (_selectedWeapons[slotIndex] == null) return;

            _selectedWeapons[slotIndex] = null;
            _selectedSlots[slotIndex].style.backgroundImage = null;
            _selectedSlots[slotIndex].RemoveFromClassList("selected-slot-filled");

            UpdateStartButton();
        }

        private void UpdateStartButton()
        {
            bool allSelected = true;
            foreach (var weapon in _selectedWeapons)
            {
                if (weapon == null)
                {
                    allSelected = false;
                    break;
                }
            }
            _startBtn.SetEnabled(allSelected);
        }

        private void OnBackButtonClicked()
        {
            GameManager.Instance.LoadScene("TitleScene");
        }

        private void OnStartButtonClicked()
        {
            for (int i = 0; i < _selectedWeapons.Length; i++)
                PlayerDataManager.Instance.SelectWeapon(i, _selectedWeapons[i]);

            GameManager.Instance.LoadScene("BattleScene");
        }
    }

}