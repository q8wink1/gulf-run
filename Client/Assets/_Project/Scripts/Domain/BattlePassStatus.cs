using System.Collections.Generic;

namespace GulfRun.Domain
{
    /// <summary>
    /// The local player's live Battle Pass state — "Paid only" (Sprint 10
    /// brief), so there is no separate Free track, only a single premium
    /// progression gated by <see cref="IsPremiumUnlocked"/>. Pure data/logic,
    /// no Unity dependency, matching <see cref="SeasonProgress"/>'s style.
    /// </summary>
    public sealed class BattlePassStatus
    {
        private readonly HashSet<int> _claimedTiers = new HashSet<int>();

        public int SeasonNumber { get; }
        public bool IsPremiumUnlocked { get; set; }
        public int CurrentTier { get; set; }
        public int CurrentXp { get; set; }
        public double SeasonExpiresAtSeconds { get; set; }

        public BattlePassStatus(int seasonNumber)
        {
            SeasonNumber = seasonNumber;
        }

        public bool IsTierClaimed(int tier) => _claimedTiers.Contains(tier);

        public void MarkTierClaimed(int tier) => _claimedTiers.Add(tier);

        public IReadOnlyCollection<int> ClaimedTiers => _claimedTiers;
    }
}
