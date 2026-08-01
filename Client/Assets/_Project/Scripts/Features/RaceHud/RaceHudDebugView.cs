using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.RaceHud
{
    /// <summary>
    /// Sprint 15 Race HUD debug: FPS, Player Speed, Position, Weapon ID,
    /// Trap ID, Network Ping. <c>panelX: 4510</c> is the next free +450 slot
    /// after MainMenuDebugView at 4060.
    /// </summary>
    public sealed class RaceHudDebugView : MonoBehaviour
    {
        [SerializeField] private bool showOnScreenDebug = true;
        [SerializeField] private int panelX = 4510;

        private float _fps;

        private void Update()
        {
            float dt = Time.unscaledDeltaTime;
            if (dt > 0f)
            {
                _fps = Mathf.Lerp(_fps <= 0f ? 1f / dt : _fps, 1f / dt, 0.1f);
            }
        }

        private void OnGUI()
        {
            if (!showOnScreenDebug)
            {
                return;
            }

            const int width = 430;
            const int lineHeight = 20;
            int y = 10;

            void Line(string text)
            {
                GUI.Label(new Rect(panelX, y, width, lineHeight), text);
                y += lineHeight;
            }

            Line("=== Race HUD Debug ===");
            Line($"FPS: {_fps:0.0}");

            IRunSpeedProvider speed = RunSpeedService.Current;
            Line($"Player Speed: {(speed != null ? speed.CurrentSpeed : 0f):0.00} m/s");

            IRaceStandingsHudProvider standings = RaceStandingsHudService.Current;
            Line($"Position: {(standings != null ? RacePositionFormatter.FormatOrdinal(standings.LocalPlace) : "—")}");
            Line($"Progress: {(standings != null ? standings.LocalProgress01 * 100f : 0f):0.0}%");

            IWeaponHudProvider weapons = WeaponHudService.Current;
            string weaponId = "—";
            if (weapons != null && weapons.LocalSlots.Count > 0 && weapons.LocalSlots[0].Weapon.HasValue)
            {
                weaponId = weapons.LocalSlots[0].Weapon.Value.ToString();
            }

            Line($"Weapon ID: {weaponId}");

            ITrapProximityHudProvider traps = TrapProximityHudService.Current;
            Line($"Trap ID: {(traps != null && traps.NearbyTrapId.HasValue ? traps.NearbyTrapId.Value.ToString() : "—")}");
            Line($"Trap Nearby: {(traps != null && traps.IsTrapNearby)}");

            INetworkDiagnosticsProvider net = NetworkDiagnosticsService.Current;
            Line($"Network Ping: {(net != null ? net.LocalPingMilliseconds : 0f):0} ms");

            IRaceTimerProvider timer = RaceTimerService.Current;
            Line($"Race Timer: {(timer != null ? timer.ElapsedRaceSeconds : 0f):0.0}s");

            IActiveEffectsHudProvider effects = ActiveEffectsHudService.Current;
            Line($"Shield: {(effects != null && effects.HasShield)} SpeedBoost: {(effects != null && effects.HasSpeedBoost)}");
        }
    }
}
