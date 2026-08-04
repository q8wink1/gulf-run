namespace GulfRun.Domain
{
    /// <summary>
    /// Sprint 23.9 — obstacle category for the 3-lane runner.
    /// Engine-free so spawn weighting / design tools can reason without Unity.
    /// </summary>
    public enum ObstacleType
    {
        /// <summary>Fixed barrier; typically dodge by lane change.</summary>
        Static = 0,

        /// <summary>Moves along a path; motion logic is future work.</summary>
        Moving = 1,

        /// <summary>Cleared by jumping over.</summary>
        Jump = 2,

        /// <summary>Cleared by sliding under.</summary>
        Slide = 3
    }
}
