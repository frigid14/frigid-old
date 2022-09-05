namespace Content.Server.Zombies
{
    [RegisterComponent]
    public sealed class ZombifyOnInitComponent : Component
    {
        /// <summary>
        /// Should we have a random appearance on spawn?
        /// </summary>
        public bool RandomAppearance = false;
    }
}
