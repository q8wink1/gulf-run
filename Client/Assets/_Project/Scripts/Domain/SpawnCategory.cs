namespace GulfRun.Domain
{
    /// <summary>
    /// The spawnable content categories. Shared between chunk spawn-point
    /// tagging and spawn-table configuration so both sides of the spawning
    /// system speak the same vocabulary. <see cref="ItemBox"/> was added in
    /// Sprint 5 — Mystery Item Boxes reuse the exact same pooled,
    /// weighted-random, per-chunk spawn pipeline built in Sprint 3
    /// (<c>ChunkContentSpawner</c>/<c>SpawnCategoryConfig</c>) instead of a
    /// parallel spawning system, which is also what makes "spawn locations
    /// change every match" and "boxes respawn using configurable rules"
    /// true for free (chunks are procedurally generated/recycled, and the
    /// category's <c>BaseSpawnChance</c>/per-entry weights are the
    /// configurable respawn rules). <see cref="Npc"/> was added in Sprint 23.6
    /// for 3D track marker placeholders (not spawned yet).
    /// </summary>
    public enum SpawnCategory
    {
        Obstacle,
        Coin,
        PowerUp,
        Decoration,
        ItemBox,
        /// <summary>Sprint 23.6 track markers — NPC placeholders; not spawned yet.</summary>
        Npc
    }
}
