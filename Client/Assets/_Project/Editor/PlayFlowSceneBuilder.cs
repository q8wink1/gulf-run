using System.Collections.Generic;
using System.IO;
using GulfRun.Features.InviteFriends;
using GulfRun.Features.LobbyScreen;
using GulfRun.Features.MainMenu;
using GulfRun.Features.MapVotingScreen;
using GulfRun.Features.PlayMenu;
using GulfRun.Features.QuickPlay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CoreSceneManager = GulfRun.Core.Managers.SceneManager;

namespace GulfRun.Editor
{
    /// <summary>
    /// Builds PlayMenu / QuickPlay / InviteFriends / LobbyScreen scenes, wires Main Menu Play Now,
    /// updates EditorBuildSettings, and validates hierarchy in batchmode.
    /// </summary>
    public static class PlayFlowSceneBuilder
    {
        private const string PlayMenuScenePath = "Assets/_Project/Scenes/PlayMenu.unity";
        private const string QuickPlayScenePath = "Assets/_Project/Scenes/QuickPlay.unity";
        private const string InviteFriendsScenePath = "Assets/_Project/Scenes/InviteFriends.unity";
        private const string LobbyScreenScenePath = "Assets/_Project/Scenes/LobbyScreen.unity";
        private const string MapVotingScenePath = "Assets/_Project/Scenes/MapVoting.unity";
        private const string MainMenuScenePath = "Assets/_Project/Scenes/MainMenu.unity";
        private const string BackgroundGuid = "a18b0000000000000000000000000001";

        private static readonly Color Gold = new Color(0.90f, 0.71f, 0.25f, 1f);
        private static readonly Color GoldBright = new Color(1f, 0.84f, 0.40f, 1f);
        private static readonly Color PanelBg = new Color(0.10f, 0.09f, 0.10f, 0.78f);
        private static readonly Color PanelBorder = new Color(0.90f, 0.71f, 0.25f, 0.55f);
        private static readonly Color TextPrimary = Color.white;
        private static readonly Color TextMuted = new Color(0.80f, 0.80f, 0.80f, 1f);
        private static readonly Color ButtonDark = new Color(0.12f, 0.10f, 0.09f, 0.92f);
        private static readonly Color CardFill = new Color(0.12f, 0.11f, 0.12f, 0.88f);
        private static readonly Color HighlightGold = new Color(0.90f, 0.71f, 0.25f, 0.12f);
        private static readonly Color SuccessGreen = new Color(0.40f, 0.85f, 0.45f, 1f);
        private static readonly Color ReadyMuted = new Color(0.55f, 0.55f, 0.55f, 1f);
        private static readonly Color ConnectingAmber = new Color(0.95f, 0.72f, 0.28f, 1f);
        private static readonly Color OnlineGreen = new Color(0.35f, 0.90f, 0.48f, 1f);
        private static readonly Color EmptySlotFill = new Color(0.10f, 0.09f, 0.10f, 0.55f);
        private static readonly Color EmptySlotBorder = new Color(0.90f, 0.71f, 0.25f, 0.22f);
        private static readonly Color AvatarTone = new Color(0.72f, 0.58f, 0.42f, 1f);
        private static readonly Color GoldButtonLabel = new Color(0.20f, 0.14f, 0.02f, 1f);

        private enum SlotReadyVisual
        {
            Ready,
            NotReady,
            Connecting
        }

        private const string QuickPlaySubtitle = "Find and join a public multiplayer match instantly.";
        private const string InviteFriendsSubtitle = "Create a private room and play with your friends.";

        [MenuItem("GulfRun/Play Flow/Build Scenes + Wire Play")]
        public static void RunFromMenu() => RunBatch();

        [MenuItem("GulfRun/Play Flow/Polish Play Menu (Sprint 20.1)")]
        public static void PolishPlayMenuFromMenu() => PolishPlayMenuBatch();

        [MenuItem("GulfRun/Play Flow/Build Lobby Screen (Sprint 21.5)")]
        public static void BuildLobbyScreenFromMenu() => BuildLobbyScreenBatch();

        [MenuItem("GulfRun/Play Flow/Build Map Voting Screen (Sprint 22.3)")]
        public static void BuildMapVotingScreenFromMenu() => BuildMapVotingScreenBatch();

        public static void RunBatch()
        {
            var failures = new List<string>();

            try
            {
                WireMainMenuPlayButton(failures);
                BuildPlayMenuScene(failures);
                BuildQuickPlayScene(failures);
                BuildInviteFriendsScene(failures);
                BuildLobbyScreenScene(failures);
                EnsureBuildSettings(failures);
                ValidateAll(failures);
            }
            catch (System.Exception ex)
            {
                failures.Add("Unhandled: " + ex);
                Debug.LogException(ex);
            }

            ExitWithFailures(failures, "[PlayFlow] PASS — PlayMenu/QuickPlay/InviteFriends/LobbyScreen built, Play wired, build settings OK.");
        }

        /// <summary>
        /// Sprint 21.5: rebuild LobbyScreen with final visual polish (hierarchy,
        /// Ready/Play touch targets, SafeArea insets, slot rhythm, typography).
        /// Keeps 21.2–21.4 structure; no networking / ready logic / matchmaking.
        /// Does not rebuild PlayMenu / QuickPlay / InviteFriends.
        /// </summary>
        public static void BuildLobbyScreenBatch()
        {
            var failures = new List<string>();

            try
            {
                BuildLobbyScreenScene(failures);
                EnsureBuildSettings(failures);
                ValidateLobbyScreen(failures);

                if (CoreSceneManager.LobbyScreenSceneName != "LobbyScreen")
                {
                    failures.Add("SceneManager.LobbyScreenSceneName mismatch.");
                }

                if (CoreSceneManager.LobbySceneName != "Lobby")
                {
                    failures.Add("SceneManager.LobbySceneName must remain Lobby (pre-race).");
                }
            }
            catch (System.Exception ex)
            {
                failures.Add("Unhandled: " + ex);
                Debug.LogException(ex);
            }

            ExitWithFailures(failures, "[PlayFlow] PASS — LobbyScreen Sprint 21.5 Final Polish OK.");
        }

        /// <summary>
        /// Sprint 22.3: rebuild MapVoting Voting HUD (TimerPanel, StatusPanel,
        /// Stats footer, VoteConfirmation) on top of Sprint 22.2 premium cards.
        /// No vote counting, countdown logic, networking, or SessionManager.
        /// Does not rebuild LobbyScreen / PlayMenu / Main Menu.
        /// </summary>
        public static void BuildMapVotingScreenBatch()
        {
            var failures = new List<string>();

            try
            {
                BuildMapVotingScreenScene(failures);
                EnsureBuildSettings(failures);
                ValidateMapVotingScreen(failures);

                if (CoreSceneManager.MapVotingSceneName != "MapVoting")
                {
                    failures.Add("SceneManager.MapVotingSceneName mismatch.");
                }
            }
            catch (System.Exception ex)
            {
                failures.Add("Unhandled: " + ex);
                Debug.LogException(ex);
            }

            ExitWithFailures(failures, "[PlayFlow] PASS — MapVoting Sprint 22.3 Voting HUD UI OK.");
        }

        /// <summary>
        /// Sprint 20.1: polish PlayMenu copy + card icons only. Does not rebuild
        /// QuickPlay / InviteFriends (preserves later lobby wiring).
        /// </summary>
        public static void PolishPlayMenuBatch()
        {
            var failures = new List<string>();

            try
            {
                PolishPlayMenuScene(failures);
                EnsureBuildSettings(failures);
                ValidatePlayMenu(failures);
                ValidateMainMenuWiring(failures);
            }
            catch (System.Exception ex)
            {
                failures.Add("Unhandled: " + ex);
                Debug.LogException(ex);
            }

            ExitWithFailures(failures, "[PlayFlow] PASS — PlayMenu Sprint 20.1 polish (copy + icons) OK.");
        }

        private static void ExitWithFailures(List<string> failures, string passMessage)
        {
            if (failures.Count == 0)
            {
                Debug.Log(passMessage);
                EditorApplication.Exit(0);
            }
            else
            {
                foreach (string failure in failures)
                {
                    Debug.LogError("[PlayFlow] FAIL: " + failure);
                }

                EditorApplication.Exit(1);
            }
        }

        private static void WireMainMenuPlayButton(List<string> failures)
        {
            Scene scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            GameObject playImage = FindDeep(scene, "PlayButtonImage");
            if (playImage == null)
            {
                failures.Add("MainMenu PlayButtonImage not found.");
                return;
            }

            EnsureEventSystem(scene);

            Image image = playImage.GetComponent<Image>();
            if (image != null)
            {
                image.raycastTarget = true;
            }

            Button button = playImage.GetComponent<Button>();
            if (button == null)
            {
                button = playImage.AddComponent<Button>();
            }

            button.transition = Selectable.Transition.None;
            button.targetGraphic = image;

            if (playImage.GetComponent<MainMenuPlayButton>() == null)
            {
                playImage.AddComponent<MainMenuPlayButton>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[PlayFlow] Wired MainMenu PlayButtonImage → PlayMenu.");
        }

        private static void BuildPlayMenuScene(List<string> failures)
        {
            Sprite background = LoadBackground(failures);
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EnsureEventSystem(scene);

            GameObject canvasGo = CreateOverlayCanvas("PlayMenuCanvas");
            PlayMenuController controller = canvasGo.AddComponent<PlayMenuController>();
            RectTransform canvasRt = canvasGo.GetComponent<RectTransform>();

            Image bg = CreateUiImage("Background", canvasRt, stretch: true);
            bg.sprite = background;
            bg.preserveAspect = false;
            bg.raycastTarget = false;

            CreateSafeArea(canvasRt);

            Button backButton = CreateLabeledButton("BackButton", canvasRt, "Back", 168f, 64f, ButtonDark, GoldBright);
            PlaceTopLeft(backButton.GetComponent<RectTransform>(), new Vector2(48f, -40f));

            RectTransform cardsRoot = CreateRect("CardsRoot", canvasRt);
            cardsRoot.anchorMin = new Vector2(0.5f, 0.5f);
            cardsRoot.anchorMax = new Vector2(0.5f, 0.5f);
            cardsRoot.pivot = new Vector2(0.5f, 0.5f);
            cardsRoot.anchoredPosition = Vector2.zero;
            cardsRoot.sizeDelta = new Vector2(1400f, 520f);

            Button quickPlay = CreateModeCard(
                "QuickPlayCard",
                cardsRoot,
                "Quick Play",
                QuickPlaySubtitle,
                new Vector2(-360f, 0f),
                ModeCardIconStyle.Lightning);

            Button invite = CreateModeCard(
                "InviteFriendsCard",
                cardsRoot,
                "Invite Friends",
                InviteFriendsSubtitle,
                new Vector2(360f, 0f),
                ModeCardIconStyle.Friends);

            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("backButton").objectReferenceValue = backButton;
            so.FindProperty("quickPlayButton").objectReferenceValue = quickPlay;
            so.FindProperty("inviteFriendsButton").objectReferenceValue = invite;
            so.ApplyModifiedPropertiesWithoutUndo();

            SaveScene(scene, PlayMenuScenePath, failures);
        }

        private static void BuildQuickPlayScene(List<string> failures)
        {
            Sprite background = LoadBackground(failures);
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EnsureEventSystem(scene);

            GameObject canvasGo = CreateOverlayCanvas("QuickPlayCanvas");
            QuickPlayController controller = canvasGo.AddComponent<QuickPlayController>();
            RectTransform canvasRt = canvasGo.GetComponent<RectTransform>();

            Image bg = CreateUiImage("Background", canvasRt, stretch: true);
            bg.sprite = background;
            bg.preserveAspect = false;
            bg.raycastTarget = false;

            CreateSafeArea(canvasRt);

            Button backButton = CreateLabeledButton("BackButton", canvasRt, "Back", 168f, 64f, ButtonDark, GoldBright);
            PlaceTopLeft(backButton.GetComponent<RectTransform>(), new Vector2(48f, -40f));

            RectTransform statusRoot = CreateRect("StatusRoot", canvasRt);
            statusRoot.anchorMin = new Vector2(0.5f, 0.5f);
            statusRoot.anchorMax = new Vector2(0.5f, 0.5f);
            statusRoot.pivot = new Vector2(0.5f, 0.5f);
            statusRoot.anchoredPosition = new Vector2(0f, 40f);
            statusRoot.sizeDelta = new Vector2(900f, 420f);

            Image statusPanel = statusRoot.gameObject.AddComponent<Image>();
            statusPanel.color = PanelBg;
            statusPanel.raycastTarget = false;

            Image spinnerImage = CreateUiImage("SearchingSpinner", statusRoot, stretch: false);
            spinnerImage.sprite = GetBuiltinKnob();
            spinnerImage.color = GoldBright;
            spinnerImage.raycastTarget = false;
            RectTransform spinnerRt = spinnerImage.rectTransform;
            spinnerRt.anchorMin = new Vector2(0.5f, 0.5f);
            spinnerRt.anchorMax = new Vector2(0.5f, 0.5f);
            spinnerRt.pivot = new Vector2(0.5f, 0.5f);
            spinnerRt.anchoredPosition = new Vector2(0f, 70f);
            spinnerRt.sizeDelta = new Vector2(120f, 120f);

            Text statusText = CreateUiText(
                "StatusText",
                statusRoot,
                "Searching for available players...",
                34,
                FontStyle.Bold,
                TextPrimary,
                TextAnchor.MiddleCenter);
            RectTransform statusRt = statusText.rectTransform;
            statusRt.anchorMin = new Vector2(0f, 0.15f);
            statusRt.anchorMax = new Vector2(1f, 0.45f);
            statusRt.offsetMin = new Vector2(24f, 0f);
            statusRt.offsetMax = new Vector2(-24f, 0f);

            RectTransform labelsRoot = CreateRect("StatusLabels", statusRoot);
            labelsRoot.anchorMin = new Vector2(0.5f, 0f);
            labelsRoot.anchorMax = new Vector2(0.5f, 0f);
            labelsRoot.pivot = new Vector2(0.5f, 0f);
            labelsRoot.anchoredPosition = new Vector2(0f, 24f);
            labelsRoot.sizeDelta = new Vector2(820f, 48f);

            GameObject playersFound = CreateHiddenLabel("PlayersFoundLabel", labelsRoot, "Players Found");
            GameObject joining = CreateHiddenLabel("JoiningRoomLabel", labelsRoot, "Joining Room");
            GameObject creating = CreateHiddenLabel("CreatingRoomLabel", labelsRoot, "Creating Room");
            GameObject waiting = CreateHiddenLabel("WaitingForPlayersLabel", labelsRoot, "Waiting For Players");

            Button cancel = CreateLabeledButton("CancelButton", canvasRt, "Cancel", 280f, 80f, ButtonDark, GoldBright);
            RectTransform cancelRt = cancel.GetComponent<RectTransform>();
            cancelRt.anchorMin = new Vector2(0.5f, 0f);
            cancelRt.anchorMax = new Vector2(0.5f, 0f);
            cancelRt.pivot = new Vector2(0.5f, 0f);
            cancelRt.anchoredPosition = new Vector2(0f, 64f);

            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("backButton").objectReferenceValue = backButton;
            so.FindProperty("cancelButton").objectReferenceValue = cancel;
            so.FindProperty("statusText").objectReferenceValue = statusText;
            so.FindProperty("spinner").objectReferenceValue = spinnerRt;
            so.FindProperty("playersFoundLabel").objectReferenceValue = playersFound;
            so.FindProperty("joiningRoomLabel").objectReferenceValue = joining;
            so.FindProperty("creatingRoomLabel").objectReferenceValue = creating;
            so.FindProperty("waitingForPlayersLabel").objectReferenceValue = waiting;
            so.ApplyModifiedPropertiesWithoutUndo();

            SaveScene(scene, QuickPlayScenePath, failures);
        }

        private static void BuildInviteFriendsScene(List<string> failures)
        {
            Sprite background = LoadBackground(failures);
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EnsureEventSystem(scene);

            GameObject canvasGo = CreateOverlayCanvas("InviteFriendsCanvas");
            InviteFriendsController controller = canvasGo.AddComponent<InviteFriendsController>();
            RectTransform canvasRt = canvasGo.GetComponent<RectTransform>();

            Image bg = CreateUiImage("Background", canvasRt, stretch: true);
            bg.sprite = background;
            bg.preserveAspect = false;
            bg.raycastTarget = false;

            CreateSafeArea(canvasRt);

            Button backButton = CreateLabeledButton("BackButton", canvasRt, "Back", 168f, 64f, ButtonDark, GoldBright);
            PlaceTopLeft(backButton.GetComponent<RectTransform>(), new Vector2(48f, -40f));

            RectTransform cardsRoot = CreateRect("CardsRoot", canvasRt);
            cardsRoot.anchorMin = new Vector2(0.5f, 0.5f);
            cardsRoot.anchorMax = new Vector2(0.5f, 0.5f);
            cardsRoot.pivot = new Vector2(0.5f, 0.5f);
            cardsRoot.anchoredPosition = new Vector2(0f, 20f);
            cardsRoot.sizeDelta = new Vector2(1680f, 720f);

            // Card 1 — Friends List
            RectTransform friendsCard = CreateActionCard("FriendsListCard", cardsRoot, new Vector2(-560f, 0f), new Vector2(500f, 640f));
            CreateUiText("Title", friendsCard, "Friends List", 30, FontStyle.Bold, GoldBright, TextAnchor.UpperCenter)
                .rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 18f, 40f);
            StretchHorizontal(CreateUiText(
                "Description",
                friendsCard,
                "Display friends list; select one or more friends to invite.",
                18,
                FontStyle.Normal,
                TextMuted,
                TextAnchor.UpperCenter).rectTransform, 64f, 70f);

            RectTransform rowsRoot = CreateRect("FriendRows", friendsCard);
            rowsRoot.anchorMin = new Vector2(0.5f, 0f);
            rowsRoot.anchorMax = new Vector2(0.5f, 1f);
            rowsRoot.pivot = new Vector2(0.5f, 0.5f);
            rowsRoot.anchoredPosition = new Vector2(0f, -20f);
            rowsRoot.sizeDelta = new Vector2(440f, -160f);

            string[] fakeFriends = { "Ahmed_97", "NoorDesert", "FalconQ8", "SandRider", "GulfAce" };
            var friendButtons = new List<Button>();
            var friendHighlights = new List<Image>();
            for (int i = 0; i < fakeFriends.Length; i++)
            {
                Button row = CreateFriendRow("FriendRow_" + i, rowsRoot, fakeFriends[i], i, fakeFriends.Length);
                friendButtons.Add(row);
                friendHighlights.Add(row.GetComponent<Image>());
            }

            // Card 2 — Player ID / Invite Code
            RectTransform playerIdCard = CreateActionCard("PlayerIdCard", cardsRoot, new Vector2(0f, 0f), new Vector2(500f, 640f));
            CreateUiText("Title", playerIdCard, "Player ID / Invite Code", 28, FontStyle.Bold, GoldBright, TextAnchor.UpperCenter)
                .rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 18f, 48f);
            StretchHorizontal(CreateUiText(
                "Description",
                playerIdCard,
                "Enter a Player ID or Invitation Code to send an invite.",
                18,
                FontStyle.Normal,
                TextMuted,
                TextAnchor.UpperCenter).rectTransform, 74f, 70f);

            InputField input = CreateInputField("PlayerIdInput", playerIdCard, "Player ID or Invite Code");
            RectTransform inputRt = input.GetComponent<RectTransform>();
            inputRt.anchorMin = new Vector2(0.5f, 0.5f);
            inputRt.anchorMax = new Vector2(0.5f, 0.5f);
            inputRt.pivot = new Vector2(0.5f, 0.5f);
            inputRt.anchoredPosition = new Vector2(0f, 40f);
            inputRt.sizeDelta = new Vector2(400f, 64f);

            Button send = CreateLabeledButton("SendInvitationButton", playerIdCard, "Send Invitation", 360f, 72f, Gold, new Color(0.20f, 0.14f, 0.02f, 1f));
            RectTransform sendRt = send.GetComponent<RectTransform>();
            sendRt.anchorMin = new Vector2(0.5f, 0.5f);
            sendRt.anchorMax = new Vector2(0.5f, 0.5f);
            sendRt.pivot = new Vector2(0.5f, 0.5f);
            sendRt.anchoredPosition = new Vector2(0f, -60f);

            // Card 3 — WhatsApp Invite
            RectTransform whatsAppCard = CreateActionCard("WhatsAppCard", cardsRoot, new Vector2(560f, 0f), new Vector2(500f, 640f));
            CreateUiText("Title", whatsAppCard, "WhatsApp Invite", 30, FontStyle.Bold, GoldBright, TextAnchor.UpperCenter)
                .rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 18f, 40f);
            StretchHorizontal(CreateUiText(
                "Description",
                whatsAppCard,
                "Copy or share a WhatsApp invitation link.",
                18,
                FontStyle.Normal,
                TextMuted,
                TextAnchor.UpperCenter).rectTransform, 64f, 70f);

            Button copy = CreateLabeledButton("CopyLinkButton", whatsAppCard, "Copy Link", 360f, 72f, Gold, new Color(0.20f, 0.14f, 0.02f, 1f));
            RectTransform copyRt = copy.GetComponent<RectTransform>();
            copyRt.anchorMin = new Vector2(0.5f, 0.5f);
            copyRt.anchorMax = new Vector2(0.5f, 0.5f);
            copyRt.pivot = new Vector2(0.5f, 0.5f);
            copyRt.anchoredPosition = new Vector2(0f, 40f);

            Button share = CreateLabeledButton("ShareWhatsAppButton", whatsAppCard, "Share via WhatsApp", 360f, 72f, ButtonDark, GoldBright);
            RectTransform shareRt = share.GetComponent<RectTransform>();
            shareRt.anchorMin = new Vector2(0.5f, 0.5f);
            shareRt.anchorMax = new Vector2(0.5f, 0.5f);
            shareRt.pivot = new Vector2(0.5f, 0.5f);
            shareRt.anchoredPosition = new Vector2(0f, -60f);

            Text status = CreateUiText("StatusText", canvasRt, string.Empty, 24, FontStyle.Bold, GoldBright, TextAnchor.MiddleCenter);
            RectTransform statusRt = status.rectTransform;
            statusRt.anchorMin = new Vector2(0.5f, 0f);
            statusRt.anchorMax = new Vector2(0.5f, 0f);
            statusRt.pivot = new Vector2(0.5f, 0f);
            statusRt.anchoredPosition = new Vector2(0f, 36f);
            statusRt.sizeDelta = new Vector2(900f, 40f);

            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("backButton").objectReferenceValue = backButton;
            so.FindProperty("playerIdInput").objectReferenceValue = input;
            so.FindProperty("sendInvitationButton").objectReferenceValue = send;
            so.FindProperty("copyLinkButton").objectReferenceValue = copy;
            so.FindProperty("shareWhatsAppButton").objectReferenceValue = share;
            so.FindProperty("statusText").objectReferenceValue = status;

            SerializedProperty friendButtonsProp = so.FindProperty("friendRowButtons");
            friendButtonsProp.arraySize = friendButtons.Count;
            for (int i = 0; i < friendButtons.Count; i++)
            {
                friendButtonsProp.GetArrayElementAtIndex(i).objectReferenceValue = friendButtons[i];
            }

            SerializedProperty friendHighlightsProp = so.FindProperty("friendRowHighlights");
            friendHighlightsProp.arraySize = friendHighlights.Count;
            for (int i = 0; i < friendHighlights.Count; i++)
            {
                friendHighlightsProp.GetArrayElementAtIndex(i).objectReferenceValue = friendHighlights[i];
            }

            so.ApplyModifiedPropertiesWithoutUndo();

            SaveScene(scene, InviteFriendsScenePath, failures);
        }

        private static void BuildLobbyScreenScene(List<string> failures)
        {
            Sprite background = LoadBackground(failures);
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EnsureEventSystem(scene);

            GameObject canvasGo = CreateOverlayCanvas("LobbyScreenCanvas");
            LobbyScreenController controller = canvasGo.AddComponent<LobbyScreenController>();
            RectTransform canvasRt = canvasGo.GetComponent<RectTransform>();

            Image bg = CreateUiImage("Background", canvasRt, stretch: true);
            bg.sprite = background;
            bg.preserveAspect = false;
            bg.raycastTarget = false;

            CreateSafeArea(canvasRt);

            Button backButton = CreateLabeledButton("BackButton", canvasRt, "Back", 168f, 64f, ButtonDark, GoldBright);
            PlaceTopLeft(backButton.GetComponent<RectTransform>(), new Vector2(48f, -52f));

            // Header — Room Type / Host / Players (+ room code) — Sprint 21.5 polish
            RectTransform headerRoot = CreateRect("HeaderRoot", canvasRt);
            headerRoot.anchorMin = new Vector2(0.5f, 1f);
            headerRoot.anchorMax = new Vector2(0.5f, 1f);
            headerRoot.pivot = new Vector2(0.5f, 1f);
            headerRoot.anchoredPosition = new Vector2(0f, -56f);
            headerRoot.sizeDelta = new Vector2(1200f, 120f);

            Image headerBorder = headerRoot.gameObject.AddComponent<Image>();
            headerBorder.color = PanelBorder;
            headerBorder.raycastTarget = false;
            EnsureLobbyPanelShadow(headerRoot.gameObject);

            Image headerFill = CreateUiImage("Fill", headerRoot, stretch: true);
            headerFill.color = PanelBg;
            headerFill.raycastTarget = false;
            headerFill.rectTransform.offsetMin = new Vector2(3f, 3f);
            headerFill.rectTransform.offsetMax = new Vector2(-3f, -3f);

            Text roomType = CreateUiText("RoomTypeText", headerRoot, "Room Type: Public", 30, FontStyle.Bold, GoldBright, TextAnchor.MiddleCenter);
            RectTransform roomTypeRt = roomType.rectTransform;
            roomTypeRt.anchorMin = new Vector2(0f, 0.52f);
            roomTypeRt.anchorMax = new Vector2(1f, 1f);
            roomTypeRt.offsetMin = new Vector2(28f, 0f);
            roomTypeRt.offsetMax = new Vector2(-28f, -6f);

            Text hostName = CreateUiText("HostNameText", headerRoot, "Host: DesertFox", 22, FontStyle.Bold, TextPrimary, TextAnchor.MiddleLeft);
            RectTransform hostNameRt = hostName.rectTransform;
            hostNameRt.anchorMin = new Vector2(0f, 0f);
            hostNameRt.anchorMax = new Vector2(0.34f, 0.52f);
            hostNameRt.offsetMin = new Vector2(36f, 10f);
            hostNameRt.offsetMax = new Vector2(-8f, -4f);

            Text playerCount = CreateUiText("PlayerCountText", headerRoot, "Players: 1 / 4", 22, FontStyle.Bold, TextPrimary, TextAnchor.MiddleCenter);
            RectTransform playerCountRt = playerCount.rectTransform;
            playerCountRt.anchorMin = new Vector2(0.34f, 0f);
            playerCountRt.anchorMax = new Vector2(0.66f, 0.52f);
            playerCountRt.offsetMin = new Vector2(8f, 10f);
            playerCountRt.offsetMax = new Vector2(-8f, -4f);

            Text roomCode = CreateUiText("RoomCodeText", headerRoot, "GULF-4821", 22, FontStyle.Bold, Gold, TextAnchor.MiddleRight);
            RectTransform roomCodeRt = roomCode.rectTransform;
            roomCodeRt.anchorMin = new Vector2(0.66f, 0f);
            roomCodeRt.anchorMax = new Vector2(1f, 0.52f);
            roomCodeRt.offsetMin = new Vector2(8f, 10f);
            roomCodeRt.offsetMax = new Vector2(-36f, -4f);

            // Center — four equal player slots (Sprint 21.5 rhythm; static mock only)
            const float slotHeight = 148f;
            const float slotGap = 24f;
            float slotsHeight = (slotHeight * 4f) + (slotGap * 3f);
            RectTransform slotsRoot = CreateRect("SlotsRoot", canvasRt);
            slotsRoot.anchorMin = new Vector2(0.5f, 0.5f);
            slotsRoot.anchorMax = new Vector2(0.5f, 0.5f);
            slotsRoot.pivot = new Vector2(0.5f, 0.5f);
            slotsRoot.anchoredPosition = new Vector2(0f, 72f);
            slotsRoot.sizeDelta = new Vector2(980f, slotsHeight);

            // Mock hierarchy: host ready, guest not ready, connecting sample, empty waiting.
            // Kick visible on slots 1–2 as Host preview mock only (no kick logic).
            CreatePlayerSlot(
                "PlayerSlot_0",
                slotsRoot,
                0,
                occupied: true,
                playerName: "DesertFox",
                countryCode: "KW",
                flagColor: new Color(0.05f, 0.45f, 0.25f, 1f),
                level: 12,
                readyVisual: SlotReadyVisual.Ready,
                online: true,
                showHostBadge: true,
                showKickButton: false);

            CreatePlayerSlot(
                "PlayerSlot_1",
                slotsRoot,
                1,
                occupied: true,
                playerName: "NightOwl",
                countryCode: "AE",
                flagColor: new Color(0.05f, 0.28f, 0.55f, 1f),
                level: 8,
                readyVisual: SlotReadyVisual.NotReady,
                online: true,
                showHostBadge: false,
                showKickButton: true);

            CreatePlayerSlot(
                "PlayerSlot_2",
                slotsRoot,
                2,
                occupied: true,
                playerName: "SandWave",
                countryCode: "SA",
                flagColor: new Color(0.10f, 0.42f, 0.22f, 1f),
                level: 5,
                readyVisual: SlotReadyVisual.Connecting,
                online: false,
                showHostBadge: false,
                showKickButton: true);

            CreatePlayerSlot(
                "PlayerSlot_3",
                slotsRoot,
                3,
                occupied: false,
                playerName: string.Empty,
                countryCode: string.Empty,
                flagColor: Color.clear,
                level: 0,
                readyVisual: SlotReadyVisual.NotReady,
                online: false,
                showHostBadge: false,
                showKickButton: false);

            // Status band above footer buttons (Sprint 21.3 Ready System UI + 21.5 polish)
            RectTransform statusRoot = CreateRect("StatusRoot", canvasRt);
            statusRoot.anchorMin = new Vector2(0.5f, 0f);
            statusRoot.anchorMax = new Vector2(0.5f, 0f);
            statusRoot.pivot = new Vector2(0.5f, 0f);
            statusRoot.anchoredPosition = new Vector2(0f, 214f);
            statusRoot.sizeDelta = new Vector2(1200f, 96f);

            RectTransform lobbyStatusPanel = CreateRect("LobbyStatusPanel", statusRoot);
            lobbyStatusPanel.anchorMin = new Vector2(0.5f, 0.5f);
            lobbyStatusPanel.anchorMax = new Vector2(0.5f, 0.5f);
            lobbyStatusPanel.pivot = new Vector2(0.5f, 0.5f);
            lobbyStatusPanel.anchoredPosition = new Vector2(0f, 6f);
            lobbyStatusPanel.sizeDelta = new Vector2(980f, 72f);

            Image statusBorder = lobbyStatusPanel.gameObject.AddComponent<Image>();
            statusBorder.color = PanelBorder;
            statusBorder.raycastTarget = false;
            EnsureLobbyPanelShadow(lobbyStatusPanel.gameObject);

            Image statusFill = CreateUiImage("Fill", lobbyStatusPanel, stretch: true);
            statusFill.color = PanelBg;
            statusFill.raycastTarget = false;
            statusFill.rectTransform.offsetMin = new Vector2(3f, 3f);
            statusFill.rectTransform.offsetMax = new Vector2(-3f, -3f);

            // Primary active message; alternate copy kept as inactive placeholders.
            Text lobbyStatus = CreateUiText(
                "LobbyStatusText",
                lobbyStatusPanel,
                "Waiting for everyone to be Ready...",
                26,
                FontStyle.Bold,
                TextMuted,
                TextAnchor.MiddleCenter);
            RectTransform lobbyStatusRt = lobbyStatus.rectTransform;
            lobbyStatusRt.anchorMin = new Vector2(0f, 0f);
            lobbyStatusRt.anchorMax = new Vector2(1f, 1f);
            lobbyStatusRt.offsetMin = new Vector2(24f, 8f);
            lobbyStatusRt.offsetMax = new Vector2(-24f, -8f);

            CreateInactiveStatusMessage(lobbyStatusPanel, "StatusMsg_WaitingForPlayers", "Waiting for players...");
            CreateInactiveStatusMessage(lobbyStatusPanel, "StatusMsg_PlayersJoining", "Players joining...");
            CreateInactiveStatusMessage(lobbyStatusPanel, "StatusMsg_WaitingReady", "Waiting for everyone to be Ready...");
            CreateInactiveStatusMessage(lobbyStatusPanel, "StatusMsg_ReadyToStart", "Ready to Start");

            Text countdown = CreateUiText(
                "CountdownPlaceholder",
                statusRoot,
                "Starting in: 00:10",
                28,
                FontStyle.Bold,
                GoldBright,
                TextAnchor.MiddleCenter);
            RectTransform countdownRt = countdown.rectTransform;
            countdownRt.anchorMin = new Vector2(0.5f, 0f);
            countdownRt.anchorMax = new Vector2(0.5f, 0f);
            countdownRt.pivot = new Vector2(0.5f, 0f);
            countdownRt.anchoredPosition = new Vector2(0f, -2f);
            countdownRt.sizeDelta = new Vector2(420f, 36f);
            countdown.gameObject.SetActive(false);

            // System message footer placeholders (Sprint 21.4) — visual only.
            RectTransform messageFooter = CreateRect("MessageFooterRoot", canvasRt);
            messageFooter.anchorMin = new Vector2(0.5f, 0f);
            messageFooter.anchorMax = new Vector2(0.5f, 0f);
            messageFooter.pivot = new Vector2(0.5f, 0f);
            messageFooter.anchoredPosition = new Vector2(0f, 156f);
            messageFooter.sizeDelta = new Vector2(1200f, 36f);

            Text systemSample = CreateUiText(
                "SystemMsg_PlayerJoined",
                messageFooter,
                "Player joined...",
                20,
                FontStyle.Italic,
                new Color(TextMuted.r, TextMuted.g, TextMuted.b, 0.75f),
                TextAnchor.MiddleCenter);
            RectTransform systemSampleRt = systemSample.rectTransform;
            systemSampleRt.anchorMin = Vector2.zero;
            systemSampleRt.anchorMax = Vector2.one;
            systemSampleRt.offsetMin = Vector2.zero;
            systemSampleRt.offsetMax = Vector2.zero;

            CreateInactiveSystemMessage(messageFooter, "SystemMsg_PlayerLeft", "Player left...");
            CreateInactiveSystemMessage(messageFooter, "SystemMsg_HostChanged", "Host changed...");
            CreateInactiveSystemMessage(messageFooter, "SystemMsg_Searching", "Searching for player...");

            // Footer — Ready + Play matched touch targets (Sprint 21.5)
            RectTransform footerRoot = CreateRect("FooterRoot", canvasRt);
            footerRoot.anchorMin = new Vector2(0.5f, 0f);
            footerRoot.anchorMax = new Vector2(0.5f, 0f);
            footerRoot.pivot = new Vector2(0.5f, 0f);
            footerRoot.anchoredPosition = new Vector2(0f, 40f);
            footerRoot.sizeDelta = new Vector2(1400f, 120f);

            const float actionButtonWidth = 400f;
            const float actionButtonHeight = 104f;
            Button readyButton = CreateLabeledButton(
                "ReadyButton",
                footerRoot,
                "Ready",
                actionButtonWidth,
                actionButtonHeight,
                Gold,
                GoldButtonLabel);
            RectTransform readyRt = readyButton.GetComponent<RectTransform>();
            readyRt.anchorMin = new Vector2(0f, 0.5f);
            readyRt.anchorMax = new Vector2(0f, 0.5f);
            readyRt.pivot = new Vector2(0f, 0.5f);
            readyRt.anchoredPosition = new Vector2(48f, 0f);
            readyButton.transition = Selectable.Transition.None;
            Text readyLabel = readyButton.GetComponentInChildren<Text>();
            if (readyLabel != null)
            {
                readyLabel.fontSize = 30;
            }

            Image readyImage = readyButton.GetComponent<Image>();

            // Default Play state: disabled + greyed "Waiting for Players...".
            // Prepared chrome ("Start Match") available via controller demo toggle.
            Button playButton = CreateLabeledButton(
                "PlayButton",
                footerRoot,
                "Waiting for Players...",
                actionButtonWidth,
                actionButtonHeight,
                new Color(0.18f, 0.16f, 0.14f, 0.72f),
                new Color(0.62f, 0.60f, 0.56f, 0.85f));
            RectTransform playRt = playButton.GetComponent<RectTransform>();
            playRt.anchorMin = new Vector2(1f, 0.5f);
            playRt.anchorMax = new Vector2(1f, 0.5f);
            playRt.pivot = new Vector2(1f, 0.5f);
            playRt.anchoredPosition = new Vector2(-48f, 0f);
            playButton.interactable = false;
            playButton.transition = Selectable.Transition.None;
            ColorBlock playColors = playButton.colors;
            playColors.disabledColor = new Color(0.18f, 0.16f, 0.14f, 0.72f);
            playButton.colors = playColors;
            Text playLabel = playButton.GetComponentInChildren<Text>();
            if (playLabel != null)
            {
                playLabel.fontSize = 30;
                playLabel.color = new Color(0.62f, 0.60f, 0.56f, 0.85f);
            }

            Image playImage = playButton.GetComponent<Image>();

            // Inactive prepared-state label placeholder (named state for builders / docs).
            Text preparedLabel = CreateUiText(
                "PlayLabel_StartMatch",
                playRt,
                "Start Match",
                30,
                FontStyle.Bold,
                GoldButtonLabel,
                TextAnchor.MiddleCenter);
            RectTransform preparedLabelRt = preparedLabel.rectTransform;
            preparedLabelRt.anchorMin = Vector2.zero;
            preparedLabelRt.anchorMax = Vector2.one;
            preparedLabelRt.offsetMin = Vector2.zero;
            preparedLabelRt.offsetMax = Vector2.zero;
            preparedLabel.gameObject.SetActive(false);

            // Optional local-slot chrome for Ready button visual demo (slot 0).
            Transform slot0Tf = slotsRoot.Find("PlayerSlot_0");
            Image localReadyStatus = slot0Tf != null
                ? FindChildRecursive(slot0Tf, "ReadyStatus")?.GetComponent<Image>()
                : null;
            Text localReadyLabel = slot0Tf != null
                ? FindChildRecursive(slot0Tf, "ReadyLabel")?.GetComponent<Text>()
                : null;

            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("backButton").objectReferenceValue = backButton;
            so.FindProperty("readyButton").objectReferenceValue = readyButton;
            so.FindProperty("readyButtonImage").objectReferenceValue = readyImage;
            so.FindProperty("readyButtonLabel").objectReferenceValue = readyLabel;
            so.FindProperty("localReadyStatus").objectReferenceValue = localReadyStatus;
            so.FindProperty("localReadyLabel").objectReferenceValue = localReadyLabel;
            so.FindProperty("playButton").objectReferenceValue = playButton;
            so.FindProperty("playButtonImage").objectReferenceValue = playImage;
            so.FindProperty("playButtonLabel").objectReferenceValue = playLabel;
            so.ApplyModifiedPropertiesWithoutUndo();

            SaveScene(scene, LobbyScreenScenePath, failures);
        }

        private static void BuildMapVotingScreenScene(List<string> failures)
        {
            Sprite background = LoadBackground(failures);
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EnsureEventSystem(scene);

            GameObject canvasGo = CreateOverlayCanvas("MapVotingCanvas");
            MapVotingScreenController controller = canvasGo.AddComponent<MapVotingScreenController>();
            RectTransform canvasRt = canvasGo.GetComponent<RectTransform>();

            Image bg = CreateUiImage("Background", canvasRt, stretch: true);
            bg.sprite = background;
            bg.preserveAspect = false;
            bg.raycastTarget = false;

            CreateSafeArea(canvasRt);

            Button backButton = CreateLabeledButton("BackButton", canvasRt, "Back", 168f, 64f, ButtonDark, GoldBright);
            PlaceTopLeft(backButton.GetComponent<RectTransform>(), new Vector2(48f, -52f));

            // Sprint 22.3 — Voting Timer (top-center premium panel, static placeholder)
            RectTransform timerPanel = CreateRect("TimerPanel", canvasRt);
            timerPanel.anchorMin = new Vector2(0.5f, 1f);
            timerPanel.anchorMax = new Vector2(0.5f, 1f);
            timerPanel.pivot = new Vector2(0.5f, 1f);
            timerPanel.anchoredPosition = new Vector2(0f, -40f);
            timerPanel.sizeDelta = new Vector2(300f, 118f);

            Image timerBorder = timerPanel.gameObject.AddComponent<Image>();
            timerBorder.color = PanelBorder;
            timerBorder.raycastTarget = false;
            EnsureLobbyPanelShadow(timerPanel.gameObject);

            Image timerFill = CreateUiImage("Fill", timerPanel, stretch: true);
            timerFill.color = PanelBg;
            timerFill.raycastTarget = false;
            timerFill.rectTransform.offsetMin = new Vector2(3f, 3f);
            timerFill.rectTransform.offsetMax = new Vector2(-3f, -3f);

            Text timerText = CreateUiText(
                "TimerText",
                timerPanel,
                "20s",
                42,
                FontStyle.Bold,
                GoldBright,
                TextAnchor.MiddleCenter);
            RectTransform timerTextRt = timerText.rectTransform;
            timerTextRt.anchorMin = new Vector2(0f, 0.38f);
            timerTextRt.anchorMax = new Vector2(1f, 1f);
            timerTextRt.offsetMin = new Vector2(16f, 0f);
            timerTextRt.offsetMax = new Vector2(-16f, -6f);

            RectTransform progressTrack = CreateRect("ProgressBarTrack", timerPanel);
            progressTrack.anchorMin = new Vector2(0.5f, 0f);
            progressTrack.anchorMax = new Vector2(0.5f, 0f);
            progressTrack.pivot = new Vector2(0.5f, 0f);
            progressTrack.anchoredPosition = new Vector2(0f, 16f);
            progressTrack.sizeDelta = new Vector2(240f, 14f);

            Image progressTrackImg = progressTrack.gameObject.AddComponent<Image>();
            progressTrackImg.color = new Color(0.06f, 0.05f, 0.06f, 0.95f);
            progressTrackImg.raycastTarget = false;

            Image progressFill = CreateUiImage("ProgressBarFill", progressTrack, stretch: true);
            progressFill.color = Gold;
            progressFill.raycastTarget = false;
            progressFill.type = Image.Type.Filled;
            progressFill.fillMethod = Image.FillMethod.Horizontal;
            progressFill.fillOrigin = (int)Image.OriginHorizontal.Left;
            progressFill.fillAmount = 1f; // static placeholder — no countdown logic
            progressFill.rectTransform.offsetMin = new Vector2(2f, 2f);
            progressFill.rectTransform.offsetMax = new Vector2(-2f, -2f);

            // Sprint 22.3 — Current Status (below timer; visual placeholders only)
            RectTransform statusPanel = CreateRect("StatusPanel", canvasRt);
            statusPanel.anchorMin = new Vector2(0.5f, 1f);
            statusPanel.anchorMax = new Vector2(0.5f, 1f);
            statusPanel.pivot = new Vector2(0.5f, 1f);
            statusPanel.anchoredPosition = new Vector2(0f, -168f);
            statusPanel.sizeDelta = new Vector2(920f, 64f);

            Image statusBorder = statusPanel.gameObject.AddComponent<Image>();
            statusBorder.color = PanelBorder;
            statusBorder.raycastTarget = false;
            EnsureLobbyPanelShadow(statusPanel.gameObject);

            Image statusFill = CreateUiImage("Fill", statusPanel, stretch: true);
            statusFill.color = PanelBg;
            statusFill.raycastTarget = false;
            statusFill.rectTransform.offsetMin = new Vector2(3f, 3f);
            statusFill.rectTransform.offsetMax = new Vector2(-3f, -3f);

            Text statusText = CreateUiText(
                "StatusText",
                statusPanel,
                "Players are voting...",
                24,
                FontStyle.Bold,
                TextMuted,
                TextAnchor.MiddleCenter);
            RectTransform statusTextRt = statusText.rectTransform;
            statusTextRt.anchorMin = new Vector2(0f, 0f);
            statusTextRt.anchorMax = new Vector2(1f, 1f);
            statusTextRt.offsetMin = new Vector2(24f, 6f);
            statusTextRt.offsetMax = new Vector2(-24f, -6f);

            CreateInactiveStatusMessage(statusPanel, "StatusMsg_WaitingForPlayers", "Waiting for players...");
            CreateInactiveStatusMessage(statusPanel, "StatusMsg_PlayersVoting", "Players are voting...");
            CreateInactiveStatusMessage(statusPanel, "StatusMsg_FinalizingResults", "Finalizing results...");

            // Header — title + subtitle (below Voting HUD)
            RectTransform headerRoot = CreateRect("HeaderRoot", canvasRt);
            headerRoot.anchorMin = new Vector2(0.5f, 1f);
            headerRoot.anchorMax = new Vector2(0.5f, 1f);
            headerRoot.pivot = new Vector2(0.5f, 1f);
            headerRoot.anchoredPosition = new Vector2(0f, -242f);
            headerRoot.sizeDelta = new Vector2(1400f, 96f);

            Text title = CreateUiText(
                "TitleText",
                headerRoot,
                "Choose Your Map",
                44,
                FontStyle.Bold,
                GoldBright,
                TextAnchor.MiddleCenter);
            RectTransform titleRt = title.rectTransform;
            titleRt.anchorMin = new Vector2(0f, 0.42f);
            titleRt.anchorMax = new Vector2(1f, 1f);
            titleRt.offsetMin = new Vector2(24f, 0f);
            titleRt.offsetMax = new Vector2(-24f, -2f);

            Text subtitle = CreateUiText(
                "SubtitleText",
                headerRoot,
                "Vote together to decide the next destination.",
                22,
                FontStyle.Normal,
                TextMuted,
                TextAnchor.MiddleCenter);
            RectTransform subtitleRt = subtitle.rectTransform;
            subtitleRt.anchorMin = new Vector2(0f, 0f);
            subtitleRt.anchorMax = new Vector2(1f, 0.48f);
            subtitleRt.offsetMin = new Vector2(40f, 2f);
            subtitleRt.offsetMax = new Vector2(-40f, 0f);

            // Center — three equal premium Gulf map cards (Sprint 22.2 preserved)
            const float cardWidth = 500f;
            const float cardHeight = 680f;
            const float cardGap = 36f;
            float cardsWidth = (cardWidth * 3f) + (cardGap * 2f);
            RectTransform cardsRoot = CreateRect("CardsRoot", canvasRt);
            cardsRoot.anchorMin = new Vector2(0.5f, 0.5f);
            cardsRoot.anchorMax = new Vector2(0.5f, 0.5f);
            cardsRoot.pivot = new Vector2(0.5f, 0.5f);
            cardsRoot.anchoredPosition = new Vector2(0f, -36f);
            cardsRoot.sizeDelta = new Vector2(cardsWidth, cardHeight);

            var voteButtons = new Button[3];
            var cardBorders = new Image[3];
            var voteImages = new Image[3];
            var voteLabels = new Text[3];
            var selectedCheckmarks = new GameObject[3];
            var cardVisuals = new MapCardVisual[3];

            CreateMapCard(
                "MapCard_0",
                cardsRoot,
                0,
                "Kuwait City",
                "KW",
                new Color(0.00f, 0.45f, 0.28f, 1f),
                "Medium",
                ConnectingAmber,
                "Neon towers and desert highways racing through the capital skyline.",
                "Est. 3:30",
                new Color(0.78f, 0.52f, 0.30f, 1f),
                new Color(0.38f, 0.24f, 0.14f, 1f),
                new Color(0.95f, 0.78f, 0.42f, 0.55f),
                cardWidth,
                cardHeight,
                cardGap,
                out voteButtons[0],
                out cardBorders[0],
                out voteImages[0],
                out voteLabels[0],
                out selectedCheckmarks[0],
                out cardVisuals[0]);

            CreateMapCard(
                "MapCard_1",
                cardsRoot,
                1,
                "Dubai Marina",
                "AE",
                new Color(0.78f, 0.12f, 0.14f, 1f),
                "Hard",
                new Color(0.92f, 0.38f, 0.32f, 1f),
                "Coastal expressways beside glass towers and glittering marina lights.",
                "Est. 4:15",
                new Color(0.18f, 0.42f, 0.68f, 1f),
                new Color(0.08f, 0.22f, 0.40f, 1f),
                new Color(0.55f, 0.82f, 0.95f, 0.50f),
                cardWidth,
                cardHeight,
                cardGap,
                out voteButtons[1],
                out cardBorders[1],
                out voteImages[1],
                out voteLabels[1],
                out selectedCheckmarks[1],
                out cardVisuals[1]);

            CreateMapCard(
                "MapCard_2",
                cardsRoot,
                2,
                "Muscat Coast",
                "OM",
                new Color(0.72f, 0.10f, 0.16f, 1f),
                "Easy",
                SuccessGreen,
                "Mountain curves meeting turquoise Gulf waters at golden hour.",
                "Est. 2:45",
                new Color(0.22f, 0.58f, 0.52f, 1f),
                new Color(0.52f, 0.38f, 0.24f, 1f),
                new Color(0.95f, 0.72f, 0.38f, 0.48f),
                cardWidth,
                cardHeight,
                cardGap,
                out voteButtons[2],
                out cardBorders[2],
                out voteImages[2],
                out voteLabels[2],
                out selectedCheckmarks[2],
                out cardVisuals[2]);

            // Bottom — voting statistics placeholders (static copy only)
            RectTransform footerRoot = CreateRect("FooterRoot", canvasRt);
            footerRoot.anchorMin = new Vector2(0.5f, 0f);
            footerRoot.anchorMax = new Vector2(0.5f, 0f);
            footerRoot.pivot = new Vector2(0.5f, 0f);
            footerRoot.anchoredPosition = new Vector2(0f, 40f);
            footerRoot.sizeDelta = new Vector2(1400f, 100f);

            Image footerBorder = footerRoot.gameObject.AddComponent<Image>();
            footerBorder.color = PanelBorder;
            footerBorder.raycastTarget = false;
            EnsureLobbyPanelShadow(footerRoot.gameObject);

            Image footerFill = CreateUiImage("Fill", footerRoot, stretch: true);
            footerFill.color = PanelBg;
            footerFill.raycastTarget = false;
            footerFill.rectTransform.offsetMin = new Vector2(3f, 3f);
            footerFill.rectTransform.offsetMax = new Vector2(-3f, -3f);

            RectTransform statsRoot = CreateRect("StatsPanel", footerRoot);
            statsRoot.anchorMin = Vector2.zero;
            statsRoot.anchorMax = Vector2.one;
            statsRoot.offsetMin = new Vector2(12f, 8f);
            statsRoot.offsetMax = new Vector2(-12f, -8f);

            Text playersVoted = CreateUiText(
                "PlayersVotedText",
                statsRoot,
                "Players Voted 0/4",
                22,
                FontStyle.Bold,
                TextPrimary,
                TextAnchor.MiddleCenter);
            RectTransform playersVotedRt = playersVoted.rectTransform;
            playersVotedRt.anchorMin = new Vector2(0f, 0f);
            playersVotedRt.anchorMax = new Vector2(0.33f, 1f);
            playersVotedRt.offsetMin = new Vector2(8f, 0f);
            playersVotedRt.offsetMax = new Vector2(-8f, 0f);

            Text remainingVotes = CreateUiText(
                "RemainingVotesText",
                statsRoot,
                "Remaining Votes 4",
                22,
                FontStyle.Bold,
                TextPrimary,
                TextAnchor.MiddleCenter);
            RectTransform remainingVotesRt = remainingVotes.rectTransform;
            remainingVotesRt.anchorMin = new Vector2(0.33f, 0f);
            remainingVotesRt.anchorMax = new Vector2(0.66f, 1f);
            remainingVotesRt.offsetMin = new Vector2(8f, 0f);
            remainingVotesRt.offsetMax = new Vector2(-8f, 0f);

            Text totalVotes = CreateUiText(
                "TotalVotesText",
                statsRoot,
                "Total Votes 0",
                22,
                FontStyle.Bold,
                TextPrimary,
                TextAnchor.MiddleCenter);
            RectTransform totalVotesRt = totalVotes.rectTransform;
            totalVotesRt.anchorMin = new Vector2(0.66f, 0f);
            totalVotesRt.anchorMax = new Vector2(1f, 1f);
            totalVotesRt.offsetMin = new Vector2(8f, 0f);
            totalVotesRt.offsetMax = new Vector2(-8f, 0f);

            // Vote confirmation — hidden by default (no vote-submit logic)
            RectTransform confirmation = CreateRect("VoteConfirmation", canvasRt);
            confirmation.anchorMin = new Vector2(0.5f, 0f);
            confirmation.anchorMax = new Vector2(0.5f, 0f);
            confirmation.pivot = new Vector2(0.5f, 0f);
            confirmation.anchoredPosition = new Vector2(0f, 152f);
            confirmation.sizeDelta = new Vector2(720f, 52f);
            confirmation.gameObject.SetActive(false);

            Image confirmBorder = confirmation.gameObject.AddComponent<Image>();
            confirmBorder.color = new Color(SuccessGreen.r, SuccessGreen.g, SuccessGreen.b, 0.55f);
            confirmBorder.raycastTarget = false;
            EnsureLobbyPanelShadow(confirmation.gameObject);

            Image confirmFill = CreateUiImage("Fill", confirmation, stretch: true);
            confirmFill.color = new Color(0.08f, 0.16f, 0.10f, 0.92f);
            confirmFill.raycastTarget = false;
            confirmFill.rectTransform.offsetMin = new Vector2(3f, 3f);
            confirmFill.rectTransform.offsetMax = new Vector2(-3f, -3f);

            Text confirmText = CreateUiText(
                "ConfirmationText",
                confirmation,
                "✓ Your vote has been submitted.",
                22,
                FontStyle.Bold,
                SuccessGreen,
                TextAnchor.MiddleCenter);
            RectTransform confirmTextRt = confirmText.rectTransform;
            confirmTextRt.anchorMin = Vector2.zero;
            confirmTextRt.anchorMax = Vector2.one;
            confirmTextRt.offsetMin = new Vector2(16f, 4f);
            confirmTextRt.offsetMax = new Vector2(-16f, -4f);

            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("backButton").objectReferenceValue = backButton;
            SerializedProperty voteButtonsProp = so.FindProperty("voteButtons");
            SerializedProperty cardBordersProp = so.FindProperty("cardBorders");
            SerializedProperty voteImagesProp = so.FindProperty("voteButtonImages");
            SerializedProperty voteLabelsProp = so.FindProperty("voteButtonLabels");
            SerializedProperty checkmarksProp = so.FindProperty("selectedCheckmarks");
            SerializedProperty visualsProp = so.FindProperty("cardVisuals");
            voteButtonsProp.arraySize = 3;
            cardBordersProp.arraySize = 3;
            voteImagesProp.arraySize = 3;
            voteLabelsProp.arraySize = 3;
            checkmarksProp.arraySize = 3;
            visualsProp.arraySize = 3;
            for (int i = 0; i < 3; i++)
            {
                voteButtonsProp.GetArrayElementAtIndex(i).objectReferenceValue = voteButtons[i];
                cardBordersProp.GetArrayElementAtIndex(i).objectReferenceValue = cardBorders[i];
                voteImagesProp.GetArrayElementAtIndex(i).objectReferenceValue = voteImages[i];
                voteLabelsProp.GetArrayElementAtIndex(i).objectReferenceValue = voteLabels[i];
                checkmarksProp.GetArrayElementAtIndex(i).objectReferenceValue = selectedCheckmarks[i];
                visualsProp.GetArrayElementAtIndex(i).objectReferenceValue = cardVisuals[i];
            }

            so.ApplyModifiedPropertiesWithoutUndo();

            SaveScene(scene, MapVotingScenePath, failures);
        }

        private static void CreateMapCard(
            string name,
            RectTransform parent,
            int index,
            string mapName,
            string countryCode,
            Color flagColor,
            string difficultyLabel,
            Color difficultyColor,
            string description,
            string durationLabel,
            Color previewTop,
            Color previewBottom,
            Color previewAccent,
            float cardWidth,
            float cardHeight,
            float cardGap,
            out Button voteButton,
            out Image cardBorder,
            out Image voteImage,
            out Text voteLabel,
            out GameObject selectedCheckmark,
            out MapCardVisual cardVisual)
        {
            float x = (index - 1) * (cardWidth + cardGap);
            RectTransform card = CreateRect(name, parent);
            card.anchorMin = new Vector2(0.5f, 0.5f);
            card.anchorMax = new Vector2(0.5f, 0.5f);
            card.pivot = new Vector2(0.5f, 0.5f);
            card.anchoredPosition = new Vector2(x, 0f);
            card.sizeDelta = new Vector2(cardWidth, cardHeight);

            cardBorder = card.gameObject.AddComponent<Image>();
            cardBorder.color = PanelBorder;
            cardBorder.raycastTarget = true;
            EnsureLobbyPanelShadow(card.gameObject);

            cardVisual = card.gameObject.AddComponent<MapCardVisual>();
            SerializedObject visualSo = new SerializedObject(cardVisual);
            visualSo.FindProperty("cardShadow").objectReferenceValue = card.GetComponent<Shadow>();
            visualSo.ApplyModifiedPropertiesWithoutUndo();

            Image fill = CreateUiImage("Fill", card, stretch: true);
            fill.color = CardFill;
            fill.raycastTarget = false;
            fill.rectTransform.offsetMin = new Vector2(3f, 3f);
            fill.rectTransform.offsetMax = new Vector2(-3f, -3f);

            // Large map preview placeholder — stacked colored panels (artwork later).
            RectTransform previewRoot = CreateRect("MapPreview", card);
            previewRoot.anchorMin = new Vector2(0.5f, 1f);
            previewRoot.anchorMax = new Vector2(0.5f, 1f);
            previewRoot.pivot = new Vector2(0.5f, 1f);
            previewRoot.anchoredPosition = new Vector2(0f, -22f);
            previewRoot.sizeDelta = new Vector2(cardWidth - 40f, 300f);

            Image previewBorder = previewRoot.gameObject.AddComponent<Image>();
            previewBorder.color = new Color(Gold.r, Gold.g, Gold.b, 0.40f);
            previewBorder.raycastTarget = false;

            Image previewTopImg = CreateUiImage("PreviewTop", previewRoot, stretch: true);
            previewTopImg.color = previewTop;
            previewTopImg.raycastTarget = false;
            previewTopImg.rectTransform.offsetMin = new Vector2(3f, 96f);
            previewTopImg.rectTransform.offsetMax = new Vector2(-3f, -3f);

            Image previewBottomImg = CreateUiImage("PreviewBottom", previewRoot, stretch: true);
            previewBottomImg.color = previewBottom;
            previewBottomImg.raycastTarget = false;
            previewBottomImg.rectTransform.offsetMin = new Vector2(3f, 3f);
            previewBottomImg.rectTransform.offsetMax = new Vector2(-3f, -200f);

            Image previewAccentImg = CreateUiImage("PreviewAccent", previewRoot, stretch: false);
            previewAccentImg.color = previewAccent;
            previewAccentImg.raycastTarget = false;
            RectTransform accentRt = previewAccentImg.rectTransform;
            accentRt.anchorMin = new Vector2(0f, 0f);
            accentRt.anchorMax = new Vector2(1f, 0f);
            accentRt.pivot = new Vector2(0.5f, 0f);
            accentRt.anchoredPosition = new Vector2(0f, 3f);
            accentRt.sizeDelta = new Vector2(-6f, 28f);

            Text previewCaption = CreateUiText(
                "PreviewCaption",
                previewRoot,
                mapName.ToUpperInvariant(),
                18,
                FontStyle.Bold,
                new Color(1f, 1f, 1f, 0.88f),
                TextAnchor.MiddleCenter);
            RectTransform captionRt = previewCaption.rectTransform;
            captionRt.anchorMin = new Vector2(0f, 0f);
            captionRt.anchorMax = new Vector2(1f, 0f);
            captionRt.pivot = new Vector2(0.5f, 0f);
            captionRt.anchoredPosition = new Vector2(0f, 4f);
            captionRt.sizeDelta = new Vector2(-16f, 26f);

            // Meta row — country flag + difficulty
            RectTransform metaRow = CreateRect("MetaRow", card);
            metaRow.anchorMin = new Vector2(0.5f, 1f);
            metaRow.anchorMax = new Vector2(0.5f, 1f);
            metaRow.pivot = new Vector2(0.5f, 1f);
            metaRow.anchoredPosition = new Vector2(0f, -336f);
            metaRow.sizeDelta = new Vector2(cardWidth - 48f, 36f);

            Image flag = CreateUiImage("CountryFlag", metaRow, stretch: false);
            flag.color = flagColor;
            flag.raycastTarget = false;
            RectTransform flagRt = flag.rectTransform;
            flagRt.anchorMin = new Vector2(0f, 0.5f);
            flagRt.anchorMax = new Vector2(0f, 0.5f);
            flagRt.pivot = new Vector2(0f, 0.5f);
            flagRt.anchoredPosition = new Vector2(0f, 0f);
            flagRt.sizeDelta = new Vector2(44f, 28f);

            Text country = CreateUiText("CountryCode", metaRow, countryCode, 18, FontStyle.Bold, TextMuted, TextAnchor.MiddleLeft);
            RectTransform countryRt = country.rectTransform;
            countryRt.anchorMin = new Vector2(0f, 0.5f);
            countryRt.anchorMax = new Vector2(0f, 0.5f);
            countryRt.pivot = new Vector2(0f, 0.5f);
            countryRt.anchoredPosition = new Vector2(54f, 0f);
            countryRt.sizeDelta = new Vector2(70f, 30f);

            Image difficultyBadge = CreateUiImage("DifficultyBadge", metaRow, stretch: false);
            difficultyBadge.color = new Color(difficultyColor.r, difficultyColor.g, difficultyColor.b, 0.22f);
            difficultyBadge.raycastTarget = false;
            RectTransform difficultyBadgeRt = difficultyBadge.rectTransform;
            difficultyBadgeRt.anchorMin = new Vector2(1f, 0.5f);
            difficultyBadgeRt.anchorMax = new Vector2(1f, 0.5f);
            difficultyBadgeRt.pivot = new Vector2(1f, 0.5f);
            difficultyBadgeRt.anchoredPosition = Vector2.zero;
            difficultyBadgeRt.sizeDelta = new Vector2(128f, 30f);

            Text difficulty = CreateUiText(
                "DifficultyText",
                difficultyBadgeRt,
                difficultyLabel,
                18,
                FontStyle.Bold,
                difficultyColor,
                TextAnchor.MiddleCenter);
            RectTransform difficultyRt = difficulty.rectTransform;
            difficultyRt.anchorMin = Vector2.zero;
            difficultyRt.anchorMax = Vector2.one;
            difficultyRt.offsetMin = Vector2.zero;
            difficultyRt.offsetMax = Vector2.zero;

            Text nameText = CreateUiText("MapName", card, mapName, 34, FontStyle.Bold, GoldBright, TextAnchor.MiddleCenter);
            RectTransform nameRt = nameText.rectTransform;
            nameRt.anchorMin = new Vector2(0f, 0.36f);
            nameRt.anchorMax = new Vector2(1f, 0.46f);
            nameRt.offsetMin = new Vector2(20f, 0f);
            nameRt.offsetMax = new Vector2(-20f, 0f);

            Text descText = CreateUiText(
                "Description",
                card,
                description,
                19,
                FontStyle.Normal,
                TextMuted,
                TextAnchor.UpperCenter);
            descText.horizontalOverflow = HorizontalWrapMode.Wrap;
            descText.verticalOverflow = VerticalWrapMode.Overflow;
            RectTransform descRt = descText.rectTransform;
            descRt.anchorMin = new Vector2(0f, 0.22f);
            descRt.anchorMax = new Vector2(1f, 0.36f);
            descRt.offsetMin = new Vector2(28f, 0f);
            descRt.offsetMax = new Vector2(-28f, 0f);

            Text durationText = CreateUiText(
                "DurationText",
                card,
                durationLabel,
                20,
                FontStyle.Bold,
                Gold,
                TextAnchor.MiddleCenter);
            RectTransform durationRt = durationText.rectTransform;
            durationRt.anchorMin = new Vector2(0f, 0.155f);
            durationRt.anchorMax = new Vector2(1f, 0.22f);
            durationRt.offsetMin = new Vector2(24f, 0f);
            durationRt.offsetMax = new Vector2(-24f, 0f);

            voteButton = CreateLabeledButton("VoteButton", card, "Vote", 280f, 72f, Gold, GoldButtonLabel);
            RectTransform voteRt = voteButton.GetComponent<RectTransform>();
            voteRt.anchorMin = new Vector2(0.5f, 0f);
            voteRt.anchorMax = new Vector2(0.5f, 0f);
            voteRt.pivot = new Vector2(0.5f, 0f);
            voteRt.anchoredPosition = new Vector2(0f, 26f);
            voteButton.transition = Selectable.Transition.None;
            voteImage = voteButton.GetComponent<Image>();
            voteLabel = voteButton.GetComponentInChildren<Text>();
            if (voteLabel != null)
            {
                voteLabel.fontSize = 28;
            }

            // Selected checkmark — inactive until Vote highlight.
            selectedCheckmark = CreateRect("SelectedCheckmark", card).gameObject;
            selectedCheckmark.SetActive(false);
            RectTransform checkRt = selectedCheckmark.GetComponent<RectTransform>();
            checkRt.anchorMin = new Vector2(1f, 1f);
            checkRt.anchorMax = new Vector2(1f, 1f);
            checkRt.pivot = new Vector2(1f, 1f);
            checkRt.anchoredPosition = new Vector2(-18f, -18f);
            checkRt.sizeDelta = new Vector2(52f, 52f);

            Image checkBadge = selectedCheckmark.AddComponent<Image>();
            checkBadge.sprite = GetBuiltinKnob();
            checkBadge.color = GoldBright;
            checkBadge.raycastTarget = false;
            checkBadge.preserveAspect = true;

            Text checkMark = CreateUiText("CheckMark", checkRt, "✓", 28, FontStyle.Bold, GoldButtonLabel, TextAnchor.MiddleCenter);
            RectTransform checkMarkRt = checkMark.rectTransform;
            checkMarkRt.anchorMin = Vector2.zero;
            checkMarkRt.anchorMax = Vector2.one;
            checkMarkRt.offsetMin = Vector2.zero;
            checkMarkRt.offsetMax = Vector2.zero;

            // Locked overlay — hidden by default on all three cards.
            RectTransform lockedRoot = CreateRect("LockedRoot", card);
            lockedRoot.anchorMin = Vector2.zero;
            lockedRoot.anchorMax = Vector2.one;
            lockedRoot.offsetMin = new Vector2(3f, 3f);
            lockedRoot.offsetMax = new Vector2(-3f, -3f);
            lockedRoot.gameObject.SetActive(false);

            Image lockedDim = lockedRoot.gameObject.AddComponent<Image>();
            lockedDim.color = new Color(0.04f, 0.03f, 0.03f, 0.72f);
            lockedDim.raycastTarget = false;

            Image lockBadge = CreateUiImage("LockIcon", lockedRoot, stretch: false);
            lockBadge.sprite = GetBuiltinKnob();
            lockBadge.color = Gold;
            lockBadge.raycastTarget = false;
            lockBadge.preserveAspect = true;
            RectTransform lockBadgeRt = lockBadge.rectTransform;
            lockBadgeRt.anchorMin = new Vector2(0.5f, 0.5f);
            lockBadgeRt.anchorMax = new Vector2(0.5f, 0.5f);
            lockBadgeRt.pivot = new Vector2(0.5f, 0.5f);
            lockBadgeRt.anchoredPosition = new Vector2(0f, 22f);
            lockBadgeRt.sizeDelta = new Vector2(72f, 72f);

            Text lockGlyph = CreateUiText("LockGlyph", lockBadgeRt, "L", 30, FontStyle.Bold, GoldButtonLabel, TextAnchor.MiddleCenter);
            RectTransform lockGlyphRt = lockGlyph.rectTransform;
            lockGlyphRt.anchorMin = Vector2.zero;
            lockGlyphRt.anchorMax = Vector2.one;
            lockGlyphRt.offsetMin = Vector2.zero;
            lockGlyphRt.offsetMax = Vector2.zero;

            Text lockedLabel = CreateUiText(
                "LockedLabel",
                lockedRoot,
                "Locked",
                28,
                FontStyle.Bold,
                GoldBright,
                TextAnchor.MiddleCenter);
            RectTransform lockedLabelRt = lockedLabel.rectTransform;
            lockedLabelRt.anchorMin = new Vector2(0.5f, 0.5f);
            lockedLabelRt.anchorMax = new Vector2(0.5f, 0.5f);
            lockedLabelRt.pivot = new Vector2(0.5f, 0.5f);
            lockedLabelRt.anchoredPosition = new Vector2(0f, -36f);
            lockedLabelRt.sizeDelta = new Vector2(200f, 36f);
        }

        private static void CreateInactiveStatusMessage(RectTransform parent, string name, string message)
        {
            Text text = CreateUiText(name, parent, message, 26, FontStyle.Bold, TextMuted, TextAnchor.MiddleCenter);
            RectTransform rt = text.rectTransform;
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.offsetMin = new Vector2(24f, 8f);
            rt.offsetMax = new Vector2(-24f, -8f);
            text.gameObject.SetActive(false);
        }

        private static void CreateInactiveSystemMessage(RectTransform parent, string name, string message)
        {
            Text text = CreateUiText(
                name,
                parent,
                message,
                20,
                FontStyle.Italic,
                new Color(TextMuted.r, TextMuted.g, TextMuted.b, 0.75f),
                TextAnchor.MiddleCenter);
            RectTransform rt = text.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            text.gameObject.SetActive(false);
        }

        private static void CreatePlayerSlot(
            string name,
            RectTransform parent,
            int index,
            bool occupied,
            string playerName,
            string countryCode,
            Color flagColor,
            int level,
            SlotReadyVisual readyVisual,
            bool online,
            bool showHostBadge,
            bool showKickButton)
        {
            // Touch-friendly vertical rhythm for mobile (CanvasScaler 1920×1080 match 0.5).
            const float slotHeight = 148f;
            const float gap = 24f;
            float totalHeight = (slotHeight * 4f) + (gap * 3f);
            float topY = totalHeight * 0.5f - slotHeight * 0.5f;
            float y = topY - index * (slotHeight + gap);

            RectTransform slot = CreateRect(name, parent);
            slot.anchorMin = new Vector2(0.5f, 0.5f);
            slot.anchorMax = new Vector2(0.5f, 0.5f);
            slot.pivot = new Vector2(0.5f, 0.5f);
            slot.anchoredPosition = new Vector2(0f, y);
            slot.sizeDelta = new Vector2(960f, slotHeight);

            Image border = slot.gameObject.AddComponent<Image>();
            border.color = occupied ? PanelBorder : EmptySlotBorder;
            border.raycastTarget = false;
            EnsureLobbyPanelShadow(slot.gameObject);

            Image fill = CreateUiImage("Fill", slot, stretch: true);
            fill.color = occupied ? CardFill : EmptySlotFill;
            fill.raycastTarget = false;
            fill.rectTransform.offsetMin = new Vector2(3f, 3f);
            fill.rectTransform.offsetMax = new Vector2(-3f, -3f);

            // Host badge exists on every slot; visible only as design mock on slot 0.
            CreateHostBadge(slot, showHostBadge);

            // Kick exists on every non-host slot; Host preview shows on occupied guests only.
            if (index != 0)
            {
                CreateKickButton(slot, showKickButton);
            }

            if (!occupied)
            {
                CreateEmptySlotContent(slot);
                return;
            }

            CreateCircularAvatar(slot, out RectTransform avatarRt);

            Image onlineDot = CreateUiImage("OnlineIndicator", avatarRt, stretch: false);
            onlineDot.sprite = GetBuiltinKnob();
            onlineDot.color = online ? OnlineGreen : ReadyMuted;
            onlineDot.raycastTarget = false;
            onlineDot.preserveAspect = true;
            RectTransform onlineRt = onlineDot.rectTransform;
            onlineRt.anchorMin = new Vector2(1f, 0f);
            onlineRt.anchorMax = new Vector2(1f, 0f);
            onlineRt.pivot = new Vector2(0.5f, 0.5f);
            onlineRt.anchoredPosition = new Vector2(-6f, 8f);
            onlineRt.sizeDelta = new Vector2(22f, 22f);
            onlineDot.transform.SetAsLastSibling();

            Text nameText = CreateUiText("PlayerName", slot, playerName, 30, FontStyle.Bold, TextPrimary, TextAnchor.MiddleLeft);
            RectTransform nameRt = nameText.rectTransform;
            nameRt.anchorMin = new Vector2(0f, 0.5f);
            nameRt.anchorMax = new Vector2(0f, 0.5f);
            nameRt.pivot = new Vector2(0f, 0.5f);
            nameRt.anchoredPosition = new Vector2(148f, 22f);
            nameRt.sizeDelta = new Vector2(300f, 42f);

            Image flag = CreateUiImage("CountryFlag", slot, stretch: false);
            flag.color = flagColor;
            flag.raycastTarget = false;
            RectTransform flagRt = flag.rectTransform;
            flagRt.anchorMin = new Vector2(0f, 0.5f);
            flagRt.anchorMax = new Vector2(0f, 0.5f);
            flagRt.pivot = new Vector2(0f, 0.5f);
            flagRt.anchoredPosition = new Vector2(148f, -24f);
            flagRt.sizeDelta = new Vector2(40f, 26f);

            Text country = CreateUiText("CountryCode", slot, countryCode, 18, FontStyle.Bold, TextMuted, TextAnchor.MiddleLeft);
            RectTransform countryRt = country.rectTransform;
            countryRt.anchorMin = new Vector2(0f, 0.5f);
            countryRt.anchorMax = new Vector2(0f, 0.5f);
            countryRt.pivot = new Vector2(0f, 0.5f);
            countryRt.anchoredPosition = new Vector2(196f, -24f);
            countryRt.sizeDelta = new Vector2(80f, 28f);

            Image levelBadge = CreateUiImage("LevelBadge", slot, stretch: false);
            levelBadge.sprite = GetBuiltinKnob();
            levelBadge.color = Gold;
            levelBadge.raycastTarget = false;
            levelBadge.preserveAspect = true;
            RectTransform levelBadgeRt = levelBadge.rectTransform;
            levelBadgeRt.anchorMin = new Vector2(0.5f, 0.5f);
            levelBadgeRt.anchorMax = new Vector2(0.5f, 0.5f);
            levelBadgeRt.pivot = new Vector2(0.5f, 0.5f);
            levelBadgeRt.anchoredPosition = new Vector2(72f, 0f);
            levelBadgeRt.sizeDelta = new Vector2(78f, 78f);

            Text levelText = CreateUiText("LevelText", levelBadgeRt, "Lv " + level, 16, FontStyle.Bold, GoldButtonLabel, TextAnchor.MiddleCenter);
            RectTransform levelTextRt = levelText.rectTransform;
            levelTextRt.anchorMin = Vector2.zero;
            levelTextRt.anchorMax = Vector2.one;
            levelTextRt.offsetMin = Vector2.zero;
            levelTextRt.offsetMax = Vector2.zero;

            ResolveReadyVisual(readyVisual, out Color readyColor, out string readyLabelText);

            Image readyDot = CreateUiImage("ReadyStatus", slot, stretch: false);
            readyDot.sprite = GetBuiltinKnob();
            readyDot.color = readyColor;
            readyDot.raycastTarget = false;
            readyDot.preserveAspect = true;
            RectTransform readyDotRt = readyDot.rectTransform;
            readyDotRt.anchorMin = new Vector2(1f, 0.5f);
            readyDotRt.anchorMax = new Vector2(1f, 0.5f);
            readyDotRt.pivot = new Vector2(1f, 0.5f);
            readyDotRt.anchoredPosition = new Vector2(-200f, 14f);
            readyDotRt.sizeDelta = new Vector2(24f, 24f);

            Text readyLabel = CreateUiText(
                "ReadyLabel",
                slot,
                readyLabelText,
                22,
                FontStyle.Bold,
                readyColor,
                TextAnchor.MiddleRight);
            RectTransform readyLabelRt = readyLabel.rectTransform;
            readyLabelRt.anchorMin = new Vector2(1f, 0.5f);
            readyLabelRt.anchorMax = new Vector2(1f, 0.5f);
            readyLabelRt.pivot = new Vector2(1f, 0.5f);
            readyLabelRt.anchoredPosition = new Vector2(-40f, -16f);
            readyLabelRt.sizeDelta = new Vector2(180f, 34f);
        }

        private static void CreateEmptySlotContent(RectTransform slot)
        {
            Image plusRing = CreateUiImage("EmptyPlusRing", slot, stretch: false);
            plusRing.sprite = GetBuiltinKnob();
            plusRing.color = new Color(GoldBright.r, GoldBright.g, GoldBright.b, 0.28f);
            plusRing.raycastTarget = false;
            plusRing.preserveAspect = true;
            RectTransform plusRingRt = plusRing.rectTransform;
            plusRingRt.anchorMin = new Vector2(0.5f, 0.5f);
            plusRingRt.anchorMax = new Vector2(0.5f, 0.5f);
            plusRingRt.pivot = new Vector2(0.5f, 0.5f);
            plusRingRt.anchoredPosition = new Vector2(-210f, 0f);
            plusRingRt.sizeDelta = new Vector2(72f, 72f);

            Text plusMark = CreateUiText("EmptyPlusMark", plusRingRt, "+", 40, FontStyle.Bold, GoldBright, TextAnchor.MiddleCenter);
            RectTransform plusMarkRt = plusMark.rectTransform;
            plusMarkRt.anchorMin = Vector2.zero;
            plusMarkRt.anchorMax = Vector2.one;
            plusMarkRt.offsetMin = Vector2.zero;
            plusMarkRt.offsetMax = Vector2.zero;

            Text empty = CreateUiText(
                "EmptySlotLabel",
                slot,
                "+ Waiting for Player",
                28,
                FontStyle.Bold,
                new Color(TextMuted.r, TextMuted.g, TextMuted.b, 0.92f),
                TextAnchor.MiddleLeft);
            RectTransform emptyRt = empty.rectTransform;
            emptyRt.anchorMin = new Vector2(0.5f, 0.5f);
            emptyRt.anchorMax = new Vector2(0.5f, 0.5f);
            emptyRt.pivot = new Vector2(0f, 0.5f);
            emptyRt.anchoredPosition = new Vector2(-150f, 0f);
            emptyRt.sizeDelta = new Vector2(520f, 48f);
        }

        private static void CreateCircularAvatar(RectTransform slot, out RectTransform avatarRt)
        {
            Image avatarFrame = CreateUiImage("Avatar", slot, stretch: false);
            avatarFrame.sprite = GetBuiltinKnob();
            avatarFrame.color = GoldBright;
            avatarFrame.raycastTarget = false;
            avatarFrame.preserveAspect = true;
            avatarRt = avatarFrame.rectTransform;
            avatarRt.anchorMin = new Vector2(0f, 0.5f);
            avatarRt.anchorMax = new Vector2(0f, 0.5f);
            avatarRt.pivot = new Vector2(0.5f, 0.5f);
            avatarRt.anchoredPosition = new Vector2(78f, 0f);
            avatarRt.sizeDelta = new Vector2(96f, 96f);

            Mask mask = avatarFrame.gameObject.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            Image avatarImage = CreateUiImage("AvatarImage", avatarRt, stretch: true);
            avatarImage.sprite = GetBuiltinKnob();
            avatarImage.color = AvatarTone;
            avatarImage.raycastTarget = false;
            avatarImage.preserveAspect = true;
            avatarImage.rectTransform.offsetMin = new Vector2(6f, 6f);
            avatarImage.rectTransform.offsetMax = new Vector2(-6f, -6f);
        }

        private static void CreateHostBadge(RectTransform slot, bool visible)
        {
            // Premium gold HOST badge — placed beside the room host name row.
            Image badge = CreateUiImage("HostBadge", slot, stretch: false);
            badge.color = GoldBright;
            badge.raycastTarget = false;
            RectTransform badgeRt = badge.rectTransform;
            badgeRt.anchorMin = new Vector2(0f, 0.5f);
            badgeRt.anchorMax = new Vector2(0f, 0.5f);
            badgeRt.pivot = new Vector2(0f, 0.5f);
            badgeRt.anchoredPosition = new Vector2(460f, 22f);
            badgeRt.sizeDelta = new Vector2(96f, 34f);

            Image badgeInner = CreateUiImage("HostBadgeInner", badgeRt, stretch: true);
            badgeInner.color = Gold;
            badgeInner.raycastTarget = false;
            badgeInner.rectTransform.offsetMin = new Vector2(2f, 2f);
            badgeInner.rectTransform.offsetMax = new Vector2(-2f, -2f);

            Text label = CreateUiText("HostLabel", badgeRt, "HOST", 16, FontStyle.Bold, GoldButtonLabel, TextAnchor.MiddleCenter);
            RectTransform labelRt = label.rectTransform;
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = Vector2.zero;
            labelRt.offsetMax = Vector2.zero;
            label.transform.SetAsLastSibling();

            badge.gameObject.SetActive(visible);
        }

        /// <summary>
        /// Small Kick placeholder on non-host slots. No kick logic — Host preview
        /// mock activates on occupied guest slots only.
        /// </summary>
        private static void CreateKickButton(RectTransform slot, bool visible)
        {
            Button kick = CreateLabeledButton(
                "KickButton",
                slot,
                "Kick",
                88f,
                40f,
                new Color(0.42f, 0.14f, 0.12f, 0.92f),
                new Color(1f, 0.82f, 0.78f, 1f));
            RectTransform kickRt = kick.GetComponent<RectTransform>();
            kickRt.anchorMin = new Vector2(1f, 0.5f);
            kickRt.anchorMax = new Vector2(1f, 0.5f);
            kickRt.pivot = new Vector2(1f, 0.5f);
            kickRt.anchoredPosition = new Vector2(-36f, 28f);
            kick.transition = Selectable.Transition.None;
            kick.interactable = false;
            Text kickLabel = kick.GetComponentInChildren<Text>();
            if (kickLabel != null)
            {
                kickLabel.fontSize = 18;
            }

            kick.gameObject.SetActive(visible);
        }

        private static void ResolveReadyVisual(SlotReadyVisual visual, out Color color, out string label)
        {
            switch (visual)
            {
                case SlotReadyVisual.Ready:
                    color = SuccessGreen;
                    label = "Ready";
                    break;
                case SlotReadyVisual.Connecting:
                    color = ConnectingAmber;
                    label = "Connecting";
                    break;
                default:
                    color = ReadyMuted;
                    label = "Not Ready";
                    break;
            }
        }

        private static void EnsureBuildSettings(List<string> failures)
        {
            InsertSceneAfter(PlayMenuScenePath, MainMenuScenePath, failures);
            InsertSceneAfter(QuickPlayScenePath, PlayMenuScenePath, failures);
            InsertSceneAfter(InviteFriendsScenePath, QuickPlayScenePath, failures);
            InsertSceneAfter(LobbyScreenScenePath, InviteFriendsScenePath, failures);
            InsertSceneAfter(MapVotingScenePath, LobbyScreenScenePath, failures);
        }

        private static void InsertSceneAfter(string scenePath, string afterPath, List<string> failures)
        {
            string sceneGuid = AssetDatabase.AssetPathToGUID(scenePath);
            if (string.IsNullOrEmpty(sceneGuid))
            {
                failures.Add(scenePath + " GUID missing after save.");
                return;
            }

            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int existing = scenes.FindIndex(s => s.path == scenePath);
            if (existing >= 0)
            {
                scenes.RemoveAt(existing);
            }

            int afterIndex = scenes.FindIndex(s => s.path == afterPath);
            var entry = new EditorBuildSettingsScene(scenePath, true);
            if (afterIndex >= 0)
            {
                scenes.Insert(afterIndex + 1, entry);
            }
            else
            {
                scenes.Add(entry);
            }

            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("[PlayFlow] EditorBuildSettings inserted " + scenePath + " after " + afterPath);
        }

        private static void ValidateAll(List<string> failures)
        {
            ValidatePlayMenu(failures);
            ValidateQuickPlay(failures);
            ValidateInviteFriends(failures);
            ValidateLobbyScreen(failures);
            ValidateMainMenuWiring(failures);

            if (CoreSceneManager.PlayMenuSceneName != "PlayMenu")
            {
                failures.Add("SceneManager.PlayMenuSceneName mismatch.");
            }

            if (CoreSceneManager.QuickPlaySceneName != "QuickPlay")
            {
                failures.Add("SceneManager.QuickPlaySceneName mismatch.");
            }

            if (CoreSceneManager.InviteFriendsSceneName != "InviteFriends")
            {
                failures.Add("SceneManager.InviteFriendsSceneName mismatch.");
            }

            if (CoreSceneManager.LobbyScreenSceneName != "LobbyScreen")
            {
                failures.Add("SceneManager.LobbyScreenSceneName mismatch.");
            }

            if (CoreSceneManager.LobbySceneName != "Lobby")
            {
                failures.Add("SceneManager.LobbySceneName must remain Lobby (pre-race).");
            }
        }

        private static void ValidatePlayMenu(List<string> failures)
        {
            Scene scene = EditorSceneManager.OpenScene(PlayMenuScenePath, OpenSceneMode.Single);
            Require(FindDeep(scene, "PlayMenuCanvas"), "PlayMenuCanvas", failures);
            Require(FindDeep(scene, "Background"), "PlayMenu Background", failures);
            Require(FindDeep(scene, "SafeArea"), "PlayMenu SafeArea", failures);
            Require(FindDeep(scene, "BackButton"), "PlayMenu BackButton", failures);
            Require(FindDeep(scene, "CardsRoot"), "CardsRoot", failures);
            GameObject quickPlayCard = FindDeep(scene, "QuickPlayCard");
            GameObject inviteFriendsCard = FindDeep(scene, "InviteFriendsCard");
            Require(quickPlayCard, "QuickPlayCard", failures);
            Require(inviteFriendsCard, "InviteFriendsCard", failures);
            Require(quickPlayCard != null ? FindChildRecursive(quickPlayCard.transform, "Icon")?.gameObject : null, "QuickPlayCard Icon", failures);
            Require(inviteFriendsCard != null ? FindChildRecursive(inviteFriendsCard.transform, "Icon")?.gameObject : null, "InviteFriendsCard Icon", failures);

            Text qpText = quickPlayCard != null
                ? FindChildRecursive(quickPlayCard.transform, "Description")?.GetComponent<Text>()
                : null;
            Text ifText = inviteFriendsCard != null
                ? FindChildRecursive(inviteFriendsCard.transform, "Description")?.GetComponent<Text>()
                : null;
            if (qpText == null || qpText.text != QuickPlaySubtitle)
            {
                failures.Add("QuickPlayCard subtitle mismatch.");
            }

            if (ifText == null || ifText.text != InviteFriendsSubtitle)
            {
                failures.Add("InviteFriendsCard subtitle mismatch.");
            }

            RequireCanvasScaler(FindDeep(scene, "PlayMenuCanvas"), failures);
            RequireInBuild(PlayMenuScenePath, failures);
            RequireBackgroundGuid(FindDeep(scene, "Background"), failures);
        }

        private static void ValidateQuickPlay(List<string> failures)
        {
            Scene scene = EditorSceneManager.OpenScene(QuickPlayScenePath, OpenSceneMode.Single);
            Require(FindDeep(scene, "QuickPlayCanvas"), "QuickPlayCanvas", failures);
            Require(FindDeep(scene, "Background"), "QuickPlay Background", failures);
            Require(FindDeep(scene, "BackButton"), "QuickPlay BackButton", failures);
            Require(FindDeep(scene, "CancelButton"), "CancelButton", failures);
            Require(FindDeep(scene, "StatusText"), "StatusText", failures);
            Require(FindDeep(scene, "SearchingSpinner"), "SearchingSpinner", failures);
            Require(FindDeep(scene, "PlayersFoundLabel"), "PlayersFoundLabel", failures);
            Require(FindDeep(scene, "JoiningRoomLabel"), "JoiningRoomLabel", failures);
            Require(FindDeep(scene, "CreatingRoomLabel"), "CreatingRoomLabel", failures);
            Require(FindDeep(scene, "WaitingForPlayersLabel"), "WaitingForPlayersLabel", failures);
            RequireCanvasScaler(FindDeep(scene, "QuickPlayCanvas"), failures);
            RequireInBuild(QuickPlayScenePath, failures);
            RequireBackgroundGuid(FindDeep(scene, "Background"), failures);
        }

        private static void ValidateInviteFriends(List<string> failures)
        {
            Scene scene = EditorSceneManager.OpenScene(InviteFriendsScenePath, OpenSceneMode.Single);
            Require(FindDeep(scene, "InviteFriendsCanvas"), "InviteFriendsCanvas", failures);
            Require(FindDeep(scene, "Background"), "InviteFriends Background", failures);
            Require(FindDeep(scene, "BackButton"), "InviteFriends BackButton", failures);
            Require(FindDeep(scene, "FriendsListCard"), "FriendsListCard", failures);
            Require(FindDeep(scene, "PlayerIdCard"), "PlayerIdCard", failures);
            Require(FindDeep(scene, "WhatsAppCard"), "WhatsAppCard", failures);
            Require(FindDeep(scene, "PlayerIdInput"), "PlayerIdInput", failures);
            Require(FindDeep(scene, "SendInvitationButton"), "SendInvitationButton", failures);
            Require(FindDeep(scene, "CopyLinkButton"), "CopyLinkButton", failures);
            Require(FindDeep(scene, "ShareWhatsAppButton"), "ShareWhatsAppButton", failures);
            Require(FindDeep(scene, "FriendRow_0"), "FriendRow_0", failures);
            RequireCanvasScaler(FindDeep(scene, "InviteFriendsCanvas"), failures);
            RequireInBuild(InviteFriendsScenePath, failures);
            RequireBackgroundGuid(FindDeep(scene, "Background"), failures);
        }

        private static void ValidateLobbyScreen(List<string> failures)
        {
            Scene scene = EditorSceneManager.OpenScene(LobbyScreenScenePath, OpenSceneMode.Single);
            Require(FindDeep(scene, "LobbyScreenCanvas"), "LobbyScreenCanvas", failures);
            Require(FindDeep(scene, "Background"), "LobbyScreen Background", failures);
            Require(FindDeep(scene, "SafeArea"), "LobbyScreen SafeArea", failures);
            Require(FindDeep(scene, "BackButton"), "LobbyScreen BackButton", failures);
            Require(FindDeep(scene, "HeaderRoot"), "HeaderRoot", failures);
            Require(FindDeep(scene, "RoomTypeText"), "RoomTypeText", failures);
            Require(FindDeep(scene, "HostNameText"), "HostNameText", failures);
            Require(FindDeep(scene, "PlayerCountText"), "PlayerCountText", failures);
            Require(FindDeep(scene, "RoomCodeText"), "RoomCodeText", failures);
            Require(FindDeep(scene, "SlotsRoot"), "SlotsRoot", failures);
            Require(FindDeep(scene, "PlayerSlot_0"), "PlayerSlot_0", failures);
            Require(FindDeep(scene, "PlayerSlot_1"), "PlayerSlot_1", failures);
            Require(FindDeep(scene, "PlayerSlot_2"), "PlayerSlot_2", failures);
            Require(FindDeep(scene, "PlayerSlot_3"), "PlayerSlot_3", failures);
            Require(FindDeep(scene, "FooterRoot"), "FooterRoot", failures);
            Require(FindDeep(scene, "ReadyButton"), "ReadyButton", failures);
            Require(FindDeep(scene, "PlayButton"), "PlayButton", failures);
            Require(FindDeep(scene, "PlayLabel_StartMatch"), "PlayLabel_StartMatch", failures);
            Require(FindDeep(scene, "StatusRoot"), "StatusRoot", failures);
            Require(FindDeep(scene, "LobbyStatusPanel"), "LobbyStatusPanel", failures);
            Require(FindDeep(scene, "LobbyStatusText"), "LobbyStatusText", failures);
            Require(FindDeep(scene, "StatusMsg_WaitingForPlayers"), "StatusMsg_WaitingForPlayers", failures);
            Require(FindDeep(scene, "StatusMsg_PlayersJoining"), "StatusMsg_PlayersJoining", failures);
            Require(FindDeep(scene, "StatusMsg_WaitingReady"), "StatusMsg_WaitingReady", failures);
            Require(FindDeep(scene, "StatusMsg_ReadyToStart"), "StatusMsg_ReadyToStart", failures);
            Require(FindDeep(scene, "CountdownPlaceholder"), "CountdownPlaceholder", failures);
            Require(FindDeep(scene, "MessageFooterRoot"), "MessageFooterRoot", failures);
            Require(FindDeep(scene, "SystemMsg_PlayerJoined"), "SystemMsg_PlayerJoined", failures);
            Require(FindDeep(scene, "SystemMsg_PlayerLeft"), "SystemMsg_PlayerLeft", failures);
            Require(FindDeep(scene, "SystemMsg_HostChanged"), "SystemMsg_HostChanged", failures);
            Require(FindDeep(scene, "SystemMsg_Searching"), "SystemMsg_Searching", failures);

            GameObject occupied = FindDeep(scene, "PlayerSlot_0");
            Require(occupied != null ? FindChildRecursive(occupied.transform, "Avatar")?.gameObject : null, "PlayerSlot_0 Avatar", failures);
            Require(occupied != null ? FindChildRecursive(occupied.transform, "AvatarImage")?.gameObject : null, "PlayerSlot_0 AvatarImage", failures);
            Require(occupied != null ? FindChildRecursive(occupied.transform, "PlayerName")?.gameObject : null, "PlayerSlot_0 PlayerName", failures);
            Require(occupied != null ? FindChildRecursive(occupied.transform, "CountryFlag")?.gameObject : null, "PlayerSlot_0 CountryFlag", failures);
            Require(occupied != null ? FindChildRecursive(occupied.transform, "LevelBadge")?.gameObject : null, "PlayerSlot_0 LevelBadge", failures);
            Require(occupied != null ? FindChildRecursive(occupied.transform, "ReadyStatus")?.gameObject : null, "PlayerSlot_0 ReadyStatus", failures);
            Require(occupied != null ? FindChildRecursive(occupied.transform, "OnlineIndicator")?.gameObject : null, "PlayerSlot_0 OnlineIndicator", failures);
            Require(occupied != null ? FindChildRecursive(occupied.transform, "HostBadge")?.gameObject : null, "PlayerSlot_0 HostBadge", failures);

            GameObject hostBadge = occupied != null ? FindChildRecursive(occupied.transform, "HostBadge")?.gameObject : null;
            if (hostBadge == null || !hostBadge.activeSelf)
            {
                failures.Add("PlayerSlot_0 HostBadge must be active (design mock).");
            }

            GameObject slot0Kick = occupied != null ? FindChildRecursive(occupied.transform, "KickButton")?.gameObject : null;
            if (slot0Kick != null)
            {
                failures.Add("PlayerSlot_0 must not have KickButton (host slot).");
            }

            if (occupied != null && occupied.GetComponentInChildren<Mask>(true) == null)
            {
                failures.Add("PlayerSlot_0 Avatar must use a Mask for circular clip.");
            }

            GameObject slot1 = FindDeep(scene, "PlayerSlot_1");
            Require(slot1 != null ? FindChildRecursive(slot1.transform, "PlayerName")?.gameObject : null, "PlayerSlot_1 PlayerName", failures);
            Require(slot1 != null ? FindChildRecursive(slot1.transform, "ReadyLabel")?.gameObject : null, "PlayerSlot_1 ReadyLabel", failures);
            GameObject slot1Host = slot1 != null ? FindChildRecursive(slot1.transform, "HostBadge")?.gameObject : null;
            if (slot1Host == null || slot1Host.activeSelf)
            {
                failures.Add("PlayerSlot_1 HostBadge must exist and be inactive.");
            }

            GameObject slot1Kick = slot1 != null ? FindChildRecursive(slot1.transform, "KickButton")?.gameObject : null;
            if (slot1Kick == null || !slot1Kick.activeSelf)
            {
                failures.Add("PlayerSlot_1 KickButton must exist and be active (Host preview mock).");
            }

            Text slot1Ready = slot1 != null ? FindChildRecursive(slot1.transform, "ReadyLabel")?.GetComponent<Text>() : null;
            if (slot1Ready == null || slot1Ready.text != "Not Ready")
            {
                failures.Add("PlayerSlot_1 ReadyLabel expected 'Not Ready' (visual mock).");
            }

            GameObject slot2 = FindDeep(scene, "PlayerSlot_2");
            Text slot2Ready = slot2 != null ? FindChildRecursive(slot2.transform, "ReadyLabel")?.GetComponent<Text>() : null;
            if (slot2Ready == null || slot2Ready.text != "Connecting")
            {
                failures.Add("PlayerSlot_2 ReadyLabel expected 'Connecting' (visual mock).");
            }

            GameObject slot2Kick = slot2 != null ? FindChildRecursive(slot2.transform, "KickButton")?.gameObject : null;
            if (slot2Kick == null || !slot2Kick.activeSelf)
            {
                failures.Add("PlayerSlot_2 KickButton must exist and be active (Host preview mock).");
            }

            GameObject empty = FindDeep(scene, "PlayerSlot_3");
            Require(empty != null ? FindChildRecursive(empty.transform, "EmptySlotLabel")?.gameObject : null, "PlayerSlot_3 EmptySlotLabel", failures);
            Text emptyLabel = empty != null ? FindChildRecursive(empty.transform, "EmptySlotLabel")?.GetComponent<Text>() : null;
            if (emptyLabel == null || emptyLabel.text != "+ Waiting for Player")
            {
                failures.Add("PlayerSlot_3 EmptySlotLabel expected '+ Waiting for Player'.");
            }

            GameObject emptyHost = empty != null ? FindChildRecursive(empty.transform, "HostBadge")?.gameObject : null;
            if (emptyHost == null || emptyHost.activeSelf)
            {
                failures.Add("PlayerSlot_3 HostBadge must exist and be inactive.");
            }

            GameObject emptyKick = empty != null ? FindChildRecursive(empty.transform, "KickButton")?.gameObject : null;
            if (emptyKick == null || emptyKick.activeSelf)
            {
                failures.Add("PlayerSlot_3 KickButton must exist and be inactive.");
            }

            GameObject playButtonGo = FindDeep(scene, "PlayButton");
            Button playButton = playButtonGo != null ? playButtonGo.GetComponent<Button>() : null;
            if (playButton == null || playButton.interactable)
            {
                failures.Add("PlayButton must exist and be disabled (Waiting for Players...).");
            }

            Text playLabel = playButtonGo != null
                ? FindChildRecursive(playButtonGo.transform, "Label")?.GetComponent<Text>()
                : null;
            if (playLabel == null || playLabel.text != "Waiting for Players...")
            {
                failures.Add("PlayButton label expected 'Waiting for Players...'.");
            }

            RectTransform playRt = playButtonGo != null ? playButtonGo.GetComponent<RectTransform>() : null;
            GameObject readyButtonGo = FindDeep(scene, "ReadyButton");
            RectTransform readyRt = readyButtonGo != null ? readyButtonGo.GetComponent<RectTransform>() : null;
            if (readyRt == null || playRt == null)
            {
                failures.Add("ReadyButton/PlayButton RectTransforms missing.");
            }
            else
            {
                if (Mathf.Abs(readyRt.sizeDelta.x - playRt.sizeDelta.x) > 0.5f
                    || Mathf.Abs(readyRt.sizeDelta.y - playRt.sizeDelta.y) > 0.5f)
                {
                    failures.Add("ReadyButton and PlayButton must share identical sizeDelta (Sprint 21.5).");
                }

                if (readyRt.sizeDelta.y < 100f || readyRt.sizeDelta.y > 110f
                    || readyRt.sizeDelta.x < 380f)
                {
                    failures.Add("Ready/Play expected ~400x104 touch targets (100–110 height, >=380 width).");
                }
            }

            GameObject safeAreaGo = FindDeep(scene, "SafeArea");
            RectTransform safeRt = safeAreaGo != null ? safeAreaGo.GetComponent<RectTransform>() : null;
            if (safeRt == null
                || Mathf.Abs(safeRt.sizeDelta.x + 96f) > 0.5f
                || Mathf.Abs(safeRt.sizeDelta.y + 86f) > 0.5f
                || Mathf.Abs(safeRt.anchoredPosition.y + 9f) > 0.5f)
            {
                failures.Add("SafeArea expected Main Menu insets (sizeDelta -96/-86, y -9).");
            }

            Vector2 expectedSlot = new Vector2(960f, 148f);
            for (int i = 0; i < 4; i++)
            {
                RectTransform slotRt = FindDeep(scene, "PlayerSlot_" + i)?.GetComponent<RectTransform>();
                if (slotRt == null
                    || Mathf.Abs(slotRt.sizeDelta.x - expectedSlot.x) > 0.5f
                    || Mathf.Abs(slotRt.sizeDelta.y - expectedSlot.y) > 0.5f)
                {
                    failures.Add("PlayerSlot_" + i + " expected identical sizeDelta 960x148.");
                }
            }

            GameObject preparedLabelGo = FindDeep(scene, "PlayLabel_StartMatch");
            if (preparedLabelGo == null || preparedLabelGo.activeSelf)
            {
                failures.Add("PlayLabel_StartMatch must exist and be inactive by default.");
            }

            GameObject countdownGo = FindDeep(scene, "CountdownPlaceholder");
            if (countdownGo == null || countdownGo.activeSelf)
            {
                failures.Add("CountdownPlaceholder must exist and be inactive by default.");
            }

            Text countdown = countdownGo != null ? countdownGo.GetComponent<Text>() : null;
            if (countdown == null || countdown.text != "Starting in: 00:10")
            {
                failures.Add("CountdownPlaceholder expected 'Starting in: 00:10'.");
            }

            Text lobbyStatus = FindDeep(scene, "LobbyStatusText")?.GetComponent<Text>();
            if (lobbyStatus == null || lobbyStatus.text != "Waiting for everyone to be Ready...")
            {
                failures.Add("LobbyStatusText expected 'Waiting for everyone to be Ready...'.");
            }

            GameObject joiningMsg = FindDeep(scene, "StatusMsg_PlayersJoining");
            if (joiningMsg == null || joiningMsg.activeSelf)
            {
                failures.Add("StatusMsg_PlayersJoining must exist and be inactive (example copy).");
            }

            GameObject systemJoined = FindDeep(scene, "SystemMsg_PlayerJoined");
            if (systemJoined == null || !systemJoined.activeSelf)
            {
                failures.Add("SystemMsg_PlayerJoined must exist and be active (sample footer message).");
            }

            GameObject systemLeft = FindDeep(scene, "SystemMsg_PlayerLeft");
            if (systemLeft == null || systemLeft.activeSelf)
            {
                failures.Add("SystemMsg_PlayerLeft must exist and be inactive.");
            }

            Text roomType = FindDeep(scene, "RoomTypeText")?.GetComponent<Text>();
            if (roomType == null || roomType.text != "Room Type: Public")
            {
                failures.Add("RoomTypeText expected 'Room Type: Public'.");
            }

            Text hostName = FindDeep(scene, "HostNameText")?.GetComponent<Text>();
            if (hostName == null || hostName.text != "Host: DesertFox")
            {
                failures.Add("HostNameText expected 'Host: DesertFox'.");
            }

            Text playerCount = FindDeep(scene, "PlayerCountText")?.GetComponent<Text>();
            if (playerCount == null || playerCount.text != "Players: 1 / 4")
            {
                failures.Add("PlayerCountText expected 'Players: 1 / 4'.");
            }

            Text roomCode = FindDeep(scene, "RoomCodeText")?.GetComponent<Text>();
            if (roomCode == null || roomCode.text != "GULF-4821")
            {
                failures.Add("RoomCodeText expected 'GULF-4821'.");
            }

            GameObject readyButtonGo = FindDeep(scene, "ReadyButton");
            RectTransform readyRt = readyButtonGo != null ? readyButtonGo.GetComponent<RectTransform>() : null;
            if (readyRt == null || readyRt.sizeDelta.x < 320f || readyRt.sizeDelta.y < 90f)
            {
                failures.Add("ReadyButton expected large premium size (>= 320x90).");
            }

            LobbyScreenController controller = FindDeep(scene, "LobbyScreenCanvas")?.GetComponent<LobbyScreenController>();
            if (controller == null)
            {
                failures.Add("LobbyScreenCanvas missing LobbyScreenController.");
            }
            else
            {
                SerializedObject so = new SerializedObject(controller);
                if (so.FindProperty("readyButton").objectReferenceValue == null)
                {
                    failures.Add("LobbyScreenController.readyButton must be wired.");
                }

                if (so.FindProperty("readyButtonLabel").objectReferenceValue == null)
                {
                    failures.Add("LobbyScreenController.readyButtonLabel must be wired.");
                }

                if (so.FindProperty("playButton").objectReferenceValue == null)
                {
                    failures.Add("LobbyScreenController.playButton must be wired.");
                }

                if (so.FindProperty("playButtonLabel").objectReferenceValue == null)
                {
                    failures.Add("LobbyScreenController.playButtonLabel must be wired.");
                }
            }

            RequireCanvasScaler(FindDeep(scene, "LobbyScreenCanvas"), failures);
            RequireInBuild(LobbyScreenScenePath, failures);
            RequireBackgroundGuid(FindDeep(scene, "Background"), failures);

            // Pre-race Lobby scene must remain untouched in build list.
            RequireInBuild("Assets/_Project/Scenes/Lobby.unity", failures);
        }

        private static void ValidateMapVotingScreen(List<string> failures)
        {
            Scene scene = EditorSceneManager.OpenScene(MapVotingScenePath, OpenSceneMode.Single);
            Require(FindDeep(scene, "MapVotingCanvas"), "MapVotingCanvas", failures);
            Require(FindDeep(scene, "Background"), "MapVoting Background", failures);
            Require(FindDeep(scene, "SafeArea"), "MapVoting SafeArea", failures);
            Require(FindDeep(scene, "BackButton"), "MapVoting BackButton", failures);
            Require(FindDeep(scene, "TimerPanel"), "MapVoting TimerPanel", failures);
            Require(FindDeep(scene, "TimerText"), "MapVoting TimerText", failures);
            Require(FindDeep(scene, "ProgressBarTrack"), "MapVoting ProgressBarTrack", failures);
            Require(FindDeep(scene, "ProgressBarFill"), "MapVoting ProgressBarFill", failures);
            Require(FindDeep(scene, "StatusPanel"), "MapVoting StatusPanel", failures);
            Require(FindDeep(scene, "StatusText"), "MapVoting StatusText", failures);
            Require(FindDeep(scene, "StatusMsg_WaitingForPlayers"), "MapVoting StatusMsg_WaitingForPlayers", failures);
            Require(FindDeep(scene, "StatusMsg_PlayersVoting"), "MapVoting StatusMsg_PlayersVoting", failures);
            Require(FindDeep(scene, "StatusMsg_FinalizingResults"), "MapVoting StatusMsg_FinalizingResults", failures);
            Require(FindDeep(scene, "HeaderRoot"), "MapVoting HeaderRoot", failures);
            Require(FindDeep(scene, "TitleText"), "MapVoting TitleText", failures);
            Require(FindDeep(scene, "SubtitleText"), "MapVoting SubtitleText", failures);
            Require(FindDeep(scene, "CardsRoot"), "MapVoting CardsRoot", failures);
            Require(FindDeep(scene, "MapCard_0"), "MapCard_0", failures);
            Require(FindDeep(scene, "MapCard_1"), "MapCard_1", failures);
            Require(FindDeep(scene, "MapCard_2"), "MapCard_2", failures);
            Require(FindDeep(scene, "FooterRoot"), "MapVoting FooterRoot", failures);
            Require(FindDeep(scene, "StatsPanel"), "MapVoting StatsPanel", failures);
            Require(FindDeep(scene, "PlayersVotedText"), "MapVoting PlayersVotedText", failures);
            Require(FindDeep(scene, "RemainingVotesText"), "MapVoting RemainingVotesText", failures);
            Require(FindDeep(scene, "TotalVotesText"), "MapVoting TotalVotesText", failures);
            Require(FindDeep(scene, "VoteConfirmation"), "MapVoting VoteConfirmation", failures);
            Require(FindDeep(scene, "ConfirmationText"), "MapVoting ConfirmationText", failures);

            Text title = FindDeep(scene, "TitleText")?.GetComponent<Text>();
            if (title == null || title.text != "Choose Your Map")
            {
                failures.Add("MapVoting TitleText must read 'Choose Your Map'.");
            }

            Text subtitle = FindDeep(scene, "SubtitleText")?.GetComponent<Text>();
            if (subtitle == null || subtitle.text != "Vote together to decide the next destination.")
            {
                failures.Add("MapVoting SubtitleText copy mismatch.");
            }

            Text timer = FindDeep(scene, "TimerText")?.GetComponent<Text>();
            if (timer == null || timer.text != "20s")
            {
                failures.Add("MapVoting TimerText must read '20s'.");
            }

            Text status = FindDeep(scene, "StatusText")?.GetComponent<Text>();
            if (status == null || status.text != "Players are voting...")
            {
                failures.Add("MapVoting StatusText must read 'Players are voting...'.");
            }

            Text playersVoted = FindDeep(scene, "PlayersVotedText")?.GetComponent<Text>();
            if (playersVoted == null || playersVoted.text != "Players Voted 0/4")
            {
                failures.Add("MapVoting PlayersVotedText must read 'Players Voted 0/4'.");
            }

            Text remainingVotes = FindDeep(scene, "RemainingVotesText")?.GetComponent<Text>();
            if (remainingVotes == null || remainingVotes.text != "Remaining Votes 4")
            {
                failures.Add("MapVoting RemainingVotesText must read 'Remaining Votes 4'.");
            }

            Text totalVotes = FindDeep(scene, "TotalVotesText")?.GetComponent<Text>();
            if (totalVotes == null || totalVotes.text != "Total Votes 0")
            {
                failures.Add("MapVoting TotalVotesText must read 'Total Votes 0'.");
            }

            Text confirmation = FindDeep(scene, "ConfirmationText")?.GetComponent<Text>();
            if (confirmation == null || confirmation.text != "✓ Your vote has been submitted.")
            {
                failures.Add("MapVoting ConfirmationText copy mismatch.");
            }

            GameObject voteConfirmation = FindDeep(scene, "VoteConfirmation");
            if (voteConfirmation != null && voteConfirmation.activeSelf)
            {
                failures.Add("MapVoting VoteConfirmation must start inactive.");
            }

            GameObject waitingMsg = FindDeep(scene, "StatusMsg_WaitingForPlayers");
            if (waitingMsg != null && waitingMsg.activeSelf)
            {
                failures.Add("MapVoting StatusMsg_WaitingForPlayers must start inactive.");
            }

            GameObject votingMsg = FindDeep(scene, "StatusMsg_PlayersVoting");
            if (votingMsg != null && votingMsg.activeSelf)
            {
                failures.Add("MapVoting StatusMsg_PlayersVoting must start inactive.");
            }

            GameObject finalizingMsg = FindDeep(scene, "StatusMsg_FinalizingResults");
            if (finalizingMsg != null && finalizingMsg.activeSelf)
            {
                failures.Add("MapVoting StatusMsg_FinalizingResults must start inactive.");
            }

            Image progressFill = FindDeep(scene, "ProgressBarFill")?.GetComponent<Image>();
            if (progressFill == null || progressFill.type != Image.Type.Filled)
            {
                failures.Add("MapVoting ProgressBarFill must use Image.Type.Filled placeholder.");
            }

            string[] expectedNames = { "Kuwait City", "Dubai Marina", "Muscat Coast" };
            string[] expectedCountries = { "KW", "AE", "OM" };
            string[] expectedDifficulties = { "Medium", "Hard", "Easy" };
            string[] expectedDurations = { "Est. 3:30", "Est. 4:15", "Est. 2:45" };
            for (int i = 0; i < 3; i++)
            {
                GameObject card = FindDeep(scene, "MapCard_" + i);
                if (card == null)
                {
                    continue;
                }

                Require(FindChildRecursive(card.transform, "MapPreview")?.gameObject, "MapCard_" + i + " MapPreview", failures);
                Require(FindChildRecursive(card.transform, "MapName")?.gameObject, "MapCard_" + i + " MapName", failures);
                Require(FindChildRecursive(card.transform, "CountryFlag")?.gameObject, "MapCard_" + i + " CountryFlag", failures);
                Require(FindChildRecursive(card.transform, "CountryCode")?.gameObject, "MapCard_" + i + " CountryCode", failures);
                Require(FindChildRecursive(card.transform, "DifficultyText")?.gameObject, "MapCard_" + i + " DifficultyText", failures);
                Require(FindChildRecursive(card.transform, "Description")?.gameObject, "MapCard_" + i + " Description", failures);
                Require(FindChildRecursive(card.transform, "DurationText")?.gameObject, "MapCard_" + i + " DurationText", failures);
                Require(FindChildRecursive(card.transform, "VoteButton")?.gameObject, "MapCard_" + i + " VoteButton", failures);
                Require(FindChildRecursive(card.transform, "SelectedCheckmark")?.gameObject, "MapCard_" + i + " SelectedCheckmark", failures);
                Require(FindChildRecursive(card.transform, "LockedRoot")?.gameObject, "MapCard_" + i + " LockedRoot", failures);

                if (card.GetComponent<MapCardVisual>() == null)
                {
                    failures.Add("MapCard_" + i + " missing MapCardVisual.");
                }

                Text mapName = FindChildRecursive(card.transform, "MapName")?.GetComponent<Text>();
                if (mapName == null || mapName.text != expectedNames[i])
                {
                    failures.Add("MapCard_" + i + " MapName expected '" + expectedNames[i] + "'.");
                }

                Text country = FindChildRecursive(card.transform, "CountryCode")?.GetComponent<Text>();
                if (country == null || country.text != expectedCountries[i])
                {
                    failures.Add("MapCard_" + i + " CountryCode expected '" + expectedCountries[i] + "'.");
                }

                Text difficulty = FindChildRecursive(card.transform, "DifficultyText")?.GetComponent<Text>();
                if (difficulty == null || difficulty.text != expectedDifficulties[i])
                {
                    failures.Add("MapCard_" + i + " DifficultyText expected '" + expectedDifficulties[i] + "'.");
                }

                Text duration = FindChildRecursive(card.transform, "DurationText")?.GetComponent<Text>();
                if (duration == null || duration.text != expectedDurations[i])
                {
                    failures.Add("MapCard_" + i + " DurationText expected '" + expectedDurations[i] + "'.");
                }

                GameObject checkmark = FindChildRecursive(card.transform, "SelectedCheckmark")?.gameObject;
                if (checkmark != null && checkmark.activeSelf)
                {
                    failures.Add("MapCard_" + i + " SelectedCheckmark must start inactive.");
                }

                GameObject locked = FindChildRecursive(card.transform, "LockedRoot")?.gameObject;
                if (locked != null && locked.activeSelf)
                {
                    failures.Add("MapCard_" + i + " LockedRoot must start inactive.");
                }
            }

            // Old OnGUI vote session must not be active on this UI-only scene.
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name == "MapVotingUI" && root.activeInHierarchy)
                {
                    failures.Add("Legacy MapVotingUI must not remain active on MapVoting scene.");
                }
            }

            GameObject safeAreaGo = FindDeep(scene, "SafeArea");
            if (safeAreaGo != null)
            {
                RectTransform safeRt = safeAreaGo.GetComponent<RectTransform>();
                if (safeRt == null ||
                    safeRt.sizeDelta != new Vector2(-96f, -86f) ||
                    !Mathf.Approximately(safeRt.anchoredPosition.y, -9f))
                {
                    failures.Add("MapVoting SafeArea expected Main Menu insets (sizeDelta -96/-86, y -9).");
                }
            }

            MapVotingScreenController controller = FindDeep(scene, "MapVotingCanvas")?.GetComponent<MapVotingScreenController>();
            if (controller == null)
            {
                failures.Add("MapVotingCanvas missing MapVotingScreenController.");
            }
            else
            {
                SerializedObject so = new SerializedObject(controller);
                if (so.FindProperty("backButton").objectReferenceValue == null)
                {
                    failures.Add("MapVotingScreenController.backButton must be wired.");
                }

                SerializedProperty voteButtons = so.FindProperty("voteButtons");
                if (voteButtons == null || voteButtons.arraySize != 3)
                {
                    failures.Add("MapVotingScreenController.voteButtons must have 3 entries.");
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (voteButtons.GetArrayElementAtIndex(i).objectReferenceValue == null)
                        {
                            failures.Add("MapVotingScreenController.voteButtons[" + i + "] must be wired.");
                        }
                    }
                }

                SerializedProperty checkmarks = so.FindProperty("selectedCheckmarks");
                if (checkmarks == null || checkmarks.arraySize != 3)
                {
                    failures.Add("MapVotingScreenController.selectedCheckmarks must have 3 entries.");
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (checkmarks.GetArrayElementAtIndex(i).objectReferenceValue == null)
                        {
                            failures.Add("MapVotingScreenController.selectedCheckmarks[" + i + "] must be wired.");
                        }
                    }
                }

                SerializedProperty visuals = so.FindProperty("cardVisuals");
                if (visuals == null || visuals.arraySize != 3)
                {
                    failures.Add("MapVotingScreenController.cardVisuals must have 3 entries.");
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (visuals.GetArrayElementAtIndex(i).objectReferenceValue == null)
                        {
                            failures.Add("MapVotingScreenController.cardVisuals[" + i + "] must be wired.");
                        }
                    }
                }
            }

            RequireCanvasScaler(FindDeep(scene, "MapVotingCanvas"), failures);
            RequireInBuild(MapVotingScenePath, failures);
            RequireBackgroundGuid(FindDeep(scene, "Background"), failures);
        }

        private static void ValidateMainMenuWiring(List<string> failures)
        {
            Scene mainMenu = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            GameObject play = FindDeep(mainMenu, "PlayButtonImage");
            if (play == null || play.GetComponent<MainMenuPlayButton>() == null || play.GetComponent<Button>() == null)
            {
                failures.Add("MainMenu PlayButtonImage missing Button/MainMenuPlayButton.");
            }

            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                failures.Add("MainMenu EventSystem missing (needed for Play click).");
            }
        }

        private static Sprite LoadBackground(List<string> failures)
        {
            Sprite background = LoadSprite(BackgroundGuid, "MainMenuBackground");
            if (background == null)
            {
                failures.Add("Background sprite GUID missing.");
            }

            return background;
        }

        private static GameObject CreateOverlayCanvas(string name)
        {
            GameObject canvasGo = new GameObject(name, typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            RectTransform canvasRt = canvasGo.GetComponent<RectTransform>();
            canvasRt.anchorMin = Vector2.zero;
            canvasRt.anchorMax = Vector2.one;
            canvasRt.offsetMin = Vector2.zero;
            canvasRt.offsetMax = Vector2.zero;
            return canvasGo;
        }

        private static RectTransform CreateSafeArea(RectTransform canvasRt)
        {
            RectTransform safeArea = CreateRect("SafeArea", canvasRt);
            safeArea.anchorMin = Vector2.zero;
            safeArea.anchorMax = Vector2.one;
            safeArea.anchoredPosition = new Vector2(0f, -9f);
            safeArea.sizeDelta = new Vector2(-96f, -86f);
            return safeArea;
        }

        private enum ModeCardIconStyle
        {
            Lightning,
            Friends
        }

        private static void PolishPlayMenuScene(List<string> failures)
        {
            if (!File.Exists(PlayMenuScenePath))
            {
                failures.Add("PlayMenu scene missing; run full Play Flow build first.");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(PlayMenuScenePath, OpenSceneMode.Single);
            PolishModeCard(FindDeep(scene, "QuickPlayCard"), QuickPlaySubtitle, ModeCardIconStyle.Lightning, failures);
            PolishModeCard(FindDeep(scene, "InviteFriendsCard"), InviteFriendsSubtitle, ModeCardIconStyle.Friends, failures);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene))
            {
                failures.Add("Failed to save polished PlayMenu scene.");
                return;
            }

            Debug.Log("[PlayFlow] Polished PlayMenu copy + icons (Sprint 20.1).");
        }

        private static void PolishModeCard(GameObject cardGo, string subtitle, ModeCardIconStyle iconStyle, List<string> failures)
        {
            if (cardGo == null)
            {
                failures.Add("Mode card missing for polish: " + iconStyle);
                return;
            }

            RectTransform card = cardGo.GetComponent<RectTransform>();
            EnsureCardShadow(cardGo);

            Transform description = card.Find("Description");
            Text descText = description != null ? description.GetComponent<Text>() : null;
            if (descText == null)
            {
                failures.Add(cardGo.name + " Description text missing.");
            }
            else
            {
                descText.text = subtitle;
                RectTransform descRt = descText.rectTransform;
                descRt.anchorMin = new Vector2(0f, 0.08f);
                descRt.anchorMax = new Vector2(1f, 0.42f);
                descRt.offsetMin = new Vector2(40f, 16f);
                descRt.offsetMax = new Vector2(-40f, 0f);
            }

            Transform title = card.Find("Title");
            if (title != null)
            {
                RectTransform titleRt = title.GetComponent<RectTransform>();
                titleRt.anchorMin = new Vector2(0f, 0.42f);
                titleRt.anchorMax = new Vector2(1f, 0.62f);
                titleRt.offsetMin = new Vector2(28f, 0f);
                titleRt.offsetMax = new Vector2(-28f, 0f);
            }

            Transform existingIcon = card.Find("Icon");
            if (existingIcon != null)
            {
                Object.DestroyImmediate(existingIcon.gameObject);
            }

            CreateModeCardIcon(card, iconStyle);
        }

        private static Button CreateModeCard(
            string name,
            RectTransform parent,
            string title,
            string description,
            Vector2 anchoredPos,
            ModeCardIconStyle iconStyle)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = new Vector2(620f, 420f);

            Image border = go.GetComponent<Image>();
            border.color = PanelBorder;
            border.raycastTarget = true;

            Button button = go.GetComponent<Button>();
            button.targetGraphic = border;
            button.transition = Selectable.Transition.ColorTint;

            EnsureCardShadow(go);

            Image fill = CreateUiImage("Fill", rt, stretch: true);
            fill.color = CardFill;
            fill.raycastTarget = false;
            fill.rectTransform.offsetMin = new Vector2(4f, 4f);
            fill.rectTransform.offsetMax = new Vector2(-4f, -4f);

            CreateModeCardIcon(rt, iconStyle);

            Text titleText = CreateUiText("Title", rt, title, 40, FontStyle.Bold, GoldBright, TextAnchor.MiddleCenter);
            RectTransform titleRt = titleText.rectTransform;
            titleRt.anchorMin = new Vector2(0f, 0.42f);
            titleRt.anchorMax = new Vector2(1f, 0.62f);
            titleRt.offsetMin = new Vector2(28f, 0f);
            titleRt.offsetMax = new Vector2(-28f, 0f);

            Text desc = CreateUiText("Description", rt, description, 22, FontStyle.Normal, TextMuted, TextAnchor.UpperCenter);
            desc.horizontalOverflow = HorizontalWrapMode.Wrap;
            desc.verticalOverflow = VerticalWrapMode.Overflow;
            RectTransform descRt = desc.rectTransform;
            descRt.anchorMin = new Vector2(0f, 0.08f);
            descRt.anchorMax = new Vector2(1f, 0.42f);
            descRt.offsetMin = new Vector2(40f, 16f);
            descRt.offsetMax = new Vector2(-40f, 0f);

            return button;
        }

        private static void EnsureCardShadow(GameObject cardGo)
        {
            Shadow shadow = cardGo.GetComponent<Shadow>();
            if (shadow == null)
            {
                shadow = cardGo.AddComponent<Shadow>();
            }

            shadow.effectColor = new Color(0f, 0f, 0f, 0.55f);
            shadow.effectDistance = new Vector2(0f, -8f);
            shadow.useGraphicAlpha = true;
        }

        /// <summary>
        /// Softer drop shadow for Lobby panels / slots (Sprint 21.5 polish).
        /// Uses Unity UI Shadow GUID — never Outline.
        /// </summary>
        private static void EnsureLobbyPanelShadow(GameObject panelGo)
        {
            Shadow shadow = panelGo.GetComponent<Shadow>();
            if (shadow == null)
            {
                shadow = panelGo.AddComponent<Shadow>();
            }

            shadow.effectColor = new Color(0f, 0f, 0f, 0.42f);
            shadow.effectDistance = new Vector2(0f, -6f);
            shadow.useGraphicAlpha = true;
        }

        private static void CreateModeCardIcon(RectTransform card, ModeCardIconStyle style)
        {
            RectTransform iconRoot = CreateRect("Icon", card);
            iconRoot.SetAsLastSibling();
            iconRoot.anchorMin = new Vector2(0.5f, 0.72f);
            iconRoot.anchorMax = new Vector2(0.5f, 0.94f);
            iconRoot.pivot = new Vector2(0.5f, 0.5f);
            iconRoot.anchoredPosition = Vector2.zero;
            iconRoot.sizeDelta = new Vector2(110f, 0f);

            Image badge = CreateUiImage("Badge", iconRoot, stretch: true);
            badge.sprite = GetBuiltinKnob();
            badge.color = new Color(GoldBright.r, GoldBright.g, GoldBright.b, 0.22f);
            badge.raycastTarget = false;
            badge.preserveAspect = true;

            if (style == ModeCardIconStyle.Lightning)
            {
                CreateBoltPiece(iconRoot, "BoltStem", new Vector2(6f, 2f), new Vector2(16f, 54f), -28f);
                CreateBoltPiece(iconRoot, "BoltSlash", new Vector2(-8f, -6f), new Vector2(42f, 12f), -28f);
                CreateBoltPiece(iconRoot, "BoltTip", new Vector2(10f, -22f), new Vector2(14f, 22f), -28f);
            }
            else
            {
                CreatePersonGlyph(iconRoot, new Vector2(-18f, 8f));
                CreatePersonGlyph(iconRoot, new Vector2(18f, 8f));
            }
        }

        private static void CreateBoltPiece(RectTransform parent, string name, Vector2 anchoredPos, Vector2 size, float zRot)
        {
            Image piece = CreateUiImage(name, parent, stretch: false);
            piece.color = GoldBright;
            piece.raycastTarget = false;
            RectTransform rt = piece.rectTransform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;
            rt.localEulerAngles = new Vector3(0f, 0f, zRot);
        }

        private static void CreatePersonGlyph(RectTransform parent, Vector2 anchoredPos)
        {
            Image head = CreateUiImage("Head", parent, stretch: false);
            head.sprite = GetBuiltinKnob();
            head.color = GoldBright;
            head.raycastTarget = false;
            head.preserveAspect = true;
            RectTransform headRt = head.rectTransform;
            headRt.anchorMin = new Vector2(0.5f, 0.5f);
            headRt.anchorMax = new Vector2(0.5f, 0.5f);
            headRt.pivot = new Vector2(0.5f, 0.5f);
            headRt.anchoredPosition = anchoredPos + new Vector2(0f, 12f);
            headRt.sizeDelta = new Vector2(28f, 28f);

            Image body = CreateUiImage("Body", parent, stretch: false);
            body.sprite = GetBuiltinKnob();
            body.color = GoldBright;
            body.raycastTarget = false;
            body.preserveAspect = true;
            RectTransform bodyRt = body.rectTransform;
            bodyRt.anchorMin = new Vector2(0.5f, 0.5f);
            bodyRt.anchorMax = new Vector2(0.5f, 0.5f);
            bodyRt.pivot = new Vector2(0.5f, 0.5f);
            bodyRt.anchoredPosition = anchoredPos + new Vector2(0f, -14f);
            bodyRt.sizeDelta = new Vector2(36f, 28f);
        }

        private static RectTransform CreateActionCard(string name, RectTransform parent, Vector2 anchoredPos, Vector2 size)
        {
            RectTransform card = CreateRect(name, parent);
            card.anchorMin = new Vector2(0.5f, 0.5f);
            card.anchorMax = new Vector2(0.5f, 0.5f);
            card.pivot = new Vector2(0.5f, 0.5f);
            card.anchoredPosition = anchoredPos;
            card.sizeDelta = size;

            Image border = card.gameObject.AddComponent<Image>();
            border.color = PanelBorder;
            border.raycastTarget = false;

            Image fill = CreateUiImage("Fill", card, stretch: true);
            fill.color = CardFill;
            fill.raycastTarget = false;
            fill.rectTransform.offsetMin = new Vector2(4f, 4f);
            fill.rectTransform.offsetMax = new Vector2(-4f, -4f);
            return card;
        }

        private static Button CreateFriendRow(string name, RectTransform parent, string label, int index, int total)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            float height = 1f / total;
            rt.anchorMin = new Vector2(0f, 1f - (index + 1) * height);
            rt.anchorMax = new Vector2(1f, 1f - index * height);
            rt.offsetMin = new Vector2(8f, 4f);
            rt.offsetMax = new Vector2(-8f, -4f);

            Image image = go.GetComponent<Image>();
            image.color = HighlightGold;
            image.raycastTarget = true;

            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.ColorTint;

            Text text = CreateUiText("Label", rt, label, 22, FontStyle.Bold, TextPrimary, TextAnchor.MiddleLeft);
            RectTransform textRt = text.rectTransform;
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = new Vector2(18f, 0f);
            textRt.offsetMax = new Vector2(-18f, 0f);
            return button;
        }

        private static InputField CreateInputField(string name, Transform parent, string placeholder)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(InputField));
            go.transform.SetParent(parent, false);
            Image image = go.GetComponent<Image>();
            image.color = new Color(0.08f, 0.07f, 0.08f, 0.95f);

            Text text = CreateUiText("Text", go.transform, string.Empty, 22, FontStyle.Normal, TextPrimary, TextAnchor.MiddleLeft);
            RectTransform textRt = text.rectTransform;
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = new Vector2(14f, 6f);
            textRt.offsetMax = new Vector2(-14f, -6f);
            text.supportRichText = false;
            text.raycastTarget = false;

            Text placeholderText = CreateUiText("Placeholder", go.transform, placeholder, 20, FontStyle.Italic, TextMuted, TextAnchor.MiddleLeft);
            RectTransform placeholderRt = placeholderText.rectTransform;
            placeholderRt.anchorMin = Vector2.zero;
            placeholderRt.anchorMax = Vector2.one;
            placeholderRt.offsetMin = new Vector2(14f, 6f);
            placeholderRt.offsetMax = new Vector2(-14f, -6f);
            placeholderText.raycastTarget = false;

            InputField field = go.GetComponent<InputField>();
            field.textComponent = text;
            field.placeholder = placeholderText;
            field.transition = Selectable.Transition.None;
            return field;
        }

        private static GameObject CreateHiddenLabel(string name, Transform parent, string value)
        {
            Text text = CreateUiText(name, parent, value, 20, FontStyle.Bold, GoldBright, TextAnchor.MiddleCenter);
            RectTransform rt = text.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            text.gameObject.SetActive(false);
            return text.gameObject;
        }

        private static void SaveScene(Scene scene, string path, List<string> failures)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)) ?? "Assets/_Project/Scenes");
            bool saved = EditorSceneManager.SaveScene(scene, path);
            if (!saved)
            {
                failures.Add("Failed to save " + path);
                return;
            }

            AssetDatabase.Refresh();
            Debug.Log("[PlayFlow] Saved " + path);
        }

        private static void EnsureEventSystem(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.GetComponent<EventSystem>() != null || root.GetComponentInChildren<EventSystem>(true) != null)
                {
                    return;
                }
            }

            GameObject es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(es, scene);
        }

        private static Sprite LoadSprite(string guid, string label)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Missing asset for " + label + " guid " + guid);
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Sprite GetBuiltinKnob()
        {
            return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        }

        private static Font ResolveUiFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return font;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private static Image CreateUiImage(string name, Transform parent, bool stretch)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            if (stretch)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            return go.GetComponent<Image>();
        }

        private static Text CreateUiText(string name, Transform parent, string value, int size, FontStyle style,
            Color color, TextAnchor anchor)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>();
            text.text = value;
            text.font = ResolveUiFont();
            text.fontSize = size;
            text.fontStyle = style;
            text.color = color;
            text.alignment = anchor;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static Button CreateLabeledButton(string name, Transform parent, string label, float width, float height,
            Color bgColor, Color textColor)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(width, height);

            Image image = go.GetComponent<Image>();
            image.color = bgColor;
            image.raycastTarget = true;

            Button button = go.GetComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.ColorTint;
            ColorBlock colors = button.colors;
            colors.highlightedColor = Color.Lerp(bgColor, Color.white, 0.12f);
            colors.pressedColor = Color.Lerp(bgColor, Color.black, 0.15f);
            button.colors = colors;

            Text text = CreateUiText("Label", rt, label, 24, FontStyle.Bold, textColor, TextAnchor.MiddleCenter);
            RectTransform textRt = text.rectTransform;
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;
            return button;
        }

        private static void PlaceTopLeft(RectTransform rt, Vector2 anchoredPos)
        {
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = anchoredPos;
        }

        private static void StretchHorizontal(RectTransform rt, float top, float height)
        {
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0f, -top);
            rt.sizeDelta = new Vector2(-40f, height);
        }

        private static void RequireCanvasScaler(GameObject canvasGo, List<string> failures)
        {
            if (canvasGo == null)
            {
                return;
            }

            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            if (scaler == null || scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                failures.Add(canvasGo.name + " CanvasScaler Scale With Screen Size missing.");
            }
            else if (scaler.referenceResolution != new Vector2(1920f, 1080f) ||
                     !Mathf.Approximately(scaler.matchWidthOrHeight, 0.5f))
            {
                failures.Add(canvasGo.name + " CanvasScaler expected 1920x1080 match 0.5.");
            }
        }

        private static void RequireBackgroundGuid(GameObject bg, List<string> failures)
        {
            if (bg == null)
            {
                return;
            }

            Image img = bg.GetComponent<Image>();
            string guid = img != null && img.sprite != null
                ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(img.sprite))
                : null;
            if (guid != BackgroundGuid)
            {
                failures.Add(bg.transform.root.name + " Background must reuse MainMenuBackground GUID.");
            }
        }

        private static void RequireInBuild(string scenePath, List<string> failures)
        {
            bool inBuild = false;
            foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
            {
                if (s.path == scenePath && s.enabled)
                {
                    inBuild = true;
                    break;
                }
            }

            if (!inBuild)
            {
                failures.Add(scenePath + " not in EditorBuildSettings.");
            }
        }

        private static GameObject FindDeep(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name == name)
                {
                    return root;
                }

                Transform found = FindChildRecursive(root.transform, name);
                if (found != null)
                {
                    return found.gameObject;
                }
            }

            return null;
        }

        private static Transform FindChildRecursive(Transform parent, string name)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }

                Transform nested = FindChildRecursive(child, name);
                if (nested != null)
                {
                    return nested;
                }
            }

            return null;
        }

        private static void Require(Object obj, string label, List<string> failures)
        {
            if (obj == null)
            {
                failures.Add("Missing " + label);
            }
        }
    }
}
