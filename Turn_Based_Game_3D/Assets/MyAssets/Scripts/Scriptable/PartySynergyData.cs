using System.Collections.Generic;
using UnityEngine;
using Assets.MyAssets.Scripts.Struct;

namespace Assets.MyAssets.Scripts.Scriptable
{

[System.Serializable]
public class SynergyEntry
{
    public CharacterType Character;
    public int RequiredCount = 2;
    public StatData Bonus;
}

[CreateAssetMenu(fileName = "PartySynergyPoolData", menuName = "Scriptable Objects/PartySynergyPoolData")]
public class PartySynergyPoolData : ScriptableObject
{
    public List<SynergyEntry> Entries; // 6종 각 1개
}

}
