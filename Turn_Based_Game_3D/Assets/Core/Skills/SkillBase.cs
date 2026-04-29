public abstract class SkillBase
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }

    protected SkillBase(string name, string description)
    {
        Name = name;
        Description = description;
    }
}