namespace Content.Server._Frigid.Safezone;

/// <summary>
/// Really only needed to identify the spawn point of the Safezone.
/// </summary>
[RegisterComponent]
public sealed class SafezoneIdentifierComponent : Component
{
    [DataField("safezone")] [ViewVariables] public bool Safezone { get; set; } = true;
}
