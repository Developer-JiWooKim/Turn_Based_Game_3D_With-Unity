using UnityEngine;

public class PlayerUnitView : UnitView
{
    [SerializeField] private GameObject _damagePopupPrefab;

    private DamagePopup _damagePopup;

    protected override void OnAwake()
    {
        _unitAnimator = GetComponent<UnitAnimator>();
        _unitHUD      = GetComponentInChildren<UnitHUD>();

        // 데미지 팝업 미리 생성
        if (_damagePopupPrefab != null)
        {
            GameObject popup = Instantiate(_damagePopupPrefab, transform.position, Quaternion.identity);
            _damagePopup = popup.GetComponent<DamagePopup>();
            _damagePopup.gameObject.SetActive(false);
        }
    }
    protected override void ShowDamagePopup(int damage)
    {
        if (_damagePopup == null) return;

        _damagePopup.transform.position = transform.position + Vector3.up * 2f;
        _damagePopup.ShowDamagePopup(damage);
    }

    public async Awaitable PlayAttackAnimAsync(int weaponIndex, Transform target = null)
    {
        PlayerBattleUnit player = _linkedUnit as PlayerBattleUnit;
        if (player == null) return;

        WeaponData weapon = player.Weapons[weaponIndex];
        // WeaponType weaponType = player.Weapons[weaponIndex].weaponType; TODO#: 지울예정
        //await (_unitAnimator as PlayerAnimator)?.PlayAttackAnimAsync(weaponType, target);
        await (_unitAnimator as PlayerAnimator)?.PlayAttackAnimAsync(weapon.weaponType, weapon.rangeType, target);
    }
}
