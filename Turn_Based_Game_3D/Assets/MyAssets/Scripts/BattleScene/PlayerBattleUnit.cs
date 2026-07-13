using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;
using Assets.MyAssets.Scripts.Manager;

namespace Assets.MyAssets.Scripts.BattleScene
{

public class PlayerBattleUnit : BattleUnit
{
    private PartyMember _partyMember;
    public PartyMember PartyMember => _partyMember;
    public PlayerData PlayerData => _partyMember.Data;
    public WeaponData[] Weapons { get; private set; }

    // 런타임 총알 수 관리
    private int[] _currentAmmos;
    public int[] CurrentAmmos => _currentAmmos;

    public PlayerBattleUnit(PartyMember partyMember, WeaponData[] weapons) : base(partyMember.Data)
    {
        _partyMember = partyMember;
        Weapons      = weapons;

        _currentAmmos = new int[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            _currentAmmos[i] = weapons[i].MaxAmmo;
        }
    }

    public bool CanUseWeapon(int weaponIndex)
    {
        WeaponData weapon = Weapons[weaponIndex];
        return CurrentStamina >= weapon.StaminaCost;
    }

    public void UseWeapon(int weaponIndex)
    {
        WeaponData weapon = Weapons[weaponIndex];
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

}
