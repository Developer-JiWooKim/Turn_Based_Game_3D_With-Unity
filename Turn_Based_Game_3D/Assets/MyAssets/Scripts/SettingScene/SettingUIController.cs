using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;

namespace Assets.MyAssets.Scripts.SettingScene
{

    /// <summary>
    /// SettingScene 흐름 조율 전담 - 캐릭터 선택 완료 시 성향 포인트 배분 패널로 전환하는 것만 담당하고,
    /// 실제 UI 로직은 CharacterSelectUIController/PointAllocationUIController가 각자 갖는다.
    /// </summary>
    public class SettingUIController : MonoBehaviour
    {
        [SerializeField] private CharacterSelectUIController _characterSelectUI;
        [SerializeField] private PointAllocationUIController _pointAllocationUI;

        private void Start()
        {
            if (_characterSelectUI == null) { Debug.LogError("SettingUIController-Start(): _characterSelectUI is null"); return; }
            _characterSelectUI.OnCharacterConfirmed += HandleCharacterConfirmed;
            _characterSelectUI.Show();

            if (_pointAllocationUI == null) { Debug.LogError("SettingUIController-Start(): _pointAllocationUI is null"); return; }
            _pointAllocationUI.Hide();
        }

        private void OnDestroy()
        {
            _characterSelectUI.OnCharacterConfirmed -= HandleCharacterConfirmed;
        }

        private void HandleCharacterConfirmed(PlayerData character)
        {
            _characterSelectUI.Hide();
            _pointAllocationUI.Show(character);
        }
    }
}