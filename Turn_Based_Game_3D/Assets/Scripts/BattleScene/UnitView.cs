using UnityEngine;

public abstract class UnitView : MonoBehaviour
{
    protected BattleController _battleController;

    protected BattleUnit     _linkedUnit;
    protected UnitAnimator   _unitAnimator;
    protected UnitHUD        _unitHUD;

    public BattleUnit LinkedUnit => _linkedUnit;

    protected abstract void OnAwake();

    private void Awake() => OnAwake();

    public void UnitLink(BattleUnit unit)
    {
        _linkedUnit = unit;

        _unitHUD?.SetUnitName(unit.Name);
        _unitHUD?.UpdateHp(unit.CurrentHp, unit.MaxHp);
    }

    public void Subscribe(BattleController battleController)
    {
        _battleController = battleController;

        _battleController.OnUnitDamaged += HandleUnitDamaged;
        _battleController.OnBattleEnd   += HandleBattleEnd;
    }

    private void OnDestroy() => Unsubscribe();
    private void Unsubscribe()
    {
        if (_battleController == null) return;

        _battleController.OnUnitDamaged -= HandleUnitDamaged;
        _battleController.OnBattleEnd   -= HandleBattleEnd;
    }

    private async void HandleUnitDamaged(IDamageable unit, int damage)
    {
        if (unit != _linkedUnit) return;

        await OnDamagedAsync(damage);
    }

    private void HandleBattleEnd(bool result)
    {
        // TODO#: 전투 종료 시 처리 (애니메이션 등)
        Debug.Log($"전투 결과 : {result} / HandleBattleEnd 호출:전투 종료 시 처리 (애니메이션 등)");
    }

    public async Awaitable OnDamagedAsync(int damage)
    {
        if (_unitAnimator != null)
            await _unitAnimator.PlayHitAnimAsync();

        _unitHUD?.UpdateHp(_linkedUnit.CurrentHp, _linkedUnit.MaxHp);
        _unitHUD?.ShowDamagePopup(damage);
    }

    public async Awaitable OnDeathAsync()
    {
        if (_unitAnimator != null)
            await _unitAnimator.PlayDeathAnimAsync();
    }

    public virtual void Reset()
    {
        // BattleController 구독 해제
        Unsubscribe();

        _battleController = null;
        _linkedUnit       = null;

        // HUD 초기화
        _unitHUD?.Reset();
    }
}
