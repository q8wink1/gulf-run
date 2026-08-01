using GulfRun.Core;
using GulfRun.Core.Services;
using GulfRun.Features.Online.Configuration;
using UnityEngine;

namespace GulfRun.Features.Online.Championships
{
    /// <summary>
    /// Sprint 13 addition: the Championships/Events screen the Main Menu's
    /// Right Menu "Championships" and "Events" buttons open — reads
    /// exclusively through <see cref="ChampionshipManager"/>, the same
    /// "manager owns the data, view only renders it" split every other
    /// screen in this project follows. Two tabs: "Championships" (the
    /// active recurring Championship — Weekly/Monthly/Season/Weekend/
    /// Special Event) and "Events" (the active Country Event — National
    /// Day/Ramadan/Eid/Summer/Winter), so one screen honestly covers both
    /// Right Menu entries with the exact same live data the Event Banner
    /// already surfaces (see <see cref="ChampionshipManager.GetActiveBannerMessages"/>).
    /// </summary>
    public sealed class ChampionshipsView : SceneSingleton<ChampionshipsView>, IMenuScreenOpener
    {
        private enum Tab
        {
            Championships,
            Events
        }

        private bool _open;
        private Tab _tab = Tab.Championships;
        private GUIStyle _titleStyle;
        private GUIStyle _tabStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _labelStyle;

        private void OnEnable()
        {
            MenuScreenRouter.Register(MenuScreen.Championships, this);
            MenuScreenRouter.Register(MenuScreen.Events, this);
        }

        private void OnDisable()
        {
            MenuScreenRouter.Unregister(MenuScreen.Championships, this);
            MenuScreenRouter.Unregister(MenuScreen.Events, this);
        }

        /// <summary>Sprint 13 (Main Menu Right Menu "Championships"/"Events" buttons) — <see cref="IMenuScreenOpener"/>. Opens directly on whichever tab was requested.</summary>
        public void OpenScreen(MenuScreen screen)
        {
            _open = true;
            _tab = screen == MenuScreen.Events ? Tab.Events : Tab.Championships;
        }

        public void Close() => _open = false;

        private void OnGUI()
        {
            EnsureStyles();

            if (!_open)
            {
                return;
            }

            DrawPanel();
        }

        private void DrawPanel()
        {
            const float panelWidth = 480f;
            const float panelHeight = 340f;
            float x = (Screen.width - panelWidth) * 0.5f;
            float y = (Screen.height - panelHeight) * 0.5f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);
            GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 28f, 26f), "CHAMPIONSHIPS & EVENTS", _titleStyle);

            if (GUI.Button(new Rect(x + panelWidth - 34f, y + 8f, 24f, 24f), "X"))
            {
                Close();
                return;
            }

            float rowY = y + 40f;
            DrawTabs(x + 14f, rowY, panelWidth - 28f);
            rowY += 34f;

            if (ChampionshipManager.Instance == null)
            {
                GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 24f), "Championships are not available yet.", _labelStyle);
                return;
            }

            if (_tab == Tab.Championships)
            {
                DrawChampionshipTab(x + 14f, rowY, panelWidth - 28f);
            }
            else
            {
                DrawEventTab(x + 14f, rowY, panelWidth - 28f);
            }
        }

        private void DrawTabs(float x, float y, float width)
        {
            Tab[] tabs = { Tab.Championships, Tab.Events };
            float tabWidth = width / tabs.Length;
            for (int i = 0; i < tabs.Length; i++)
            {
                GUI.color = _tab == tabs[i] ? Color.yellow : Color.white;
                if (GUI.Button(new Rect(x + i * tabWidth, y, tabWidth - 2f, 26f), tabs[i].ToString(), _tabStyle))
                {
                    _tab = tabs[i];
                }

                GUI.color = Color.white;
            }
        }

        private void DrawChampionshipTab(float x, float y, float width)
        {
            ChampionshipManager manager = ChampionshipManager.Instance;
            if (!manager.HasActiveChampionship)
            {
                GUI.Label(new Rect(x, y, width, 24f), "No Championship is currently live.", _labelStyle);
                return;
            }

            ChampionshipCatalogConfig.ChampionshipEntry entry = manager.ActiveChampionship;

            Color previous = GUI.color;
            GUI.color = entry.PlaceholderColor;
            GUI.Box(new Rect(x, y, width, 60f), string.Empty);
            GUI.color = previous;

            GUI.Label(new Rect(x + 10f, y + 6f, width - 20f, 24f), entry.DisplayName + " (" + entry.Type + ")", _headerStyle);
            GUI.Label(new Rect(x + 10f, y + 30f, width - 20f, 22f), entry.Description, _labelStyle);

            y += 72f;
            GUI.Label(new Rect(x, y, width, 22f), "Headline Reward: " + entry.RewardDisplayName, _labelStyle);
        }

        private void DrawEventTab(float x, float y, float width)
        {
            ChampionshipManager manager = ChampionshipManager.Instance;
            if (!manager.HasActiveCountryEvent)
            {
                GUI.Label(new Rect(x, y, width, 24f), "No Special Event is currently live.", _labelStyle);
                return;
            }

            CountryEventCatalogConfig.CountryEventEntry entry = manager.ActiveCountryEvent;

            Color previous = GUI.color;
            GUI.color = entry.PlaceholderColor;
            GUI.Box(new Rect(x, y, width, 60f), string.Empty);
            GUI.color = previous;

            string title = entry.Country.HasValue ? entry.DisplayName + " (" + entry.Country.Value + ")" : entry.DisplayName;
            GUI.Label(new Rect(x + 10f, y + 6f, width - 20f, 24f), title, _headerStyle);
            GUI.Label(new Rect(x + 10f, y + 30f, width - 20f, 22f), entry.Description, _labelStyle);

            y += 72f;
            GUI.Label(new Rect(x, y, width, 22f), "Category: " + entry.Category, _labelStyle);
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _titleStyle.normal.textColor = Color.white;

            _tabStyle = new GUIStyle(GUI.skin.button) { fontSize = 12 };

            _headerStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _headerStyle.normal.textColor = Color.white;

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleLeft, wordWrap = true };
            _labelStyle.normal.textColor = Color.white;
        }
    }
}
