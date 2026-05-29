using UnityEngine;

public class PlayerBattleUnit : BattleUnit
{
    private PlayerData _playerData;
    public PlayerData PlayerData => _playerData;
    public WeaponData[] Weapons { get; private set; }

    // 런타임 총알 수 관리
    private int[] _currentAmmos;
    public int[] CurrentAmmos => _currentAmmos;

    public PlayerBattleUnit(PlayerData data, WeaponData[] weapons) : base(data)
    {
        _playerData = data;
        Weapons = weapons;

        _currentAmmos = new int[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            _currentAmmos[i] = weapons[i].MaxAmmo;
        }
    }

    public bool CanUseWeapon(int weaponIndex)
    {
        WeaponData weapon = Weapons[weaponIndex];

        if (weapon.weaponType == WeaponType.Gun)
            return _currentAmmos[weaponIndex] > 0;
        else
            return CurrentStamina >= weapon.StaminaCost;
    }

    public void UseWeapon(int weaponIndex)
    {
        WeaponData weapon = Weapons[weaponIndex];

        if (weapon.weaponType == WeaponType.Gun)
            _currentAmmos[weaponIndex]--;
        else
            UseStamina(weapon.StaminaCost);
    }

    public bool HasAnyUsableWeapon()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            bool canUse = CanUseWeapon(i);

            if (canUse) return true;
        }
        return false;
    }

    public void ResetAmmos()
    {
        for (int i = 0; i < Weapons.Length; i++)
            _currentAmmos[i] = Weapons[i].MaxAmmo;
    }
}
