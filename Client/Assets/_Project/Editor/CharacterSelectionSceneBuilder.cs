using System.Collections.Generic;
using System.IO;
using GulfRun.Features.CharacterSelection;
using GulfRun.Features.MainMenu;
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
    /// Builds CharacterSelection.unity, wires Main Menu Play Now, updates
    /// EditorBuildSettings, and validates hierarchy in batchmode.
    /// </summary>
    public static class CharacterSelectionSceneBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/CharacterSelection.unity";
        private const string MainMenuScenePath = "Assets/_Project/Scenes/MainMenu.unity";
        private const string BackgroundGuid = "a18b0000000000000000000000000001";
        private const string RunnerGuid = "a18c2000000000000000000000000001";

        // Gulf palette (matches MainMenuTheme without referencing Features.MainMenu UI styles).
        private static readonly Color Gold = new Color(0.90f, 0.71f, 0.25f, 1f);
        private static readonly Color GoldBright = new Color(1f, 0.84f, 0.40f, 1f);
        private static readonly Color SandDark = new Color(0.52f, 0.42f, 0.30f, 0.92f);
        private static readonly Color PanelBg = new Color(0.10f, 0.09f, 0.10f, 0.72f);
        private static readonly Color TextPrimary = Color.white;
        private static readonly Color TextMuted = new Color(0.80f, 0.80f, 0.80f, 1f);
        private static readonly Color PlatformFill = new Color(0.18f, 0.14f, 0.12f, 0.88f);
        private static readonly Color PlatformRing = new Color(0.90f, 0.71f, 0.25f, 0.55f);
        private static readonly Color ButtonDark = new Color(0.12f, 0.10f, 0.09f, 0.92f);

        [MenuItem("GulfRun/Character Selection/Build Scene + Wire Play")]
        public static void RunFromMenu() => RunBatch();

        public static void RunBatch()
        {
            var failures = new List<string>();

            try
            {
                WireMainMenuPlayButton(failures);
                BuildCharacterSelectionScene(failures);
                EnsureBuildSettings(failures);
                ValidateCharacterSelectionScene(failures);
            }
            catch (System.Exception ex)
            {
                failures.Add("Unhandled: " + ex);
                Debug.LogException(ex);
            }

            if (failures.Count == 0)
            {
                Debug.Log("[CharacterSelection] PASS — scene built, Play wired, build settings OK, 0 failures.");
                EditorApplication.Exit(0);
            }
            else
            {
                foreach (string failure in failures)
                {
                    Debug.LogError("[CharacterSelection] FAIL: " + failure);
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
            Debug.Log("[CharacterSelection] Wired MainMenu PlayButtonImage → CharacterSelection.");
        }

        private static void BuildCharacterSelectionScene(List<string> failures)
        {
            Sprite background = LoadSprite(BackgroundGuid, "MainMenuBackground");
            Sprite runner = LoadSprite(RunnerGuid, "Runner");
            if (background == null)
            {
                failures.Add("Background sprite GUID missing.");
            }

            if (runner == null)
            {
                failures.Add("Runner sprite GUID missing.");
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            EnsureEventSystem(scene);

            GameObject canvasGo = new GameObject("CharacterSelectionCanvas", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CharacterSelectionController));
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

            // Background (full bleed, same sprite as Main Menu)
            Image bg = CreateUiImage("Background", canvasRt, stretch: true);
            bg.sprite = background;
            bg.preserveAspect = false;
            bg.raycastTarget = false;
            bg.color = Color.white;

            // SafeArea (same margins pattern as Main Menu)
            RectTransform safeArea = CreateRect("SafeArea", canvasRt);
            safeArea.anchorMin = Vector2.zero;
            safeArea.anchorMax = Vector2.one;
            safeArea.anchoredPosition = new Vector2(0f, -9f);
            safeArea.sizeDelta = new Vector2(-96f, -86f);

            // Back (top-left)
            Button backButton = CreateLabeledButton("BackButton", canvasRt, "Back", 168f, 64f, ButtonDark, GoldBright);
            RectTransform backRt = backButton.GetComponent<RectTransform>();
            backRt.anchorMin = new Vector2(0f, 1f);
            backRt.anchorMax = new Vector2(0f, 1f);
            backRt.pivot = new Vector2(0f, 1f);
            backRt.anchoredPosition = new Vector2(48f, -40f);

            // CharacterStage — named root for future multi-character content
            RectTransform stage = CreateRect("CharacterStage", canvasRt);
            stage.anchorMin = new Vector2(0.5f, 0.5f);
            stage.anchorMax = new Vector2(0.5f, 0.5f);
            stage.pivot = new Vector2(0.5f, 0.5f);
            stage.anchoredPosition = new Vector2(0f, 40f);
            stage.sizeDelta = new Vector2(720f, 620f);

            // Future-ready empty root (more character prefabs / slots later)
            CreateRect("CharacterSlotsRoot", stage);

            Image platformOuter = CreateUiImage("Platform", stage, stretch: false);
            platformOuter.sprite = GetBuiltinKnob();
            platformOuter.color = PlatformRing;
            platformOuter.raycastTarget = false;
            platformOuter.preserveAspect = true;
            RectTransform platformRt = platformOuter.rectTransform;
            platformRt.anchorMin = new Vector2(0.5f, 0f);
            platformRt.anchorMax = new Vector2(0.5f, 0f);
            platformRt.pivot = new Vector2(0.5f, 0.5f);
            platformRt.anchoredPosition = new Vector2(0f, 36f);
            platformRt.sizeDelta = new Vector2(420f, 110f);

            Image platformInner = CreateUiImage("PlatformFill", platformRt, stretch: true);
            platformInner.sprite = GetBuiltinKnob();
            platformInner.color = PlatformFill;
            platformInner.raycastTarget = false;
            platformInner.rectTransform.offsetMin = new Vector2(18f, 14f);
            platformInner.rectTransform.offsetMax = new Vector2(-18f, -14f);

            Image character = CreateUiImage("CharacterImage", stage, stretch: false);
            character.sprite = runner;
            character.preserveAspect = true;
            character.raycastTarget = false;
            character.color = Color.white;
            RectTransform charRt = character.rectTransform;
            charRt.anchorMin = new Vector2(0.5f, 0.5f);
            charRt.anchorMax = new Vector2(0.5f, 0.5f);
            charRt.pivot = new Vector2(0.5f, 0.5f);
            charRt.anchoredPosition = new Vector2(0f, 48f);
            // ~45% of 1080 reference height
            charRt.sizeDelta = new Vector2(420f, 486f);

            // Arrows
            Button arrowLeft = CreateLabeledButton("ArrowLeft", canvasRt, "◀", 88f, 88f, SandDark, GoldBright);
            PlaceMiddle(arrowLeft.GetComponent<RectTransform>(), new Vector2(-520f, 60f));

            Button arrowRight = CreateLabeledButton("ArrowRight", canvasRt, "▶", 88f, 88f, SandDark, GoldBright);
            PlaceMiddle(arrowRight.GetComponent<RectTransform>(), new Vector2(520f, 60f));

            // Info panel under character
            RectTransform info = CreateRect("InfoPanel", canvasRt);
            info.anchorMin = new Vector2(0.5f, 0.5f);
            info.anchorMax = new Vector2(0.5f, 0.5f);
            info.pivot = new Vector2(0.5f, 1f);
            info.anchoredPosition = new Vector2(0f, -280f);
            info.sizeDelta = new Vector2(520f, 120f);

            Image infoBg = info.gameObject.AddComponent<Image>();
            infoBg.color = PanelBg;
            infoBg.raycastTarget = false;

            Text nameText = CreateUiText("CharacterName", info, "Runner", 36, FontStyle.Bold, GoldBright, TextAnchor.MiddleCenter);
            RectTransform nameRt = nameText.rectTransform;
            nameRt.anchorMin = new Vector2(0f, 0.66f);
            nameRt.anchorMax = new Vector2(1f, 1f);
            nameRt.offsetMin = new Vector2(12f, 0f);
            nameRt.offsetMax = new Vector2(-12f, -8f);

            Text countryText = CreateUiText("Country", info, "United Arab Emirates", 22, FontStyle.Normal, TextPrimary, TextAnchor.MiddleCenter);
            RectTransform countryRt = countryText.rectTransform;
            countryRt.anchorMin = new Vector2(0f, 0.33f);
            countryRt.anchorMax = new Vector2(1f, 0.66f);
            countryRt.offsetMin = new Vector2(12f, 0f);
            countryRt.offsetMax = new Vector2(-12f, 0f);

            Text statusText = CreateUiText("Status", info, "Unlocked", 20, FontStyle.Normal, TextMuted, TextAnchor.MiddleCenter);
            RectTransform statusRt = statusText.rectTransform;
            statusRt.anchorMin = new Vector2(0f, 0f);
            statusRt.anchorMax = new Vector2(1f, 0.33f);
            statusRt.offsetMin = new Vector2(12f, 8f);
            statusRt.offsetMax = new Vector2(-12f, 0f);

            // Select Character — primary, bottom center, larger
            Button select = CreateLabeledButton("SelectCharacterButton", canvasRt, "Select Character", 480f, 96f, Gold, new Color(0.20f, 0.14f, 0.02f, 1f));
            RectTransform selectRt = select.GetComponent<RectTransform>();
            selectRt.anchorMin = new Vector2(0.5f, 0f);
            selectRt.anchorMax = new Vector2(0.5f, 0f);
            selectRt.pivot = new Vector2(0.5f, 0f);
            selectRt.anchoredPosition = new Vector2(0f, 56f);
            Text selectLabel = select.GetComponentInChildren<Text>();
            if (selectLabel != null)
            {
                selectLabel.fontSize = 32;
                selectLabel.fontStyle = FontStyle.Bold;
            }

            CharacterSelectionController controller = canvasGo.GetComponent<CharacterSelectionController>();
            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("backButton").objectReferenceValue = backButton;
            so.FindProperty("selectCharacterButton").objectReferenceValue = select;
            so.FindProperty("arrowLeftButton").objectReferenceValue = arrowLeft;
            so.FindProperty("arrowRightButton").objectReferenceValue = arrowRight;
            so.ApplyModifiedPropertiesWithoutUndo();

            // Draw order: Background, SafeArea, CharacterStage, arrows, Info, Back, Select
            bg.transform.SetSiblingIndex(0);
            safeArea.SetSiblingIndex(1);
            stage.SetSiblingIndex(2);
            arrowLeft.transform.SetSiblingIndex(3);
            arrowRight.transform.SetSiblingIndex(4);
            info.SetSiblingIndex(5);
            backButton.transform.SetSiblingIndex(6);
            select.transform.SetSiblingIndex(7);

            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(ScenePath)) ?? "Assets/_Project/Scenes");
            bool saved = EditorSceneManager.SaveScene(scene, ScenePath);
            if (!saved)
            {
                failures.Add("Failed to save " + ScenePath);
                return;
            }

            AssetDatabase.Refresh();
            Debug.Log("[CharacterSelection] Saved " + ScenePath);
        }

        private static void EnsureBuildSettings(List<string> failures)
        {
            string sceneGuid = AssetDatabase.AssetPathToGUID(ScenePath);
            if (string.IsNullOrEmpty(sceneGuid))
            {
                failures.Add("CharacterSelection scene GUID missing after save.");
                return;
            }

            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int existing = scenes.FindIndex(s => s.path == ScenePath);
            if (existing >= 0)
            {
                scenes.RemoveAt(existing);
            }

            int mainMenuIndex = scenes.FindIndex(s => s.path == MainMenuScenePath);
            var entry = new EditorBuildSettingsScene(ScenePath, true);
            if (mainMenuIndex >= 0)
            {
                scenes.Insert(mainMenuIndex + 1, entry);
            }
            else
            {
                scenes.Add(entry);
            }

            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("[CharacterSelection] EditorBuildSettings updated after MainMenu.");
        }

        private static void ValidateCharacterSelectionScene(List<string> failures)
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Require(FindDeep(scene, "CharacterSelectionCanvas"), "CharacterSelectionCanvas", failures);
            Require(FindDeep(scene, "Background"), "Background", failures);
            Require(FindDeep(scene, "SafeArea"), "SafeArea", failures);
            Require(FindDeep(scene, "BackButton"), "BackButton", failures);
            Require(FindDeep(scene, "CharacterStage"), "CharacterStage", failures);
            Require(FindDeep(scene, "Platform"), "Platform", failures);
            Require(FindDeep(scene, "CharacterImage"), "CharacterImage", failures);
            Require(FindDeep(scene, "CharacterSlotsRoot"), "CharacterSlotsRoot", failures);
            Require(FindDeep(scene, "ArrowLeft"), "ArrowLeft", failures);
            Require(FindDeep(scene, "ArrowRight"), "ArrowRight", failures);
            Require(FindDeep(scene, "InfoPanel"), "InfoPanel", failures);
            Require(FindDeep(scene, "CharacterName"), "CharacterName", failures);
            Require(FindDeep(scene, "Country"), "Country", failures);
            Require(FindDeep(scene, "Status"), "Status", failures);
            Require(FindDeep(scene, "SelectCharacterButton"), "SelectCharacterButton", failures);
            Require(UnityEngine.Object.FindObjectOfType<EventSystem>(), "EventSystem", failures);

            GameObject canvasGo = FindDeep(scene, "CharacterSelectionCanvas");
            if (canvasGo != null)
            {
                CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
                if (scaler == null || scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                {
                    failures.Add("CanvasScaler Scale With Screen Size missing.");
                }
                else if (scaler.referenceResolution != new Vector2(1920f, 1080f) ||
                         !Mathf.Approximately(scaler.matchWidthOrHeight, 0.5f))
                {
                    failures.Add("CanvasScaler expected 1920x1080 match 0.5.");
                }

                if (canvasGo.GetComponent<CharacterSelectionController>() == null)
                {
                    failures.Add("CharacterSelectionController missing on canvas.");
                }
            }

            GameObject character = FindDeep(scene, "CharacterImage");
            if (character != null)
            {
                RectTransform rt = character.GetComponent<RectTransform>();
                if (rt == null || !Mathf.Approximately(rt.sizeDelta.y, 486f))
                {
                    failures.Add("CharacterImage height should be ~486 (45% of 1080).");
                }

                Image img = character.GetComponent<Image>();
                if (img == null || img.sprite == null || !img.preserveAspect)
                {
                    failures.Add("CharacterImage must use Runner sprite with preserveAspect.");
                }
            }

            GameObject bg = FindDeep(scene, "Background");
            if (bg != null)
            {
                Image img = bg.GetComponent<Image>();
                string guid = img != null && img.sprite != null
                    ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(img.sprite))
                    : null;
                if (guid != BackgroundGuid)
                {
                    failures.Add("Background must reuse MainMenuBackground GUID.");
                }
            }

            bool inBuild = false;
            foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
            {
                if (s.path == ScenePath && s.enabled)
                {
                    inBuild = true;
                    break;
                }
            }

            if (!inBuild)
            {
                failures.Add("CharacterSelection not in EditorBuildSettings.");
            }

            // Confirm Main Menu play wiring still present
            Scene mainMenu = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            GameObject play = FindDeep(mainMenu, "PlayButtonImage");
            if (play == null || play.GetComponent<MainMenuPlayButton>() == null || play.GetComponent<Button>() == null)
            {
                failures.Add("MainMenu PlayButtonImage missing Button/MainMenuPlayButton.");
            }

            if (UnityEngine.Object.FindObjectOfType<EventSystem>() == null)
            {
                failures.Add("MainMenu EventSystem missing (needed for Play click).");
            }

            // Constant sanity
            if (CoreSceneManager.CharacterSelectionSceneName != "CharacterSelection")
            {
                failures.Add("SceneManager.CharacterSelectionSceneName mismatch.");
            }
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
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.fontStyle = style;
            text.color = color;
            text.alignment = anchor;
            text.raycastTarget = false;
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

        private static void PlaceMiddle(RectTransform rt, Vector2 anchoredPos)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
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
