namespace GulfRun.Domain
{
    /// <summary>The 10 reward categories from the Sprint 9 brief, plus <see cref="ExclusiveEmote"/> added in Sprint 10 so the Battle Pass's "Exclusive Emotes" line (a category Sprint 9 didn't need) has a real reward type. Appended at the end — every existing ordinal (0-9), and every catalog `.asset` that already serializes them by number, is unaffected.</summary>
    public enum RewardType
    {
        Coins,
        Gems,
        ExclusiveSkin,
        ExclusiveOutfit,
        VictoryPose,
        Title,
        Badge,
        ProfileFrame,
        ChampionEffect,
        LimitedCosmetic,

        /// <summary>Added in Sprint 10 for Battle Pass "Exclusive Emotes" tiers.</summary>
        ExclusiveEmote,

        /// <summary>Added in Sprint 11 for Daily Mission / Login Reward entries that grant Battle Pass XP directly (brief: "Mission rewards include: ... Battle Pass XP").</summary>
        BattlePassXp
    }
}
