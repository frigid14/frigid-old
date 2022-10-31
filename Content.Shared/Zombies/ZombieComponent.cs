using Content.Shared.Roles;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Zombies;
[RegisterComponent, NetworkedComponent]
public sealed class ZombieComponent : Component
{
    /// <summary>
    /// The coefficient of the damage reduction applied when a zombie
    /// attacks another zombie. longe name
    /// </summary>
    [ViewVariables]
    public float OtherZombieDamageCoefficient = 0.5f;

    /// <summary>
    /// The amount of infection damage one zombie deals.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float ZombieInfectionDamage = 0.1f;

    [ViewVariables(VVAccess.ReadWrite)]
    public float ZombieMovementRandomSpeedMinimum = 0.2f;

    [ViewVariables(VVAccess.ReadWrite)]
    public float ZombieMovementRandomSpeedMaximum = 0.8f;

    /// <summary>
    /// The skin color of the zombie
    /// </summary>
    [DataField("skinColor")]
    public Color SkinColor = new(0.45f, 0.51f, 0.29f);

    /// <summary>
    /// The eye color of the zombie
    /// </summary>
    [DataField("eyeColor")]
    public Color EyeColor = new(0.96f, 0.13f, 0.24f);

    /// <summary>
    /// The base layer to apply to any 'external' humanoid layers upon zombification.
    /// </summary>
    [DataField("baseLayerExternal")]
    public string BaseLayerExternal = "MobHumanoidMarkingMatchSkin";

    /// <summary>
    /// The attack arc of the zombie
    /// </summary>
    [DataField("attackArc", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string AttackAnimation = "WeaponArcClaw";

    /// <summary>
    /// The role prototype of the zombie antag role
    /// </summary>
    [ViewVariables, DataField("zombieRoldId", customTypeSerializer: typeof(PrototypeIdSerializer<AntagPrototype>))]
    public readonly string ZombieRoleId = "Zombie";
}
