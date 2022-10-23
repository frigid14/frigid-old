using Robust.Shared.Prototypes;

namespace Content.Shared._Frigid.Skills;

[Prototype("skill")]
public sealed class SkillDataPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; } = default!;

    /// <summary>
    /// The name of this skill
    /// </summary>
    [ViewVariables]
    [DataField("name")]
    public string Name { get; } = default!;

    /// <summary>
    /// The maximum level this skill can reach.
    /// </summary>
    [ViewVariables]
    [DataField("maxLevel")]
    public ushort MaxLevel { get; } = 10;

    /// <summary>
    /// Default skill level.
    /// </summary>
    [ViewVariables]
    [DataField("defaultLevel")]
    public ushort DefaultLevel { get; } = 0;

    /// <summary>
    /// Default XP.
    /// </summary>
    [ViewVariables]
    [DataField("defaultExp")]
    public ushort DefaultXP { get; } = 0;

    /// <summary>
    /// The limit to reach in order to level up.
    /// </summary>
    [ViewVariables]
    [DataField("maxXp")]
    public ushort MaxExperience { get; } = 100;

    /// <summary>
    /// Whether this skill is to be displayed in the client's skills menu.
    /// </summary>
    [ViewVariables]
    [DataField("display")]
    public bool DisplayInSkills { get; } = true;
}
