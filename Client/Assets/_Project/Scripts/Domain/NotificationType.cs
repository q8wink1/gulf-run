namespace GulfRun.Domain
{
    /// <summary>The 7 notification categories from the Sprint 9 brief, plus the 5 Store/Economy categories added in Sprint 10 (appended — existing ordinals are untouched, and unlike <see cref="RewardType"/> nothing serializes this one by number in any catalog asset).</summary>
    public enum NotificationType
    {
        FriendRequest,
        TournamentStarting,
        TournamentEnding,
        RewardsReady,
        Promotion,
        Relegation,
        NewEvent,

        /// <summary>Sprint 10: a new Special Offer became active.</summary>
        NewOffer,

        /// <summary>Sprint 10: a Limited-Time Deal (Special Offer nearing expiry, or a Gem/Coin Limited Offer).</summary>
        LimitedTimeDeal,

        /// <summary>Sprint 10: the local player's active premium Battle Pass season is about to expire.</summary>
        BattlePassExpiring,

        /// <summary>Sprint 10: a new item was added to the Store catalog.</summary>
        NewStoreItem,

        /// <summary>Sprint 10: a Store purchase completed successfully.</summary>
        PurchaseSuccess
    }
}
