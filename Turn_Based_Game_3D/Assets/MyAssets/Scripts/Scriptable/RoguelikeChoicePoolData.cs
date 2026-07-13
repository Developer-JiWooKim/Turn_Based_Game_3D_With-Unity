using System.Collections.Generic;
using UnityEngine;

namespace Assets.MyAssets.Scripts.Scriptable
{

[CreateAssetMenu(fileName = "RoguelikeChoicePoolData", menuName = "Scriptable Objects/RoguelikeChoicePoolData")]
public class RoguelikeChoicePoolData : ScriptableObject
{
    public List<RoguelikeChoiceData> AllChoices; // 9종 전부
}

}
