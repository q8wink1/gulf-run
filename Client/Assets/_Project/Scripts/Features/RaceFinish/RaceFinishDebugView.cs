using System.Collections.Generic;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.RaceFinish.Standings;
using UnityEngine;

namespace GulfRun.Features.RaceFinish
{
    /// <summary>
    /// Debug overlay: Current Rank, Finish Time, Elimination Status, Reward
    /// Calculation, and the full final results table once known — plus a
    /// host-only button to simulate remote participants' race progress so
    /// the elimination/finish/ceremony flow can be exercised end-to-end
    /// under the offline loopback transport (same OnGUI-placeholder approach
    /// as MultiplayerDebugView/WeaponsDebugView/TrapsDebugView). Placed
    /// further right than TrapsDebugView's panel so all four can be shown at
    /// once.
    /// </summary>
    public sealed class RaceFinishDebugView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool showOnScreenDebug = true;
        [SerializeField] private int panelX = 1810;

        private readonly Dictionary<int, float> _simulatedBotDistance = new Dictionary<int, float>();

        private void OnGUI()
        {
            if (!showOnScreenDebug)
            {
                return;
            }

            int y = 10;
            const int lineHeight = 18;
            const int width = 420;

            void Line(string text)
            {
                GUI.Label(new Rect(panelX, y, width, lineHeight), text);
                y += lineHeight;
            }

            IMatchTransport transport = MatchTransportService.Current;
            RaceStandingsTracker standings = RaceStandingsTracker.Instance;
            int localId = transport != null ? transport.LocalConnectionId : -1;

            Line($"[Race Finish] Ceremony Phase (host, synchronized): {(standings != null ? standings.CurrentPhase.ToString() : "n/a")}  |  Local View: {(standings != null ? standings.LocalDisplayPhase.ToString() : "n/a")}");

            if (standings != null && standings.LiveResults.TryGetValue(localId, out PlayerRaceResult result))
            {
                string rankText = result.FinishPosition > 0 ? $"#{result.FinishPosition}" : $"resolved (order {result.ResolutionOrder}, not yet ranked)";
                Line($"Current Rank: {rankText}");
                Line($"Finish Time: {result.FinishTimeSeconds:F1}s  Reason: {result.Reason}  Distance: {result.DistanceMetersReached:F1}m");
            }
            else
            {
                Line("Current Rank: racing (not yet resolved)");
            }

            if (standings != null && standings.TryGetEliminationStatus(localId, out EliminationStatusEvent elim) && elim.Status != EliminationStatus.Safe)
            {
                Line($"Elimination Status: {elim.Status} ({elim.WarningSecondsRemaining}s)");
            }
            else
            {
                Line("Elimination Status: Safe");
            }

            if (standings != null && standings.TryGetReward(localId, out RaceRewardBreakdown reward))
            {
                Line($"Reward: Coins {reward.CoinsCollected} + Bonus {reward.BonusCoins} = {reward.TotalReward}  RankPts {reward.RankPoints}  XP {reward.Experience}");
            }

            if (standings != null && standings.FinalResults != null)
            {
                y += 6;
                Line("Final Results:");
                foreach (PlayerRaceResult r in standings.FinalResults)
                {
                    Line($"  #{r.FinishPosition} conn {r.ConnectionId}: {r.Reason} @ {r.FinishTimeSeconds:F1}s ({r.CoinsCollected} coins)");
                }
            }

            y += 6;
            DrawControls(transport, ref y, width);
        }

        private void DrawControls(IMatchTransport transport, ref int y, int width)
        {
            _ = width;

            if (transport == null || !transport.IsHost)
            {
                return;
            }

            const int buttonHeight = 24;
            const int buttonWidth = 300;

            if (GUI.Button(new Rect(panelX, y, buttonWidth, buttonHeight), "Simulate Remote Race Progress (+40m, +5 coins)"))
            {
                SimulateBotProgress(transport, 40f, 5);
            }

            y += buttonHeight + 4;
        }

        private void SimulateBotProgress(IMatchTransport transport, float distanceDeltaMeters, int coinsDelta)
        {
            if (!(transport is LocalLoopbackTransport loopback))
            {
                return;
            }

            foreach (MatchParticipant participant in loopback.Participants)
            {
                int connectionId = participant.Identity.ConnectionId;
                if (connectionId == loopback.LocalConnectionId)
                {
                    continue;
                }

                float previousDistance = _simulatedBotDistance.TryGetValue(connectionId, out float known) ? known : 0f;
                float newDistance = previousDistance + distanceDeltaMeters;
                _simulatedBotDistance[connectionId] = newDistance;

                var report = new RaceProgressReport(connectionId, newDistance, coinsDelta, Time.timeAsDouble);
                loopback.SimulateRemoteRaceProgress(report);
            }
        }
#endif
    }
}
