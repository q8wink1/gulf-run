using GulfRun.Core.Managers;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.MainMenu.UI;
using UnityEngine;

namespace GulfRun.Features.MainMenu.Bottom
{
    /// <summary>
    /// Sprint 14 Matchmaking "PRIVATE ROOM": Create Room / Join by Room Code /
    /// Copy + Share Room Code. Registers as <see cref="MenuScreen.PrivateRoom"/>
    /// so Social/Right-menu entry points can open it without Features.MainMenu
    /// knowing Features.Multiplayer. On successful create/join,
    /// <see cref="PlayButtonView"/>'s existing IsInMatch edge detection loads
    /// the Pre-Race Lobby scene.
    /// </summary>
    public sealed class PrivateRoomPanelView : MonoBehaviour, IMenuScreenOpener
    {
        private bool _isOpen;
        private string _joinCodeInput = string.Empty;
        private string _statusMessage = string.Empty;
        private ButtonPressAnimator _createAnim;
        private ButtonPressAnimator _joinAnim;
        private ButtonPressAnimator _copyAnim;
        private ButtonPressAnimator _closeAnim;

        private void OnEnable() => MenuScreenRouter.Register(MenuScreen.PrivateRoom, this);

        private void OnDisable() => MenuScreenRouter.Unregister(MenuScreen.PrivateRoom, this);

        public void OpenScreen(MenuScreen screen)
        {
            if (screen == MenuScreen.PrivateRoom)
            {
                _isOpen = true;
                _statusMessage = string.Empty;
            }
        }

        private void OnGUI()
        {
            if (!_isOpen)
            {
                return;
            }

            float width = Mathf.Min(420f, Screen.width - 32f);
            float height = 280f;
            float x = (Screen.width - width) * 0.5f;
            float y = (Screen.height - height) * 0.5f;

            MainMenuTheme.DrawPanel(new Rect(x, y, width, height));
            GUI.Label(new Rect(x + 16f, y + 12f, width - 32f, 28f), "Private Room", MainMenuTheme.Title);
            MainMenuTheme.DrawGoldAccentLine(x + 16f, y + 42f, width - 32f);

            IMatchLobbySummaryProvider lobby = MatchLobbySummaryService.Current;
            string displayName = LocalProfileProviderService.Current != null && LocalProfileProviderService.Current.HasProfile
                ? LocalProfileProviderService.Current.LocalProfile.Nickname
                : "Player";

            if (lobby != null && lobby.IsInMatch && lobby.IsPrivateRoom)
            {
                GUI.Label(new Rect(x + 16f, y + 56f, width - 32f, 22f), "Room Code: " + lobby.LocalRoomCode, MainMenuTheme.Label);
                Rect copyRect = _copyAnim.Apply(new Rect(x + 16f, y + 86f, 160f, 32f), 2f);
                if (GUI.Button(copyRect, "Copy Code", MainMenuTheme.PanelButton))
                {
                    _copyAnim.NotifyPressed();
                    GUIUtility.systemCopyBuffer = lobby.LocalRoomCode.Value;
                    _statusMessage = "Room code copied.";
                }

                if (GUI.Button(new Rect(x + 188f, y + 86f, 160f, 32f), "Share Code", MainMenuTheme.PanelButton))
                {
                    GUIUtility.systemCopyBuffer = "Join my GulfRun room: " + lobby.LocalRoomCode.Value;
                    _statusMessage = "Share text copied (no OS share sheet yet).";
                }

                GUI.Label(new Rect(x + 16f, y + 130f, width - 32f, 40f), "Entering Pre-Race Lobby…", MainMenuTheme.MutedLabel);
            }
            else
            {
                Rect createRect = _createAnim.Apply(new Rect(x + 16f, y + 56f, width - 32f, 40f), 3f);
                if (GUI.Button(createRect, "Create Private Room", MainMenuTheme.GoldButton))
                {
                    _createAnim.NotifyPressed();
                    lobby?.CreatePrivateRoom(displayName);
                    _statusMessage = lobby != null ? "Room created." : "Matchmaking unavailable.";
                }

                GUI.Label(new Rect(x + 16f, y + 110f, width - 32f, 20f), "Or join with a Room Code:", MainMenuTheme.MutedLabel);
                _joinCodeInput = GUI.TextField(new Rect(x + 16f, y + 136f, width - 32f, 28f), _joinCodeInput, 6);

                Rect joinRect = _joinAnim.Apply(new Rect(x + 16f, y + 176f, width - 32f, 36f), 3f);
                if (GUI.Button(joinRect, "Join Private Room", MainMenuTheme.PanelButton))
                {
                    _joinAnim.NotifyPressed();
                    if (string.IsNullOrWhiteSpace(_joinCodeInput))
                    {
                        _statusMessage = "Enter a 6-character Room Code.";
                    }
                    else
                    {
                        lobby?.JoinPrivateRoom(_joinCodeInput.Trim().ToUpperInvariant(), displayName);
                        _statusMessage = "Join requested (needs a real remote transport under loopback).";
                    }
                }
            }

            if (!string.IsNullOrEmpty(_statusMessage))
            {
                GUI.Label(new Rect(x + 16f, y + 220f, width - 32f, 20f), _statusMessage, MainMenuTheme.MutedLabel);
            }

            Rect closeRect = _closeAnim.Apply(new Rect(x + width - 96f, y + 12f, 80f, 26f), 2f);
            if (GUI.Button(closeRect, "Close", MainMenuTheme.PanelButton))
            {
                _closeAnim.NotifyPressed();
                _isOpen = false;
            }
        }
    }
}
