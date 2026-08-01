using System.Collections.Generic;

namespace GulfRun.Domain
{
    /// <summary>
    /// Every field the Sprint 9 "Player Statistics" section lists, plus the
    /// running per-character play-count needed to resolve "Favourite
    /// Character" honestly (the character played in the most matches, not
    /// a hardcoded default) rather than inventing a fake value. Pure,
    /// engine-free accumulator — see
    /// <c>Features.Online.Statistics.PlayerStatisticsTracker</c> for the
    /// MonoBehaviour that owns one instance and feeds it from
    /// <c>Core.Services.PlayerStatEventService</c>.
    /// </summary>
    public sealed class PlayerMatchStatistics
    {
        private readonly Dictionary<string, int> _matchesByCharacter = new Dictionary<string, int>();
        private float _positionSum;
        private float _finishTimeSumSeconds;
        private int _timedMatches;

        public int MatchesPlayed { get; private set; }
        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public int Top3Finishes { get; private set; }
        public int CoinsCollected { get; private set; }
        public int WeaponsUsed { get; private set; }
        public int TrapsHit { get; private set; }
        public float DistanceRunMeters { get; private set; }
        public int JumpCount { get; private set; }
        public float BestFinishTimeSeconds { get; private set; } = -1f;

        public float WinRate => MatchesPlayed > 0 ? (float)Wins / MatchesPlayed : 0f;
        public float AveragePosition => MatchesPlayed > 0 ? _positionSum / MatchesPlayed : 0f;
        public float AverageFinishTimeSeconds => _timedMatches > 0 ? _finishTimeSumSeconds / _timedMatches : 0f;

        public void RecordMatch(PlayerMatchOutcome outcome, CharacterId characterPlayed)
        {
            MatchesPlayed++;

            if (outcome.FinishPosition == 1)
            {
                Wins++;
            }
            else if (outcome.FinishPosition > 1)
            {
                Losses++;
            }

            if (outcome.FinishPosition >= 1 && outcome.FinishPosition <= 3)
            {
                Top3Finishes++;
            }

            _positionSum += outcome.FinishPosition > 0 ? outcome.FinishPosition : 0;

            if (outcome.Reason == FinishReason.Completed)
            {
                _finishTimeSumSeconds += outcome.FinishTimeSeconds;
                _timedMatches++;
                if (BestFinishTimeSeconds < 0f || outcome.FinishTimeSeconds < BestFinishTimeSeconds)
                {
                    BestFinishTimeSeconds = outcome.FinishTimeSeconds;
                }
            }

            CoinsCollected += outcome.CoinsCollected;
            DistanceRunMeters += outcome.DistanceMetersReached;

            if (!characterPlayed.IsNone)
            {
                _matchesByCharacter.TryGetValue(characterPlayed.Value, out int count);
                _matchesByCharacter[characterPlayed.Value] = count + 1;
            }
        }

        public void RecordWeaponUsed() => WeaponsUsed++;

        public void RecordTrapHit() => TrapsHit++;

        public void RecordJump() => JumpCount++;

        /// <summary>The character played in the most matches so far, or <see cref="CharacterId.None"/> if none have been played yet.</summary>
        public CharacterId ResolveFavouriteCharacter()
        {
            string best = null;
            int bestCount = 0;
            foreach (KeyValuePair<string, int> entry in _matchesByCharacter)
            {
                if (entry.Value > bestCount)
                {
                    bestCount = entry.Value;
                    best = entry.Key;
                }
            }

            return best != null ? new CharacterId(best) : CharacterId.None;
        }
    }
}
