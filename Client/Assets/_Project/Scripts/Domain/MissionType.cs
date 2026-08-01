namespace GulfRun.Domain
{
    /// <summary>
    /// The fixed vocabulary of trackable player actions a Daily Mission can
    /// target — the same "fixed enum vocabulary, data-driven instances"
    /// split <see cref="WeaponId"/>/<see cref="TrapId"/> already use: a new
    /// fundamental action type needs a new member here (and a new
    /// <c>Core.Services.PlayerStatEventService</c> hook), but every actual
    /// Daily Mission (its target amount, difficulty, and reward) is
    /// authored data in <c>Features.Progression.Configuration.MissionPoolCatalogConfig</c>,
    /// never a code change. Covers all 9 brief example mission types.
    /// </summary>
    public enum MissionType
    {
        /// <summary>"Finish 3 races" — completed (not eliminated) races.</summary>
        FinishRaces,

        /// <summary>"Win 1 race" — 1st place finishes.</summary>
        WinRaces,

        /// <summary>"Collect 100 coins" — Coins collected on-track during a race (see <see cref="PlayerMatchOutcome.CoinsCollected"/>), not Store Coin Pack purchases.</summary>
        CollectCoins,

        /// <summary>"Open 5 Item Boxes."</summary>
        OpenItemBoxes,

        /// <summary>"Use 8 Weapons" — confirmed weapon activations.</summary>
        UseWeapons,

        /// <summary>"Avoid 10 Traps" — an active trap instance expiring without ever hitting the local player (see Sprint 11 report for this honestly-scoped "avoided" definition).</summary>
        AvoidTraps,

        /// <summary>"Jump 30 times" — confirmed ground/double jumps.</summary>
        PerformJumps,

        /// <summary>"Reach Top 3 twice" — top-3 race finishes.</summary>
        ReachTopThree,

        /// <summary>"Login today" — satisfied once per day the local player opens the game, independent of the Login Streak claim.</summary>
        LoginToday
    }
}
