using System.Collections.Generic;
using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.RaceHud.Configuration;
using UnityEngine;

namespace GulfRun.Features.RaceHud.UI
{
    /// <summary>
    /// Quick in-race emote bar (😀😂😎👏💪❤️) — floats the glyph above the
    /// character briefly for local + remote senders.
    /// </summary>
    public sealed class EmoteBarView : MonoBehaviour
    {
        [SerializeField] private RaceHudConfig config;

        private readonly List<FloatingEmote> _active = new List<FloatingEmote>(8);
        private float _cooldownRemaining;
        private IMatchTransport _transport;

        private static readonly RaceEmoteId[] Order =
        {
            RaceEmoteId.Smile,
            RaceEmoteId.Laugh,
            RaceEmoteId.Cool,
            RaceEmoteId.Clap,
            RaceEmoteId.Flex,
            RaceEmoteId.Heart
        };

        private void OnEnable()
        {
            _transport = MatchTransportService.Current;
            if (_transport != null)
            {
                _transport.RaceEmoteReceived += HandleEmoteReceived;
            }
        }

        private void OnDisable()
        {
            if (_transport != null)
            {
                _transport.RaceEmoteReceived -= HandleEmoteReceived;
            }
        }

        private void Update()
        {
            if (_cooldownRemaining > 0f)
            {
                _cooldownRemaining -= Time.unscaledDeltaTime;
            }

            float duration = config != null ? config.EmoteDisplaySeconds : 1.6f;
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                FloatingEmote emote = _active[i];
                emote.Elapsed += Time.unscaledDeltaTime;
                if (emote.Elapsed >= duration)
                {
                    _active.RemoveAt(i);
                }
                else
                {
                    _active[i] = emote;
                }
            }
        }

        private void OnGUI()
        {
            IGameStateProvider state = GameStateService.Current;
            if (state == null || state.CurrentState != GameLoopState.Running)
            {
                return;
            }

            float scale = HudLayoutScale.Resolve(Screen.width, Screen.height);
            DrawButtons(scale);
            DrawFloating(scale);
        }

        private void DrawButtons(float scale)
        {
            float size = 36f * scale;
            float total = Order.Length * (size + 6f * scale);
            float x = (Screen.width - total) * 0.5f;
            float y = Screen.height - 70f * scale;

            for (int i = 0; i < Order.Length; i++)
            {
                RaceEmoteId id = Order[i];
                Rect rect = new Rect(x + i * (size + 6f * scale), y, size, size);
                RaceHudTheme.DrawPanel(rect);
                if (GUI.Button(rect, RaceEmoteGlyphResolver.ResolveGlyph(id), RaceHudTheme.Label(scale)))
                {
                    TrySend(id);
                }
            }
        }

        private void DrawFloating(float scale)
        {
            float duration = config != null ? config.EmoteDisplaySeconds : 1.6f;
            for (int i = 0; i < _active.Count; i++)
            {
                FloatingEmote emote = _active[i];
                float t = emote.Elapsed / duration;
                float alpha = 1f - t;
                float rise = t * 48f * scale;
                Color previous = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, alpha);
                Rect rect = new Rect(Screen.width * 0.5f - 24f * scale, Screen.height * 0.55f - rise, 48f * scale, 40f * scale);
                GUI.Label(rect, RaceEmoteGlyphResolver.ResolveGlyph(emote.Id), RaceHudTheme.Title(scale));
                GUI.color = previous;
            }
        }

        private void TrySend(RaceEmoteId id)
        {
            if (_cooldownRemaining > 0f || _transport == null)
            {
                return;
            }

            _cooldownRemaining = config != null ? config.EmoteCooldownSeconds : 1.2f;
            _transport.SendRaceEmote(id);
        }

        private void HandleEmoteReceived(int connectionId, RaceEmoteId emote)
        {
            _active.Add(new FloatingEmote(emote, 0f));
        }

        private struct FloatingEmote
        {
            public RaceEmoteId Id;
            public float Elapsed;

            public FloatingEmote(RaceEmoteId id, float elapsed)
            {
                Id = id;
                Elapsed = elapsed;
            }
        }
    }
}
