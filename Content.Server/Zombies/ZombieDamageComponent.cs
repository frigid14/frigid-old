namespace Content.Server.Zombies;

[RegisterComponent]
public sealed class ZombieDamageComponent : Component
{
    /// <summary>
    /// The accumulated infection damage, reaching 1f will immediately infect you. Antibiotics can lower this damage.
    /// </summary>
    [ViewVariables] public float InfectionDamage = 0f;
}
