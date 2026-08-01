namespace GulfRun.Features.Intro
{
    /// <summary>
    /// Sprint 14 "GULFRUN BRAND INTRO": the one source of truth for every
    /// cue's timing (brief: "Duration: 2–3 seconds"). Every Intro view
    /// (<see cref="IntroBackgroundView"/>, <see cref="IntroFalconView"/>,
    /// <see cref="IntroLogoView"/>) and <see cref="IntroSequenceController"/>
    /// reads these same constants instead of hardcoding overlapping magic
    /// numbers per file (Code Quality: "No hardcoded values"). All values
    /// are seconds elapsed since the Intro scene started
    /// (<see cref="IntroSequenceController.ElapsedSeconds"/>).
    /// </summary>
    public static class IntroTimeline
    {
        /// <summary>Dunes + wind particles are fully visible by this time.</summary>
        public const float DunesFadeInEnd = 0.30f;

        /// <summary>"A Falcon appears flying across the screen."</summary>
        public const float FalconFlyAcrossStart = 0.10f;
        public const float FalconFlyAcrossEnd = 1.20f;

        /// <summary>"A Palm Tree silhouette slowly appears."</summary>
        public const float PalmTreeFadeInStart = 0.50f;
        public const float PalmTreeFadeInEnd = 1.10f;

        /// <summary>"The Falcon circles above the dunes."</summary>
        public const float FalconCircleStart = FalconFlyAcrossEnd;
        public const float FalconCircleEnd = 2.30f;

        /// <summary>"The GulfRun logo fades in with a premium golden shine."</summary>
        public const float LogoFadeInStart = 1.20f;
        public const float LogoFadeInEnd = 2.00f;
        public const float ShineSweepStart = 1.90f;
        public const float ShineSweepEnd = 2.55f;

        /// <summary>Natural end of the sequence if never skipped — kept inside the "2–3 seconds" brief.</summary>
        public const float SequenceEnd = 2.65f;

        /// <summary>"Smooth fade into the Main Lobby" duration.</summary>
        public const float FadeToBlackDuration = 0.35f;

        /// <summary>Minimum time before the Skip prompt can appear, so it never flashes on the very first frame.</summary>
        public const float SkipButtonGraceSeconds = 0.30f;
    }
}
