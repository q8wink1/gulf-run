using GulfRun.Core;
using GulfRun.Core.Managers;
using UnityEngine;

namespace GulfRun.Features.Intro
{
    /// <summary>
    /// Sprint 14 "GULFRUN BRAND INTRO": the Intro scene's composition root
    /// and its only piece of shared timing state — every other Intro view
    /// (<see cref="IntroBackgroundView"/>, <see cref="IntroFalconView"/>,
    /// <see cref="IntroLogoView"/>) independently reads
    /// <see cref="ElapsedSeconds"/> off this <see cref="SceneSingleton{T}"/>
    /// against the shared <see cref="IntroTimeline"/> constants and draws
    /// itself — the same "widgets read shared state independently" shape
    /// Sprint 13's Main Menu views use. This controller owns only the
    /// cross-cutting concerns a single view cannot: sound cue firing, the
    /// Skip prompt/"player may skip it after the first launch", the
    /// fade-to-black overlay, and the final scene handoff.
    /// </summary>
    public sealed class IntroSequenceController : SceneSingleton<IntroSequenceController>
    {
        [Header("Sound (SOUND: premium startup / soft desert wind / falcon wing / golden shimmer)")]
        [SerializeField] private AudioClip startupSound;
        [SerializeField] private AudioClip desertWindAmbience;
        [SerializeField] private AudioClip falconWingSound;
        [SerializeField] private AudioClip logoShimmerSound;
        [SerializeField] private AudioClip introMusic;
        [SerializeField, Range(0f, 1f)] private float introMusicVolume = 0.5f;
        [SerializeField, Range(0f, 1f)] private float desertWindVolume = 0.4f;

        private double _startedAtSeconds;
        private bool _falconSoundPlayed;
        private bool _shimmerSoundPlayed;
        private bool _transitionStarted;
        private double _transitionStartedAtSeconds;
        private bool _skipIsAllowedThisLaunch;

        private GUIStyle _skipButtonStyle;

        /// <summary>Seconds elapsed since this Intro scene instance started — every Intro view's single clock.</summary>
        public double ElapsedSeconds => Time.timeAsDouble - _startedAtSeconds;

        public bool IsTransitioning => _transitionStarted;

        /// <summary>Brief: "The player may skip it after the first launch" — true only from the device's second+ launch onward, and only once a short grace period has passed.</summary>
        public bool IsSkipAvailable => _skipIsAllowedThisLaunch && !_transitionStarted && ElapsedSeconds >= IntroTimeline.SkipButtonGraceSeconds;

        protected override void Awake()
        {
            base.Awake();
            _startedAtSeconds = Time.timeAsDouble;
            _skipIsAllowedThisLaunch = SaveManager.Instance != null && SaveManager.Instance.HasSeenIntro;
        }

        private void Start()
        {
            if (AudioManager.Instance == null)
            {
                return;
            }

            if (startupSound != null)
            {
                AudioManager.Instance.PlayOneShot(startupSound, 0.9f);
            }

            AudioManager.Instance.PlayAmbient(desertWindAmbience, desertWindVolume);

            if (introMusic != null)
            {
                AudioManager.Instance.PlayMusic(introMusic, introMusicVolume, loop: false);
            }
        }

        private void Update()
        {
            double t = ElapsedSeconds;

            if (!_falconSoundPlayed && t >= IntroTimeline.FalconFlyAcrossStart)
            {
                _falconSoundPlayed = true;
                AudioManager.Instance?.PlayOneShot(falconWingSound, 0.7f);
            }

            if (!_shimmerSoundPlayed && t >= IntroTimeline.ShineSweepStart)
            {
                _shimmerSoundPlayed = true;
                AudioManager.Instance?.PlayOneShot(logoShimmerSound, 0.8f);
            }

            if (!_transitionStarted && t >= IntroTimeline.SequenceEnd)
            {
                BeginTransition();
            }

            if (_transitionStarted && Time.timeAsDouble - _transitionStartedAtSeconds >= IntroTimeline.FadeToBlackDuration)
            {
                CompleteTransition();
            }
        }

        /// <summary>Called by the Skip button/tap. A no-op unless <see cref="IsSkipAvailable"/>.</summary>
        public void RequestSkip()
        {
            if (IsSkipAvailable)
            {
                BeginTransition();
            }
        }

        private void BeginTransition()
        {
            _transitionStarted = true;
            _transitionStartedAtSeconds = Time.timeAsDouble;
            AudioManager.Instance?.StopAmbient();
            AudioManager.Instance?.FadeMusicTo(0f, IntroTimeline.FadeToBlackDuration);
        }

        private void CompleteTransition()
        {
            // Guard against Update() firing this twice in the same frame window.
            enabled = false;
            SaveManager.Instance?.MarkIntroSeen();
            SceneManager.Instance?.LoadMainMenu();
        }

        /// <summary>0 (fully visible scene) .. 1 (fully black) — drives the "Smooth fade into the Main Lobby. No loading stutter." black overlay.</summary>
        public float TransitionAlpha01()
        {
            if (!_transitionStarted)
            {
                return 0f;
            }

            return Mathf.Clamp01((float)((Time.timeAsDouble - _transitionStartedAtSeconds) / IntroTimeline.FadeToBlackDuration));
        }

        private void OnGUI()
        {
            DrawSkipButton();
            DrawFadeOverlay();
        }

        private void DrawSkipButton()
        {
            if (!IsSkipAvailable)
            {
                return;
            }

            EnsureStyles();
            const float width = 110f;
            const float height = 36f;
            Rect rect = new Rect(Screen.width - width - 20f, Screen.height - height - 20f, width, height);
            if (GUI.Button(rect, "Skip »", _skipButtonStyle))
            {
                RequestSkip();
            }
        }

        private void DrawFadeOverlay()
        {
            float alpha = TransitionAlpha01();
            if (alpha <= 0f)
            {
                return;
            }

            Color previous = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, alpha);
            GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), string.Empty);
            GUI.color = previous;
        }

        private void EnsureStyles()
        {
            if (_skipButtonStyle != null)
            {
                return;
            }

            _skipButtonStyle = new GUIStyle(GUI.skin.button) { fontSize = 13, fontStyle = FontStyle.Bold };
        }
    }
}
