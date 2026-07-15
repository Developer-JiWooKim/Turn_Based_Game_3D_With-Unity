using System;

namespace Assets.MyAssets.Scripts.Manager
{

/// <summary>
/// 런을 넘어 영구히 유지되는 진행 상황. JsonUtility 직렬화를 위한 순수 데이터 클래스.
/// </summary>
[Serializable]
public class MetaProgressData
{
    public int BestStageReached;
    public int PermanentPoints;
    public int[] CategoryInvestedPoints = new int[9]; // 인덱스 = (int)RoguelikeChoiceCategory
}

}
