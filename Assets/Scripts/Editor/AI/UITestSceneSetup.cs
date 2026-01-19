using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Sc.Editor.AI
{
    /// <summary>
    /// UI 시스템 테스트용 씬/프리팹 자동 생성 도구.
    /// Bootstrap: Partial (NavigationManager + EventSystem 생성)
    /// </summary>
    public static class UITestSceneSetup
    {
        private const string PrefabPath = "Assets/Prefabs/UI/Tests";
        private const string ScenePath = "Assets/Scenes";

        #region Menu Items

        [MenuItem("SC Tools/Setup/Scenes/Setup Test Scene", priority = 200)]
        public static void SetupTestScene()
        {
            EnsureFolders();

            // 1. Canvas 생성
            var canvas = CreateOrGetCanvas();

            // 2. EventSystem 생성 (버튼 클릭에 필수)
            CreateEventSystem();

            // 3. NavigationManager 생성
            CreateNavigationManager();

            // 3. Screen Container 생성
            var screenContainer = CreateContainer(canvas.transform, "ScreenContainer", 0);

            // 4. Popup Container 생성
            var popupContainer = CreateContainer(canvas.transform, "PopupContainer", 100);

            // 5. Test Control Panel 생성
            var controlPanel = CreateTestControlPanel(canvas.transform);

            // 6. TestScreen 프리팹 생성
            var screenPrefab = CreateTestScreenPrefab();

            // 7. TestPopup 프리팹 생성
            var popupPrefab = CreateTestPopupPrefab();

            // 8. UITestSetup 설정
            SetupUITestController(controlPanel, screenPrefab, popupPrefab, screenContainer, popupContainer);

            // 씬 저장
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Debug.Log("[UITestSceneSetup] Test scene setup complete!");
        }

        [MenuItem("SC Tools/Setup/Prefabs/Test/Create UI Test Prefabs", priority = 113)]
        public static void CreateTestPrefabsOnly()
        {
            EnsureFolders();

            CreateTestScreenPrefab();
            CreateTestPopupPrefab();

            AssetDatabase.Refresh();
            Debug.Log("[UITestSceneSetup] Test prefabs created!");
        }

        [MenuItem("SC Tools/Setup/Scenes/Clear Test Objects", priority = 290)]
        public static void ClearTestObjects()
        {
            var canvas = GameObject.Find("TestCanvas");
            if (canvas != null) Object.DestroyImmediate(canvas);

            var navManager = GameObject.Find("NavigationManager");
            if (navManager != null) Object.DestroyImmediate(navManager);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[UITestSceneSetup] Test objects cleared!");
        }

        #endregion

        #region Canvas & Containers

        private static Canvas CreateOrGetCanvas()
        {
            var existing = GameObject.Find("TestCanvas");
            if (existing != null)
            {
                return existing.GetComponent<Canvas>();
            }

            var canvasGo = new GameObject("TestCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            return canvas;
        }

        private static void CreateEventSystem()
        {
            EditorUIHelpers.EnsureEventSystem();
        }

        private static void CreateNavigationManager()
        {
            var existing = GameObject.Find("NavigationManager");
            if (existing != null) return;

            var go = new GameObject("NavigationManager");
            go.AddComponent<Common.UI.NavigationManager>();
        }

        private static RectTransform CreateContainer(Transform parent, string name, int sortingOrder)
        {
            var existing = parent.Find(name);
            if (existing != null)
            {
                return existing.GetComponent<RectTransform>();
            }

            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            var sortGroup = go.AddComponent<Canvas>();
            sortGroup.overrideSorting = true;
            sortGroup.sortingOrder = sortingOrder;

            go.AddComponent<GraphicRaycaster>();

            return rect;
        }

        #endregion

        #region Test Screen Prefab

        private static GameObject CreateTestScreenPrefab()
        {
            var prefabPath = $"{PrefabPath}/TestScreen.prefab";

            // 이미 존재하면 로드
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (existingPrefab != null)
            {
                return existingPrefab;
            }

            // Panel 생성
            var panel = CreatePanel("TestScreen", new Color(0.1f, 0.1f, 0.2f, 0.9f));

            // Title Text (중앙 기준)
            var titleText = CreateCenteredText(panel.transform, "TitleText", "Screen Title",
                new Vector2(0, 150), new Vector2(400, 60), 32);

            // Counter Text (중앙 기준)
            var counterText = CreateCenteredText(panel.transform, "CounterText", "Counter: 0",
                new Vector2(0, 80), new Vector2(300, 40), 24);

            // Buttons (중앙 기준)
            var popupBtn = CreateCenteredButton(panel.transform, "PopupButton", "Open Popup",
                new Vector2(0, 0), new Vector2(200, 50));

            var nextBtn = CreateCenteredButton(panel.transform, "NextScreenButton", "Next Screen",
                new Vector2(0, -60), new Vector2(200, 50));

            var backBtn = CreateCenteredButton(panel.transform, "BackButton", "Back",
                new Vector2(0, -120), new Vector2(200, 50));

            // TestScreen 컴포넌트 추가
            var testScreen = panel.AddComponent<Common.UI.Tests.TestScreen>();

            // SerializedObject로 필드 할당
            var so = new SerializedObject(testScreen);
            so.FindProperty("_titleText").objectReferenceValue = titleText;
            so.FindProperty("_counterText").objectReferenceValue = counterText;
            so.FindProperty("_popupButton").objectReferenceValue = popupBtn;
            so.FindProperty("_nextScreenButton").objectReferenceValue = nextBtn;
            so.FindProperty("_backButton").objectReferenceValue = backBtn;
            so.ApplyModifiedPropertiesWithoutUndo();

            // 프리팹 저장
            var prefab = PrefabUtility.SaveAsPrefabAsset(panel, prefabPath);
            Object.DestroyImmediate(panel);

            Debug.Log($"[UITestSceneSetup] Created: {prefabPath}");
            return prefab;
        }

        #endregion

        #region Test Popup Prefab

        private static GameObject CreateTestPopupPrefab()
        {
            var prefabPath = $"{PrefabPath}/TestPopup.prefab";

            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (existingPrefab != null)
            {
                return existingPrefab;
            }

            // Popup Panel (작은 크기)
            var panel = CreatePanel("TestPopup", new Color(0.2f, 0.2f, 0.3f, 0.95f), new Vector2(400, 300));

            // Message Text (중앙 기준)
            var messageText = CreateCenteredText(panel.transform, "MessageText", "Popup Message",
                new Vector2(0, 50), new Vector2(350, 80), 24);

            // Buttons (중앙 기준)
            var confirmBtn = CreateCenteredButton(panel.transform, "ConfirmButton", "Confirm",
                new Vector2(-70, -80), new Vector2(120, 45));

            var closeBtn = CreateCenteredButton(panel.transform, "CloseButton", "Close",
                new Vector2(70, -80), new Vector2(120, 45));

            // TestPopup 컴포넌트 추가
            var testPopup = panel.AddComponent<Common.UI.Tests.TestPopup>();

            var so = new SerializedObject(testPopup);
            so.FindProperty("_messageText").objectReferenceValue = messageText;
            so.FindProperty("_confirmButton").objectReferenceValue = confirmBtn;
            so.FindProperty("_closeButton").objectReferenceValue = closeBtn;
            so.ApplyModifiedPropertiesWithoutUndo();

            // 프리팹 저장
            var prefab = PrefabUtility.SaveAsPrefabAsset(panel, prefabPath);
            Object.DestroyImmediate(panel);

            Debug.Log($"[UITestSceneSetup] Created: {prefabPath}");
            return prefab;
        }

        #endregion

        #region Test Control Panel

        private static GameObject CreateTestControlPanel(Transform canvasTransform)
        {
            var existing = canvasTransform.Find("TestControlPanel");
            if (existing != null)
            {
                return existing.gameObject;
            }

            // Control Panel (왼쪽 하단)
            var panel = new GameObject("TestControlPanel");
            panel.transform.SetParent(canvasTransform, false);

            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 0);
            rect.anchoredPosition = new Vector2(20, 20);
            rect.sizeDelta = new Vector2(180, 320);

            var bg = panel.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.7f);

            // Title (좌상단 기준, 중앙 정렬)
            CreateText(panel.transform, "Title", "UI Test Controls",
                new Vector2(10, -5), new Vector2(160, 30), 16, TextAnchor.MiddleCenter);

            // Buttons (좌상단 기준으로 배치)
            float yPos = -40;
            float yStep = 40;

            CreateButton(panel.transform, "CreateScreenBtn", "Create Screen",
                new Vector2(15, yPos), new Vector2(150, 35));
            yPos -= yStep;

            CreateButton(panel.transform, "ShowScreenBtn", "Show Screen",
                new Vector2(15, yPos), new Vector2(150, 35));
            yPos -= yStep;

            CreateButton(panel.transform, "HideScreenBtn", "Hide Screen",
                new Vector2(15, yPos), new Vector2(150, 35));
            yPos -= yStep;

            CreateButton(panel.transform, "CreatePopupBtn", "Create Popup",
                new Vector2(15, yPos), new Vector2(150, 35));
            yPos -= yStep;

            CreateButton(panel.transform, "ShowPopupBtn", "Show Popup",
                new Vector2(15, yPos), new Vector2(150, 35));
            yPos -= yStep;

            CreateButton(panel.transform, "HidePopupBtn", "Hide Popup",
                new Vector2(15, yPos), new Vector2(150, 35));
            yPos -= yStep;

            CreateButton(panel.transform, "TestFullFlowBtn", "Test Full Flow",
                new Vector2(15, yPos), new Vector2(150, 35));

            return panel;
        }

        private static void SetupUITestController(GameObject controlPanel, GameObject screenPrefab,
            GameObject popupPrefab, RectTransform screenContainer, RectTransform popupContainer)
        {
            var testSetup = controlPanel.GetComponent<Common.UI.Tests.UITestSetup>();
            if (testSetup == null)
            {
                testSetup = controlPanel.AddComponent<Common.UI.Tests.UITestSetup>();
            }

            var so = new SerializedObject(testSetup);

            // Prefabs
            so.FindProperty("_screenPrefab").objectReferenceValue =
                screenPrefab.GetComponent<Common.UI.Tests.TestScreen>();
            so.FindProperty("_popupPrefab").objectReferenceValue =
                popupPrefab.GetComponent<Common.UI.Tests.TestPopup>();

            // Containers
            so.FindProperty("_screenContainer").objectReferenceValue = screenContainer;
            so.FindProperty("_popupContainer").objectReferenceValue = popupContainer;

            // Buttons
            so.FindProperty("_createScreenButton").objectReferenceValue =
                controlPanel.transform.Find("CreateScreenBtn")?.GetComponent<Button>();
            so.FindProperty("_showScreenButton").objectReferenceValue =
                controlPanel.transform.Find("ShowScreenBtn")?.GetComponent<Button>();
            so.FindProperty("_hideScreenButton").objectReferenceValue =
                controlPanel.transform.Find("HideScreenBtn")?.GetComponent<Button>();
            so.FindProperty("_createPopupButton").objectReferenceValue =
                controlPanel.transform.Find("CreatePopupBtn")?.GetComponent<Button>();
            so.FindProperty("_showPopupButton").objectReferenceValue =
                controlPanel.transform.Find("ShowPopupBtn")?.GetComponent<Button>();
            so.FindProperty("_hidePopupButton").objectReferenceValue =
                controlPanel.transform.Find("HidePopupBtn")?.GetComponent<Button>();
            so.FindProperty("_testFullFlowButton").objectReferenceValue =
                controlPanel.transform.Find("TestFullFlowBtn")?.GetComponent<Button>();

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion

        #region UI Helpers

        private static GameObject CreatePanel(string name, Color bgColor, Vector2? size = null)
        {
            var panel = new GameObject(name);
            var rect = panel.AddComponent<RectTransform>();

            if (size.HasValue)
            {
                rect.sizeDelta = size.Value;
            }
            else
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.sizeDelta = Vector2.zero;
            }

            var image = panel.AddComponent<Image>();
            image.color = bgColor;

            return panel;
        }

        private static Text CreateText(Transform parent, string name, string content,
            Vector2 position, Vector2 size, int fontSize, TextAnchor textAnchor = TextAnchor.MiddleCenter)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            // 부모 좌상단 기준으로 배치
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            var text = go.AddComponent<Text>();
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = textAnchor;
            text.color = Color.white;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return text;
        }

        private static Button CreateButton(Transform parent, string name, string label,
            Vector2 position, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            // 부모 좌상단 기준으로 배치
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            var image = go.AddComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.4f, 1f);

            var button = go.AddComponent<Button>();
            var colors = button.colors;
            colors.highlightedColor = new Color(0.4f, 0.4f, 0.5f, 1f);
            colors.pressedColor = new Color(0.2f, 0.2f, 0.3f, 1f);
            button.colors = colors;

            // Label - 버튼 중앙에 배치
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;
            labelRect.anchoredPosition = Vector2.zero;

            var labelText = labelGo.AddComponent<Text>();
            labelText.text = label;
            labelText.fontSize = 14;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.color = Color.white;
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return button;
        }

        /// <summary>
        /// 중앙 기준 Text 생성 (Screen/Popup 내부용)
        /// </summary>
        private static Text CreateCenteredText(Transform parent, string name, string content,
            Vector2 offset, Vector2 size, int fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;

            var text = go.AddComponent<Text>();
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return text;
        }

        /// <summary>
        /// 중앙 기준 Button 생성 (Screen/Popup 내부용)
        /// </summary>
        private static Button CreateCenteredButton(Transform parent, string name, string label,
            Vector2 offset, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = offset;
            rect.sizeDelta = size;

            var image = go.AddComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.4f, 1f);

            var button = go.AddComponent<Button>();
            var colors = button.colors;
            colors.highlightedColor = new Color(0.4f, 0.4f, 0.5f, 1f);
            colors.pressedColor = new Color(0.2f, 0.2f, 0.3f, 1f);
            button.colors = colors;

            // Label
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.sizeDelta = Vector2.zero;
            labelRect.anchoredPosition = Vector2.zero;

            var labelText = labelGo.AddComponent<Text>();
            labelText.text = label;
            labelText.fontSize = 14;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.color = Color.white;
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return button;
        }

        #endregion

        #region Utility

        private static void EnsureFolders()
        {
            EditorUIHelpers.EnsureFolder(PrefabPath);
        }

        #endregion
    }
}
