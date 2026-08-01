namespace GulfRun.Domain
{
    /// <summary>One active effect chip for the Race HUD duration bar row.</summary>
    public readonly struct ActiveHudEffectSnapshot
    {
        public readonly HudEffectKind Kind;
        public readonly float RemainingSeconds;
        public readonly float TotalSeconds;

        public ActiveHudEffectSnapshot(HudEffectKind kind, float remainingSeconds, float totalSeconds)
        {
            Kind = kind;
            RemainingSeconds = remainingSeconds;
            TotalSeconds = totalSeconds;
        }

        public float NormalizedRemaining =>
            TotalSeconds > 0f
                ? (float)System.Math.Max(0d, System.Math.Min(1d, RemainingSeconds / TotalSeconds))
                : 0f;
    }
}
