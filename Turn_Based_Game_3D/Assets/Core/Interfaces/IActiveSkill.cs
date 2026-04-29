public interface IActiveSkill
{
    int StaminaCost { get; }
    string Name { get; }

    /// <summary>
    /// TODO#: Creature가 아닌 다른 매개변수로 받을 수도 있음
    /// </summary>
    bool CanUse(Creature caster);
    void Execute(Creature caster, IDamageable[] targets);
}