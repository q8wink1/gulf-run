using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.RaceHud.Configuration;
using UnityEngine;

namespace GulfRun.Features.RaceHud.Audio
{
    /// <summary>
    /// Lightweight in-race audio cues (running loop, jump/land, trap warning).
    /// Clip fields are optional — same "no final audio assets yet" posture as
    /// every prior sprint; wiring is ready the moment clips are assigned.
    /// </summary>
    public sealed class RaceAudioDirector : MonoBehaviour
    {
        [SerializeField] private RaceHudConfig config;

        private PlayerMovementState _lastMovement = PlayerMovementState.Idle;
        private bool _trapWarned;
        private bool _running;

        private void OnEnable()
        {
            IGameStateProvider state = GameStateService.Current;
            if (state != null)
            {
                state.StateChanged += HandleStateChanged;
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
            if (!_running || AudioManager.Instance == null || config == null)
            {
                return;
            }

            ILocalPlayerStateProvider local = LocalPlayerStateService.Current;
            if (local != null)
            {
                PlayerMovementState movement = local.AnimationState;
                if ((movement == PlayerMovementState.Jumping || movement == PlayerMovementState.DoubleJumping)
                    && _lastMovement != PlayerMovementState.Jumping
                    && _lastMovement != PlayerMovementState.DoubleJumping)
                {
                    Play(config.JumpClip);
                }
                else if (movement == PlayerMovementState.Landing
                         || ((movement == PlayerMovementState.Running || movement == PlayerMovementState.Idle)
                             && (_lastMovement == PlayerMovementState.Jumping
                                 || _lastMovement == PlayerMovementState.Falling
                                 || _lastMovement == PlayerMovementState.DoubleJumping)))
                {
                    Play(config.LandingClip);
                }

                _lastMovement = movement;
            }

            ITrapProximityHudProvider traps = TrapProximityHudService.Current;
            if (traps != null && traps.IsTrapNearby)
            {
                if (!_trapWarned)
                {
                    _trapWarned = true;
                    Play(config.TrapWarningClip);
                }
            }
            else
            {
                _trapWarned = false;
            }
        }

        private void HandleStateChanged(GameLoopState state)
        {
            _running = state == GameLoopState.Running;
            if (_running && config != null && config.RunningLoopClip != null && AudioManager.Instance != null)
            {
                // Ambient channel keeps ceremony music independent; running loop is a soft bed.
                AudioManager.Instance.PlayAmbient(config.RunningLoopClip, 0.35f, true);
            }

            if (state == GameLoopState.GameOver || state == GameLoopState.Ready)
            {
                AudioManager.Instance?.StopAmbient();
            }
        }

        private static void Play(AudioClip clip)
        {
            if (clip != null)
            {
                AudioManager.Instance.PlayOneShot(clip);
            }
        }
    }
}
