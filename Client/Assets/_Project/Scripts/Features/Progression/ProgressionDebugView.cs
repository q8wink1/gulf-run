using System;
using GulfRun.Core.Backend;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Progression.Login;
using GulfRun.Features.Progression.Missions;
using UnityEngine;

namespace GulfRun.Features.Progression
{
    /// <summary>
    /// Debug panel required by the Sprint 11 brief: Mission IDs, Mission
    /// Progress, Reward IDs, Temporary Item Timers, Login Streak. Same
    /// on-screen, dev-build-only placeholder style as
    /// <c>Features.Store.StoreDebugView</c>. <c>panelX: 3160</c> is
    /// Gameplay.unity's next free slot after Sprint 10's
    /// <c>StoreDebugView</c> at <c>panelX: 2710</c>.
    /// </summary>
    public sealed class ProgressionDebugView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool showOnScreenDebug = true;
        [SerializeField] private int panelX = 3160;
        [SerializeField] private int panelY = 10;

        private void OnGUI()
        {
            if (!showOnScreenDebug)
            {
                return;
            }

            int y = panelY;
            const int lineHeight = 18;
            const int width = 480;

            void Line(string text)
            {
                GUI.Label(new Rect(panelX, y, width, lineHeight), text);
                y += lineHeight;
            }

            MissionManager missions = MissionManager.Instance;
            if (missions != null)
            {
                var active = missions.ActiveMissions;
                Line("[Missions] Reset in: " + Math.Max(0, (int)(missions.MissionsResetAtSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds())) + "s");
                for (int i = 0; i < active.Count; i++)
                {
                    ActiveMission m = active[i];
                    Line("  [" + i + "] " + m.SourceMissionId + " (" + m.Type + "/" + m.Difficulty + ") " + m.CurrentAmount + "/" + m.TargetAmount + (m.IsClaimed ? " Claimed" : m.IsCompleted ? " Ready" : string.Empty) + " -> " + m.RewardType + " x" + m.RewardAmount);
                }
            }

            LoginRewardManager login = LoginRewardManager.Instance;
            if (login != null)
            {
                LoginStreakStatus status = login.Status;
                Line("[Login Streak] Day " + status.CurrentStreakDay + " — Total Logins: " + status.TotalLoginsEver + " — Claimed Today: " + login.HasClaimedToday());
                Line("[Login Streak] Active Calendar: " + (string.IsNullOrEmpty(login.ActiveSpecialEventLabel) ? "Standard" : login.ActiveSpecialEventLabel));
            }

            if (CosmeticGrantService.Current != null)
            {
                var temporary = CosmeticGrantService.Current.GetTemporaryCosmetics();
                double now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                Line("[Temporary Cosmetics] Count: " + temporary.Count);
                for (int i = 0; i < temporary.Count; i++)
                {
                    TemporaryCosmeticOwnership grant = temporary[i];
                    Line("  " + grant.Id + " — expires in " + FormatSeconds(grant.RemainingSeconds(now)));
                }
            }

            var ledger = ProgressionBackendService.Current.GetProgressionRewardLedger();
            Line("[Reward Ledger] Count: " + ledger.Count);
            for (int i = 0; i < ledger.Count; i++)
            {
                Line("  " + ledger[i].LedgerKey + " (" + ledger[i].RewardType + ")");
            }
        }

        private static string FormatSeconds(double seconds)
        {
            if (seconds <= 0d)
            {
                return "expired";
            }

            var span = TimeSpan.FromSeconds(seconds);
            return span.Days > 0 ? span.Days + "d " + span.Hours + "h" : span.Hours + "h " + span.Minutes + "m";
        }
#endif
    }
}
