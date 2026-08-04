namespace GulfRun.Domain
{
    /// <summary>
    /// Sprint 23.12 — how coins (and optional gems) are laid out from a spawn marker.
    /// </summary>
    public enum CollectiblePattern
    {
        /// <summary>One collectible at the marker pose.</summary>
        Single = 0,
        /// <summary>Several along +Z at the marker lane.</summary>
        Line = 1,
        /// <summary>Arc across Left / Center / Right lanes.</summary>
        Arc = 2
    }
}
