using GulfRun.Core.Managers;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.RaceFinish.Configuration;
using GulfRun.Features.RaceFinish.Standings;
using UnityEngine;

namespace GulfRun.Features.RaceFinish.Ceremony
{
    /// <summary>
    /// Victory Ceremony presentation: top 3 finishers together, 1st place
    /// centered with a bounce ("Champion Animation"), 2nd on the left, 3rd on
    /// the right — 4th place onward is never shown. Functional OnGUI
    /// placeholder — same "works in real builds today, replaced by a real
    /// Canvas once the Unity Editor is available" posture as
    /// <c>CountdownView</c>/<c>MultiplayerDebugView</c> (UI Toolkit is the
    /// project's eventual UI target per docs/02-architecture/TECHNICAL_STACK.md,
    /// but no sprint has authored real UI assets yet — see the Sprint 7
    /// report). Contains no gameplay logic; only reads
    /// <see cref="RaceStandingsTracker"/>.
    /// </summary>
    public sealed class PodiumCeremonyView : MonoBehaviour
    {
        [SerializeField] private RaceFinishConfig config;

        private GUIStyle _titleStyle;
        private GUIStyle _placeStyle;
        private GUIStyle _detailStyle;
        private bool _musicStartedThisCeremony;

        private void OnEnable()
        {
            MatchTransportService.Current.RaceEndPhaseChanged += HandlePhaseChanged;
        }

        private void OnDisable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            if (transport != null)
            {
                transport.RaceEndPhaseChanged -= HandlePhaseChanged;
            }
        }

        private void HandlePhaseChanged(RaceEndPhase phase)
        {
            if (phase == RaceEndPhase.Podium)
            {
                if (config != null && config.VictoryMusicClip != null && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayMusic(config.VictoryMusicClip);
                }

                _musicStartedThisCeremony = true;
            }
            else if (_musicStartedThisCeremony)
            {
                AudioManager.Instance?.StopMusic();
                _musicStartedThisCeremony = false;
            }
        }

        private void OnGUI()
        {
            RaceStandingsTracker standings = RaceStandingsTracker.Instance;
            if (standings == null || standings.CurrentPhase != RaceEndPhase.Podium || standings.FinalResults == null)
            {
                return;
            }

            EnsureStyles();

            GUI.Label(new Rect(0, 40, Screen.width, 50), "VICTORY CEREMONY", _titleStyle);

            float centerX = Screen.width * 0.5f;
            float championBounce = Mathf.Sin(Time.time * 3f) * 10f; // "Champion Animation" — simple bounce, replaced by real animation once art exists.

            DrawPlace(standings, 1, centerX - 110f, 150f + championBounce, "1ST — CHAMPION (Large Trophy)");
            DrawPlace(standings, 2, centerX - 360f, 220f, "2ND — SILVER MEDAL");
            DrawPlace(standings, 3, centerX + 140f, 220f, "3RD — BRONZE MEDAL");

            if (GUI.Button(new Rect(Screen.width - 170f, Screen.height - 60f, 150f, 40f), "Skip >>"))
            {
                MatchTransportService.Current.RequestSkipRaceEndPhase();
            }
        }

        private void DrawPlace(RaceStandingsTracker standings, int place, float x, float y, string label)
        {
            PlayerRaceResult? found = null;
            foreach (PlayerRaceResult result in standings.FinalResults)
            {
                if (result.FinishPosition == place)
                {
                    found = result;
                    break;
                }
            }

            if (found == null)
            {
                return;
            }

            PlayerRaceResult result2 = found.Value;
            IMatchTransport transport = MatchTransportService.Current;
            bool isLocal = transport != null && transport.LocalConnectionId == result2.ConnectionId;
            string displayName = ResolveDisplayName(result2.ConnectionId) + (isLocal ? " (You)" : string.Empty);

            GUI.Box(new Rect(x, y, 220f, 170f), string.Empty);
            GUI.Label(new Rect(x, y + 8f, 220f, 28f), label, _placeStyle);
            GUI.Label(new Rect(x, y + 44f, 220f, 26f), displayName, _detailStyle);
            GUI.Label(new Rect(x, y + 78f, 220f, 24f), $"Time: {result2.FinishTimeSeconds:F1}s", _detailStyle);
            GUI.Label(new Rect(x, y + 104f, 220f, 24f), result2.Reason == FinishReason.Completed ? "Finished" : "Eliminated", _detailStyle);
            GUI.Label(new Rect(x, y + 130f, 220f, 24f), $"Coins: {result2.CoinsCollected}", _detailStyle);
        }

        private static string ResolveDisplayName(int connectionId)
        {
            IMatchTransport transport = MatchTransportService.Current;
            if (transport == null)
            {
                return $"Player {connectionId}";
            }

            foreach (MatchParticipant participant in transport.Participants)
            {
                if (participant.Identity.ConnectionId == connectionId)
                {
                    return participant.Identity.DisplayName;
                }
            }

            return $"Player {connectionId}";
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 34,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _titleStyle.normal.textColor = Color.yellow;

            _placeStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _placeStyle.normal.textColor = Color.white;

            _detailStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleCenter
            };
            _detailStyle.normal.textColor = Color.white;
        }
    }
}
