namespace GulfRun.Domain
{
    /// <summary>
    /// One player's post-race reward, computed host-side by
    /// <see cref="RaceRewardCalculator"/> so results "cannot be modified by
    /// players" (P011 RES-001/002) — a client only ever renders a value it
    /// received from the host, never computes its own. Broadcast to every
    /// client, but the Reward Screen presentation layer only ever displays
    /// the entry matching the local connection id, satisfying "players do
    /// not see other players' reward totals" without requiring a real
    /// per-connection unicast channel (none exists on <c>IMatchTransport</c>
    /// yet — see the Sprint 7 report for this documented simplification).
    /// </summary>
    public readonly struct RaceRewardBreakdown
    {
        public readonly int ConnectionId;
        public readonly int CoinsCollected;
        public readonly int BonusCoins;
        public readonly int RankPoints;
        public readonly int Experience;

        /// <summary>CoinsCollected + BonusCoins — the total coin-wallet credit for this race.</summary>
        public readonly int TotalReward;

        public RaceRewardBreakdown(int connectionId, int coinsCollected, int bonusCoins, int rankPoints, int experience, int totalReward)
        {
            ConnectionId = connectionId;
            CoinsCollected = coinsCollected;
            BonusCoins = bonusCoins;
            RankPoints = rankPoints;
            Experience = experience;
            TotalReward = totalReward;
        }
    }
}
