using System.Collections.Generic;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.RaceHud.Configuration;
using UnityEngine;

namespace GulfRun.Features.RaceHud.UI
{
    /// <summary>
    /// Production OnGUI Race HUD: position, lap, speed, shield, weapon slot,
    /// coins/gems, race timer, active effects, trap warning, minimap progress.
    /// Reads exclusively through Core.Services seams.
    /// </summary>
    public sealed class RaceHudView : MonoBehaviour
    {
        [SerializeField] private RaceHudConfig config;

        private int _displayedPlace = 1;
        private float _placePunchRemaining;
        private float _weaponGlowRemaining;
        private WeaponId? _lastWeapon;

        private void OnGUI()
        {
            IGameStateProvider state = GameStateService.Current;
            if (state == null)
            {
                return;
            }

            GameLoopState loop = state.CurrentState;
            if (loop != GameLoopState.Running && loop != GameLoopState.Countdown && loop != GameLoopState.Paused)
            {
                return;
            }

            // Hide chrome during ceremony phases — finish banner / podium own the screen.
            IRaceStandingsHudProvider standings = RaceStandingsHudService.Current;
            if (standings != null && standings.CeremonyPhase != RaceEndPhase.None)
            {
                return;
            }

            float scale = HudLayoutScale.Resolve(Screen.width, Screen.height);
            float pad = config != null ? config.EdgePadding * scale : 12f * scale;

            TickAnimations();
            DrawPosition(pad, scale, standings);
            DrawTopRight(pad, scale);
            DrawWeaponSlot(pad, scale);
            DrawEffects(pad, scale);
            DrawTrapWarning(pad, scale);
            DrawMinimap(pad, scale, standings);
        }

        private void TickAnimations()
        {
            IRaceStandingsHudProvider standings = RaceStandingsHudService.Current;
            int place = standings != null ? standings.LocalPlace : 1;
            if (place != _displayedPlace)
            {
                _displayedPlace = place;
                _placePunchRemaining = config != null ? config.PositionChangePunchSeconds : 0.4f;
            }

            if (_placePunchRemaining > 0f)
            {
                _placePunchRemaining -= Time.unscaledDeltaTime;
            }

            IWeaponHudProvider weapons = WeaponHudService.Current;
            if (weapons != null && weapons.LocalSlots.Count > 0)
            {
                WeaponId? current = weapons.LocalSlots[0].Weapon;
                if (current.HasValue && current != _lastWeapon)
                {
                    _weaponGlowRemaining = config != null ? config.WeaponPickupGlowSeconds : 0.8f;
                }

                _lastWeapon = current;
            }

            if (_weaponGlowRemaining > 0f)
            {
                _weaponGlowRemaining -= Time.unscaledDeltaTime;
            }
        }

        private void DrawPosition(float pad, float scale, IRaceStandingsHudProvider standings)
        {
            int place = standings != null ? standings.LocalPlace : _displayedPlace;
            string ordinal = RacePositionFormatter.FormatOrdinal(place);
            float punch = _placePunchRemaining > 0f ? 1.15f : 1f;
            float width = 140f * scale * punch;
            float height = 64f * scale * punch;
            Rect rect = new Rect(pad, pad, width, height);
            RaceHudTheme.DrawPanel(rect);
            GUI.Label(rect, ordinal, RaceHudTheme.Title(scale * punch));

            int maxLaps = config != null ? Mathf.Max(1, config.MaxLaps) : 1;
            GUI.Label(new Rect(pad, pad + height + 4f, 160f * scale, 22f * scale), $"LAP 1/{maxLaps}", RaceHudTheme.Muted(scale));
        }

        private void DrawTopRight(float pad, float scale)
        {
            float chipW = 120f * scale;
            float chipH = 28f * scale;
            float x = Screen.width - pad - chipW;
            float y = pad;

            IRaceProgressProvider progress = RaceProgressService.Current;
            int coins = progress != null ? progress.CoinsCollected : 0;
            DrawChip(new Rect(x, y, chipW, chipH), scale, $"🪙 {coins}", RaceHudTheme.Gold);
            y += chipH + 6f * scale;

            if (config == null || config.ShowGemCounter)
            {
                int gems = 0;
                ILocalProfileProvider profile = LocalProfileProviderService.Current;
                if (profile != null && profile.HasProfile)
                {
                    gems = profile.LocalProfile.Gems;
                }

                DrawChip(new Rect(x, y, chipW, chipH), scale, $"💎 {gems}", RaceHudTheme.Shield);
                y += chipH + 6f * scale;
            }

            float elapsed = RaceTimerService.Current != null
                ? RaceTimerService.Current.ElapsedRaceSeconds
                : 0f;
            int minutes = (int)(elapsed / 60f);
            int seconds = (int)(elapsed % 60f);
            DrawChip(new Rect(x, y, chipW, chipH), scale, $"{minutes:00}:{seconds:00}", RaceHudTheme.TextPrimary);
            y += chipH + 6f * scale;

            IRunSpeedProvider speed = RunSpeedService.Current;
            float mps = speed != null ? speed.CurrentSpeed : 0f;
            DrawChip(new Rect(x, y, chipW, chipH), scale, $"{mps:0.0} m/s", RaceHudTheme.SpeedBoost);
            y += chipH + 6f * scale;

            IActiveEffectsHudProvider effects = ActiveEffectsHudService.Current;
            bool shield = effects != null && effects.HasShield;
            DrawChip(new Rect(x, y, chipW, chipH), scale, shield ? "◆ SHIELD" : "◇ —", shield ? RaceHudTheme.Shield : RaceHudTheme.TextMuted);
        }

        private void DrawWeaponSlot(float pad, float scale)
        {
            IWeaponHudProvider weapons = WeaponHudService.Current;
            WeaponHudSlotSnapshot slot = default;
            if (weapons != null && weapons.LocalSlots.Count > 0)
            {
                slot = weapons.LocalSlots[0];
            }

            float size = 72f * scale;
            Rect rect = new Rect(pad, Screen.height - pad - size - 36f * scale, size, size);
            RaceHudTheme.DrawPanel(rect);

            if (_weaponGlowRemaining > 0f)
            {
                Color previous = GUI.color;
                GUI.color = new Color(RaceHudTheme.GoldBright.r, RaceHudTheme.GoldBright.g, RaceHudTheme.GoldBright.b, 0.55f);
                GUI.Box(new Rect(rect.x - 4f, rect.y - 4f, rect.width + 8f, rect.height + 8f), string.Empty);
                GUI.color = previous;
            }

            if (slot.IsEmpty)
            {
                GUI.Label(rect, "—", RaceHudTheme.Title(scale));
            }
            else
            {
                string shortName = ShortWeaponName(slot.Weapon.Value);
                GUI.Label(new Rect(rect.x + 6f, rect.y + 8f, rect.width - 12f, 28f * scale), shortName, RaceHudTheme.Label(scale));
                GUI.Label(new Rect(rect.x + 6f, rect.y + 36f * scale, rect.width - 12f, 24f * scale), $"x{slot.UsesRemaining}", RaceHudTheme.Muted(scale));
                if (slot.CooldownRemaining01 > 0.01f)
                {
                    RaceHudTheme.DrawBar(
                        new Rect(rect.x + 6f, rect.yMax - 10f * scale, rect.width - 12f, 6f * scale),
                        1f - slot.CooldownRemaining01,
                        RaceHudTheme.Gold);
                }
            }
        }

        private void DrawEffects(float pad, float scale)
        {
            IActiveEffectsHudProvider effects = ActiveEffectsHudService.Current;
            if (effects == null)
            {
                return;
            }

            IReadOnlyList<ActiveHudEffectSnapshot> list = effects.ActiveEffects;
            float y = pad + 100f * scale;
            for (int i = 0; i < list.Count; i++)
            {
                ActiveHudEffectSnapshot effect = list[i];
                Rect rect = new Rect(pad, y, 150f * scale, 28f * scale);
                RaceHudTheme.DrawPanel(rect);
                string label = $"{HudEffectKindResolver.ResolveShapeTag(effect.Kind)} {HudEffectKindResolver.ResolveLabel(effect.Kind)}";
                GUI.Label(new Rect(rect.x + 6f, rect.y, rect.width - 12f, 16f * scale), label, RaceHudTheme.Muted(scale));
                RaceHudTheme.DrawBar(
                    new Rect(rect.x + 6f, rect.yMax - 8f * scale, rect.width - 12f, 5f * scale),
                    effect.NormalizedRemaining,
                    RaceHudTheme.ColorFor(effect.Kind));
                y += 34f * scale;
            }
        }

        private void DrawTrapWarning(float pad, float scale)
        {
            ITrapProximityHudProvider traps = TrapProximityHudService.Current;
            if (traps == null || !traps.IsTrapNearby)
            {
                return;
            }

            float size = 36f * scale;
            Rect rect = new Rect(Screen.width * 0.5f - size * 0.5f, pad + 8f * scale, size, size);
            Color previous = GUI.color;
            float pulse = 0.55f + 0.45f * Mathf.Abs(Mathf.Sin(Time.time * 8f));
            GUI.color = new Color(RaceHudTheme.Danger.r, RaceHudTheme.Danger.g, RaceHudTheme.Danger.b, pulse);
            GUI.Box(rect, string.Empty);
            GUI.color = previous;
            GUI.Label(new Rect(rect.x - 40f * scale, rect.yMax + 2f, size + 80f * scale, 18f * scale), "!", RaceHudTheme.Label(scale));
        }

        private void DrawMinimap(float pad, float scale, IRaceStandingsHudProvider standings)
        {
            float width = Mathf.Min(Screen.width - pad * 2f, 420f * scale);
            float height = 18f * scale;
            Rect bar = new Rect((Screen.width - width) * 0.5f, Screen.height - pad - height, width, height);
            RaceHudTheme.DrawPanel(new Rect(bar.x - 4f, bar.y - 10f * scale, bar.width + 8f, bar.height + 20f * scale));
            RaceHudTheme.DrawBar(bar, 1f, SandTrack());

            // Finish line marker
            float finishX = bar.xMax - 4f;
            Color previous = GUI.color;
            GUI.color = RaceHudTheme.FinishLine;
            GUI.Box(new Rect(finishX, bar.y - 4f, 4f, bar.height + 8f), string.Empty);
            GUI.color = previous;

            if (standings == null)
            {
                return;
            }

            IReadOnlyList<RaceProgressMarker> markers = standings.Markers;
            for (int i = 0; i < markers.Count; i++)
            {
                RaceProgressMarker marker = markers[i];
                float mx = bar.x + marker.Progress01 * bar.width;
                float size = marker.IsLocal ? 12f * scale : 8f * scale;
                GUI.color = marker.IsLocal ? RaceHudTheme.LocalMarker : RaceHudTheme.OpponentMarker;
                GUI.Box(new Rect(mx - size * 0.5f, bar.y + (bar.height - size) * 0.5f, size, size), string.Empty);
            }

            GUI.color = previous;
        }

        private static Color SandTrack() => new Color(0.45f, 0.38f, 0.28f, 0.85f);

        private static void DrawChip(Rect rect, float scale, string text, Color accent)
        {
            RaceHudTheme.DrawPanel(rect);
            Color previous = GUI.color;
            GUI.color = accent;
            GUI.Box(new Rect(rect.x + 2f, rect.y + 2f, 4f, rect.height - 4f), string.Empty);
            GUI.color = previous;
            GUI.Label(new Rect(rect.x + 10f, rect.y, rect.width - 12f, rect.height), text, RaceHudTheme.Label(scale));
        }

        private static string ShortWeaponName(WeaponId id)
        {
            string name = id.ToString();
            return name.Length <= 8 ? name : name.Substring(0, 8);
        }
    }
}
