using System.Collections.Generic;
using GulfRun.Core.Countries;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Matchmaking.UI;
using UnityEngine;

namespace GulfRun.Features.Matchmaking.Lobby
{
    /// <summary>
    /// Sprint 14 Matchmaking "PLAYER CARDS": Character silhouette, Country
    /// Flag, Name, League, Trophy Count, Connection Quality, Ready Status,
    /// Voice icon, Owner badge, BOT tag, and host Kick control.
    /// </summary>
    public sealed class PlayerCardRosterView : MonoBehaviour
    {
        [SerializeField] private CountryCatalogConfig countryCatalog;

        private readonly List<MatchParticipant> _scratch = new List<MatchParticipant>(4);
        private LobbyButtonPressAnimator _kickAnim;

        private void OnGUI()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby == null || !lobby.IsInMatch)
            {
                return;
            }

            _scratch.Clear();
            foreach (MatchParticipant p in lobby.Participants)
            {
                _scratch.Add(p);
            }

            _scratch.Sort((a, b) => a.Identity.ConnectionId.CompareTo(b.Identity.ConnectionId));

            float cardWidth = Mathf.Min(210f, (Screen.width - 48f) / 4f);
            float cardHeight = 210f;
            float totalWidth = _scratch.Count * cardWidth + Mathf.Max(0, _scratch.Count - 1) * 12f;
            float startX = (Screen.width - totalWidth) * 0.5f;
            float y = Screen.height * 0.30f;

            ILocalProfileProvider localProfile = LocalProfileProviderService.Current;
            VoiceChatMode voice = SettingsManagerVoiceMode();

            for (int i = 0; i < _scratch.Count; i++)
            {
                MatchParticipant p = _scratch[i];
                Rect card = new Rect(startX + i * (cardWidth + 12f), y, cardWidth, cardHeight);
                DrawCard(card, p, lobby, localProfile, voice);
            }
        }

        private void DrawCard(Rect card, MatchParticipant p, IMatchLobbySummaryProvider lobby, ILocalProfileProvider localProfile, VoiceChatMode voice)
        {
            float breathe = CelebrationAnimation.EvaluateOffset(Time.timeAsDouble + p.Identity.ConnectionId, 3f, 0.4f);
            PreRaceLobbyTheme.DrawPanel(new Rect(card.x, card.y + breathe, card.width, card.height));

            bool isLocal = p.Identity.ConnectionId == lobby.LocalConnectionId;
            bool isBot = lobby.IsBot(p.Identity.ConnectionId);
            ConnectionQuality quality = lobby.GetConnectionQuality(p.Identity.ConnectionId);

            Color previous = GUI.color;
            GUI.color = new Color(0.12f, 0.12f, 0.14f, 0.95f);
            GUI.Box(new Rect(card.x + 16f, card.y + 14f + breathe, card.width - 32f, 70f), string.Empty);
            GUI.color = previous;

            string name = p.Identity.DisplayName;
            string league = "Bronze";
            int trophies = 0;
            if (isLocal && localProfile != null && localProfile.HasProfile)
            {
                league = localProfile.LocalProfile.Season.CurrentLeague.ToString();
                trophies = localProfile.LocalProfile.Season.TrophyCount;
            }

            GUI.Label(new Rect(card.x + 10f, card.y + 90f + breathe, card.width - 20f, 20f), name, PreRaceLobbyTheme.Header);
            DrawFlag(p.Identity.Country, card.x + 10f, card.y + 112f + breathe);
            GUI.Label(new Rect(card.x + 56f, card.y + 112f + breathe, card.width - 66f, 18f), league + " • " + trophies + " trophies", PreRaceLobbyTheme.Muted);

            Color qColor = PreRaceLobbyTheme.ColorFor(quality);
            previous = GUI.color;
            GUI.color = qColor;
            GUI.Box(new Rect(card.x + 10f, card.y + 138f + breathe, 14f, 14f), string.Empty);
            GUI.color = previous;
            GUI.Label(new Rect(card.x + 28f, card.y + 136f + breathe, card.width - 40f, 18f), quality.ToString(), PreRaceLobbyTheme.Muted);

            string ready = p.Ready == PlayerReadyState.Ready ? "READY" : "NOT READY";
            previous = GUI.color;
            GUI.color = p.Ready == PlayerReadyState.Ready ? PreRaceLobbyTheme.Success : PreRaceLobbyTheme.TextMuted;
            GUI.Label(new Rect(card.x + 10f, card.y + 156f + breathe, card.width - 20f, 18f), ready, PreRaceLobbyTheme.Label);
            GUI.color = previous;

            string badges = string.Empty;
            if (p.IsHost)
            {
                badges += "OWNER ";
            }

            if (isBot)
            {
                badges += "BOT ";
            }

            if (isLocal)
            {
                badges += voice == VoiceChatMode.Muted ? "🎤✕" : voice == VoiceChatMode.PushToTalk ? "🎤PTT" : "🎤";
            }

            GUI.Label(new Rect(card.x + 10f, card.y + 176f + breathe, card.width - 20f, 18f), badges.Trim(), PreRaceLobbyTheme.Muted);

            if (lobby.IsHost && !isLocal)
            {
                Rect kick = _kickAnim.Apply(new Rect(card.x + card.width - 70f, card.y + 8f + breathe, 58f, 22f), 2f);
                if (GUI.Button(kick, "Kick", PreRaceLobbyTheme.PanelButton))
                {
                    _kickAnim.NotifyPressed();
                    lobby.KickPlayer(p.Identity.ConnectionId);
                }
            }
        }

        private void DrawFlag(GulfCountry country, float x, float y)
        {
            Color flagColor = Color.white;
            string code = country.ToString();
            if (countryCatalog != null && countryCatalog.TryGetEntry(country, out CountryCatalogConfig.CountryEntry entry))
            {
                flagColor = entry.PlaceholderColor;
                code = entry.Code;
            }

            Color previous = GUI.color;
            GUI.color = flagColor;
            GUI.Box(new Rect(x, y, 36f, 22f), string.Empty);
            GUI.color = previous;
            GUI.Label(new Rect(x, y + 22f, 36f, 14f), code, PreRaceLobbyTheme.Muted);
        }

        private static VoiceChatMode SettingsManagerVoiceMode()
        {
            GulfRun.Core.Managers.SettingsManager settings = GulfRun.Core.Managers.SettingsManager.Instance;
            return settings != null ? settings.VoiceChatMode : VoiceChatMode.Muted;
        }
    }
}
