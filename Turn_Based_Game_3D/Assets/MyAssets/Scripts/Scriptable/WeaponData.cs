using UnityEngine;
using Assets.MyAssets.Scripts.Struct;

namespace Assets.MyAssets.Scripts.Scriptable
{

public enum WeaponType
{
    None = -1,
    Gun,
    Sword,
    Crossbow,
    Drone
}

public enum WeaponRangeType
{
    Melee,
    Ranged,
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Information")]
    public string WeaponName;
    public WeaponType weaponType;
    public WeaponRangeType rangeType;
    public string Description;
    public Sprite WeaponIcon;
    public GameObject WeaponPrefab; // 3D 프리뷰용

    [Header("Weapon Stats")]
    public int Damage;
    public int MaxAmmo;      // 총 종류만 사용
    public int StaminaCost;  // 근접 종류만 사용

    [Header("Bonus Stats")]
    public StatData bonusStat; // 무기 장착 시 추가 스탯


}

}
