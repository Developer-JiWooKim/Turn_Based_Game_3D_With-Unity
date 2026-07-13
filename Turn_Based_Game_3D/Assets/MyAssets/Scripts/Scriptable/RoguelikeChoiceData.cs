using UnityEngine;
using Assets.MyAssets.Scripts.Struct;

namespace Assets.MyAssets.Scripts.Scriptable
{

[CreateAssetMenu(fileName = "RoguelikeChoiceData", menuName = "Scriptable Objects/RoguelikeChoiceData")]
public class RoguelikeChoiceData : ScriptableObject
{
    [Header("Choice Information")]
    public RoguelikeChoiceCategory Category;
    public string ChoiceName;
    [TextArea] public string Description;
    public float BaseWeight; // 카테고리 등장 가중치 기본값 (Phase 8에서 영구 포인트로 조정 예정)

    [Header("Effect (카테고리별로 관련 필드만 사용)")]
    public StatData Bonus;          // AtkUp/SpdUp/DefensiveUp/CritUp 전용
    public int HealPercent;         // Heal 전용
    public int EnemyHpDownPercent;  // EnemyHpDownNextStage 전용
    public int EnemyAtkDownPercent; // EnemyAtkDownNextStage 전용
}

}
