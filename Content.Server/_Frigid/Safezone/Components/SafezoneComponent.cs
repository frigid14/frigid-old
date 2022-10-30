using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server._Frigid.Safezone;

[RegisterComponent]
public sealed class SafezoneComponent : Component
{
    [DataField("startTeleportSound")]
    public SoundSpecifier StartTeleportSound { get; set; } = new SoundPathSpecifier("/Audio/Effects/chime.ogg");

    [DataField("endTeleportSound")]
    public SoundSpecifier EndTeleportSound { get; set; } = new SoundPathSpecifier("/Audio/Magic/disintegrate,igg");

    /// <summary>
    /// Whether the current user is entering the safezone, used to stop spamming the doafter.
    /// </summary>
    [ViewVariables] public bool EnteringSafezone { get; set; } = false;

    /// <summary>
    /// Whether the current user is in or out of the Safezone, defaults to false.
    /// </summary>
    [ViewVariables] public bool InSafezone { get; set; } = false;

    [DataField("actionId", customTypeSerializer:typeof(PrototypeIdSerializer<InstantActionPrototype>))]
    public string ActionId = "SafezoneTeleport";
}
public sealed class SafezoneTeleportActionEvent : InstantActionEvent
{

};

