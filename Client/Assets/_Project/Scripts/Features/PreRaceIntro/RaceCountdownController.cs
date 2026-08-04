using System.Collections;
using GulfRun.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GulfRun.Features.PreRaceIntro
{
    /// <summary>
    /// Sprint 23.2 — visual race countdown overlay (3 → 2 → 1 → GO!) on PreRaceIntro.
    /// Presentation only: no player movement, controls, obstacles, or networking.
    /// AudioSources are placeholders and are never played.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RaceCountdownController : MonoBehaviour
    {
        private static readonly Color DigitColor = new Color(1f, 0.84f, 0.40f, 1f);
        private static readonly Color GoColor = new Color(1f, 0.92f, 0.55f, 1f);
        private static readonly Color GoGlowColor = new Color(1f, 0.84f, 0.40f, 0.55f);

        [SerializeField] private GameObject countdownOverlay;
        [SerializeField] private Text countdownText;
        [SerializeField] private Image goGlow;
        [SerializeField] private Image transitionFade;
        [SerializeField] private GameObject continueButton;
        [SerializeField] private AudioSource countdownBeepSource;
        [SerializeField] private AudioSource goSoundSource;

        [SerializeField] private float introHoldSeconds = 1.75f;
        [SerializeField] private float digitSeconds = 0.9f;
        [SerializeField] private float goSeconds = 1.2f;
        [SerializeField] private float transitionFadeSeconds = 0.45f;
        [SerializeField] private float startScale = 1.7f;
        [SerializeField] private float settleScale = 1f;

        private Coroutine _sequence;
        private bool _holdSkipped;

        private void OnEnable()
        {
            if (continueButton != null)
            {
                continueButton.SetActive(false);
            }

            ResetVisuals();
            _holdSkipped = false;
            _sequence = StartCoroutine(RunSequence());
        }

        private void OnDisable()
        {
            if (_sequence != null)
            {
                StopCoroutine(_sequence);
                _sequence = null;
            }
        }

        /// <summary>Optional Editor stub — skip the brief intro hold and begin digits.</summary>
        public void SkipHoldAndStart()
        {
            _holdSkipped = true;
        }

        private void ResetVisuals()
        {
            if (countdownOverlay != null)
            {
                countdownOverlay.SetActive(false);
            }

            if (countdownText != null)
            {
                countdownText.text = string.Empty;
                Color c = DigitColor;
                c.a = 0f;
                countdownText.color = c;
                countdownText.rectTransform.localScale = Vector3.one * startScale;
            }

            if (goGlow != null)
            {
                Color glow = GoGlowColor;
                glow.a = 0f;
                goGlow.color = glow;
                goGlow.gameObject.SetActive(true);
            }

            if (transitionFade != null)
            {
                Color fade = transitionFade.color;
                fade.a = 0f;
                transitionFade.color = fade;
                transitionFade.gameObject.SetActive(true);
            }
        }

        private IEnumerator RunSequence()
        {
            float hold = introHoldSeconds;
            while (hold > 0f && !_holdSkipped)
            {
                hold -= Time.unscaledDeltaTime;
                yield return null;
            }

            if (countdownOverlay != null)
            {
                countdownOverlay.SetActive(true);
            }

            yield return AnimateDigit("3", digitSeconds, isGo: false);
            yield return AnimateDigit("2", digitSeconds, isGo: false);
            yield return AnimateDigit("1", digitSeconds, isGo: false);
            yield return AnimateDigit("GO!", goSeconds, isGo: true);

            yield return FadeToGameplay();
            LoadGameplay();
        }

        private IEnumerator AnimateDigit(string label, float duration, bool isGo)
        {
            // Audio placeholders — intentional no-ops until clips are wired later.
            _ = isGo ? goSoundSource : countdownBeepSource;

            if (countdownText == null)
            {
                yield return new WaitForSecondsRealtime(duration);
                yield break;
            }

            countdownText.text = label;
            countdownText.fontSize = isGo ? 148 : 168;
            Color baseColor = isGo ? GoColor : DigitColor;
            countdownText.color = WithAlpha(baseColor, 0f);
            countdownText.rectTransform.localScale = Vector3.one * startScale;

            if (goGlow != null)
            {
                goGlow.color = WithAlpha(GoGlowColor, 0f);
            }

            float half = duration * 0.45f;
            float hold = duration - (half * 2f);
            if (hold < 0.05f)
            {
                hold = 0.05f;
                half = (duration - hold) * 0.5f;
            }

            // Scale down + fade in.
            float elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / half);
                float eased = EaseOutCubic(t);
                countdownText.rectTransform.localScale = Vector3.one * Mathf.Lerp(startScale, settleScale, eased);
                countdownText.color = WithAlpha(baseColor, eased);

                if (isGo && goGlow != null)
                {
                    goGlow.color = WithAlpha(GoGlowColor, eased * 0.9f);
                    float pulse = 1f + (0.08f * Mathf.Sin(Time.unscaledTime * 8f));
                    goGlow.rectTransform.localScale = Vector3.one * pulse;
                }

                yield return null;
            }

            countdownText.rectTransform.localScale = Vector3.one * settleScale;
            countdownText.color = baseColor;

            float holdElapsed = 0f;
            while (holdElapsed < hold)
            {
                holdElapsed += Time.unscaledDeltaTime;
                if (isGo && goGlow != null)
                {
                    float pulse = 1f + (0.1f * Mathf.Sin(Time.unscaledTime * 9f));
                    goGlow.rectTransform.localScale = Vector3.one * pulse;
                    goGlow.color = WithAlpha(GoGlowColor, 0.75f + (0.15f * Mathf.Sin(Time.unscaledTime * 7f)));
                }

                yield return null;
            }

            // Fade out (GO holds a touch longer visually via shorter fade share).
            elapsed = 0f;
            Color startText = countdownText.color;
            Color startGlow = goGlow != null ? goGlow.color : Color.clear;
            while (elapsed < half)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / half);
                float eased = EaseInCubic(t);
                countdownText.color = WithAlpha(startText, 1f - eased);
                if (goGlow != null)
                {
                    goGlow.color = WithAlpha(startGlow, startGlow.a * (1f - eased));
                }

                yield return null;
            }

            countdownText.color = WithAlpha(baseColor, 0f);
            if (goGlow != null)
            {
                goGlow.color = WithAlpha(GoGlowColor, 0f);
            }
        }

        private IEnumerator FadeToGameplay()
        {
            if (transitionFade == null || transitionFadeSeconds <= 0.01f)
            {
                yield break;
            }

            // Visual-only camera/gameplay handoff placeholder — full-screen fade.
            Color baseFade = transitionFade.color;
            float elapsed = 0f;
            while (elapsed < transitionFadeSeconds)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = EaseInCubic(Mathf.Clamp01(elapsed / transitionFadeSeconds));
                transitionFade.color = WithAlpha(baseFade, t);
                yield return null;
            }

            transitionFade.color = WithAlpha(baseFade, 1f);
        }

        private static void LoadGameplay()
        {
            if (SceneManager.Instance != null)
            {
                SceneManager.Instance.LoadGameplay();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.GameplaySceneName);
        }

        private static Color WithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        private static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);

        private static float EaseInCubic(float t) => t * t * t;
    }
}
