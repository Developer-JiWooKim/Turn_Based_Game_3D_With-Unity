using System;

/// <summary>
/// 공용 스탯 구조체
/// </summary>
[Serializable]
public struct StatData
{
    public int Hp;
    public int Mp;

    public uint AttackPower;
    public uint DefenseValue;

    public int Speed;
}
