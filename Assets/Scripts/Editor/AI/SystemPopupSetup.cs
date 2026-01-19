using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using Sc.Common.UI;
using Sc.Common.UI.Tests;

namespace Sc.Editor.AI
{
    /// <summary>
    /// SystemPopup 프리팹 생성 에디터 도구.
    /// SC Tools/Setup/Prefabs/Dialog 메뉴에서 접근.
    /// Bootstrap: None (프리팹 생성 전용)
    /// </summary>
    public static class SystemPopupSetup
    {
        [MenuItem("SC Tools/Setup/Prefabs/Dialog/Create All Dialog Prefabs", false, 140)]
        public static void CreateAllDialogPrefabs()
        {
            var canvas = EditorUIHelpers.FindOrCreateCanvas("SystemPopupTestCanvas");

            CreateConfirmPopupPrefab(canvas.transform);
            CreateCostConfirmPopupPrefab(canvas.transform);
            CreateTestController();

            Debug.Log("[SystemPopupSetup] 모든 Dialog 프리팹 생성 완료");
        }

        [MenuItem("SC Tools/Setup/Prefabs/Dialog/Create ConfirmPopup Prefab", false, 141)]
        public static void CreateConfirmPopupPrefab()
        {
            var canvas = EditorUIHelpers.FindOrCreateCanvas("PopupCanvas");
            CreateConfirmPopupPrefab(canvas.transform);
        }

        [MenuItem("SC Tools/Setup/Prefabs/Dialog/Create CostConfirmPopup Prefab", false, 142)]
        public static void CreateCostConfirmPopupPrefab()
        {
            var canvas = EditorUIHelpers.FindOrCreateCanvas("PopupCanvas");
            CreateCostConfirmPopupPrefab(canvas.transform);
        }

        #region Prefab Creation

        private static void CreateConfirmPopupPrefab(Transform parent)
        {
            var existing = parent.Find("ConfirmPopup");
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            // Root
            var root = EditorUIHelpers.CreateUIObject("ConfirmPopup", parent);
            var popup = root.AddComponent<ConfirmPopup>();
            root.SetActive(false);

            // Canvas + Raycaster
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
            var bgObj = EditorUIHelpers.CreateUIObject("Background", root.transform);
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
            var panel = EditorUIHelpers.CreateCenteredPanel("Panel", new Vector2(400, 250),
                new Color(0.2f, 0.2f, 0.2f, 1f), root.transform);

            // Title
            var titleObj = EditorUIHelpers.CreateUIObject("TitleText", panel.transform);
            var titleText = EditorUIHelpers.AddText(titleObj, "확인", 28);
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.fontStyle = FontStyles.Bold;
            var titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, -20);
            titleRect.sizeDelta = new Vector2(-40, 40);

            // Message
            var msgObj = EditorUIHelpers.CreateUIObject("MessageText", panel.transform);
            var msgText = EditorUIHelpers.AddText(msgObj, "메시지", 20);
            msgText.alignment = TextAlignmentOptions.Center;
            var msgRect = msgObj.GetComponent<RectTransform>();
            msgRect.anchorMin = new Vector2(0, 0.3f);
            msgRect.anchorMax = new Vector2(1, 0.8f);
            msgRect.offsetMin = new Vector2(20, 0);
            msgRect.offsetMax = new Vector2(-20, 0);

            // Button Group
            var btnGroup = EditorUIHelpers.CreateUIObject("ButtonGroup", panel.transform);
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

            // Buttons
            var cancelBtn = EditorUIHelpers.CreateLayoutButton(btnGroup.transform, "CancelButton", "취소",
                new Color(0.4f, 0.4f, 0.4f));
            var cancelBtnText = cancelBtn.GetComponentInChildren<TextMeshProUGUI>();

            var confirmBtn = EditorUIHelpers.CreateLayoutButton(btnGroup.transform, "ConfirmButton", "확인",
                new Color(0.2f, 0.6f, 0.2f));
            var confirmBtnText = confirmBtn.GetComponentInChildren<TextMeshProUGUI>();

            // Serialize references
            var so = new SerializedObject(popup);
            so.FindProperty("_titleText").objectReferenceValue = titleText;
            so.FindProperty("_messageText").objectReferenceValue = msgText;
            so.FindProperty("_confirmButton").objectReferenceValue = confirmBtn;
            so.FindProperty("_cancelButton").objectReferenceValue = cancelBtn;
            so.FindProperty("_confirmButtonText").objectReferenceValue = confirmBtnText;
            so.FindProperty("_cancelButtonText").objectReferenceValue = cancelBtnText;
            so.FindProperty("_backgroundButton").objectReferenceValue = bgButton;
            so.ApplyModifiedProperties();

            Debug.Log("[SystemPopupSetup] ConfirmPopup 생성 완료");
        }

        private static void CreateCostConfirmPopupPrefab(Transform parent)
        {
            var existing = parent.Find("CostConfirmPopup");
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            // Root
            var root = EditorUIHelpers.CreateUIObject("CostConfirmPopup", parent);
            var popup = root.AddComponent<CostConfirmPopup>();
            root.SetActive(false);

            // Canvas + Raycaster
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
            var bgObj = EditorUIHelpers.CreateUIObject("Background", root.transform);
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
            var panel = EditorUIHelpers.CreateCenteredPanel("Panel", new Vector2(400, 300),
                new Color(0.2f, 0.2f, 0.2f, 1f), root.transform);

            // Title
            var titleObj = EditorUIHelpers.CreateUIObject("TitleText", panel.transform);
            var titleText = EditorUIHelpers.AddText(titleObj, "확인", 28);
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.fontStyle = FontStyles.Bold;
            var titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, -20);
            titleRect.sizeDelta = new Vector2(-40, 40);

            // Message
            var msgObj = EditorUIHelpers.CreateUIObject("MessageText", panel.transform);
            var msgText = EditorUIHelpers.AddText(msgObj, "메시지", 18);
            msgText.alignment = TextAlignmentOptions.Center;
            var msgRect = msgObj.GetComponent<RectTransform>();
            msgRect.anchorMin = new Vector2(0, 0.55f);
            msgRect.anchorMax = new Vector2(1, 0.8f);
            msgRect.offsetMin = new Vector2(20, 0);
            msgRect.offsetMax = new Vector2(-20, 0);

            // Cost Display
            var costDisplay = EditorUIHelpers.CreateUIObject("CostDisplay", panel.transform);
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
            var iconObj = EditorUIHelpers.CreateUIObject("CostIcon", costDisplay.transform);
            var iconImage = iconObj.AddComponent<Image>();
            iconImage.color = Color.yellow;
            iconObj.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);

            // Cost Amount
            var costAmtObj = EditorUIHelpers.CreateUIObject("CostAmountText", costDisplay.transform);
            var costAmtText = EditorUIHelpers.AddText(costAmtObj, "-100", 24);
            costAmtText.fontStyle = FontStyles.Bold;
            costAmtText.alignment = TextAlignmentOptions.Left;
            costAmtObj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 40);

            // Current Amount
            var curAmtObj = EditorUIHelpers.CreateUIObject("CurrentAmountText", costDisplay.transform);
            var curAmtText = EditorUIHelpers.AddText(curAmtObj, "(보유: 500)", 16);
            curAmtText.color = new Color(0.7f, 0.7f, 0.7f);
            curAmtText.alignment = TextAlignmentOptions.Left;
            curAmtObj.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 40);

            // Button Group
            var btnGroup = EditorUIHelpers.CreateUIObject("ButtonGroup", panel.transform);
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

            // Buttons
            var cancelBtn = EditorUIHelpers.CreateLayoutButton(btnGroup.transform, "CancelButton", "취소",
                new Color(0.4f, 0.4f, 0.4f));
            var cancelBtnText = cancelBtn.GetComponentInChildren<TextMeshProUGUI>();

            var confirmBtn = EditorUIHelpers.CreateLayoutButton(btnGroup.transform, "ConfirmButton", "확인",
                new Color(0.2f, 0.6f, 0.2f));
            var confirmBtnText = confirmBtn.GetComponentInChildren<TextMeshProUGUI>();

            // Serialize references
            var so = new SerializedObject(popup);
            so.FindProperty("_titleText").objectReferenceValue = titleText;
            so.FindProperty("_messageText").objectReferenceValue = msgText;
            so.FindProperty("_confirmButton").objectReferenceValue = confirmBtn;
            so.FindProperty("_cancelButton").objectReferenceValue = cancelBtn;
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
    }
}
