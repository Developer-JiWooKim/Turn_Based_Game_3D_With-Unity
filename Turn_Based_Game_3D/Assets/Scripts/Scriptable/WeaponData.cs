using UnityEngine;

public enum WeaponType
{
    None = -1,
    Rifle,
    Sword,
    Hammer,
    Drone
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Information")]
    public string WeaponName;
    public WeaponType weaponType;
    public string Description;
    public Sprite WeaponIcon;
    public GameObject WeaponPrefab; // 3D 프리뷰용

    [Header("Weapon Skill")]
    public PlayerSkillData skill; // 이 무기의 스킬

    [Header("Bonus Stats")]
    public StatData bonusStat; // 무기 장착 시 추가 스탯
}
