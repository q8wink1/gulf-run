using GulfRun.Core;
using GulfRun.Core.Managers;
using GulfRun.Domain;
using GulfRun.Features.Store.BattlePass;
using GulfRun.Features.Store.Inventory;
using UnityEngine;

namespace GulfRun.Features.Store.Wallet
{
    /// <summary>
    /// The Player Wallet screen from the Sprint 10 brief: "Display Coins,
    /// Gems, Owned Cosmetics, Battle Pass Status" — kept as its own small
    /// always-available panel (distinct from the full <see cref="StoreView"/>
    /// storefront) since the brief lists Wallet as its own section, the
    /// same "one screen per brief section" split Sprint 9 used for
    /// Profile/Leaderboard/Friends/Hall of Fame.
    /// </summary>
    public sealed class PlayerWalletView : SceneSingleton<PlayerWalletView>
    {
        private bool _open;
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;

        private void OnGUI()
        {
            EnsureStyles();

            if (GUI.Button(new Rect(1050, 10, 140, 34), _open ? "Close Wallet" : "Wallet"))
            {
                _open = !_open;
            }

            if (!_open)
            {
                return;
            }

            const float panelWidth = 320f;
            const float panelHeight = 180f;
            float x = 1050f;
            float y = 50f;

            GUI.Box(new Rect(x, y, panelWidth, panelHeight), string.Empty);
            GUI.Label(new Rect(x + 14f, y + 8f, panelWidth - 28f, 24f), "WALLET", _titleStyle);

            int coins = EconomyManager.Instance != null ? EconomyManager.Instance.Coins : 0;
            int gems = EconomyManager.Instance != null ? EconomyManager.Instance.Gems : 0;
            int ownedCosmetics = InventoryManager.Instance != null ? InventoryManager.Instance.GetOwnedCosmetics().Count : 0;

            float rowY = y + 38f;
            const float lineHeight = 22f;
            Line(x + 14f, ref rowY, lineHeight, panelWidth - 28f, "Coins: " + coins);
            Line(x + 14f, ref rowY, lineHeight, panelWidth - 28f, "Gems: " + gems);
            Line(x + 14f, ref rowY, lineHeight, panelWidth - 28f, "Owned Cosmetics: " + ownedCosmetics);

            BattlePassManager battlePass = BattlePassManager.Instance;
            if (battlePass != null && battlePass.Season != null)
            {
                BattlePassStatus status = battlePass.Status;
                string passLine = battlePass.Season.SeasonDisplayName + " — Tier " + battlePass.CurrentTier() + "/" + battlePass.Season.TotalTierCount + " — " + (status.IsPremiumUnlocked ? "Premium" : "Not Premium");
                Line(x + 14f, ref rowY, lineHeight, panelWidth - 28f, passLine);
            }
        }

        private void Line(float x, ref float y, float lineHeight, float width, string text)
        {
            GUI.Label(new Rect(x, y, width, lineHeight), text, _labelStyle);
            y += lineHeight;
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            _titleStyle.normal.textColor = Color.white;

            _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 13, alignment = TextAnchor.MiddleLeft };
            _labelStyle.normal.textColor = Color.white;
        }
    }
}
