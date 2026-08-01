using GulfRun.Core;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Store.Configuration;
using UnityEngine;

namespace GulfRun.Features.Store.BattlePass
{
    /// <summary>
    /// Sprint 13 addition: the Battle Pass screen the Main Menu's Left
    /// Menu "Battle Pass" button opens — reads exclusively through
    /// <see cref="BattlePassManager"/>/<see cref="BattlePassManager.Status"/>,
    /// the same "manager owns the data, view only renders it" split every
    /// other screen in this project follows (e.g. <c>StoreView</c>,
    /// <c>MissionsView</c>). No dedicated Battle Pass screen existed
    /// before this sprint — <c>StoreView</c>'s "Battle Pass" tab only ever
    /// covered the Premium purchase button, not the full tier track.
    /// </summary>
    public sealed class BattlePassView : SceneSingleton<BattlePassView>, IMenuScreenOpener
    {
        private bool _open;
        private string _lastFeedback = string.Empty;
        private Vector2 _scroll;
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _rowStyle;
        private GUIStyle _feedbackStyle;

        private void OnEnable() => MenuScreenRouter.Register(MenuScreen.BattlePass, this);

        private void OnDisable() => MenuScreenRouter.Unregister(MenuScreen.BattlePass, this);

        /// <summary>Sprint 13 (Main Menu Left Menu "Battle Pass" button) — <see cref="IMenuScreenOpener"/>.</summary>
        public void OpenScreen(MenuScreen screen) => _open = true;

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
            const float panelWidth = 560f;
            const float panelHeight = 560f;
            float x = (Screen.width - panelWidth) * 0.5f;
            float y = (Screen.height - panelHeight) * 0.5f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);

            if (GUI.Button(new Rect(x + panelWidth - 34f, y + 8f, 24f, 24f), "X"))
            {
                Close();
                return;
            }

            BattlePassManager manager = BattlePassManager.Instance;
            if (manager == null || manager.Season == null)
            {
                GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 28f, 26f), "BATTLE PASS", _titleStyle);
                GUI.Label(new Rect(x + 14f, y + 40f, panelWidth - 28f, 22f), "No active season.", _labelStyle);
                return;
            }

            BattlePassSeasonConfig season = manager.Season;
            var status = manager.Status;

            GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 60f, 26f), season.SeasonDisplayName.ToUpperInvariant(), _titleStyle);

            float rowY = y + 40f;
            string premiumLine = status.IsPremiumUnlocked ? "Premium: Unlocked" : "Premium: Locked (" + season.PremiumPrice.DisplayString + ")";
            GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 20f), premiumLine, _labelStyle);
            rowY += 22f;

            GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 20f), "Current Tier: " + manager.CurrentTier() + " (" + status.CurrentXp + " XP)", _labelStyle);
            rowY += 24f;

            if (!status.IsPremiumUnlocked)
            {
                if (GUI.Button(new Rect(x + 14f, rowY, 200f, 26f), "Unlock Premium", _rowStyle))
                {
                    var result = manager.PurchasePremium();
                    _lastFeedback = result == PurchaseResult.Success ? "Premium unlocked!" : "Purchase failed: " + result;
                }

                rowY += 30f;
            }

            if (!string.IsNullOrEmpty(_lastFeedback))
            {
                GUI.Label(new Rect(x + 14f, rowY, panelWidth - 28f, 20f), _lastFeedback, _feedbackStyle);
                rowY += 22f;
            }

            DrawTierList(season, status, manager, x + 14f, rowY, panelWidth - 28f, y + panelHeight - rowY - 14f);
        }

        private void DrawTierList(BattlePassSeasonConfig season, BattlePassStatus status, BattlePassManager manager, float x, float y, float width, float height)
        {
            const float rowHeight = 46f;
            Rect viewport = new Rect(x, y, width, height);
            Rect content = new Rect(0f, 0f, width - 20f, season.Tiers.Count * rowHeight);
            _scroll = GUI.BeginScrollView(viewport, _scroll, content);

            for (int i = 0; i < season.Tiers.Count; i++)
            {
                DrawTierRow(season.Tiers[i], status, manager, i * rowHeight, content.width, rowHeight);
            }

            GUI.EndScrollView();
        }

        private void DrawTierRow(BattlePassSeasonConfig.BattlePassTierEntry tier, BattlePassStatus status, BattlePassManager manager, float rowY, float width, float rowHeight)
        {
            bool reached = manager.CurrentTier() >= tier.Tier;
            bool claimed = status.IsTierClaimed(tier.Tier);

            string header = "Tier " + tier.Tier + " (" + tier.XpRequired + " XP): " + tier.RewardDisplayName;
            GUI.Label(new Rect(0f, rowY + 4f, width - 110f, 22f), header, _labelStyle);

            if (claimed)
            {
                GUI.Label(new Rect(width - 100f, rowY + 4f, 96f, 24f), "Claimed", _labelStyle);
            }
            else if (reached && status.IsPremiumUnlocked)
            {
                if (GUI.Button(new Rect(width - 100f, rowY + 2f, 96f, 24f), "Claim", _rowStyle))
                {
                    _lastFeedback = manager.TryClaimTier(tier.Tier) ? "Claimed Tier " + tier.Tier + "!" : "Unable to claim.";
                }
            }
            else if (!status.IsPremiumUnlocked)
            {
                GUI.Label(new Rect(width - 100f, rowY + 4f, 96f, 24f), "Premium", _labelStyle);
            }
            else
            {
                GUI.Label(new Rect(width - 100f, rowY + 4f, 96f, 24f), "Locked", _labelStyle);
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

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleLeft };
            _labelStyle.normal.textColor = Color.white;

            _rowStyle = new GUIStyle(GUI.skin.button) { fontSize = 12 };

            _feedbackStyle = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _feedbackStyle.normal.textColor = Color.green;
        }
    }
}
