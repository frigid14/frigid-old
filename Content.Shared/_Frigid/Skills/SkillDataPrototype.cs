using Robust.Shared.Prototypes;

namespace Content.Shared._Frigid.Skills;

[Prototype("skill")]
public sealed class SkillDataPrototype : IPrototype
{
    [ViewVariables] [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// The name of this skill
    /// </summary>
    [ViewVariables]
    [DataField("name")]
    public string Name { get; } = default!;

    /// <summary>
    /// The maximum this skill can reach.
    /// </summary>
    [ViewVariables]
    [DataField("maxValue")]
    public ushort MaxValue { get; } = 10;

    [ViewVariables]
    [DataField("defaultValue")]
    public ushort DefaultValue { get; } = 0;

    /// <summary>
    /// Whether this skill is to be displayed in the client's skills menu.
    /// </summary>
    [ViewVariables]
    [DataField("display")]
    public bool DisplayInSkills { get; } = true;
}
