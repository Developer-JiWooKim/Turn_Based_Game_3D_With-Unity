using System;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    private static PlayerDataManager _instance;
    public static PlayerDataManager Instance => _instance;

    [SerializeField] private PlayerData _playerData;

    private WeaponData[] _selectedWeapons = new WeaponData[3];

    private int _currentLevel = 1;
    private int _currentExp = 0;

    // #TODO: 현재는 테스트를 위해 하드 코딩으로 레벨업 시 올라갈 수치 입력
    private const int HpPerLevel = 20;
    private const int AtkPerLevel = 3;
    private const int SpdPerLevel = 1;

    public int CurrentLevel             => _currentLevel;
    public int CurrentExp               => _currentExp;
    public PlayerData PlayerData        => _playerData;
    public WeaponData[] SelectedWeapons => _selectedWeapons;

    public event Action<int> OnLevelUp; // 현재 레벨 전달

    private void Awake() => Initialize();
    private void Initialize()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
    }

    public void SelectWeapon(int slot, WeaponData weapon)
    {
        if (slot < 0 || slot >= _selectedWeapons.Length || weapon == null) return;

        _selectedWeapons[slot] = weapon;
    }

    public void ClearSelectWeapons()
    {
        Array.Clear(_selectedWeapons, 0, _selectedWeapons.Length);
    }

    public void LevelUp()
    {
        _currentLevel++;
        OnLevelUp?.Invoke(_currentLevel);
    }

    // 레벨에 따른 스탯 증가값 반환
    public int GetBonusHp() => (_currentLevel - 1) * HpPerLevel;
    public int GetBonusAtk() => (_currentLevel - 1) * AtkPerLevel;
    public int GetBonusSpd() => (_currentLevel - 1) * SpdPerLevel;

    // 게임 오버 시 초기화
    public void ResetPlayerProgress()
    {
        _currentLevel = 1;
        _currentExp = 0;
    }
}
