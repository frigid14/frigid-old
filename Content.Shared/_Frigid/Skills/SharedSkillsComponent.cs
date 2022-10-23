namespace Content.Shared._Frigid.Skills;

[RegisterComponent]
public sealed class SharedSkillsComponent : Component
{
    [ViewVariables]
    public List<SharedSkillSystem.SkillStruct> Skills { get; set; } = new();
}
