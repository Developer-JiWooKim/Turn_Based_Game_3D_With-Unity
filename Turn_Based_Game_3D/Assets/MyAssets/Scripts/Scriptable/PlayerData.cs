using UnityEngine;
using Assets.MyAssets.Scripts.Struct;

namespace Assets.MyAssets.Scripts.Scriptable
{

public enum CharacterType
{
    None = -999,
    Warrior = 0,
    Mage,
    Archer
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Player Information")]
    public string PlayerName;

    [Header("Stats")]
    public StatData playerStat;
}

}
