using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using Sc.Common.UI;
using Sc.Common.UI.Tests;

namespace Sc.Editor.AI
{
    /// <summary>
    /// SystemPopup 테스트 환경 자동 생성 에디터 도구.
    /// SC Tools/SystemPopup 메뉴에서 접근.
    /// </summary>
    public static class SystemPopupSetup
    {
        private const string MENU_PATH = "SC Tools/SystemPopup/";
        private const string FONT_PATH = "Fonts & Materials/PretendardVariable SDF";

        private static TMP_FontAsset _cachedFont;

        private static TMP_FontAsset GetProjectFont()
        {
            if (_cachedFont == null)
            {
                _cachedFont = Resources.Load<TMP_FontAsset>(FONT_PATH);
                if (_cachedFont == null)
                {
                    Debug.LogWarning($"[SystemPopupSetup] 프로젝트 폰트를 찾을 수 없습니다: {FONT_PATH}. 기본 폰트를 사용합니다.");
                }
            }
            return _cachedFont;
        }

        [MenuItem(MENU_PATH + "Create Test Scene Setup", false, 100)]
        public static void CreateTestSceneSetup()
        {
            // Canvas 생성 또는 찾기
            var canvas = FindOrCreateCanvas("SystemPopupTestCanvas");

            // 팝업 프리팹들 생성
            CreateConfirmPopupPrefab(canvas.transform);
            CreateCostConfirmPopupPrefab(canvas.transform);

            // 테스트 컨트롤러 생성
            CreateTestController();

            Debug.Log("[SystemPopupSetup] 테스트 환경 생성 완료. Play 모드에서 OnGUI 버튼으로 테스트하세요.");
        }

        [MenuItem(MENU_PATH + "Create ConfirmPopup Prefab", false, 200)]
        public static void CreateConfirmPopupPrefab()
        {
            var canvas = FindOrCreateCanvas("PopupCanvas");
            CreateConfirmPopupPrefab(canvas.transform);
        }

        [MenuItem(MENU_PATH + "Create CostConfirmPopup Prefab", false, 201)]
        public static void CreateCostConfirmPopupPrefab()
        {
            var canvas = FindOrCreateCanvas("PopupCanvas");
            CreateCostConfirmPopupPrefab(canvas.transform);
        }

        #region Prefab Creation

        private static void CreateConfirmPopupPrefab(Transform parent)
        {
            // 기존 오브젝트 제거
            var existing = parent.Find("ConfirmPopup");
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            // 루트 생성
            var root = CreateUIObject("ConfirmPopup", parent);
            var popup = root.AddComponent<ConfirmPopup>();
            root.SetActive(false); // 초기 비활성화

            // Canvas 추가 (PopupWidget 요구사항)
            var canvas = root.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 100;
            root.AddComponent<GraphicRaycaster>();
            root.AddComponent<CanvasGroup>();

            var rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            // Background (터치 시 취소)
            var bgObj = CreateUIObject("Background", root.transform);
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.5f);
            var bgButton = bgObj.AddComponent<Button>();
            bgButton.transition = Selectable.Transition.None;
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // Panel
            var panel = CreateUIObject("Panel", root.transform);
            var panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(400, 250);

            // Title
            var titleObj = CreateUIObject("TitleText", panel.transform);
            var titleText = CreateText(titleObj, "확인", 28);
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.fontStyle = FontStyles.Bold;
            var titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, -20);
            titleRect.sizeDelta = new Vector2(-40, 40);

            // Message
            var msgObj = CreateUIObject("MessageText", panel.transform);
            var msgText = CreateText(msgObj, "메시지", 20);
            msgText.alignment = TextAlignmentOptions.Center;
            var msgRect = msgObj.GetComponent<RectTransform>();
            msgRect.anchorMin = new Vector2(0, 0.3f);
            msgRect.anchorMax = new Vector2(1, 0.8f);
            msgRect.offsetMin = new Vector2(20, 0);
            msgRect.offsetMax = new Vector2(-20, 0);

            // Button Group
            var btnGroup = CreateUIObject("ButtonGroup", panel.transform);
            var btnGroupRect = btnGroup.GetComponent<RectTransform>();
            btnGroupRect.anchorMin = new Vector2(0, 0);
            btnGroupRect.anchorMax = new Vector2(1, 0.3f);
            btnGroupRect.offsetMin = new Vector2(20, 20);
            btnGroupRect.offsetMax = new Vector2(-20, -10);
            var hlg = btnGroup.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 20;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = true;

            // Cancel Button
            var cancelObj = CreateButton("CancelButton", btnGroup.transform, "취소", new Color(0.4f, 0.4f, 0.4f));
            var cancelBtnText = cancelObj.GetComponentInChildren<TextMeshProUGUI>();

            // Confirm Button
            var confirmObj = CreateButton("ConfirmButton", btnGroup.transform, "확인", new Color(0.2f, 0.6f, 0.2f));
            var confirmBtnText = confirmObj.GetComponentInChildren<TextMeshProUGUI>();

            // SerializedObject로 참조 연결
            var so = new SerializedObject(popup);
            so.FindProperty("_titleText").objectReferenceValue = titleText;
            so.FindProperty("_messageText").objectReferenceValue = msgText;
            so.FindProperty("_confirmButton").objectReferenceValue = confirmObj.GetComponent<Button>();
            so.FindProperty("_cancelButton").objectReferenceValue = cancelObj.GetComponent<Button>();
            so.FindProperty("_confirmButtonText").objectReferenceValue = confirmBtnText;
            so.FindProperty("_cancelButtonText").objectReferenceValue = cancelBtnText;
            so.FindProperty("_backgroundButton").objectReferenceValue = bgButton;
            so.ApplyModifiedProperties();

            Debug.Log("[SystemPopupSetup] ConfirmPopup 생성 완료");
        }

        private static void CreateCostConfirmPopupPrefab(Transform parent)
        {
            // 기존 오브젝트 제거
            var existing = parent.Find("CostConfirmPopup");
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            // 루트 생성
            var root = CreateUIObject("CostConfirmPopup", parent);
            var popup = root.AddComponent<CostConfirmPopup>();
            root.SetActive(false);

            // Canvas 추가
            var canvas = root.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 100;
            root.AddComponent<GraphicRaycaster>();
            root.AddComponent<CanvasGroup>();

            var rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            // Background
            var bgObj = CreateUIObject("Background", root.transform);
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.5f);
            var bgButton = bgObj.AddComponent<Button>();
            bgButton.transition = Selectable.Transition.None;
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // Panel
            var panel = CreateUIObject("Panel", root.transform);
            var panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(400, 300);

            // Title
            var titleObj = CreateUIObject("TitleText", panel.transform);
            var titleText = CreateText(titleObj, "확인", 28);
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.fontStyle = FontStyles.Bold;
            var titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, -20);
            titleRect.sizeDelta = new Vector2(-40, 40);

            // Message
            var msgObj = CreateUIObject("MessageText", panel.transform);
            var msgText = CreateText(msgObj, "메시지", 18);
            msgText.alignment = TextAlignmentOptions.Center;
            var msgRect = msgObj.GetComponent<RectTransform>();
            msgRect.anchorMin = new Vector2(0, 0.55f);
            msgRect.anchorMax = new Vector2(1, 0.8f);
            msgRect.offsetMin = new Vector2(20, 0);
            msgRect.offsetMax = new Vector2(-20, 0);

            // Cost Display
            var costDisplay = CreateUIObject("CostDisplay", panel.transform);
            var costRect = costDisplay.GetComponent<RectTransform>();
            costRect.anchorMin = new Vector2(0, 0.35f);
            costRect.anchorMax = new Vector2(1, 0.55f);
            costRect.offsetMin = new Vector2(20, 0);
            costRect.offsetMax = new Vector2(-20, 0);
            var costHlg = costDisplay.AddComponent<HorizontalLayoutGroup>();
            costHlg.spacing = 10;
            costHlg.childAlignment = TextAnchor.MiddleCenter;
            costHlg.childControlWidth = false;
            costHlg.childControlHeight = false;

            // Cost Icon
            var iconObj = CreateUIObject("CostIcon", costDisplay.transform);
            var iconImage = iconObj.AddComponent<Image>();
            iconImage.color = Color.yellow;
            var iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(40, 40);

            // Cost Amount Text
            var costAmtObj = CreateUIObject("CostAmountText", costDisplay.transform);
            var costAmtText = CreateText(costAmtObj, "-100", 24);
            costAmtText.fontStyle = FontStyles.Bold;
            costAmtText.alignment = TextAlignmentOptions.Left;
            var costAmtRect = costAmtObj.GetComponent<RectTransform>();
            costAmtRect.sizeDelta = new Vector2(100, 40);

            // Current Amount Text
            var curAmtObj = CreateUIObject("CurrentAmountText", costDisplay.transform);
            var curAmtText = CreateText(curAmtObj, "(보유: 500)", 16);
            curAmtText.color = new Color(0.7f, 0.7f, 0.7f);
            curAmtText.alignment = TextAlignmentOptions.Left;
            var curAmtRect = curAmtObj.GetComponent<RectTransform>();
            curAmtRect.sizeDelta = new Vector2(120, 40);

            // Button Group
            var btnGroup = CreateUIObject("ButtonGroup", panel.transform);
            var btnGroupRect = btnGroup.GetComponent<RectTransform>();
            btnGroupRect.anchorMin = new Vector2(0, 0);
            btnGroupRect.anchorMax = new Vector2(1, 0.3f);
            btnGroupRect.offsetMin = new Vector2(20, 20);
            btnGroupRect.offsetMax = new Vector2(-20, -10);
            var hlg = btnGroup.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 20;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = true;

            // Cancel Button
            var cancelObj = CreateButton("CancelButton", btnGroup.transform, "취소", new Color(0.4f, 0.4f, 0.4f));
            var cancelBtnText = cancelObj.GetComponentInChildren<TextMeshProUGUI>();

            // Confirm Button
            var confirmObj = CreateButton("ConfirmButton", btnGroup.transform, "확인", new Color(0.2f, 0.6f, 0.2f));
            var confirmBtnText = confirmObj.GetComponentInChildren<TextMeshProUGUI>();

            // SerializedObject로 참조 연결
            var so = new SerializedObject(popup);
            so.FindProperty("_titleText").objectReferenceValue = titleText;
            so.FindProperty("_messageText").objectReferenceValue = msgText;
            so.FindProperty("_confirmButton").objectReferenceValue = confirmObj.GetComponent<Button>();
            so.FindProperty("_cancelButton").objectReferenceValue = cancelObj.GetComponent<Button>();
            so.FindProperty("_confirmButtonText").objectReferenceValue = confirmBtnText;
            so.FindProperty("_cancelButtonText").objectReferenceValue = cancelBtnText;
            so.FindProperty("_backgroundButton").objectReferenceValue = bgButton;
            so.FindProperty("_costIcon").objectReferenceValue = iconImage;
            so.FindProperty("_costAmountText").objectReferenceValue = costAmtText;
            so.FindProperty("_currentAmountText").objectReferenceValue = curAmtText;
            so.ApplyModifiedProperties();

            Debug.Log("[SystemPopupSetup] CostConfirmPopup 생성 완료");
        }

        private static void CreateTestController()
        {
            var existing = GameObject.Find("SystemPopupTestController");
            if (existing != null) Object.DestroyImmediate(existing);

            var go = new GameObject("SystemPopupTestController");
            go.AddComponent<SystemPopupTestController>();

            Debug.Log("[SystemPopupSetup] TestController 생성 완료 - 키보드 1~7로 테스트");
        }

        #endregion

        #region Helper Methods

        private static Canvas FindOrCreateCanvas(string name)
        {
            var existing = GameObject.Find(name);
            if (existing != null) return existing.GetComponent<Canvas>();

            var canvasGo = new GameObject(name);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            return canvas;
        }

        private static GameObject CreateUIObject(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            return go;
        }

        private static GameObject CreateButton(string name, Transform parent, string text, Color bgColor)
        {
            var btnObj = CreateUIObject(name, parent);
            var btnImage = btnObj.AddComponent<Image>();
            btnImage.color = bgColor;
            btnObj.AddComponent<Button>();

            var txtObj = CreateUIObject("Text", btnObj.transform);
            var txtComp = CreateText(txtObj, text, 20);
            txtComp.alignment = TextAlignmentOptions.Center;
            var txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            return btnObj;
        }

        private static TextMeshProUGUI CreateText(GameObject obj, string text, float fontSize)
        {
            var tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;

            var font = GetProjectFont();
            if (font != null)
            {
                tmp.font = font;
            }

            return tmp;
        }

        #endregion
    }
}
