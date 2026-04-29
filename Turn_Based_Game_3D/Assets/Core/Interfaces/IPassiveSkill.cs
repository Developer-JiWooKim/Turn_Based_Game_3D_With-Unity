
public interface IPassiveSkill
{

    bool IsActivated { get; }               // 패시브 스킬이 활성화되어 있는지 여부

    bool CheckCondition(Creature owner);    // 패시브 스킬이 발동 조건을 충족하는지 확인하는 메서드
    void Apply(Creature owner);             // 패시브 스킬의 효과를 적용하는 메서드
    void TryApply(Creature owner);          // 패시브 스킬의 발동 조건을 확인하고, 조건이 충족되면 효과를 적용하는 메서드
    void Reset();                           // 패시브 스킬의 상태를 초기화하는 메서드 (예: 전투 종료 시)
}
