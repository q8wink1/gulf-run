using GulfRun.Core.Managers;
using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.RaceFinish.Configuration;
using GulfRun.Features.RaceFinish.Standings;
using UnityEngine;

namespace GulfRun.Features.RaceFinish.Ceremony
{
    /// <summary>
    /// Victory Ceremony presentation: top 3 finishers together — 1st place
    /// centered with a Golden Trophy, a ceremony-only Gulf Bisht overlay,
    /// a Champion celebration bounce, golden confetti, and special victory
    /// music; 2nd on the left with a Silver Medal, national flag, and
    /// celebration pulse; 3rd on the right with a Bronze Medal, national
    /// flag, and celebration pulse (Sprint 7 addendum). 4th place onward is
    /// never shown. Every flag is resolved from the finisher's own
    /// <see cref="PlayerIdentity.Country"/> via <see cref="FlagCatalogConfig"/>
    /// and animates with a gentle sway (<see cref="CelebrationAnimation"/>).
    ///
    /// Renders against <see cref="RaceStandingsTracker.LocalDisplayPhase"/>,
    /// not the raw host-broadcast <see cref="RaceStandingsTracker.CurrentPhase"/>,
    /// so pressing Skip only ever affects this one client ("players may skip
    /// the ceremony individually; skipping does not interrupt other
    /// players") — every other client keeps watching the identical,
    /// synchronized podium sequence for the same duration as always.
    ///
    /// Functional OnGUI placeholder — same "works in real builds today,
    /// replaced by a real Canvas once the Unity Editor is available" posture
    /// as <c>CountdownView</c>/<c>MultiplayerDebugView</c> (UI Toolkit is the
    /// project's eventual UI target per docs/02-architecture/TECHNICAL_STACK.md,
    /// but no sprint has authored real UI assets yet). Contains no gameplay
    /// logic; only reads <see cref="RaceStandingsTracker"/>.
    /// </summary>
    public sealed class PodiumCeremonyView : MonoBehaviour
    {
        [SerializeField] private RaceFinishConfig config;
        [SerializeField] private FlagCatalogConfig flagCatalog;

        private GUIStyle _titleStyle;
        private GUIStyle _placeStyle;
        private GUIStyle _detailStyle;
        private GUIStyle _bishtStyle;
        private RaceEndPhase _lastLocalDisplayPhase = RaceEndPhase.None;
        private double _localPodiumStartSeconds;
        private bool _musicPlaying;

        private void Update()
        {
            RaceStandingsTracker standings = RaceStandingsTracker.Instance;
            RaceEndPhase current = standings != null ? standings.LocalDisplayPhase : RaceEndPhase.None;

            if (current == _lastLocalDisplayPhase)
            {
                return;
            }

            _lastLocalDisplayPhase = current;

            if (current == RaceEndPhase.Podium)
            {
                _localPodiumStartSeconds = Time.timeAsDouble;
                StartChampionAudio();
                RaiseCelebrateCueIfLocalTopThree();
            }
            else if (_musicPlaying)
            {
                // A local skip stops MY OWN music the instant I stop watching —
                // it must never stop the ceremony (or its music) for anyone else,
                // since only this client's Update loop observes LocalDisplayPhase.
                AudioManager.Instance?.StopMusic();
                _musicPlaying = false;
            }
        }

        /// <summary>Sprint 8 "ANIMATION: Celebrate" hook — raised once per ceremony, only for a local player who actually placed top-3 (the only case this client's own avatar has anything to celebrate).</summary>
        private void RaiseCelebrateCueIfLocalTopThree()
        {
            RaceStandingsTracker standings = RaceStandingsTracker.Instance;
            IMatchTransport transport = MatchTransportService.Current;
            if (standings == null || standings.FinalResults == null || transport == null)
            {
                return;
            }

            foreach (PlayerRaceResult result in standings.FinalResults)
            {
                if (result.ConnectionId == transport.LocalConnectionId && result.FinishPosition >= 1 && result.FinishPosition <= 3)
                {
                    CharacterAnimationCueService.RaiseLocalCue(CharacterAnimationState.Celebrate);
                    return;
                }
            }
        }

        private void StartChampionAudio()
        {
            if (config == null || AudioManager.Instance == null)
            {
                return;
            }

            if (config.VictoryMusicClip != null)
            {
                AudioManager.Instance.PlayMusic(config.VictoryMusicClip);
                _musicPlaying = true;
            }

            // "Special victory music" for 1st place specifically: a one-shot
            // fanfare layered on top of the looping ceremony music, distinct
            // from it so the champion's moment has its own audio cue.
            if (config.ChampionFanfareClip != null)
            {
                AudioManager.Instance.PlayOneShot(config.ChampionFanfareClip);
            }
        }

        private void OnGUI()
        {
            RaceStandingsTracker standings = RaceStandingsTracker.Instance;
            if (standings == null || standings.LocalDisplayPhase != RaceEndPhase.Podium || standings.FinalResults == null)
            {
                return;
            }

            EnsureStyles();

            GUI.Label(new Rect(0, 40, Screen.width, 50), "VICTORY CEREMONY", _titleStyle);

            float centerX = Screen.width * 0.5f;
            double elapsed = Time.timeAsDouble - _localPodiumStartSeconds;

            // "Champion Animation" / celebration pulses — one shared pure
            // function, different amplitude/frequency per place so the
            // champion's motion reads as bigger/more energetic than 2nd/3rd's.
            float championBounce = CelebrationAnimation.EvaluateOffset(elapsed, 10f, 0.6f);
            float silverPulse = CelebrationAnimation.EvaluateOffset(elapsed, 4f, 0.8f);
            float bronzePulse = CelebrationAnimation.EvaluateOffset(elapsed, 4f, 0.8f);

            DrawConfetti(elapsed, centerX);

            DrawPlace(standings, 1, centerX - 110f, 150f + championBounce, "1ST — CHAMPION", elapsed, isChampion: true);
            DrawPlace(standings, 2, centerX - 360f, 220f + silverPulse, "2ND — SILVER MEDAL", elapsed, isChampion: false);
            DrawPlace(standings, 3, centerX + 140f, 220f + bronzePulse, "3RD — BRONZE MEDAL", elapsed, isChampion: false);

            if (GUI.Button(new Rect(Screen.width - 170f, Screen.height - 60f, 150f, 40f), "Skip >>"))
            {
                // Notify the host (used only for the "everyone skipped" early-advance
                // fast path — see RaceFinishAuthority.HandleSkipRequested) AND advance
                // this client's own local view immediately; the second call is what
                // actually makes the skip individual/instant for this player alone.
                MatchTransportService.Current.RequestSkipRaceEndPhase();
                standings.RequestLocalSkip();
            }
        }

        /// <summary>Golden confetti behind the champion — a placeholder particle simulation until real VFX exist.</summary>
        private void DrawConfetti(double elapsed, float centerX)
        {
            int particleCount = config != null ? config.ConfettiParticleCount : 0;
            if (particleCount <= 0)
            {
                return;
            }

            float fallSpeed = config.ConfettiFallSpeed;
            const float areaWidth = 260f;
            const float areaHeight = 260f;
            float areaLeft = centerX - areaWidth * 0.5f;
            const float areaTop = 90f;

            Color previous = GUI.color;
            GUI.color = new Color { r = 1f, g = 0.85f, b = 0.1f, a = 0.9f };

            for (int i = 0; i < particleCount; i++)
            {
                ConfettiParticle particle = ConfettiSimulation.Evaluate(i, elapsed, fallSpeed);
                float px = areaLeft + particle.NormalizedX * areaWidth;
                float py = areaTop + particle.NormalizedY * areaHeight;
                GUI.Box(new Rect(px, py, 4f, 8f), string.Empty);
            }

            GUI.color = previous;
        }

        private void DrawPlace(RaceStandingsTracker standings, int place, float x, float y, string label, double elapsed, bool isChampion)
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

            DrawFlag(x, y, elapsed, ResolveCountry(result2.ConnectionId));

            if (isChampion)
            {
                // The Bisht is cosmetic for the ceremony only — it exists purely
                // as this one label, drawn only while LocalDisplayPhase is
                // Podium, so it is automatically gone (for this client) the
                // instant the ceremony moves on or is skipped, with no extra
                // "remove it" step required anywhere.
                GUI.Label(new Rect(x, y - 26f, 220f, 22f), "Wearing Ceremonial Gulf Bisht", _bishtStyle);
            }

            GUI.Box(new Rect(x, y, 220f, 170f), string.Empty);
            GUI.Label(new Rect(x, y + 8f, 220f, 28f), label, _placeStyle);
            GUI.Label(new Rect(x, y + 44f, 220f, 26f), displayName, _detailStyle);
            GUI.Label(new Rect(x, y + 78f, 220f, 24f), $"Time: {result2.FinishTimeSeconds:F1}s", _detailStyle);
            GUI.Label(new Rect(x, y + 104f, 220f, 24f), result2.Reason == FinishReason.Completed ? "Finished" : "Eliminated", _detailStyle);
            GUI.Label(new Rect(x, y + 130f, 220f, 24f), isChampion ? "Golden Trophy" : (place == 2 ? "Silver Medal" : "Bronze Medal"), _detailStyle);
            GUI.Label(new Rect(x, y + 152f, 220f, 20f), $"Coins: {result2.CoinsCollected}", _detailStyle);
        }

        /// <summary>Draws the finisher's national flag behind their podium position, swaying gently — "flags animate gently during the ceremony."</summary>
        private void DrawFlag(float placeX, float placeY, double elapsed, GulfCountry country)
        {
            if (flagCatalog == null || !flagCatalog.TryGetFlag(country, out FlagCatalogConfig.FlagEntry flag))
            {
                return;
            }

            float amplitude = config != null ? config.FlagWaveAmplitudeDegrees : 12f;
            float frequency = config != null ? config.FlagWaveFrequencyHz : 0.5f;
            float sway = CelebrationAnimation.EvaluateOffset(elapsed, amplitude, frequency);

            const float flagWidth = 90f;
            const float flagHeight = 60f;
            float flagX = placeX + 65f + sway;
            float flagY = placeY - 74f;

            Color previous = GUI.color;
            GUI.color = flag.PlaceholderColor;
            GUI.Box(new Rect(flagX, flagY, flagWidth, flagHeight), string.Empty);
            GUI.color = previous;

            GUI.Label(new Rect(flagX, flagY, flagWidth, flagHeight), flag.Code, _placeStyle);
        }

        private static GulfCountry ResolveCountry(int connectionId)
        {
            IMatchTransport transport = MatchTransportService.Current;
            if (transport == null)
            {
                return default;
            }

            foreach (MatchParticipant participant in transport.Participants)
            {
                if (participant.Identity.ConnectionId == connectionId)
                {
                    return participant.Identity.Country;
                }
            }

            return default;
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

            _bishtStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter
            };
            _bishtStyle.normal.textColor = Color.yellow;
        }
    }
}
