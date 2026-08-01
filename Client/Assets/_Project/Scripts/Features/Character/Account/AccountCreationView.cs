using GulfRun.Core.Countries;
using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Character.Account
{
    /// <summary>
    /// One-time Account Creation screen: Display Name + Country, exactly as
    /// the Sprint 8 brief specifies ("During first account creation: Player
    /// enters: Display Name, Country. Country selection happens ONLY ONCE.").
    /// Visible only while <see cref="SaveManager.HasAccount"/> is false;
    /// disappears forever for the rest of the session the instant
    /// <see cref="SaveManager.CreateAccount"/> succeeds, since that call is
    /// itself the permanent lock ("Country cannot be changed later" —
    /// enforced in <see cref="SaveManager"/>, not here, so no amount of UI
    /// back-and-forth before confirming can bypass it).
    ///
    /// Functional OnGUI placeholder — same posture as every other Sprint
    /// 4-7 UI (<c>CountdownView</c>, <c>MultiplayerDebugView</c>,
    /// <c>PodiumCeremonyView</c>): a real UI Toolkit screen is the eventual
    /// target (docs/02-architecture/TECHNICAL_STACK.md) once the Editor is
    /// available to author one.
    /// </summary>
    public sealed class AccountCreationView : MonoBehaviour
    {
        [SerializeField] private CountryCatalogConfig countryCatalog;

        private string _displayNameInput = string.Empty;
        private GulfCountry _selectedCountry = GulfCountry.SaudiArabia;
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;

        private void OnGUI()
        {
            if (!PersistentUiScope.AllowsMainMenuChrome)
            {
                return;
            }

            SaveManager save = SaveManager.Instance;
            if (save == null || save.HasAccount)
            {
                return;
            }

            EnsureStyles();

            float panelWidth = 460f;
            float panelX = Screen.width * 0.5f - panelWidth * 0.5f;
            float y = Screen.height * 0.5f - 220f;

            GUI.Box(new Rect(panelX - 20f, y - 20f, panelWidth + 40f, 440f), string.Empty);
            GUI.Label(new Rect(panelX, y, panelWidth, 36f), "CREATE YOUR ACCOUNT", _titleStyle);
            y += 46f;

            GUI.Label(new Rect(panelX, y, panelWidth, 22f), "Display Name:", _labelStyle);
            y += 24f;
            _displayNameInput = GUI.TextField(new Rect(panelX, y, panelWidth, 30f), _displayNameInput, 24);
            y += 40f;

            GUI.Label(new Rect(panelX, y, panelWidth, 22f), "Country (permanent — cannot be changed later):", _labelStyle);
            y += 26f;

            y = DrawCountryGrid(panelX, y, panelWidth);
            y += 12f;

            bool canCreate = !string.IsNullOrWhiteSpace(_displayNameInput);
            if (canCreate && GUI.Button(new Rect(panelX, y, panelWidth, 40f), "Create Account"))
            {
                save.CreateAccount(_displayNameInput.Trim(), _selectedCountry);
            }
            else if (!canCreate)
            {
                GUI.Label(new Rect(panelX, y, panelWidth, 30f), "Enter a Display Name to continue.", _labelStyle);
            }
        }

        private float DrawCountryGrid(float panelX, float y, float panelWidth)
        {
            GulfCountry[] countries = (GulfCountry[])System.Enum.GetValues(typeof(GulfCountry));
            const int columns = 2;
            const float buttonHeight = 30f;
            const float spacing = 6f;
            float buttonWidth = (panelWidth - spacing) / columns;

            for (int i = 0; i < countries.Length; i++)
            {
                int column = i % columns;
                int row = i / columns;
                float x = panelX + column * (buttonWidth + spacing);
                float rowY = y + row * (buttonHeight + spacing);

                string label = ResolveDisplayName(countries[i]);
                bool isSelected = countries[i] == _selectedCountry;

                GUI.Label(new Rect(x, rowY, buttonWidth, buttonHeight), isSelected ? $"[{label}]" : label, _labelStyle);
                if (GUI.Button(new Rect(x, rowY, buttonWidth, buttonHeight), string.Empty))
                {
                    _selectedCountry = countries[i];
                }
            }

            int rowCount = (countries.Length + columns - 1) / columns;
            return y + rowCount * (buttonHeight + spacing);
        }

        private string ResolveDisplayName(GulfCountry country) =>
            countryCatalog != null && countryCatalog.TryGetEntry(country, out CountryCatalogConfig.CountryEntry entry)
                ? entry.DisplayName
                : country.ToString();

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _titleStyle.normal.textColor = Color.white;

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft
            };
            _labelStyle.normal.textColor = Color.white;
        }
    }
}
