using UnityEngine;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerInputSystem _playerInputSystem;

    private BattleController  _battleController;
    private TargetSelector    _targetSelector;
    private Camera            _camera;

    private bool _isPlayerTurn = false;

    private void Awake() => Initialize();

    private void Initialize()
    {
        _camera = Camera.main;
    }

    public void Subscribe(BattleController battleController, 
                          TargetSelector targetSelector)
    {
        _battleController  = battleController;
        _targetSelector    = targetSelector;

        _battleController.OnTurnStart += HandleTurnStart;

        _playerInputSystem.OnNextInput           += HandleNext;
        _playerInputSystem.OnPrevInput           += HandlePrev;
        _playerInputSystem.OnSelectTargetInput   += HandleSelectTarget;
    }

    private void HandleSelectTarget(Vector2 vector)
    {
        if (!_isPlayerTurn) return;

        Ray ray = _camera.ScreenPointToRay(vector);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            EnemyUnitView view = hit.collider.GetComponentInParent<EnemyUnitView>(); // 몬스터 프리팹 자체에 EnemyUnitView가 붙어있는데 부모로 찾아야되는가?

            if (view != null)
            {
                int index = _targetSelector.EnemyViews.IndexOf(view); // 타겟 셀렉터에는 이런 메소드 없음
                if (index >= 0)
                {
                    _targetSelector.SelectTarget(index);
                }
            }
        }
    }

    private void HandlePrev()
    {
        if (!_isPlayerTurn) return;
        _targetSelector.SelectPrev();
    }

    private void HandleNext()
    {
        if (!_isPlayerTurn) return;
        _targetSelector.SelectNext();
    }

    private void Unsubscribe()
    {
        if (_battleController != null)
        {
            _battleController.OnTurnStart -= HandleTurnStart;
        }

        if (_playerInputSystem != null)
        {
            _playerInputSystem.OnNextInput         -= HandleNext;
            _playerInputSystem.OnPrevInput         -= HandlePrev;
            _playerInputSystem.OnSelectTargetInput -= HandleSelectTarget;
        }
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void HandleTurnStart(BattleUnit unit)
    {
        _isPlayerTurn = unit is PlayerBattleUnit;
    }

    /// <summary>
    /// 플레이어가 사용할 스킬 버튼을 누르면 대상과 사용한 스킬을 알림
    /// </summary>
    public void NotifyPlayerActed(int weaponIndex)
    {
        _battleController.OnPlayerAction(_targetSelector.CurrentTarget?.LinkedUnit as IDamageable, weaponIndex); // BattleController가 타겟 찾음
    }
}

}
