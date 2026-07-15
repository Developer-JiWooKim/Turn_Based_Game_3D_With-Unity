using System.Collections.Generic;
using UnityEngine;

namespace Assets.MyAssets.Scripts.Scriptable
{

[CreateAssetMenu(fileName = "RoguelikeChoicePoolData", menuName = "Scriptable Objects/RoguelikeChoicePoolData")]
public class RoguelikeChoicePoolData : ScriptableObject
{
    public List<RoguelikeChoiceData> AllChoices; // 9종 전부
    public float WeightPerInvestedPoint = 5f; // 성향 포인트 1점당 해당 카테고리 가중치 가산치
}

}
