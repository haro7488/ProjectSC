using System.IO;
using Sc.Common.UI.Widgets;
using Sc.Contents.Lobby;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Editor.Wizard.Generators
{
    /// <summary>
    /// LobbyScreen 탭 시스템 설정 에디터 도구
    /// </summary>
    public static class LobbyScreenSetup
    {
        private const string PREFAB_ROOT = "Assets/Prefabs/UI";
        private const string WIDGETS_PATH = PREFAB_ROOT + "/Widgets";
        private const string MVP_PATH = PREFAB_ROOT + "/MVP";

        #region Public API

        public static void SetupTabSystemMenu()
        {
            // TabButton 프리팹 생성
            CreateTabButtonPrefab();

            // LobbyScreen 프리팹 업데이트
            var success = SetupLobbyScreenPrefab();

            if (success)
            {
                EditorUtility.DisplayDialog("완료",
                    "LobbyScreen 탭 시스템 설정 완료!\n\n" +
                    "생성된 항목:\n" +
                    "- TabButton.prefab\n" +
                    "- LobbyScreen에 TabGroupWidget 추가\n" +
                    "- 4개 탭 컨텐츠 (Home, Character, Gacha, Settings)",
                    "확인");
            }
            else
            {
                EditorUtility.DisplayDialog("오류",
                    "LobbyScreen 프리팹을 찾을 수 없습니다.\n" +
                    $"경로: {MVP_PATH}/LobbyScreen.prefab",
                    "확인");
            }
        }

        public static void CreateTabButtonPrefabMenu()
        {
            CreateTabButtonPrefab();
            EditorUtility.DisplayDialog("완료", "TabButton 프리팹 생성 완료", "확인");
        }

        #endregion

        #region TabButton Prefab

        public static void CreateTabButtonPrefab()
        {
            EnsureDirectoryExists(WIDGETS_PATH);

            var prefabPath = $"{WIDGETS_PATH}/TabButton.prefab";

            // 이미 존재하면 스킵
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.Log("[LobbyScreenSetup] TabButton 프리팹이 이미 존재함");
                return;
            }

            var go = CreateTabButtonGameObject();
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[LobbyScreenSetup] TabButton 프리팹 생성 완료");
        }

        private static GameObject CreateTabButtonGameObject()
        {
            // Root
            var root = new GameObject("TabButton");
            var rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(120, 60);

            // Background Image
            var bg = root.AddComponent<Image>();
            bg.color = new Color(0.3f, 0.3f, 0.35f, 1f);

            // Button
            var button = root.AddComponent<Button>();
            button.targetGraphic = bg;
            var colors = button.colors;
            colors.normalColor = new Color(0.3f, 0.3f, 0.35f);
            colors.highlightedColor = new Color(0.4f, 0.4f, 0.45f);
            colors.pressedColor = new Color(0.25f, 0.25f, 0.3f);
            button.colors = colors;

            // Label
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(root.transform, false);
            var labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(10, 5);
            labelRect.offsetMax = new Vector2(-10, -5);

            var labelText = labelGo.AddComponent<TextMeshProUGUI>();
            labelText.text = "Tab";
            labelText.fontSize = 18;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;

            // Selected Indicator
            var indicatorGo = new GameObject("SelectedIndicator");
            indicatorGo.transform.SetParent(root.transform, false);
            var indicatorRect = indicatorGo.AddComponent<RectTransform>();
            indicatorRect.anchorMin = new Vector2(0, 0);
            indicatorRect.anchorMax = new Vector2(1, 0);
            indicatorRect.pivot = new Vector2(0.5f, 0);
            indicatorRect.sizeDelta = new Vector2(0, 3);
            indicatorRect.anchoredPosition = Vector2.zero;

            var indicatorImage = indicatorGo.AddComponent<Image>();
            indicatorImage.color = new Color(0.3f, 0.7f, 1f, 1f);
            indicatorGo.SetActive(false);

            // Badge
            var badgeGo = new GameObject("Badge");
            badgeGo.transform.SetParent(root.transform, false);
            var badgeRect = badgeGo.AddComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(1, 1);
            badgeRect.anchorMax = new Vector2(1, 1);
            badgeRect.pivot = new Vector2(1, 1);
            badgeRect.sizeDelta = new Vector2(24, 24);
            badgeRect.anchoredPosition = new Vector2(-5, -5);

            var badgeImage = badgeGo.AddComponent<Image>();
            badgeImage.color = new Color(1f, 0.3f, 0.3f, 1f);

            // Badge Count Text
            var badgeTextGo = new GameObject("BadgeCount");
            badgeTextGo.transform.SetParent(badgeGo.transform, false);
            var badgeTextRect = badgeTextGo.AddComponent<RectTransform>();
            badgeTextRect.anchorMin = Vector2.zero;
            badgeTextRect.anchorMax = Vector2.one;
            badgeTextRect.offsetMin = Vector2.zero;
            badgeTextRect.offsetMax = Vector2.zero;

            var badgeCountText = badgeTextGo.AddComponent<TextMeshProUGUI>();
            badgeCountText.text = "1";
            badgeCountText.fontSize = 12;
            badgeCountText.alignment = TextAlignmentOptions.Center;
            badgeCountText.color = Color.white;
            badgeTextGo.SetActive(false);

            badgeGo.SetActive(false);

            // TabButton 컴포넌트 추가
            var tabButton = root.AddComponent<TabButton>();

            // SerializedObject로 private 필드 설정
            var so = new SerializedObject(tabButton);
            so.FindProperty("_button").objectReferenceValue = button;
            so.FindProperty("_labelText").objectReferenceValue = labelText;
            so.FindProperty("_backgroundImage").objectReferenceValue = bg;
            so.FindProperty("_selectedIndicator").objectReferenceValue = indicatorGo;
            so.FindProperty("_badge").objectReferenceValue = badgeGo;
            so.FindProperty("_badgeCountText").objectReferenceValue = badgeCountText;
            so.ApplyModifiedPropertiesWithoutUndo();

            return root;
        }

        #endregion

        #region LobbyScreen Setup

        public static bool SetupLobbyScreenPrefab()
        {
            var prefabPath = $"{MVP_PATH}/LobbyScreen.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError($"[LobbyScreenSetup] LobbyScreen 프리팹을 찾을 수 없음: {prefabPath}");
                return false;
            }

            // 프리팹 인스턴스 생성
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            try
            {
                // LobbyScreen 컴포넌트 가져오기
                var lobbyScreen = instance.GetComponent<LobbyScreen>();
                if (lobbyScreen == null)
                {
                    Debug.LogError("[LobbyScreenSetup] LobbyScreen 컴포넌트를 찾을 수 없음");
                    return false;
                }

                // 이미 탭 시스템이 설정되어 있는지 확인
                var existingTabGroup = instance.GetComponentInChildren<TabGroupWidget>();
                if (existingTabGroup != null)
                {
                    Debug.Log("[LobbyScreenSetup] 탭 시스템이 이미 설정되어 있음");
                    return true;
                }

                // TabGroupWidget 생성
                var tabGroupGo = CreateTabGroupWidget(instance.transform);

                // Tab Contents 생성
                var tabContents = CreateTabContents(instance.transform);

                // TabButton 프리팹 로드
                var tabButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{WIDGETS_PATH}/TabButton.prefab");
                if (tabButtonPrefab == null)
                {
                    Debug.LogWarning("[LobbyScreenSetup] TabButton 프리팹을 찾을 수 없음. 먼저 생성하세요.");
                }

                // TabGroupWidget 설정
                var tabGroupWidget = tabGroupGo.GetComponent<TabGroupWidget>();
                var tabGroupSo = new SerializedObject(tabGroupWidget);
                tabGroupSo.FindProperty("_tabButtonContainer").objectReferenceValue =
                    tabGroupGo.transform.Find("TabBar");

                if (tabButtonPrefab != null)
                {
                    var tabButtonComponent = tabButtonPrefab.GetComponent<TabButton>();
                    tabGroupSo.FindProperty("_tabButtonPrefab").objectReferenceValue = tabButtonComponent;
                }

                tabGroupSo.ApplyModifiedPropertiesWithoutUndo();

                // LobbyScreen에 탭 시스템 연결
                var lobbySo = new SerializedObject(lobbyScreen);
                lobbySo.FindProperty("_tabGroup").objectReferenceValue = tabGroupWidget;

                var tabContentsProperty = lobbySo.FindProperty("_tabContents");
                tabContentsProperty.arraySize = tabContents.Length;
                for (int i = 0; i < tabContents.Length; i++)
                {
                    tabContentsProperty.GetArrayElementAtIndex(i).objectReferenceValue = tabContents[i];
                }

                lobbySo.ApplyModifiedPropertiesWithoutUndo();

                // 프리팹에 적용
                PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.UserAction);

                Debug.Log("[LobbyScreenSetup] LobbyScreen 탭 시스템 설정 완료");
                return true;
            }
            finally
            {
                Object.DestroyImmediate(instance);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private static GameObject CreateTabGroupWidget(Transform parent)
        {
            // TabGroupWidget 루트
            var tabGroupGo = new GameObject("TabSystem");
            tabGroupGo.transform.SetParent(parent, false);
            var tabGroupRect = tabGroupGo.AddComponent<RectTransform>();
            tabGroupRect.anchorMin = Vector2.zero;
            tabGroupRect.anchorMax = Vector2.one;
            tabGroupRect.offsetMin = Vector2.zero;
            tabGroupRect.offsetMax = Vector2.zero;

            // Tab Content Area (중앙)
            var contentAreaGo = new GameObject("TabContentArea");
            contentAreaGo.transform.SetParent(tabGroupGo.transform, false);
            var contentAreaRect = contentAreaGo.AddComponent<RectTransform>();
            contentAreaRect.anchorMin = new Vector2(0, 0.1f);
            contentAreaRect.anchorMax = Vector2.one;
            contentAreaRect.offsetMin = Vector2.zero;
            contentAreaRect.offsetMax = Vector2.zero;

            // Tab Bar (하단)
            var tabBarGo = new GameObject("TabBar");
            tabBarGo.transform.SetParent(tabGroupGo.transform, false);
            var tabBarRect = tabBarGo.AddComponent<RectTransform>();
            tabBarRect.anchorMin = Vector2.zero;
            tabBarRect.anchorMax = new Vector2(1, 0.1f);
            tabBarRect.offsetMin = Vector2.zero;
            tabBarRect.offsetMax = Vector2.zero;

            // TabBar Background
            var tabBarBg = tabBarGo.AddComponent<Image>();
            tabBarBg.color = new Color(0.15f, 0.15f, 0.2f, 1f);

            // HorizontalLayoutGroup
            var layout = tabBarGo.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            layout.spacing = 5;
            layout.padding = new RectOffset(10, 10, 5, 5);

            // TabGroupWidget 컴포넌트 추가
            tabGroupGo.AddComponent<TabGroupWidget>();

            return tabGroupGo;
        }

        private static LobbyTabContent[] CreateTabContents(Transform parent)
        {
            var tabSystem = parent.Find("TabSystem");
            var contentArea = tabSystem?.Find("TabContentArea");

            if (contentArea == null)
            {
                Debug.LogError("[LobbyScreenSetup] TabContentArea를 찾을 수 없음");
                return new LobbyTabContent[0];
            }

            var contents = new LobbyTabContent[4];

            // Home Tab
            contents[0] = CreateTabContent<HomeTabContent>(contentArea, "HomeTab");

            // Character Tab
            contents[1] = CreateTabContent<CharacterTabContent>(contentArea, "CharacterTab");

            // Gacha Tab
            contents[2] = CreateTabContent<GachaTabContent>(contentArea, "GachaTab");

            // Settings Tab
            contents[3] = CreateTabContent<SettingsTabContent>(contentArea, "SettingsTab");

            // 모든 탭 비활성화 (첫 번째만 활성화는 런타임에)
            foreach (var content in contents)
            {
                content.gameObject.SetActive(false);
            }

            return contents;
        }

        private static T CreateTabContent<T>(Transform parent, string name) where T : LobbyTabContent
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // 배경
            var bg = go.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.5f);

            // 플레이스홀더 텍스트
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0.5f, 0.5f);
            labelRect.anchorMax = new Vector2(0.5f, 0.5f);
            labelRect.sizeDelta = new Vector2(300, 50);

            var labelText = labelGo.AddComponent<TextMeshProUGUI>();
            labelText.text = name.Replace("Tab", " Tab");
            labelText.fontSize = 24;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;

            // 컴포넌트 추가
            return go.AddComponent<T>();
        }

        #endregion

        #region Helpers

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }
        }

        #endregion
    }
}
