using System.Collections.Generic;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Matchmaking.UI;
using UnityEngine;

namespace GulfRun.Features.Matchmaking.Lobby
{
    /// <summary>Sprint 14 Matchmaking Quick Chat: Ready / Good Luck / Wait / Hello presets + rolling feed.</summary>
    public sealed class QuickChatView : MonoBehaviour
    {
        private readonly Queue<string> _feed = new Queue<string>(8);
        private LobbyButtonPressAnimator[] _anims = new LobbyButtonPressAnimator[4];

        private void OnEnable()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby != null)
            {
                lobby.QuickChatReceived += HandleQuickChat;
            }
        }

        private void OnDisable()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby != null)
            {
                lobby.QuickChatReceived -= HandleQuickChat;
            }
        }

        private void OnGUI()
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            if (lobby == null || !lobby.IsInMatch)
            {
                return;
            }

            float x = 16f;
            float y = Screen.height - 210f;
            PreRaceLobbyTheme.DrawPanel(new Rect(x, y, 250f, 100f));
            GUI.Label(new Rect(x + 8f, y + 6f, 230f, 18f), "Quick Chat", PreRaceLobbyTheme.Muted);

            QuickChatMessage[] messages =
            {
                QuickChatMessage.Ready,
                QuickChatMessage.GoodLuck,
                QuickChatMessage.Wait,
                QuickChatMessage.Hello
            };

            for (int i = 0; i < messages.Length; i++)
            {
                float bx = x + 8f + (i % 2) * 118f;
                float by = y + 30f + (i / 2) * 32f;
                Rect rect = _anims[i].Apply(new Rect(bx, by, 110f, 28f), 2f);
                if (GUI.Button(rect, QuickChatMessageTextResolver.ResolveText(messages[i]), PreRaceLobbyTheme.PanelButton))
                {
                    _anims[i].NotifyPressed();
                    lobby.SendQuickChat(messages[i]);
                }
            }

            float feedY = y - 78f;
            PreRaceLobbyTheme.DrawPanel(new Rect(x, feedY, 250f, 72f));
            int line = 0;
            foreach (string entry in _feed)
            {
                GUI.Label(new Rect(x + 8f, feedY + 6f + line * 16f, 234f, 16f), entry, PreRaceLobbyTheme.Muted);
                line++;
            }
        }

        private void HandleQuickChat(int connectionId, QuickChatMessage message)
        {
            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            string name = "Player";
            if (lobby != null)
            {
                foreach (MatchParticipant p in lobby.Participants)
                {
                    if (p.Identity.ConnectionId == connectionId)
                    {
                        name = p.Identity.DisplayName;
                        break;
                    }
                }
            }

            _feed.Enqueue(name + ": " + QuickChatMessageTextResolver.ResolveText(message));
            while (_feed.Count > 4)
            {
                _feed.Dequeue();
            }
        }
    }
}
