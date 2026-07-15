using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;
using Assets.MyAssets.Scripts.Manager;

namespace Assets.MyAssets.Scripts.SettingScene
{

    /// <summary>
    /// 캐릭터 선택 패널 동작 전담 - 3D 프리뷰 풀 관리, 선택 상태 추적. UI Toolkit 요소는 전혀 모르고
    /// CharacterSelectUI가 발행하는 도메인 이벤트만 다룬다. 선택 완료 시 OnCharacterConfirmed로 알리고
    /// 실제 파티 확정/다음 패널 전환은 SettingSceneController가 처리한다.
    /// </summary>
    public class CharacterSelectUIController : MonoBehaviour
    {
        [SerializeField] private CharacterSelectUI _ui;
        [SerializeField] private PlayerData[] _characterDatas;
        [SerializeField] private Transform _previewPoint;
        [SerializeField] private RenderTexture _previewRenderTexture;
        [SerializeField] private PreviewRotator _previewRotator;

        private GameObject _currentPreviewObj;
        private PlayerData _currentCharacter; // 현재 하이라이트/선택된 캐릭터

        private readonly Dictionary<PlayerData, GameObject> _previewPool = new Dictionary<PlayerData, GameObject>();

        public event Action<PlayerData> OnCharacterConfirmed;

        private void Awake() => Initialize();
        private void Initialize()
        {
            _ui.SetPreviewTexture(_previewRenderTexture);
            _ui.RegisterPreviewDragCallbacks(
                () => { if (_previewRotator != null) _previewRotator.OnDragStart(); },
                () => { if (_previewRotator != null) _previewRotator.OnDragEnd(); });

            _ui.OnBackClicked += OnBackButtonClicked;
            _ui.OnSelectClicked += OnSelectButtonClicked;
            _ui.OnCharacterClicked += OnCharacterButtonClicked;

            InitPreviewPool();
            _ui.BuildCharacterGrid(_characterDatas);
            _ui.SetSelectEnabled(false);
        }

        private void OnDestroy()
        {
            _ui.OnBackClicked -= OnBackButtonClicked;
            _ui.OnSelectClicked -= OnSelectButtonClicked;
            _ui.OnCharacterClicked -= OnCharacterButtonClicked;
        }

        public void Show() => _ui.Show();
        public void Hide() => _ui.Hide();

        private void InitPreviewPool()
        {
            foreach (var character in _characterDatas)
            {
                if (character.CharacterPrefab == null) continue; // 프리팹 준비 전에는 건너뜀

                GameObject obj = Instantiate(character.CharacterPrefab, _previewPoint.position, _previewPoint.rotation, _previewPoint);

                int layer = LayerMask.NameToLayer("WeaponPreview");
                foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
                    t.gameObject.layer = layer;

                obj.SetActive(false);
                _previewPool[character] = obj;
            }
        }

        private void OnCharacterButtonClicked(PlayerData character)
        {
            _ui.SetSelectedHighlight(_currentCharacter, character);
            _currentCharacter = character;

            _ui.SetCharacterInfo(
                character.PlayerName,
                character.playerStat.Hp,
                character.playerStat.AttackPower,
                character.playerStat.DefenseValue,
                character.playerStat.Speed);

            UpdatePreview(character);
            _ui.SetSelectEnabled(true);
        }

        private void UpdatePreview(PlayerData character)
        {
            if (_currentPreviewObj != null)
                _currentPreviewObj.SetActive(false);

            if (!_previewPool.TryGetValue(character, out GameObject previewObj)) return;

            _currentPreviewObj = previewObj;
            _previewPoint.rotation = Quaternion.identity;

            _currentPreviewObj.SetActive(true);
        }

        private void OnBackButtonClicked()
        {
            GameManager.Instance.LoadScene("TitleScene");
        }

        private void OnSelectButtonClicked()
        {
            if (_currentCharacter == null) return;

            OnCharacterConfirmed?.Invoke(_currentCharacter);
        }
    }
}
