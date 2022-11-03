namespace Content.Server._Frigid.Staggerable;

/// <summary>
/// Used to define a humanoid that can be staggered.
/// </summary>
[RegisterComponent, Access(typeof(StaggerableSystem))]
public sealed class StaggerableComponent : Component
{
    /// <summary>
    /// How many times has this component been staggered?
    /// </summary>
    public bool Staggered { get; set; } = false;
}
