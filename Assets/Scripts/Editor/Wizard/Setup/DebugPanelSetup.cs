using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Sc.Editor.Wizard
{
    /// <summary>
    /// 디버그 패널 셋업 도구.
    /// Main 씬에 DebugNavigationPanel을 추가합니다.
    /// </summary>
    public static class DebugPanelSetup
    {
        public static void AddDebugNavigationPanel()
        {
            // UIRoot 찾기
            var uiRoot = GameObject.Find("UIRoot");
            if (uiRoot == null)
            {
                EditorUtility.DisplayDialog("Error", "UIRoot가 씬에 없습니다. Main 씬에서 실행해주세요.", "OK");
                return;
            }

            // 기존 DebugCanvas 확인
            var existingDebug = GameObject.Find("DebugCanvas");
            if (existingDebug != null)
            {
                EditorUtility.DisplayDialog("Info", "DebugCanvas가 이미 존재합니다.", "OK");
                Selection.activeGameObject = existingDebug;
                return;
            }

            // DebugCanvas 생성
            var debugCanvas = new GameObject("DebugCanvas");
            debugCanvas.transform.SetParent(uiRoot.transform, false);

            var canvas = debugCanvas.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 200;

            var canvasScaler = debugCanvas.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            debugCanvas.AddComponent<GraphicRaycaster>();

            // Panel 생성
            var panel = new GameObject("Panel");
            panel.transform.SetParent(debugCanvas.transform, false);

            var panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = new Vector2(100, 100);
            panelRect.offsetMax = new Vector2(-100, -100);

            var panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.9f);

            // Header
            var header = CreateText(panel.transform, "Header", "Debug Navigation (F12 to toggle)", 24);
            var headerRect = header.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.anchoredPosition = new Vector2(0, -10);
            headerRect.sizeDelta = new Vector2(0, 40);

            // Screen Buttons Container
            var screenLabel = CreateText(panel.transform, "ScreenLabel", "-- Screens --", 18);
            var screenLabelRect = screenLabel.GetComponent<RectTransform>();
            screenLabelRect.anchorMin = new Vector2(0, 1);
            screenLabelRect.anchorMax = new Vector2(0.5f, 1);
            screenLabelRect.pivot = new Vector2(0.5f, 1);
            screenLabelRect.anchoredPosition = new Vector2(0, -60);
            screenLabelRect.sizeDelta = new Vector2(0, 30);

            var screenContainer = new GameObject("ScreenButtonContainer");
            screenContainer.transform.SetParent(panel.transform, false);
            var screenContainerRect = screenContainer.AddComponent<RectTransform>();
            screenContainerRect.anchorMin = new Vector2(0, 0);
            screenContainerRect.anchorMax = new Vector2(0.5f, 1);
            screenContainerRect.pivot = new Vector2(0, 1);
            screenContainerRect.anchoredPosition = new Vector2(20, -100);
            screenContainerRect.sizeDelta = new Vector2(-40, -120);

            var screenLayout = screenContainer.AddComponent<VerticalLayoutGroup>();
            screenLayout.spacing = 5;
            screenLayout.childControlWidth = true;
            screenLayout.childControlHeight = false;
            screenLayout.childForceExpandWidth = true;
            screenLayout.childForceExpandHeight = false;

            // Popup Buttons Container
            var popupLabel = CreateText(panel.transform, "PopupLabel", "-- Popups --", 18);
            var popupLabelRect = popupLabel.GetComponent<RectTransform>();
            popupLabelRect.anchorMin = new Vector2(0.5f, 1);
            popupLabelRect.anchorMax = new Vector2(1, 1);
            popupLabelRect.pivot = new Vector2(0.5f, 1);
            popupLabelRect.anchoredPosition = new Vector2(0, -60);
            popupLabelRect.sizeDelta = new Vector2(0, 30);

            var popupContainer = new GameObject("PopupButtonContainer");
            popupContainer.transform.SetParent(panel.transform, false);
            var popupContainerRect = popupContainer.AddComponent<RectTransform>();
            popupContainerRect.anchorMin = new Vector2(0.5f, 0);
            popupContainerRect.anchorMax = new Vector2(1, 1);
            popupContainerRect.pivot = new Vector2(0, 1);
            popupContainerRect.anchoredPosition = new Vector2(20, -100);
            popupContainerRect.sizeDelta = new Vector2(-40, -120);

            var popupLayout = popupContainer.AddComponent<VerticalLayoutGroup>();
            popupLayout.spacing = 5;
            popupLayout.childControlWidth = true;
            popupLayout.childControlHeight = false;
            popupLayout.childForceExpandWidth = true;
            popupLayout.childForceExpandHeight = false;

            // Button Prefab 생성
            var buttonPrefab = CreateButtonPrefab(debugCanvas.transform);
            buttonPrefab.SetActive(false);

            // DebugNavigationPanel 컴포넌트 추가
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var debugPanel = debugCanvas.AddComponent<Sc.DebugTools.DebugNavigationPanel>();

            // SerializeField 설정
            var serializedObj = new SerializedObject(debugPanel);
            serializedObj.FindProperty("_panel").objectReferenceValue = panel;
            serializedObj.FindProperty("_screenButtonContainer").objectReferenceValue = screenContainer.transform;
            serializedObj.FindProperty("_popupButtonContainer").objectReferenceValue = popupContainer.transform;
            serializedObj.FindProperty("_buttonPrefab").objectReferenceValue = buttonPrefab.GetComponent<Button>();
            serializedObj.ApplyModifiedProperties();
#endif

            // Panel 초기 비활성화
            panel.SetActive(false);

            EditorUtility.SetDirty(debugCanvas);
            Selection.activeGameObject = debugCanvas;

            UnityEngine.Debug.Log("[DebugPanelSetup] DebugNavigationPanel added to scene. Press F12 to toggle.");
            EditorUtility.DisplayDialog("Success", "DebugNavigationPanel이 추가되었습니다.\nF12 키로 토글할 수 있습니다.", "OK");
        }

        private static GameObject CreateText(Transform parent, string name, string text, int fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var textComp = go.AddComponent<TextMeshProUGUI>();
            textComp.text = text;
            textComp.fontSize = fontSize;
            textComp.alignment = TextAlignmentOptions.Center;
            textComp.color = Color.white;

            return go;
        }

        private static GameObject CreateButtonPrefab(Transform parent)
        {
            var button = new GameObject("ButtonPrefab");
            button.transform.SetParent(parent, false);

            var rect = button.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 35);

            var image = button.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            var btn = button.AddComponent<Button>();
            var colors = btn.colors;
            colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
            btn.colors = colors;

            var text = new GameObject("Text");
            text.transform.SetParent(button.transform, false);

            var textRect = text.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var textComp = text.AddComponent<TextMeshProUGUI>();
            textComp.text = "Button";
            textComp.fontSize = 14;
            textComp.alignment = TextAlignmentOptions.Center;
            textComp.color = Color.white;

            return button;
        }
    }
}