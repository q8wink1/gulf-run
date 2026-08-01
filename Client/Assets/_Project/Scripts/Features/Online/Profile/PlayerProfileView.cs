using GulfRun.Core;
using GulfRun.Domain;
using GulfRun.Features.Online.Friends;
using GulfRun.Features.Online.Leaderboard;
using GulfRun.Features.Online.Leagues;
using UnityEngine;

namespace GulfRun.Features.Online.Profile
{
    /// <summary>
    /// The full "Player Profile" screen from the Sprint 9 brief: every
    /// listed field (Nickname, Player ID, Country/Flag, Character/Outfit,
    /// League, Season, Trophies, World/Gulf/Country Rank, Total
    /// Wins/Top3/Win Rate/Best Finish Time, Coins/Gems, Favourite
    /// Character), each Rank rendered as a clickable button that opens
    /// <see cref="LeaderboardView"/> scrolled to and highlighting this
    /// exact player ("Clickable Ranks" — see that view's remarks). A
    /// <see cref="SceneSingleton{T}"/> so any other view (Leaderboard,
    /// Friends, Search results) can call <see cref="ShowProfile"/> to open
    /// someone else's profile without owning its own copy of this UI.
    /// </summary>
    public sealed class PlayerProfileView : SceneSingleton<PlayerProfileView>
    {
        private bool _open;
        private PlayerId _viewedPlayerId = PlayerId.None;
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;

        public void ShowLocalProfile()
        {
            _viewedPlayerId = PlayerId.None;
            _open = true;
        }

        public void ShowProfile(PlayerId playerId)
        {
            _viewedPlayerId = playerId;
            _open = true;
        }

        private void OnGUI()
        {
            EnsureStyles();

            if (GUI.Button(new Rect(Screen.width - 170, 10, 160, 34), _open ? "Close Profile" : "My Profile"))
            {
                _open = !_open;
                if (_open)
                {
                    _viewedPlayerId = PlayerId.None;
                }
            }

            if (!_open)
            {
                return;
            }

            bool isLocal = ProfileManager.Instance != null && (_viewedPlayerId.IsNone || _viewedPlayerId == ProfileManager.Instance.LocalPlayerId);
            PlayerProfileSummary profile = null;
            bool found = ProfileManager.Instance != null && ProfileManager.Instance.TryGetProfile(isLocal ? ProfileManager.Instance.LocalPlayerId : _viewedPlayerId, out profile);

            const float panelWidth = 480f;
            const float panelHeight = 560f;
            float x = Screen.width - panelWidth - 10f;
            float y = 50f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);

            if (!found || profile == null)
            {
                GUI.Label(new Rect(x + 14f, y + 12f, panelWidth - 28f, 24f), "PLAYER PROFILE", _titleStyle);
                GUI.Label(new Rect(x + 14f, y + 50f, panelWidth - 28f, 24f), "Create an account to view a profile.", _labelStyle);
                return;
            }

            DrawProfile(profile, isLocal, x, y, panelWidth, panelHeight);
        }

        private void DrawProfile(PlayerProfileSummary profile, bool isLocal, float x, float y, float panelWidth, float panelHeight)
        {
            GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 28f, 26f), "PLAYER PROFILE" + (isLocal ? string.Empty : " (Viewing)"), _titleStyle);

            float rowY = y + 40f;
            const float rowHeight = 20f;
            float rowX = x + 14f;
            float rowWidth = panelWidth - 28f;

            void Row(string text)
            {
                GUI.Label(new Rect(rowX, rowY, rowWidth, rowHeight), text, _labelStyle);
                rowY += rowHeight;
            }

            Row("Nickname: " + profile.Nickname);
            Row("Player ID: " + profile.PlayerId.Value);
            Row("Country: " + profile.Country + "  [" + CountryFlagGlyph(profile.Country) + "]");
            Row("Status: " + profile.Status);
            Row("Current Character: " + Fallback(profile.CurrentCharacterDisplayName));
            Row("Current Outfit: " + Fallback(profile.CurrentOutfitDisplayName));

            string leagueName = LeagueManager.Instance != null ? LeagueManager.Instance.LeagueDisplayName(profile.Season.CurrentLeague) : profile.Season.CurrentLeague.ToString();
            Row("League: " + leagueName + "   Season: " + profile.Season.SeasonNumber);
            Row("Current Trophy Count: " + profile.Season.TrophyCount);

            rowY += 4f;
            DrawRankButton(rowX, ref rowY, rowWidth, "World Rank", profile.WorldRank, RankingScope.World, null, profile.PlayerId);
            DrawRankButton(rowX, ref rowY, rowWidth, "Gulf Rank", profile.GulfRank, RankingScope.Gulf, null, profile.PlayerId);
            DrawRankButton(rowX, ref rowY, rowWidth, "Country Rank", profile.CountryRank, RankingScope.Country, profile.Country, profile.PlayerId);
            rowY += 4f;

            Row("Total Wins: " + profile.TotalWins);
            Row("Top 3 Finishes: " + profile.Top3Finishes);
            Row("Win Rate: " + (profile.WinRate * 100f).ToString("F1") + "%");
            Row("Best Finish Time: " + (profile.BestFinishTimeSeconds >= 0f ? profile.BestFinishTimeSeconds.ToString("F1") + "s" : "—"));
            Row("Coins: " + profile.Coins + "   Gems: " + profile.Gems);
            Row("Favourite Character: " + Fallback(profile.FavouriteCharacterDisplayName));

            rowY += 8f;

            if (!isLocal)
            {
                DrawFriendActions(profile, rowX, ref rowY, rowWidth);
            }

            _ = panelHeight;
        }

        private void DrawRankButton(float x, ref float y, float width, string label, int rank, RankingScope scope, GulfCountry? country, PlayerId subject)
        {
            string text = label + ": " + (rank > 0 ? "#" + rank : "Unranked") + "  (click to view)";
            if (GUI.Button(new Rect(x, y, width, 22f), text))
            {
                LeaderboardView.Instance?.OpenAndFocus(scope, country, subject);
                _open = false;
            }

            y += 24f;
        }

        private void DrawFriendActions(PlayerProfileSummary profile, float x, ref float y, float width)
        {
            if (FriendManager.Instance == null)
            {
                return;
            }

            FriendLinkState link = FriendManager.Instance.GetLinkState(profile.PlayerId);
            const float buttonHeight = 26f;

            switch (link)
            {
                case FriendLinkState.None:
                    if (GUI.Button(new Rect(x, y, width, buttonHeight), "Add Friend"))
                    {
                        FriendManager.Instance.SendFriendRequest(profile.PlayerId);
                    }

                    y += buttonHeight + 4f;
                    break;

                case FriendLinkState.RequestSentByMe:
                    GUI.Label(new Rect(x, y, width, buttonHeight), "Friend Request Sent", _labelStyle);
                    y += buttonHeight;
                    if (GUI.Button(new Rect(x, y, width, buttonHeight), "Cancel Request"))
                    {
                        FriendManager.Instance.CancelFriendRequest(profile.PlayerId);
                    }

                    y += buttonHeight + 4f;
                    break;

                case FriendLinkState.RequestReceivedFromThem:
                    if (GUI.Button(new Rect(x, y, width / 2f - 4f, buttonHeight), "Accept"))
                    {
                        FriendManager.Instance.AcceptFriendRequest(profile.PlayerId);
                    }

                    if (GUI.Button(new Rect(x + width / 2f + 4f, y, width / 2f - 4f, buttonHeight), "Reject"))
                    {
                        FriendManager.Instance.RejectFriendRequest(profile.PlayerId);
                    }

                    y += buttonHeight + 4f;
                    break;

                case FriendLinkState.Friends:
                    if (GUI.Button(new Rect(x, y, width / 2f - 4f, buttonHeight), "Remove Friend"))
                    {
                        FriendManager.Instance.RemoveFriend(profile.PlayerId);
                    }

                    if (GUI.Button(new Rect(x + width / 2f + 4f, y, width / 2f - 4f, buttonHeight), "Block"))
                    {
                        FriendManager.Instance.BlockPlayer(profile.PlayerId);
                    }

                    y += buttonHeight + 4f;
                    break;

                case FriendLinkState.Blocked:
                    GUI.Label(new Rect(x, y, width, buttonHeight), "Blocked", _labelStyle);
                    y += buttonHeight + 4f;
                    break;
            }
        }

        private static string Fallback(string value) => string.IsNullOrEmpty(value) ? "—" : value;

        /// <summary>No flag art yet (see Sprint 8 remaining TODOs) — a stable per-country glyph stands in.</summary>
        private static string CountryFlagGlyph(GulfCountry country) => country.ToString().Substring(0, System.Math.Min(3, country.ToString().Length)).ToUpperInvariant();

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _titleStyle.normal.textColor = Color.white;

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 13, alignment = TextAnchor.MiddleLeft };
            _labelStyle.normal.textColor = Color.white;
        }
    }
}
