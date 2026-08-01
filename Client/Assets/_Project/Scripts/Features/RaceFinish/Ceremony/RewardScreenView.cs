using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.RaceFinish.Configuration;
using GulfRun.Features.RaceFinish.Standings;
using UnityEngine;

namespace GulfRun.Features.RaceFinish.Ceremony
{
    /// <summary>
    /// Private, per-player Reward Screen: only ever renders the LOCAL
    /// player's own <see cref="RaceRewardBreakdown"/> — every client receives
    /// every player's breakdown over the network (see
    /// <c>IMatchTransport.RaceRewardCalculated</c>), but this view filters to
    /// the local connection id, so "players do not see other players' reward
    /// totals" holds at the presentation layer. Counters animate smoothly
    /// from 0 via the pure <see cref="RewardCounterAnimation"/> helper.
    /// Contains no gameplay/economy logic — see
    /// <c>Features.RaceFinish.Rewards.RaceRewardApplier</c> for the one place
    /// the reward is actually applied to the wallet.
    ///
    /// Renders against <see cref="RaceStandingsTracker.LocalDisplayPhase"/>,
    /// not the raw host-broadcast <see cref="RaceStandingsTracker.CurrentPhase"/>,
    /// so a player who skipped the Podium sees their own reward counters
    /// start immediately rather than waiting for the host's ceremony clock
    /// (Sprint 7 addendum: individual skip never depends on other players).
    /// </summary>
    public sealed class RewardScreenView : MonoBehaviour
    {
        [SerializeField] private RaceFinishConfig config;

        private RaceEndPhase _lastPhase = RaceEndPhase.None;
        private float _phaseLocalTimer;
        private GUIStyle _titleStyle;
        private GUIStyle _lineStyle;

        private void Update()
        {
            RaceStandingsTracker standings = RaceStandingsTracker.Instance;
            RaceEndPhase current = standings != null ? standings.LocalDisplayPhase : RaceEndPhase.None;

            if (current != _lastPhase)
            {
                _lastPhase = current;
                _phaseLocalTimer = 0f;
            }

            if (current == RaceEndPhase.Reward)
            {
                _phaseLocalTimer += Time.deltaTime;
            }
        }

        private void OnGUI()
        {
            RaceStandingsTracker standings = RaceStandingsTracker.Instance;
            IMatchTransport transport = MatchTransportService.Current;
            if (standings == null || transport == null || standings.LocalDisplayPhase != RaceEndPhase.Reward)
            {
                return;
            }

            if (!standings.TryGetReward(transport.LocalConnectionId, out RaceRewardBreakdown reward))
            {
                return;
            }

            EnsureStyles();

            float duration = config != null ? config.RewardCounterAnimationSeconds : 1.5f;
            float x = Screen.width * 0.5f - 180f;
            float y = 120f;

            GUI.Box(new Rect(x - 20f, y - 20f, 400f, 300f), string.Empty);
            GUI.Label(new Rect(x, y, 360f, 40f), "YOUR REWARDS", _titleStyle);

            DrawLine(x, y + 55f, "Coins Collected", RewardCounterAnimation.EvaluateInt(_phaseLocalTimer, duration, reward.CoinsCollected));
            DrawLine(x, y + 95f, "Bonus Coins", RewardCounterAnimation.EvaluateInt(_phaseLocalTimer, duration, reward.BonusCoins));
            DrawLine(x, y + 135f, "Rank Points", RewardCounterAnimation.EvaluateInt(_phaseLocalTimer, duration, reward.RankPoints));
            DrawLine(x, y + 175f, "Experience", RewardCounterAnimation.EvaluateInt(_phaseLocalTimer, duration, reward.Experience));
            DrawLine(x, y + 225f, "Total Reward", RewardCounterAnimation.EvaluateInt(_phaseLocalTimer, duration, reward.TotalReward));

            if (GUI.Button(new Rect(Screen.width - 170f, Screen.height - 60f, 150f, 40f), "Skip >>"))
            {
                transport.RequestSkipRaceEndPhase();
                standings.RequestLocalSkip();
            }
        }

        private void DrawLine(float x, float y, string label, int value)
        {
            GUI.Label(new Rect(x, y, 340f, 30f), $"{label}: {value}", _lineStyle);
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 26,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _titleStyle.normal.textColor = Color.white;

            _lineStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleLeft
            };
            _lineStyle.normal.textColor = Color.white;
        }
    }
}
