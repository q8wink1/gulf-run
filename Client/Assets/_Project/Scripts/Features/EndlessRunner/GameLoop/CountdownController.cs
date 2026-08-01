using System;
using GulfRun.Core;
using GulfRun.Core.Services;
using GulfRun.Features.EndlessRunner.Configuration;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.GameLoop
{
    /// <summary>
    /// Drives the automatic 3-2-1-GO race-start countdown. Purely a timer +
    /// display-state producer — it does not touch the player, world, or any
    /// other gameplay system directly; <see cref="GameLoopController"/> ticks
    /// it while in <see cref="Domain.GameLoopState.Countdown"/> and transitions
    /// to Running when <see cref="Finished"/> fires. Sprint 15 Race HUD binds
    /// via <see cref="ICountdownHudProvider"/>; <see cref="CountdownView"/>
    /// remains as a legacy fallback when RaceHud is not present.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CountdownController : SceneSingleton<CountdownController>, ICountdownHudProvider
    {
        [SerializeField] private CountdownConfig config;

        public int SecondsRemaining { get; private set; }
        public bool IsFinished { get; private set; } = true;

        /// <summary>Current display value: "3", "2", "1", or "GO!".</summary>
        public string DisplayText { get; private set; } = string.Empty;

        bool ICountdownHudProvider.IsActive => !IsFinished || DisplayText == "GO!";
        bool ICountdownHudProvider.IsGo => DisplayText == "GO!";

        /// <summary>Raised whenever the whole-seconds countdown value changes.</summary>
        public event Action<int> SecondsChanged;

        /// <summary>Raised exactly once, the moment the countdown reaches GO.</summary>
        public event Action Finished;

        private float _elapsedSeconds;

        private void OnEnable() => CountdownHudService.Current = this;

        private void OnDisable()
        {
            if (ReferenceEquals(CountdownHudService.Current, this))
            {
                CountdownHudService.Current = null;
            }
        }

        /// <summary>Resets and starts a fresh countdown from <see cref="CountdownConfig.DurationSeconds"/>.</summary>
        public void BeginCountdown()
        {
            _elapsedSeconds = 0f;
            IsFinished = false;
            SecondsRemaining = Mathf.CeilToInt(config.DurationSeconds);
            DisplayText = SecondsRemaining.ToString();
            SecondsChanged?.Invoke(SecondsRemaining);
        }

        /// <summary>Advances the countdown; invoked only by <see cref="GameLoopController"/>.</summary>
        public void Tick(float deltaTime)
        {
            if (IsFinished)
            {
                return;
            }

            _elapsedSeconds += deltaTime;
            float remaining = config.DurationSeconds - _elapsedSeconds;
            int wholeSeconds = Mathf.CeilToInt(remaining);
            if (wholeSeconds < 0)
            {
                wholeSeconds = 0;
            }

            if (wholeSeconds != SecondsRemaining && wholeSeconds > 0)
            {
                SecondsRemaining = wholeSeconds;
                DisplayText = SecondsRemaining.ToString();
                SecondsChanged?.Invoke(SecondsRemaining);
            }

            if (remaining <= 0f)
            {
                SecondsRemaining = 0;
                DisplayText = "GO!";
                IsFinished = true;
                Finished?.Invoke();
            }
        }
    }
}
