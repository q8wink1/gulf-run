using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.RaceHud.Configuration;
using GulfRun.Features.RaceHud.UI;
using UnityEngine;

namespace GulfRun.Features.RaceHud.Vfx
{
    /// <summary>
    /// Placeholder dust / speed-trail OnGUI particles while running — pooled
    /// conceptually via a fixed ring buffer (no per-frame allocations).
    /// </summary>
    public sealed class RaceVfxPresenter : MonoBehaviour
    {
        [SerializeField] private RaceHudConfig config;

        private readonly DustMote[] _dust = new DustMote[32];
        private int _dustWrite;
        private float _spawnAccumulator;
        private bool _running;

        private void OnEnable()
        {
            IGameStateProvider state = GameStateService.Current;
            if (state != null)
            {
                state.StateChanged += HandleStateChanged;
                _running = state.CurrentState == GameLoopState.Running;
            }
        }

        private void OnDisable()
        {
            IGameStateProvider state = GameStateService.Current;
            if (state != null)
            {
                state.StateChanged -= HandleStateChanged;
            }
        }

        private void Update()
        {
            if (!_running)
            {
                return;
            }

            float rate = config != null ? config.DustSpawnRate : 14f;
            _spawnAccumulator += Time.deltaTime * rate;
            while (_spawnAccumulator >= 1f)
            {
                _spawnAccumulator -= 1f;
                SpawnDust();
            }

            for (int i = 0; i < _dust.Length; i++)
            {
                if (!_dust[i].Alive)
                {
                    continue;
                }

                _dust[i].Life -= Time.deltaTime;
                _dust[i].X -= Time.deltaTime * 40f;
                _dust[i].Y += Time.deltaTime * 8f;
                if (_dust[i].Life <= 0f)
                {
                    _dust[i].Alive = false;
                }
            }
        }

        private void OnGUI()
        {
            if (!_running)
            {
                return;
            }

            float scale = HudLayoutScale.Resolve(Screen.width, Screen.height);
            Color previous = GUI.color;
            float speed = RunSpeedService.Current != null ? RunSpeedService.Current.CurrentSpeed : 0f;
            float trailMin = config != null ? config.SpeedTrailMinSpeed : 8f;
            bool trail = speed >= trailMin;

            for (int i = 0; i < _dust.Length; i++)
            {
                if (!_dust[i].Alive)
                {
                    continue;
                }

                float a = Mathf.Clamp01(_dust[i].Life / 0.55f) * 0.55f;
                GUI.color = new Color(RaceHudTheme.Sand.r, RaceHudTheme.Sand.g, RaceHudTheme.Sand.b, a);
                float size = (trail ? 5f : 3f) * scale;
                GUI.Box(new Rect(_dust[i].X, _dust[i].Y, size, size), string.Empty);
            }

            GUI.color = previous;
        }

        private void SpawnDust()
        {
            int max = config != null ? Mathf.Min(config.DustParticleCount, _dust.Length) : 18;
            if (max <= 0)
            {
                return;
            }

            _dustWrite = (_dustWrite + 1) % max;
            _dust[_dustWrite] = new DustMote
            {
                Alive = true,
                Life = 0.55f,
                X = Screen.width * 0.42f + (_dustWrite % 5) * 3f,
                Y = Screen.height * 0.62f
            };
        }

        private void HandleStateChanged(GameLoopState state) => _running = state == GameLoopState.Running;

        private struct DustMote
        {
            public bool Alive;
            public float Life;
            public float X;
            public float Y;
        }
    }
}
