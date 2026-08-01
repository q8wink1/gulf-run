namespace GulfRun.Domain
{
    /// <summary>One runner mark on the Race HUD minimap progress bar (0..1 along the track).</summary>
    public readonly struct RaceProgressMarker
    {
        public readonly int ConnectionId;
        public readonly float Progress01;
        public readonly bool IsLocal;
        public readonly bool HasFinished;

        public RaceProgressMarker(int connectionId, float progress01, bool isLocal, bool hasFinished)
        {
            ConnectionId = connectionId;
            Progress01 = progress01 < 0f ? 0f : (progress01 > 1f ? 1f : progress01);
            IsLocal = isLocal;
            HasFinished = hasFinished;
        }
    }
}
