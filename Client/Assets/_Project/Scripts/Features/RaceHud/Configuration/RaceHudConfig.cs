using UnityEngine;

namespace GulfRun.Features.RaceHud.Configuration
{
    /// <summary>
    /// Every tunable Race HUD value — layout, countdown hold, emote duration,
    /// trap proximity, VFX rates, audio clips — so presentation code never
    /// hardcodes magic numbers.
    /// </summary>
    [CreateAssetMenu(fileName = "RaceHudConfig", menuName = "GulfRun/RaceHud/Race HUD Config")]
    public sealed class RaceHudConfig : ScriptableObject
    {
        [Header("Layout")]
        [SerializeField] private int maxLaps = 1;
        [SerializeField] private bool showGemCounter = true;
        [SerializeField] private float edgePadding = 12f;

        [Header("Countdown")]
        [SerializeField] private float goHoldSeconds = 0.75f;
        [SerializeField] private AudioClip countdownTickClip;
        [SerializeField] private AudioClip countdownGoClip;

        [Header("Position")]
        [SerializeField] private float positionChangePunchSeconds = 0.4f;

        [Header("Weapon Slot")]
        [SerializeField] private float weaponPickupGlowSeconds = 0.8f;

        [Header("Trap Warning")]
        [SerializeField] private float trapWarningRadiusMeters = 8f;

        [Header("Emotes")]
        [SerializeField] private float emoteDisplaySeconds = 1.6f;
        [SerializeField] private float emoteCooldownSeconds = 1.2f;

        [Header("Finish Banner")]
        [SerializeField] private float finishBannerSeconds = 3.5f;
        [SerializeField] private int fireworkParticleCount = 36;
        [SerializeField] private float fireworkBurstSpeed = 0.9f;
        [SerializeField] private int finishConfettiCount = 48;
        [SerializeField] private float finishConfettiFallSpeed = 0.35f;
        [SerializeField] private AudioClip finishCrowdClip;
        [SerializeField] private AudioClip finishFanfareClip;

        [Header("Gameplay Audio (optional clips)")]
        [SerializeField] private AudioClip runningLoopClip;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip landingClip;
        [SerializeField] private AudioClip weaponUseClip;
        [SerializeField] private AudioClip trapWarningClip;

        [Header("VFX")]
        [SerializeField] private int dustParticleCount = 18;
        [SerializeField] private float dustSpawnRate = 14f;
        [SerializeField] private float speedTrailMinSpeed = 8f;

        public int MaxLaps => maxLaps;
        public bool ShowGemCounter => showGemCounter;
        public float EdgePadding => edgePadding;
        public float GoHoldSeconds => goHoldSeconds;
        public AudioClip CountdownTickClip => countdownTickClip;
        public AudioClip CountdownGoClip => countdownGoClip;
        public float PositionChangePunchSeconds => positionChangePunchSeconds;
        public float WeaponPickupGlowSeconds => weaponPickupGlowSeconds;
        public float TrapWarningRadiusMeters => trapWarningRadiusMeters;
        public float EmoteDisplaySeconds => emoteDisplaySeconds;
        public float EmoteCooldownSeconds => emoteCooldownSeconds;
        public float FinishBannerSeconds => finishBannerSeconds;
        public int FireworkParticleCount => fireworkParticleCount;
        public float FireworkBurstSpeed => fireworkBurstSpeed;
        public int FinishConfettiCount => finishConfettiCount;
        public float FinishConfettiFallSpeed => finishConfettiFallSpeed;
        public AudioClip FinishCrowdClip => finishCrowdClip;
        public AudioClip FinishFanfareClip => finishFanfareClip;
        public AudioClip RunningLoopClip => runningLoopClip;
        public AudioClip JumpClip => jumpClip;
        public AudioClip LandingClip => landingClip;
        public AudioClip WeaponUseClip => weaponUseClip;
        public AudioClip TrapWarningClip => trapWarningClip;
        public int DustParticleCount => dustParticleCount;
        public float DustSpawnRate => dustSpawnRate;
        public float SpeedTrailMinSpeed => speedTrailMinSpeed;
    }
}
