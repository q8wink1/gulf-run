using GulfRun.Core.Countries;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.MainMenu.UI;
using UnityEngine;

namespace GulfRun.Features.MainMenu.Player
{
    /// <summary>
    /// Sprint 13 "PLAYER": the local player's character stands centered in
    /// the lobby with an idle breathing animation, small random shifts,
    /// current outfit/character name, and national flag — reads
    /// <see cref="ILocalProfileProvider"/> (Character/Outfit/Country) so
    /// Features.MainMenu never references Features.Character/Online
    /// directly, and <see cref="CountryCatalogConfig"/> directly since
    /// that catalog deliberately lives in <c>GulfRun.Core.Countries</c>
    /// for exactly this "Lobby Flag" use case (see its own remarks).
    /// A silhouette box stands in for the real stylized-3D character model
    /// (see Sprint 13 report Remaining TODOs) — the identical "no final
    /// art yet" placeholder every other screen in this project already
    /// uses for character/outfit previews (e.g. <c>CharacterMenuView</c>).
    /// </summary>
    public sealed class PlayerPreviewView : MonoBehaviour
    {
        [SerializeField] private CountryCatalogConfig countryCatalog;

        private SeededRandom _random;
        private double _nextIdleShiftAtSeconds;
        private float _idleShiftX;

        private void Awake()
        {
            _random = SeededRandom.FromTime();
            ScheduleNextIdleShift();
        }

        private void Update()
        {
            if (Time.timeAsDouble < _nextIdleShiftAtSeconds)
            {
                return;
            }

            // "Random small movements" — an occasional small horizontal step, not a continuous wobble.
            _idleShiftX = (_random.NextFloat01() - 0.5f) * 24f;
            ScheduleNextIdleShift();
        }

        private void ScheduleNextIdleShift()
        {
            _nextIdleShiftAtSeconds = Time.timeAsDouble + 2.5d + _random.NextFloat01() * 2.5d;
        }

        private void OnGUI()
        {
            ILocalProfileProvider profile = LocalProfileProviderService.Current;
            if (profile == null || !profile.HasProfile)
            {
                return;
            }

            PlayerProfileSummary summary = profile.LocalProfile;

            const float characterWidth = 120f;
            const float characterHeight = 220f;
            float centerX = Screen.width * 0.5f;
            float baselineY = Screen.height * 0.66f;

            double elapsed = Time.timeAsDouble;
            // "Idle animation" — a slow vertical breathing scale on the whole silhouette.
            float breathe = CelebrationAnimation.EvaluateOffset(elapsed, 4f, 0.35f);

            float x = centerX - characterWidth * 0.5f + _idleShiftX;
            float y = baselineY - characterHeight + breathe;

            DrawCharacterSilhouette(x, y, characterWidth, characterHeight);
            DrawNameAndOutfitLabel(summary, centerX, baselineY + 8f);
            DrawFlag(summary.Country, centerX, baselineY + 34f);
        }

        private static void DrawCharacterSilhouette(float x, float y, float width, float height)
        {
            Color previous = GUI.color;
            GUI.color = new Color(0.12f, 0.12f, 0.14f, 0.9f);
            GUI.Box(new Rect(x, y, width, height), string.Empty);

            GUI.color = MainMenuTheme.Gold;
            GUI.Box(new Rect(x + width * 0.5f - 2f, y - 4f, 4f, height + 4f), string.Empty);
            GUI.color = previous;
        }

        private static void DrawNameAndOutfitLabel(PlayerProfileSummary summary, float centerX, float y)
        {
            const float width = 320f;
            string outfitLine = string.IsNullOrEmpty(summary.CurrentOutfitDisplayName) ? summary.CurrentCharacterDisplayName : summary.CurrentCharacterDisplayName + " — " + summary.CurrentOutfitDisplayName;

            GUI.Label(new Rect(centerX - width * 0.5f, y, width, 24f), summary.Nickname, MainMenuTheme.Title);
            GUI.Label(new Rect(centerX - width * 0.5f, y + 24f, width, 20f), outfitLine, MainMenuTheme.MutedLabel);
        }

        private void DrawFlag(GulfCountry country, float centerX, float y)
        {
            const float flagWidth = 40f;
            const float flagHeight = 26f;

            Color flagColor = Color.white;
            string label = country.ToString();
            if (countryCatalog != null && countryCatalog.TryGetEntry(country, out CountryCatalogConfig.CountryEntry entry))
            {
                flagColor = entry.PlaceholderColor;
                label = entry.Code;
            }

            Color previous = GUI.color;
            GUI.color = flagColor;
            GUI.Box(new Rect(centerX - flagWidth * 0.5f, y, flagWidth, flagHeight), string.Empty);
            GUI.color = previous;

            GUI.Label(new Rect(centerX - flagWidth * 0.5f, y + flagHeight + 2f, flagWidth, 18f), label, MainMenuTheme.MutedLabel);
        }
    }
}
