using UnityEngine;
using Assets.MyAssets.Scripts.Struct;

namespace Assets.MyAssets.Scripts.Scriptable
{

public enum CharacterType
{
    None = -999,
    Knight = 0,
    Barbarian = 1,
    RogueDagger = 2,
    RogueCrossbow = 3,
    Mage = 4,
    Ranger = 5,
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Player Information")]
    public string PlayerName;
    public CharacterType Character;
    public WeaponData RepresentativeWeapon;
    public GameObject CharacterPrefab; // 3D 프리뷰/전투 스폰용, 프리팹 준비되면 인스펙터에서 연결

    [Header("Stats")]
    public StatData playerStat;
}

}
