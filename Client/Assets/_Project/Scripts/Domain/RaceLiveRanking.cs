using System.Collections.Generic;

namespace GulfRun.Domain
{
    /// <summary>
    /// Pure live-place computation from distance reports (finishers always
    /// rank above anyone still racing). Used by the Race HUD position chip
    /// and minimap before <see cref="RaceRanking.ComputeFinalPositions"/> runs.
    /// </summary>
    public static class RaceLiveRanking
    {
        public static int ComputeLocalPlace(
            int localConnectionId,
            IReadOnlyDictionary<int, float> distancesByConnection,
            IReadOnlyCollection<int> finishedConnectionIds)
        {
            if (distancesByConnection == null || distancesByConnection.Count == 0)
            {
                return 1;
            }

            if (!distancesByConnection.TryGetValue(localConnectionId, out float localDistance))
            {
                localDistance = 0f;
            }

            bool localFinished = finishedConnectionIds != null && Contains(finishedConnectionIds, localConnectionId);
            int place = 1;

            foreach (KeyValuePair<int, float> pair in distancesByConnection)
            {
                if (pair.Key == localConnectionId)
                {
                    continue;
                }

                bool otherFinished = finishedConnectionIds != null && Contains(finishedConnectionIds, pair.Key);
                if (RanksAhead(otherFinished, pair.Value, localFinished, localDistance))
                {
                    place++;
                }
            }

            return place;
        }

        private static bool RanksAhead(bool otherFinished, float otherDistance, bool localFinished, float localDistance)
        {
            if (otherFinished && !localFinished)
            {
                return true;
            }

            if (!otherFinished && localFinished)
            {
                return false;
            }

            if (otherDistance > localDistance)
            {
                return true;
            }

            return false;
        }

        private static bool Contains(IReadOnlyCollection<int> ids, int id)
        {
            foreach (int value in ids)
            {
                if (value == id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
