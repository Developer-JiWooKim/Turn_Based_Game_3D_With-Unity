using UnityEngine;

public abstract class UnitView : MonoBehaviour
{
    private BattleUnit _linkedUnit;

    protected UnitAnimator _unitAnimator;
    protected UnitHUD _unitHUD;

    public BattleUnit LinkedUnit => _linkedUnit;

    protected abstract void OnAwake();
    protected abstract void PlayHitAnim();
    protected abstract void PlayDeathAnim();


    private void Awake() => OnAwake();

    public void UnitLink(BattleUnit unit)
    {
        _linkedUnit = unit;
        _unitHUD?.SetUnitName(unit.Name);
        _unitHUD?.UpdateHp(unit.CurrentHp, unit.MaxHp);
    }

    public void OnDamaged(int damage)
    {
        _unitAnimator?.PlayHitAnim();
        _unitHUD?.UpdateHp(_linkedUnit.CurrentHp, _linkedUnit.MaxHp);
        _unitHUD?.ShowDamagePopup(damage);
    }

    public void OnDeath()
    {
        _unitAnimator?.PlayDeathAnim();
    }
}
