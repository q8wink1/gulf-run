using System;
using GulfRun.Domain;
using GulfRun.Features.Character.Configuration;
using UnityEngine;

namespace GulfRun.Features.Character.Locker
{
    /// <summary>
    /// Fake OnGUI preview animation state: Idle/Run/DoubleJump/Victory/
    /// Defeat/Celebrate with breathing, blink, and smooth transitions.
    /// </summary>
    public sealed class CharacterPreviewAnimator
    {
        private static readonly CharacterAnimationState[] CycleOrder =
        {
            CharacterAnimationState.Idle,
            CharacterAnimationState.Run,
            CharacterAnimationState.DoubleJump,
            CharacterAnimationState.Win,
            CharacterAnimationState.Lose,
            CharacterAnimationState.Celebrate
        };

        private static readonly System.Random BlinkRng = new System.Random(17);

        private CharacterAnimationState _state = CharacterAnimationState.Idle;
        private CharacterAnimationState _fromState = CharacterAnimationState.Idle;
        private float _transitionT = 1f;
        private float _blinkTimer;
        private bool _blinkClosed;
        private float _nextBlinkIn;

        public CharacterAnimationState State => _state;
        public float Transition01 => _transitionT;
        public bool EyesClosed => _blinkClosed;

        public void Reset(LockerUiConfig config)
        {
            _state = CharacterAnimationState.Idle;
            _fromState = CharacterAnimationState.Idle;
            _transitionT = 1f;
            _blinkClosed = false;
            ScheduleNextBlink(config);
        }

        public void SetState(CharacterAnimationState state, LockerUiConfig config)
        {
            if (_state == state)
            {
                return;
            }

            _fromState = _state;
            _state = state;
            _transitionT = 0f;
            if (config == null)
            {
                _transitionT = 1f;
            }
        }

        public void CycleNext(LockerUiConfig config)
        {
            int index = 0;
            for (int i = 0; i < CycleOrder.Length; i++)
            {
                if (CycleOrder[i] == _state)
                {
                    index = i;
                    break;
                }
            }

            SetState(CycleOrder[(index + 1) % CycleOrder.Length], config);
        }

        public void Tick(float deltaTime, LockerUiConfig config)
        {
            if (config == null)
            {
                return;
            }

            if (_transitionT < 1f)
            {
                float duration = Mathf.Max(0.01f, config.AnimationTransitionSeconds);
                _transitionT = Mathf.Clamp01(_transitionT + deltaTime / duration);
            }

            _blinkTimer += deltaTime;
            if (_blinkClosed)
            {
                if (_blinkTimer >= config.BlinkClosedSeconds)
                {
                    _blinkClosed = false;
                    _blinkTimer = 0f;
                    ScheduleNextBlink(config);
                }
            }
            else if (_blinkTimer >= _nextBlinkIn)
            {
                _blinkClosed = true;
                _blinkTimer = 0f;
            }
        }

        public float BreathOffsetY(LockerUiConfig config, float timeSeconds)
        {
            if (config == null)
            {
                return 0f;
            }

            return Mathf.Sin(timeSeconds * config.BreathHz * Mathf.PI * 2f) * config.BreathAmplitude;
        }

        public float IdleSwayDegrees(LockerUiConfig config, float timeSeconds)
        {
            if (config == null || _state != CharacterAnimationState.Idle)
            {
                return 0f;
            }

            return Mathf.Sin(timeSeconds * 0.7f) * config.IdleSwayDegrees;
        }

        private void ScheduleNextBlink(LockerUiConfig config)
        {
            if (config == null)
            {
                _nextBlinkIn = 3f;
                return;
            }

            float min = config.BlinkOpenMinSeconds;
            float max = config.BlinkOpenMaxSeconds;
            _nextBlinkIn = min + (float)BlinkRng.NextDouble() * (max - min);
        }
    }
}
