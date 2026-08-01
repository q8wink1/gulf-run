using GulfRun.Core;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.RaceHud.Configuration;
using UnityEngine;

namespace GulfRun.Features.RaceHud
{
    /// <summary>
    /// Scene-scoped composition root for the in-race HUD: owns the race
    /// timer clock and exposes <see cref="RaceHudConfig"/> to sibling views.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RaceHudController : SceneSingleton<RaceHudController>, IRaceTimerProvider
    {
        [SerializeField] private RaceHudConfig config;

        private float _elapsedRaceSeconds;
        private bool _racing;

        public RaceHudConfig Config => config;
        public float ElapsedRaceSeconds => _elapsedRaceSeconds;

        float IRaceTimerProvider.ElapsedRaceSeconds => _elapsedRaceSeconds;

        private void OnEnable()
        {
            RaceTimerService.Current = this;
            IGameStateProvider state = GameStateService.Current;
            if (state != null)
            {
                state.StateChanged += HandleStateChanged;
                _racing = state.CurrentState == GameLoopState.Running;
            }
        }

        private void OnDisable()
        {
            IGameStateProvider state = GameStateService.Current;
            if (state != null)
            {
                state.StateChanged -= HandleStateChanged;
            }

            if (ReferenceEquals(RaceTimerService.Current, this))
            {
                RaceTimerService.Current = null;
            }
        }

        private void Update()
        {
            if (_racing)
            {
                _elapsedRaceSeconds += Time.deltaTime;
            }
        }

        private void HandleStateChanged(GameLoopState state)
        {
            if (state == GameLoopState.Countdown || state == GameLoopState.Ready || state == GameLoopState.Restart)
            {
                _elapsedRaceSeconds = 0f;
                _racing = false;
                return;
            }

            _racing = state == GameLoopState.Running;
        }
    }
}
