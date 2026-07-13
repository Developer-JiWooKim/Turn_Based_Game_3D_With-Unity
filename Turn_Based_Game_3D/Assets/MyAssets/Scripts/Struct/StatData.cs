using System;

namespace Assets.MyAssets.Scripts.Struct
{

/// <summary>
/// 공용 스탯 구조체
/// </summary>
[Serializable]
public struct StatData
{
    public int Hp;

    public int Stamina;
    public int StaminaRecovery;

    public int AttackPower;
    public int DefenseValue;

    public int Speed;

    public int CritRate;         // 치명타 확률 (%, 0~100)
    public int CritDamageBonus;  // 치명타 시 추가 데미지 (%, 예: 50 -> 1.5배)
    public int Resistance;       // 디버프 저항력 (%) - 로그라이크 디버프 시스템 도입 전까지는 값만 보유
}

}
