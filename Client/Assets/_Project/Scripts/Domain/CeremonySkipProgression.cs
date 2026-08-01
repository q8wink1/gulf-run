namespace GulfRun.Domain
{
    /// <summary>
    /// Pure per-client "how far through the ceremony have I individually
    /// skipped ahead" progression — the mechanism behind the Sprint 7
    /// addendum's "players may skip the ceremony individually; skipping
    /// does not interrupt other players" rule. Each client tracks its own
    /// rank (Podium=0, Reward=1, Done=2) independently of the host-broadcast
    /// <see cref="RaceEndPhase"/>: pressing Skip advances the local rank by
    /// one without touching the host's clock (<see cref="AdvanceRank"/>),
    /// while an incoming host phase change only ever pulls the local rank
    /// forward, never back (<see cref="SyncRank"/>) — a client that skipped
    /// ahead of the host stays ahead until the host catches up.
    /// </summary>
    public static class CeremonySkipProgression
    {
        private const int PodiumRank = 0;
        private const int RewardRank = 1;
        private const int DoneRank = 2;

        public static int RankOf(RaceEndPhase phase)
        {
            switch (phase)
            {
                case RaceEndPhase.Podium: return PodiumRank;
                case RaceEndPhase.Reward: return RewardRank;
                default: return DoneRank;
            }
        }

        public static RaceEndPhase PhaseOfRank(int rank)
        {
            if (rank <= PodiumRank)
            {
                return RaceEndPhase.Podium;
            }

            return rank == RewardRank ? RaceEndPhase.Reward : RaceEndPhase.None;
        }

        public static int AdvanceRank(int currentRank) => currentRank >= DoneRank ? DoneRank : currentRank + 1;

        /// <summary>Never regresses: the higher of the client's own progress and the host's broadcast phase.</summary>
        public static int SyncRank(int localRank, int hostRank) => localRank > hostRank ? localRank : hostRank;
    }
}
