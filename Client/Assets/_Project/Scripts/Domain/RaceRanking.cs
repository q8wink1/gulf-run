using System.Collections.Generic;

namespace GulfRun.Domain
{
    /// <summary>
    /// Pure final-ranking computation, run exactly once per race by the
    /// host-only <c>RaceFinishAuthority</c> after every participant has
    /// resolved (finished or been eliminated). Every player who completed the
    /// race ranks above every eliminated player regardless of timing (P010
    /// FIN-001/007: "ranked by order they cross the finish line" — an
    /// elimination is not a crossing); finishers are then ordered by
    /// <see cref="PlayerRaceResult.FinishTimeSeconds"/> ascending (resolves
    /// P010's open finish-line tie-break question), and eliminated players by
    /// how far they got before being cut (distance descending), with
    /// resolution order as the final, fully deterministic tie-break.
    /// </summary>
    public static class RaceRanking
    {
        public static List<PlayerRaceResult> ComputeFinalPositions(IReadOnlyCollection<PlayerRaceResult> resolvedResults)
        {
            var finishers = new List<PlayerRaceResult>();
            var eliminated = new List<PlayerRaceResult>();

            foreach (PlayerRaceResult result in resolvedResults)
            {
                if (result.Reason == FinishReason.Completed)
                {
                    finishers.Add(result);
                }
                else
                {
                    eliminated.Add(result);
                }
            }

            finishers.Sort((a, b) => a.FinishTimeSeconds.CompareTo(b.FinishTimeSeconds));
            eliminated.Sort((a, b) =>
            {
                int byDistanceDescending = b.DistanceMetersReached.CompareTo(a.DistanceMetersReached);
                return byDistanceDescending != 0 ? byDistanceDescending : a.ResolutionOrder.CompareTo(b.ResolutionOrder);
            });

            var ranked = new List<PlayerRaceResult>(finishers.Count + eliminated.Count);
            int position = 1;

            for (int i = 0; i < finishers.Count; i++)
            {
                ranked.Add(finishers[i].WithFinishPosition(position));
                position++;
            }

            for (int i = 0; i < eliminated.Count; i++)
            {
                ranked.Add(eliminated[i].WithFinishPosition(position));
                position++;
            }

            return ranked;
        }
    }
}
