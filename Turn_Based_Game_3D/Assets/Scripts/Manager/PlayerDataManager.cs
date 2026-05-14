using System;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    private static PlayerDataManager _instance;
    public static PlayerDataManager Instance => _instance;

    [SerializeField] private PlayerData _playerData;

    private WeaponData[] _selectedWeapons = new WeaponData[3];

    public PlayerData PlayerData => _playerData;
    public WeaponData[] SelectedWeapons => _selectedWeapons;

    private void Awake() => Initialize();

    private void Initialize()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SelectWeapon(int slot, WeaponData weapon)
    {
        if (slot < 0 || weapon == null) return;

        _selectedWeapons[slot] = weapon;
    }

    public void ClearSelectWeapons()
    {
        _selectedWeapons = new WeaponData[3];
    }
}
