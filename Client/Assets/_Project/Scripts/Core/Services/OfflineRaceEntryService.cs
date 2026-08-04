namespace GulfRun.Core.Services
{
    /// <summary>
    /// Sprint 23.13 — cross-scene flag for the offline Quick Play prototype.
    /// Quick Play sets this before LoadingScreen; LoadingScreen auto-advances
    /// to Gameplay; Gameplay bootstrap starts the local race. Not networking.
    /// </summary>
    public static class OfflineRaceEntryService
    {
        /// <summary>Default LoadingScreen dwell before Gameplay (seconds).</summary>
        public const float DefaultLoadingSeconds = 2.5f;

        /// <summary>True while an offline Quick Play race session is active.</summary>
        public static bool IsActive { get; private set; }

        /// <summary>True until LoadingScreen consumes the auto-advance timer.</summary>
        public static bool PendingLoadingAutoAdvance { get; private set; }

        public static void BeginPendingEntry()
        {
            IsActive = true;
            PendingLoadingAutoAdvance = true;
        }

        /// <summary>
        /// Consumes the loading auto-advance flag. Returns true once per entry
        /// so LoadingScreen can start its 2–3s timer.
        /// </summary>
        public static bool ConsumeLoadingAutoAdvance()
        {
            if (!PendingLoadingAutoAdvance)
            {
                return false;
            }

            PendingLoadingAutoAdvance = false;
            return true;
        }

        public static void Clear()
        {
            IsActive = false;
            PendingLoadingAutoAdvance = false;
        }
    }
}
