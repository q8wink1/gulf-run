using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Online.HallOfFame
{
    /// <summary>
    /// The permanent Hall of Fame screen: every category the brief lists
    /// (Best in World/Gulf/every Country, Weekly/Monthly/Season/Tournament
    /// Champion) plus a full "Historic Champions" list — which is simply
    /// every entry ever recorded, since <see cref="HallOfFameManager"/>
    /// (backed by the append-only <c>LocalOnlineBackendService</c> ledger)
    /// never removes one.
    /// </summary>
    public sealed class HallOfFameView : MonoBehaviour
    {
        private bool _open;
        private Vector2 _scroll;
        private GUIStyle _titleStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _labelStyle;

        private void OnGUI()
        {
            EnsureStyles();

            if (GUI.Button(new Rect(520, 10, 160, 34), _open ? "Close Hall of Fame" : "Hall of Fame"))
            {
                _open = !_open;
            }

            if (!_open)
            {
                return;
            }

            DrawPanel();
        }

        private void DrawPanel()
        {
            const float panelWidth = 520f;
            const float panelHeight = 600f;
            float x = 1220f;
            float y = 50f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);
            GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 28f, 26f), "HALL OF FAME", _titleStyle);

            IReadOnlyList<HallOfFameEntry> entries = HallOfFameManager.Instance != null ? HallOfFameManager.Instance.GetEntries() : new List<HallOfFameEntry>();

            Rect viewport = new Rect(x + 14f, y + 44f, panelWidth - 28f, panelHeight - 60f);
            Rect content = new Rect(0f, 0f, panelWidth - 48f, entries.Count * 44f + 8f);
            _scroll = GUI.BeginScrollView(viewport, _scroll, content);

            float rowY = 0f;
            for (int i = 0; i < entries.Count; i++)
            {
                DrawEntry(entries[i], rowY, content.width);
                rowY += 44f;
            }

            GUI.EndScrollView();
        }

        private void DrawEntry(HallOfFameEntry entry, float y, float width)
        {
            string category = entry.Category == HallOfFameCategory.BestInCountry && entry.Country.HasValue
                ? "Best in " + entry.Country.Value
                : CategoryLabel(entry.Category);

            GUI.Label(new Rect(0f, y, width, 20f), category + " — " + entry.AchievedLabel, _headerStyle);
            GUI.Label(new Rect(0f, y + 20f, width, 20f), entry.Nickname + "  (" + entry.Score + " trophies)", _labelStyle);
        }

        private static string CategoryLabel(HallOfFameCategory category)
        {
            switch (category)
            {
                case HallOfFameCategory.BestInWorld:
                    return "Best Player in the World";
                case HallOfFameCategory.BestInGulf:
                    return "Best Gulf Player";
                case HallOfFameCategory.WeeklyChampion:
                    return "Weekly Champion";
                case HallOfFameCategory.MonthlyChampion:
                    return "Monthly Champion";
                case HallOfFameCategory.SeasonChampion:
                    return "Season Champion";
                case HallOfFameCategory.TournamentChampion:
                    return "Tournament Champion";
                default:
                    return category.ToString();
            }
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _titleStyle.normal.textColor = Color.white;

            _headerStyle = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _headerStyle.normal.textColor = Color.yellow;

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleLeft };
            _labelStyle.normal.textColor = Color.white;
        }
    }
}
