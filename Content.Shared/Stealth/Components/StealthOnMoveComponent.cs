namespace Content.Shared.Stealth.Components
{
    /// <summary>
    ///     When added to an entity with stealth component, this component will change the visibility
    ///     based on the entity's (lack of) movement.
    /// </summary>
    [RegisterComponent]
    public sealed class StealthOnMoveComponent : Component
    {
        /// <summary>
        /// Rate that effects how fast an entity's visibility passively changes.
        /// </summary>
        [DataField("passiveVisibilityRate")]
        public readonly float PassiveVisibilityRate = -0.15f;

        /// <summary>
        /// Rate for movement induced visibility changes. Scales with distance moved.
        /// </summary>
        [DataField("movementVisibilityRate")]
        public readonly float MovementVisibilityRate = 0.2f;
    }
}
