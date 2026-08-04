using System;
using GulfRun.Features.CameraSystem;
using UnityEngine;
using UnityEngine.Events;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.10 — routes obstacle player hits into placeholder feedback hooks.
    /// Fires animation / shake / SFX / speed-reduction events without applying
    /// real gameplay penalties. Optional light <see cref="CameraShake"/> only.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ObstacleGameplayBridge : MonoBehaviour
    {
        [Header("Wiring")]
        [SerializeField] private RaceManager raceManager;
        [SerializeField] private RunnerAnimatorDriver animatorDriver;
        [SerializeField] private CameraShake cameraShake;

        [Header("Feedback (prepare only)")]
        [Tooltip("When true, calls CameraShake.Shake with light defaults on hit.")]
        [SerializeField] private bool applyLightCameraShake = true;

        [SerializeField] private float hitShakeIntensity = 0.18f;
        [SerializeField] private float hitShakeDuration = 0.12f;

        [Header("Inspector Events (placeholders)")]
        [SerializeField] private UnityEvent onHitAnimation;
        [SerializeField] private UnityEvent onCameraShake;
        [SerializeField] private UnityEvent onSoundEffect;
        [SerializeField] private UnityEvent onSpeedReduction;

        /// <summary>Raised for every obstacle hit (Features subscribers).</summary>
        public event Action<Obstacle, RunnerPlayerController> ObstacleHit;

        public event Action HitAnimationRequested;
        public event Action CameraShakeRequested;
        public event Action SoundEffectRequested;
        public event Action SpeedReductionRequested;

        private void OnEnable()
        {
            Obstacle.AnyHit += HandleObstacleHit;
        }

        private void OnDisable()
        {
            Obstacle.AnyHit -= HandleObstacleHit;
        }

        private void Start()
        {
            ResolveWiring();
        }

        /// <summary>Stub: future speed penalty. Does not change player speed this sprint.</summary>
        public void PrepareSpeedReduction()
        {
            SpeedReductionRequested?.Invoke();
            if (onSpeedReduction != null)
            {
                onSpeedReduction.Invoke();
            }
        }

        /// <summary>Stub: future hit anim. Triggers animator Hit when a driver is wired.</summary>
        public void PrepareHitAnimation()
        {
            if (animatorDriver != null)
            {
                animatorDriver.PrepareHit();
            }

            HitAnimationRequested?.Invoke();
            if (onHitAnimation != null)
            {
                onHitAnimation.Invoke();
            }
        }

        /// <summary>Optional light shake + event. No race / score penalty.</summary>
        public void PrepareCameraShake()
        {
            if (applyLightCameraShake && cameraShake != null)
            {
                cameraShake.Shake(hitShakeIntensity, hitShakeDuration);
            }

            CameraShakeRequested?.Invoke();
            if (onCameraShake != null)
            {
                onCameraShake.Invoke();
            }
        }

        /// <summary>Stub: future SFX play. Event only this sprint.</summary>
        public void PrepareSoundEffect()
        {
            SoundEffectRequested?.Invoke();
            if (onSoundEffect != null)
            {
                onSoundEffect.Invoke();
            }
        }

        private void HandleObstacleHit(Obstacle obstacle, RunnerPlayerController player)
        {
            ObstacleHit?.Invoke(obstacle, player);
            PrepareHitAnimation();
            PrepareCameraShake();
            PrepareSoundEffect();
            PrepareSpeedReduction();
        }

        private void ResolveWiring()
        {
            if (raceManager == null)
            {
                raceManager = RaceManager.Instance != null
                    ? RaceManager.Instance
                    : FindObjectOfType<RaceManager>();
            }

            if (animatorDriver == null && raceManager != null && raceManager.PlayerController != null)
            {
                animatorDriver = raceManager.PlayerController.GetComponent<RunnerAnimatorDriver>();
            }

            if (cameraShake == null)
            {
                if (raceManager != null && raceManager.CameraController != null)
                {
                    cameraShake = raceManager.CameraController.GetComponent<CameraShake>();
                }

                if (cameraShake == null)
                {
                    cameraShake = FindObjectOfType<CameraShake>();
                }
            }
        }
    }
}
