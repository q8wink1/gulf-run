using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Online.Friends;
using GulfRun.Features.Online.Profile;
using UnityEngine;

namespace GulfRun.Features.Online.Leaderboard
{
    /// <summary>
    /// The World/Gulf/Country/Weekly/Monthly/Seasonal leaderboard screen.
    /// A <see cref="SceneSingleton{T}"/> (like <see cref="PlayerProfileView"/>)
    /// so <see cref="OpenAndFocus"/> is a single, reusable "Clickable Ranks"
    /// entry point every other screen (Player Profile, Search, Friends) can
    /// call: switches to the requested scope/country, scrolls the list so
    /// the requested player's row is visible, and highlights that row —
    /// exactly the "Open Leaderboard → scroll to Player → highlight" chain
    /// the brief describes. Also one of the brief's required "Add Friends
    /// from: Leaderboard" entry points (a per-row Add Friend button).
    /// </summary>
    public sealed class LeaderboardView : SceneSingleton<LeaderboardView>, IMenuScreenOpener
    {
        private const float RowHeight = 24f;
        private static readonly GulfCountry[] AllCountries = (GulfCountry[])Enum.GetValues(typeof(GulfCountry));

        private bool _open;
        private RankingScope _scope = RankingScope.World;
        private GulfCountry _browsingCountry = GulfCountry.SaudiArabia;
        private PlayerId _focusPlayerId = PlayerId.None;
        private bool _needsScrollToFocus;
        private Vector2 _scroll;
        private GUIStyle _titleStyle;
        private GUIStyle _tabStyle;
        private GUIStyle _rowStyle;
        private GUIStyle _highlightRowStyle;
        private GUIStyle _labelStyle;

        private void OnEnable() => MenuScreenRouter.Register(MenuScreen.Leaderboard, this);

        private void OnDisable() => MenuScreenRouter.Unregister(MenuScreen.Leaderboard, this);

        /// <summary>Sprint 13 (Main Menu Left Menu "Leaderboard" button) — <see cref="IMenuScreenOpener"/>. Opens on the default World scope with no player focused.</summary>
        public void OpenScreen(MenuScreen screen) => OpenAndFocus(RankingScope.World, null, PlayerId.None);

        public void Close() => _open = false;

        /// <summary>Opens the leaderboard on <paramref name="scope"/> (and <paramref name="country"/> for Country scope), scrolled to and highlighting <paramref name="focusPlayer"/>.</summary>
        public void OpenAndFocus(RankingScope scope, GulfCountry? country, PlayerId focusPlayer)
        {
            _scope = scope;
            if (country.HasValue)
            {
                _browsingCountry = country.Value;
            }

            _focusPlayerId = focusPlayer;
            _needsScrollToFocus = true;
            _open = true;
        }

        private void OnGUI()
        {
            EnsureStyles();

            if (GUI.Button(new Rect(180, 10, 160, 34), _open ? "Close Leaderboard" : "Leaderboard"))
            {
                _open = !_open;
                if (_open)
                {
                    _needsScrollToFocus = true;
                }
            }

            if (!_open)
            {
                return;
            }

            DrawPanel();
        }

        private void DrawPanel()
        {
            const float panelWidth = 620f;
            const float panelHeight = 600f;
            float x = 10f;
            float y = 50f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);
            GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 28f, 26f), "LEADERBOARD", _titleStyle);

            float rowY = y + 40f;
            DrawScopeTabs(x + 14f, rowY, panelWidth - 28f);
            rowY += 30f;

            if (_scope == RankingScope.Country)
            {
                DrawCountrySelector(x + 14f, rowY, panelWidth - 28f);
                rowY += 30f;
            }

            GulfCountry? countryFilter = _scope == RankingScope.Country ? _browsingCountry : (GulfCountry?)null;
            int topN = _focusPlayerId.IsNone ? 100 : 1000;
            IReadOnlyList<LeaderboardEntry> entries = LeaderboardManager.Instance != null
                ? LeaderboardManager.Instance.GetLeaderboard(_scope, countryFilter, topN)
                : Array.Empty<LeaderboardEntry>();

            PlayerId highlightId = !_focusPlayerId.IsNone
                ? _focusPlayerId
                : (ProfileManager.Instance != null ? ProfileManager.Instance.LocalPlayerId : PlayerId.None);

            float viewportHeight = panelHeight - (rowY - y) - 14f;
            Rect viewport = new Rect(x + 14f, rowY, panelWidth - 28f, viewportHeight);
            Rect content = new Rect(0f, 0f, panelWidth - 48f, entries.Count * RowHeight);

            if (_needsScrollToFocus)
            {
                int index = IndexOf(entries, highlightId);
                if (index >= 0)
                {
                    float targetY = index * RowHeight - viewportHeight / 2f;
                    _scroll.y = Mathf.Max(0f, targetY);
                }

                _needsScrollToFocus = false;
            }

            _scroll = GUI.BeginScrollView(viewport, _scroll, content);
            for (int i = 0; i < entries.Count; i++)
            {
                DrawRow(entries[i], i * RowHeight, content.width, entries[i].Player == highlightId);
            }

            GUI.EndScrollView();
        }

        private void DrawScopeTabs(float x, float y, float width)
        {
            RankingScope[] scopes = { RankingScope.World, RankingScope.Gulf, RankingScope.Country, RankingScope.Weekly, RankingScope.Monthly, RankingScope.Seasonal };
            float tabWidth = width / scopes.Length;
            for (int i = 0; i < scopes.Length; i++)
            {
                bool active = _scope == scopes[i];
                GUI.color = active ? Color.yellow : Color.white;
                if (GUI.Button(new Rect(x + i * tabWidth, y, tabWidth - 2f, 26f), scopes[i].ToString(), _tabStyle))
                {
                    _scope = scopes[i];
                    _focusPlayerId = PlayerId.None;
                    _needsScrollToFocus = true;
                }

                GUI.color = Color.white;
            }
        }

        private void DrawCountrySelector(float x, float y, float width)
        {
            if (GUI.Button(new Rect(x, y, 60f, 26f), "< "))
            {
                CycleCountry(-1);
            }

            GUI.Label(new Rect(x + 64f, y + 4f, width - 128f, 22f), _browsingCountry.ToString(), _labelStyle);

            if (GUI.Button(new Rect(x + width - 60f, y, 60f, 26f), " >"))
            {
                CycleCountry(1);
            }
        }

        private void CycleCountry(int direction)
        {
            int currentIndex = Array.IndexOf(AllCountries, _browsingCountry);
            int nextIndex = ((currentIndex + direction) % AllCountries.Length + AllCountries.Length) % AllCountries.Length;
            _browsingCountry = AllCountries[nextIndex];
            _needsScrollToFocus = true;
        }

        private void DrawRow(LeaderboardEntry entry, float rowY, float width, bool highlighted)
        {
            if (highlighted)
            {
                GUI.Box(new Rect(0f, rowY, width, RowHeight - 2f), string.Empty);
            }

            if (GUI.Button(new Rect(0f, rowY, 230f, RowHeight - 2f), "#" + entry.Rank + "  " + entry.Nickname, highlighted ? _highlightRowStyle : _rowStyle))
            {
                PlayerProfileView.Instance?.ShowProfile(entry.Player);
            }

            GUI.Label(new Rect(236f, rowY, 110f, RowHeight - 2f), entry.Country.ToString(), _labelStyle);
            GUI.Label(new Rect(350f, rowY, 90f, RowHeight - 2f), "Trophies: " + entry.TrophyCount, _labelStyle);

            bool isSelf = ProfileManager.Instance != null && entry.Player == ProfileManager.Instance.LocalPlayerId;
            if (!isSelf && FriendManager.Instance != null)
            {
                DrawFriendButton(entry, 446f, rowY, width - 446f - 4f);
            }
        }

        private void DrawFriendButton(LeaderboardEntry entry, float x, float rowY, float width)
        {
            FriendLinkState link = FriendManager.Instance.GetLinkState(entry.Player);
            switch (link)
            {
                case FriendLinkState.None:
                    if (GUI.Button(new Rect(x, rowY, width, RowHeight - 4f), "Add Friend"))
                    {
                        FriendManager.Instance.SendFriendRequest(entry.Player);
                    }

                    break;
                case FriendLinkState.Friends:
                    GUI.Label(new Rect(x, rowY, width, RowHeight - 2f), "Friend", _labelStyle);
                    break;
                case FriendLinkState.RequestSentByMe:
                    GUI.Label(new Rect(x, rowY, width, RowHeight - 2f), "Sent", _labelStyle);
                    break;
                case FriendLinkState.RequestReceivedFromThem:
                    GUI.Label(new Rect(x, rowY, width, RowHeight - 2f), "Pending", _labelStyle);
                    break;
                case FriendLinkState.Blocked:
                    GUI.Label(new Rect(x, rowY, width, RowHeight - 2f), "Blocked", _labelStyle);
                    break;
            }
        }

        private static int IndexOf(IReadOnlyList<LeaderboardEntry> entries, PlayerId id)
        {
            if (id.IsNone)
            {
                return -1;
            }

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Player == id)
                {
                    return i;
                }
            }

            return -1;
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _titleStyle.normal.textColor = Color.white;

            _tabStyle = new GUIStyle(GUI.skin.button) { fontSize = 12 };

            _rowStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft, fontSize = 13 };

            _highlightRowStyle = new GUIStyle(_rowStyle) { fontStyle = FontStyle.Bold };
            _highlightRowStyle.normal.textColor = Color.yellow;

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleLeft };
            _labelStyle.normal.textColor = Color.white;
        }
    }
}
