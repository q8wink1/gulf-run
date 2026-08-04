namespace GulfRun.Domain
{
    /// <summary>
    /// Sprint 23.9 — player action expected to clear an obstacle.
    /// Collision consequences are not implemented yet; this is authoring data only.
    /// </summary>
    public enum ObstacleRequiredAction
    {
        /// <summary>No specific action — typically switch lanes to avoid.</summary>
        None = 0,

        /// <summary>Jump over the obstacle.</summary>
        Jump = 1,

        /// <summary>Slide under the obstacle.</summary>
        Slide = 2,

        /// <summary>Change to a free lane.</summary>
        SwitchLane = 3
    }
}
