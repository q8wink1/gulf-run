using GulfRun.Core.Backend;
using GulfRun.Core.Managers;
using GulfRun.Domain;
using GulfRun.Features.Online.Championships;
using GulfRun.Features.Online.Friends;
using GulfRun.Features.Online.Leaderboard;
using GulfRun.Features.Online.Profile;
using UnityEngine;

namespace GulfRun.Features.Online
{
    /// <summary>
    /// Debug panel required by the Sprint 9 brief: Current Rank,
    /// Leaderboard Refresh, Friend Count, Backend Status, Tournament
    /// Status — plus dev-build-only buttons to simulate advancing the
    /// active Championship/Country Event, since no real calendar/scheduler
    /// exists yet (see Sprint 9 report Remaining TODOs). Same on-screen,
    /// dev-build-only placeholder style as <c>MultiplayerDebugView</c>/
    /// <c>CharacterDebugView</c>.
    /// </summary>
    public sealed class OnlineDebugView : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private bool showOnScreenDebug = true;
        [SerializeField] private int panelX = 2260;
        [SerializeField] private int panelY = 10;

        private void OnGUI()
        {
            if (!showOnScreenDebug)
            {
                return;
            }

            int y = panelY;
            const int lineHeight = 18;
            const int width = 440;

            void Line(string text)
            {
                GUI.Label(new Rect(panelX, y, width, lineHeight), text);
                y += lineHeight;
            }

            bool hasAccount = SaveManager.Instance != null && SaveManager.Instance.HasAccount;
            Line("[Online] Has Account: " + hasAccount);

            if (!hasAccount)
            {
                return;
            }

            PlayerId localId = SaveManager.Instance.GetAccount().PlayerId;
            Line("Player ID: " + localId.Value);

            if (LeaderboardManager.Instance != null)
            {
                int worldRank = LeaderboardManager.Instance.GetPlayerRank(RankingScope.World, null, localId);
                Line("Current Rank (World): " + (worldRank > 0 ? "#" + worldRank : "Unranked"));
                Line("Leaderboard Last Refreshed: " + LeaderboardManager.Instance.LastRefreshedAtSeconds.ToString("F1") + "s (game time)");
            }

            Line("Friend Count: " + (FriendManager.Instance != null ? FriendManager.Instance.GetFriends().Count : 0));
            Line("Backend Status: " + (OnlineBackendService.Current is LocalOnlineBackendService ? "Mock/Local (in-memory)" : "Custom"));

            if (ChampionshipManager.Instance != null)
            {
                Line("Tournament Status: " + (ChampionshipManager.Instance.HasActiveChampionship ? ChampionshipManager.Instance.ActiveChampionship.DisplayName + " (Active)" : "None Active"));
                Line("Country Event: " + (ChampionshipManager.Instance.HasActiveCountryEvent ? ChampionshipManager.Instance.ActiveCountryEvent.DisplayName : "None Active"));
            }

            if (ProfileManager.Instance != null && ProfileManager.Instance.LocalProfile != null)
            {
                Line("Online Status: " + ProfileManager.Instance.LocalProfile.Status);
            }

            y += 6;
            DrawControls(ref y);
        }

        private void DrawControls(ref int y)
        {
            const int buttonWidth = 210;
            const int buttonHeight = 24;

            if (ChampionshipManager.Instance == null)
            {
                return;
            }

            if (GUI.Button(new Rect(panelX, y, buttonWidth, buttonHeight), "Simulate Advance Championship"))
            {
                ChampionshipManager.Instance.AdvanceToNextChampionship();
            }

            if (GUI.Button(new Rect(panelX + buttonWidth + 8, y, buttonWidth, buttonHeight), "Simulate Advance Country Event"))
            {
                ChampionshipManager.Instance.AdvanceToNextCountryEvent();
            }

            y += buttonHeight + 4;
        }
#endif
    }
}
