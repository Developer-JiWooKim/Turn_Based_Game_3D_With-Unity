using UnityEngine;

namespace Assets.MyAssets.Scripts.Scriptable
{

    /// <summary>
    /// 전투 데미지 공식에서 쓰이는 밸런싱 상수 모음. 캐릭터/몬스터별 수치가 아닌
    /// 공식 자체의 기준치를 담당하며, 인스펙터에서 조정 가능하도록 SO로 분리했다.
    /// </summary>
    [CreateAssetMenu(fileName = "BattleBalanceData", menuName = "Scriptable Objects/BattleBalanceData")]
    public class BattleBalanceData : ScriptableObject
    {
        [Header("데미지 공식: 최종 데미지 = ATK x (DefenseConstant / (DefenseConstant + DEF))")]
        public int DefenseConstant = 100;
    }

}
