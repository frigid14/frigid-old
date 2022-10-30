namespace Content.Server._Frigid.Safezone;

/// <summary>
/// Really only needed to identify the spawn point of the Safezone.
/// </summary>
[RegisterComponent]
public sealed class SafezoneIdentifierComponent : Component
{
    /// <summary>
    /// Whether this is a Safezone teleport or a regular Zone teleport
    /// </summary>
    [DataField("safezone")]
    [ViewVariables]
    public bool Safezone { get; set; } = true;
}
