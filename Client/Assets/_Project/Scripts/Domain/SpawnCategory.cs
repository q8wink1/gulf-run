namespace GulfRun.Domain
{
    /// <summary>
    /// The four spawnable content categories requested by Sprint 3. Shared
    /// between chunk spawn-point tagging and spawn-table configuration so
    /// both sides of the spawning system speak the same vocabulary.
    /// </summary>
    public enum SpawnCategory
    {
        Obstacle,
        Coin,
        PowerUp,
        Decoration
    }
}
