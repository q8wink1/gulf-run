using System.Collections;
using GulfRun.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.GameplayHud
{
    /// <summary>
    /// Sprint 23.3 / 23.12 — Gameplay HUD controller. Pause toggles a visual-only
    /// Pause Menu; coin/gem counters update from on-track collectibles.
    /// Optional notification demo cycles placeholder toasts.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameplayHudController : SceneSingleton<GameplayHudController>
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private RectTransform notificationRoot;
        [SerializeField] private Text notificationText;
        [SerializeField] private Graphic notificationBackground;
        [SerializeField] private Text coinsText;
        [SerializeField] private Text gemsText;
        [SerializeField] private bool playNotificationDemo = true;
        [SerializeField] private float notificationDemoIntervalSeconds = 4.5f;
        [SerializeField] private float notificationVisibleSeconds = 1.6f;

        private static readonly string[] DemoMessages =
        {
            "+10 Coins",
            "Mission Completed",
            "New Record"
        };

        private Coroutine _demoRoutine;
        private Coroutine _toastRoutine;
        private int _demoIndex;
        private bool _paused;
        private int _coins;
        private int _gems;
        private Color _textBase = Color.white;
        private Color _bgBase = Color.white;

        public int Coins => _coins;
        public int Gems => _gems;

        protected override void Awake()
        {
            base.Awake();
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }

            if (notificationText != null)
            {
                _textBase = notificationText.color;
            }

            if (notificationBackground != null)
            {
                _bgBase = notificationBackground.color;
            }

            ResetNotificationVisual();
            RefreshCurrencyLabels();
        }

        private void OnEnable()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(TogglePauseMenu);
            }

            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(ClosePauseMenu);
            }

            if (playNotificationDemo)
            {
                _demoRoutine = StartCoroutine(NotificationDemoLoop());
            }
        }

        private void OnDisable()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveListener(TogglePauseMenu);
            }

            if (resumeButton != null)
            {
                resumeButton.onClick.RemoveListener(ClosePauseMenu);
            }

            if (_demoRoutine != null)
            {
                StopCoroutine(_demoRoutine);
                _demoRoutine = null;
            }

            if (_toastRoutine != null)
            {
                StopCoroutine(_toastRoutine);
                _toastRoutine = null;
            }
        }

        /// <summary>Adds to the session coin counter and refreshes the HUD chip.</summary>
        public void AddCoins(int amount)
        {
            if (amount == 0)
            {
                return;
            }

            int next = _coins + amount;
            _coins = next < 0 ? 0 : next;
            RefreshCurrencyLabels();
        }

        /// <summary>Adds to the session gem counter and refreshes the HUD chip.</summary>
        public void AddGems(int amount)
        {
            if (amount == 0)
            {
                return;
            }

            int next = _gems + amount;
            _gems = next < 0 ? 0 : next;
            RefreshCurrencyLabels();
        }

        /// <summary>Resets session counters (e.g. race restart).</summary>
        public void ResetCurrencyCounters()
        {
            _coins = 0;
            _gems = 0;
            RefreshCurrencyLabels();
        }

        public void TogglePauseMenu()
        {
            if (_paused)
            {
                ClosePauseMenu();
            }
            else
            {
                OpenPauseMenu();
            }
        }

        public void OpenPauseMenu()
        {
            _paused = true;
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }
        }

        public void ClosePauseMenu()
        {
            _paused = false;
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
        }

        private void RefreshCurrencyLabels()
        {
            if (coinsText != null)
            {
                coinsText.text = "COINS  " + _coins;
            }

            if (gemsText != null)
            {
                gemsText.text = "GEMS  " + _gems;
            }
        }

        private IEnumerator NotificationDemoLoop()
        {
            yield return new WaitForSecondsRealtime(1.25f);

            while (enabled)
            {
                ShowNotification(DemoMessages[_demoIndex % DemoMessages.Length]);
                _demoIndex++;
                yield return new WaitForSecondsRealtime(notificationDemoIntervalSeconds);
            }
        }

        private void ShowNotification(string message)
        {
            if (_toastRoutine != null)
            {
                StopCoroutine(_toastRoutine);
            }

            _toastRoutine = StartCoroutine(AnimateNotification(message));
        }

        private IEnumerator AnimateNotification(string message)
        {
            if (notificationText != null)
            {
                notificationText.text = message;
            }

            if (notificationRoot != null)
            {
                notificationRoot.gameObject.SetActive(true);
                notificationRoot.anchoredPosition = new Vector2(0f, 24f);
            }

            float fadeIn = 0.22f;
            float hold = Mathf.Max(0.2f, notificationVisibleSeconds);
            float fadeOut = 0.28f;

            yield return FadeNotification(0f, 1f, fadeIn, 24f, 0f);
            yield return new WaitForSecondsRealtime(hold);
            yield return FadeNotification(1f, 0f, fadeOut, 0f, -18f);

            ResetNotificationVisual();
            _toastRoutine = null;
        }

        private IEnumerator FadeNotification(float from, float to, float duration, float slideFromY, float slideToY)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = t * t * (3f - (2f * t));
                ApplyNotificationAlpha(Mathf.Lerp(from, to, eased));

                if (notificationRoot != null)
                {
                    float y = Mathf.Lerp(slideFromY, slideToY, eased);
                    notificationRoot.anchoredPosition = new Vector2(0f, y);
                }

                yield return null;
            }

            ApplyNotificationAlpha(to);
        }

        private void ApplyNotificationAlpha(float alpha)
        {
            if (notificationText != null)
            {
                Color c = _textBase;
                c.a = _textBase.a * alpha;
                notificationText.color = c;
            }

            if (notificationBackground != null)
            {
                Color c = _bgBase;
                c.a = _bgBase.a * alpha;
                notificationBackground.color = c;
            }
        }

        private void ResetNotificationVisual()
        {
            ApplyNotificationAlpha(0f);
            if (notificationRoot != null)
            {
                notificationRoot.anchoredPosition = Vector2.zero;
                notificationRoot.gameObject.SetActive(false);
            }
        }
    }
}
