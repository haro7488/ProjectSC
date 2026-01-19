using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace Sc.Editor.AI
{
    /// <summary>
    /// PlayMode 테스트용 프리팹 자동 생성 도구.
    /// Bootstrap: None (프리팹 생성 전용)
    /// </summary>
    public static class PlayModeTestSetup
    {
        private const string TestPrefabPath = "Assets/Prefabs/Tests";

        #region Menu Items

        [MenuItem("SC Tools/Setup/Prefabs/Create All Test Prefabs", priority = 100)]
        public static void CreateAllTestPrefabs()
        {
            EditorUIHelpers.EnsureFolder(TestPrefabPath);

            CreateSimpleTestScreenPrefab();
            CreateSimpleTestPopupPrefab();
            CreateTestSystemPopupPrefab();

            AssetDatabase.Refresh();
            Debug.Log("[PlayModeTestSetup] All test prefabs created!");
        }

        [MenuItem("SC Tools/Setup/Prefabs/Test/Create Screen Prefab", priority = 110)]
        public static void CreateSimpleTestScreenPrefab()
        {
            EditorUIHelpers.EnsureFolder(TestPrefabPath);

            var prefabPath = $"{TestPrefabPath}/SimpleTestScreen.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.Log($"[PlayModeTestSetup] Already exists: {prefabPath}");
                return;
            }

            // Root Panel
            var panel = EditorUIHelpers.CreateFullscreenPanel(
                "SimpleTestScreen",
                new Color(0.15f, 0.15f, 0.25f, 0.95f));
            panel.AddComponent<CanvasGroup>();

            // Title
            EditorUIHelpers.CreateCenteredText(panel.transform, "TitleText", "Test Screen",
                new Vector2(0, 200), new Vector2(600, 80), 48);

            // Info Text
            EditorUIHelpers.CreateCenteredText(panel.transform, "InfoText", "Screen Info",
                new Vector2(0, 100), new Vector2(500, 50), 24);

            // Buttons
            EditorUIHelpers.CreateCenteredButton(panel.transform, "ActionButton", "Action",
                new Vector2(0, -50), new Vector2(200, 60));
            EditorUIHelpers.CreateCenteredButton(panel.transform, "BackButton", "Back",
                new Vector2(0, -130), new Vector2(200, 60));

            // Save
            var prefab = PrefabUtility.SaveAsPrefabAsset(panel, prefabPath);
            Object.DestroyImmediate(panel);
            Debug.Log($"[PlayModeTestSetup] Created: {prefabPath}");
        }

        [MenuItem("SC Tools/Setup/Prefabs/Test/Create Popup Prefab", priority = 111)]
        public static void CreateSimpleTestPopupPrefab()
        {
            EditorUIHelpers.EnsureFolder(TestPrefabPath);

            var prefabPath = $"{TestPrefabPath}/SimpleTestPopup.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.Log($"[PlayModeTestSetup] Already exists: {prefabPath}");
                return;
            }

            // Root with dim background
            var root = EditorUIHelpers.CreateFullscreenPanel(
                "SimpleTestPopup",
                new Color(0, 0, 0, 0.5f));
            root.AddComponent<CanvasGroup>();

            // Content Panel
            var content = EditorUIHelpers.CreateCenteredPanel(
                "Content",
                new Vector2(500, 350),
                new Color(0.2f, 0.2f, 0.3f, 1f),
                root.transform);

            // Title
            EditorUIHelpers.CreateCenteredText(content.transform, "TitleText", "Popup Title",
                new Vector2(0, 120), new Vector2(450, 50), 32);

            // Message
            EditorUIHelpers.CreateCenteredText(content.transform, "MessageText", "Popup message content",
                new Vector2(0, 40), new Vector2(450, 80), 20);

            // Buttons
            EditorUIHelpers.CreateCenteredButton(content.transform, "ConfirmButton", "Confirm",
                new Vector2(-80, -100), new Vector2(140, 50));
            EditorUIHelpers.CreateCenteredButton(content.transform, "CancelButton", "Cancel",
                new Vector2(80, -100), new Vector2(140, 50));

            // Save
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Object.DestroyImmediate(root);
            Debug.Log($"[PlayModeTestSetup] Created: {prefabPath}");
        }

        [MenuItem("SC Tools/Setup/Prefabs/Test/Create SystemPopup Prefab", priority = 112)]
        public static void CreateTestSystemPopupPrefab()
        {
            EditorUIHelpers.EnsureFolder(TestPrefabPath);

            var prefabPath = $"{TestPrefabPath}/TestSystemPopup.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.Log($"[PlayModeTestSetup] Already exists: {prefabPath}");
                return;
            }

            // Root
            var root = EditorUIHelpers.CreateUIObject("TestSystemPopup");
            var rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.sizeDelta = Vector2.zero;

            // Background
            EditorUIHelpers.CreateDimBackground(root.transform, "Background", 0.6f);
            root.AddComponent<CanvasGroup>();

            // Content panel
            var content = EditorUIHelpers.CreateCenteredPanel(
                "Content",
                new Vector2(600, 400),
                new Color(0.18f, 0.18f, 0.28f, 1f),
                root.transform);

            // Header
            var header = EditorUIHelpers.CreateUIObject("Header", content.transform);
            var headerRect = header.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0, 80);
            var headerBg = header.AddComponent<Image>();
            headerBg.color = new Color(0.25f, 0.25f, 0.4f, 1f);

            // Title in header
            EditorUIHelpers.CreateStretchedText(header.transform, "TitleText", "System Message", 28);

            // Message
            EditorUIHelpers.CreateCenteredText(content.transform, "MessageText",
                "This is a system popup message.",
                new Vector2(0, 20), new Vector2(550, 120), 22);

            // Button Container
            var buttonContainer = EditorUIHelpers.CreateUIObject("ButtonContainer", content.transform);
            var bcRect = buttonContainer.GetComponent<RectTransform>();
            bcRect.anchorMin = new Vector2(0.5f, 0);
            bcRect.anchorMax = new Vector2(0.5f, 0);
            bcRect.pivot = new Vector2(0.5f, 0);
            bcRect.anchoredPosition = new Vector2(0, 30);
            bcRect.sizeDelta = new Vector2(400, 60);

            var hlg = buttonContainer.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 20;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = false;

            // Buttons
            EditorUIHelpers.CreateLayoutButton(buttonContainer.transform, "ConfirmButton", "Confirm",
                new Color(0.2f, 0.5f, 0.3f, 1f));
            EditorUIHelpers.CreateLayoutButton(buttonContainer.transform, "CancelButton", "Cancel",
                new Color(0.5f, 0.3f, 0.3f, 1f));

            // Save
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Object.DestroyImmediate(root);
            Debug.Log($"[PlayModeTestSetup] Created: {prefabPath}");
        }

        [MenuItem("SC Tools/Setup/Prefabs/Test/Verify Test Prefabs", priority = 150)]
        public static void VerifyTestScene()
        {
            var scenePath = "Assets/Scenes/Test/Main.unity";
            if (!System.IO.File.Exists(scenePath))
            {
                Debug.LogWarning($"[PlayModeTestSetup] Test scene not found: {scenePath}");
                return;
            }

            Debug.Log($"[PlayModeTestSetup] Test scene exists: {scenePath}");

            var prefabs = new[]
            {
                $"{TestPrefabPath}/SimpleTestScreen.prefab",
                $"{TestPrefabPath}/SimpleTestPopup.prefab",
                $"{TestPrefabPath}/TestSystemPopup.prefab"
            };

            foreach (var path in prefabs)
            {
                var exists = AssetDatabase.LoadAssetAtPath<GameObject>(path) != null;
                var status = exists ? "✓" : "✗";
                Debug.Log($"  {status} {path}");
            }
        }

        [MenuItem("SC Tools/Setup/Prefabs/Delete All Test Prefabs", priority = 190)]
        public static void DeleteAllTestPrefabs()
        {
            if (!EditorUtility.DisplayDialog("Delete Test Prefabs",
                "Are you sure you want to delete all PlayMode test prefabs?",
                "Delete", "Cancel"))
            {
                return;
            }

            var prefabs = new[]
            {
                $"{TestPrefabPath}/SimpleTestScreen.prefab",
                $"{TestPrefabPath}/SimpleTestPopup.prefab",
                $"{TestPrefabPath}/TestSystemPopup.prefab"
            };

            foreach (var path in prefabs)
            {
                if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
                {
                    AssetDatabase.DeleteAsset(path);
                    Debug.Log($"[PlayModeTestSetup] Deleted: {path}");
                }
            }

            AssetDatabase.Refresh();
        }

        #endregion
    }
}
