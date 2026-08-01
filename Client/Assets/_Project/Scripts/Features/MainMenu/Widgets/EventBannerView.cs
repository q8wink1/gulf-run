using System.Collections.Generic;
using GulfRun.Core.Services;
using GulfRun.Features.MainMenu.UI;
using UnityEngine;

namespace GulfRun.Features.MainMenu.Widgets
{
    /// <summary>
    /// Sprint 13 "EVENT BANNER": a top rotating banner covering Ramadan/
    /// National Days/Battle Pass/Limited Offers/Special Events. Reads
    /// exclusively through <see cref="EventBannerRegistry.CollectActiveMessages"/> —
    /// see <c>ChampionshipManager</c>/<c>LoginRewardManager</c>/
    /// <c>BattlePassManager</c>/<c>StoreManager</c>'s
    /// <see cref="IEventBannerSource"/> implementations for what actually
    /// feeds it — so this view has zero knowledge of which Feature any
    /// given message came from.
    /// </summary>
    public sealed class EventBannerView : MonoBehaviour
    {
        private const float BannerHeight = 30f;
        private const float SecondsPerMessage = 4f;

        private GUIStyle _messageStyle;

        private void OnGUI()
        {
            List<string> messages = EventBannerRegistry.CollectActiveMessages();
            if (messages.Count == 0)
            {
                return;
            }

            EnsureStyles();

            int index = (int)(Time.timeAsDouble / SecondsPerMessage) % messages.Count;
            string message = messages[index];

            const float y = 56f;
            MainMenuTheme.DrawPanel(new Rect(0f, y, Screen.width, BannerHeight));
            GUI.Label(new Rect(0f, y, Screen.width, BannerHeight), message, _messageStyle);
        }

        private void EnsureStyles()
        {
            if (_messageStyle != null)
            {
                return;
            }

            _messageStyle = new GUIStyle(MainMenuTheme.Header) { alignment = TextAnchor.MiddleCenter };
            _messageStyle.normal.textColor = MainMenuTheme.GoldBright;
        }
    }
}
