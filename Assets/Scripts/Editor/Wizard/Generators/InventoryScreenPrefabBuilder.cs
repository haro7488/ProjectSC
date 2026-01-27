using Sc.Contents.Inventory;
using Sc.Contents.Inventory.Widgets;
using Sc.Editor.AI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Editor.Wizard.Generators
{
    /// <summary>
    /// InventoryScreen 전용 프리팹 빌더.
    /// 밝은 녹색 테마 - 배낭 화면 UI 생성.
    /// </summary>
    public static class InventoryScreenPrefabBuilder
    {
        #region Colors (Bright Green Inventory Theme)

        // Background colors
        private static readonly Color BgLight = new Color32(200, 230, 200, 255);
        private static readonly Color BgPattern = new Color32(220, 240, 220, 255);
        private static readonly Color BgCard = Color.white;

        // Tab colors
        private static readonly Color TabSelected = new Color32(100, 180, 100, 255);
        private static readonly Color TabNormal = new Color32(240, 240, 240, 255);
        private static readonly Color TabTextSelected = Color.white;
        private static readonly Color TabTextNormal = new Color32(80, 80, 80, 255);

        // Filter bar colors
        private static readonly Color FilterBarBg = new Color32(245, 245, 245, 255);

        // Rarity colors
        private static readonly Color RarityCommon = new Color32(150, 200, 255, 255);
        private static readonly Color RarityRare = new Color32(150, 230, 150, 255);
        private static readonly Color RarityEpic = new Color32(255, 180, 100, 255);
        private static readonly Color RarityLegend = new Color32(200, 150, 255, 255);

        // Text colors
        private static readonly Color TextPrimary = new Color32(40, 40, 40, 255);
        private static readonly Color TextSecondary = new Color32(100, 100, 100, 255);
        private static readonly Color TextMuted = new Color32(150, 150, 150, 255);

        #endregion

        #region Constants

        private const float HEADER_HEIGHT = 60f;
        private const float LEFT_TAB_WIDTH = 120f;
        private const float FILTER_BAR_HEIGHT = 50f;
        private const float ITEM_DETAIL_WIDTH = 300f;
        private const int GRID_COLUMN_COUNT = 7;
        private const float ITEM_SLOT_SIZE = 80f;
        private const float ITEM_SLOT_SPACING = 8f;

        #endregion

        #region Font Helper

        /// <summary>
        /// TextMeshProUGUI에 프로젝트 폰트 적용.
        /// </summary>
        private static void ApplyFont(TextMeshProUGUI tmp)
        {
            var font = EditorUIHelpers.GetProjectFont();
            if (font != null) tmp.font = font;
        }

        #endregion

        /// <summary>
        /// InventoryScreen 프리팹용 GameObject 생성.
        /// </summary>
        public static GameObject Build()
        {
            var root = CreateRoot();

            // 1. Background
            CreateBackground(root);

            // 2. SafeArea
            var safeArea = CreateSafeArea(root);

            // 3. Header
            var (backButton, homeButton) = CreateHeader(safeArea);

            // 4. Content
            var content = CreateContent(safeArea);

            // 5. LeftSideTab
            var tabWidget = CreateLeftSideTab(content);

            // 6. MainArea
            var mainArea = CreateMainArea(content);

            // 7. FilterBar
            var (categoryDropdown, sortDropdown, settingsButton) = CreateFilterBar(mainArea);

            // 8. ItemGrid
            var (itemScrollRect, itemGridContainer, itemCardPrefab) = CreateItemGrid(mainArea);

            // 9. ItemDetailPanel
            var itemDetailWidget = CreateItemDetailPanel(content);

            // 10. EmptyState
            var (emptyStateObject, emptyStateText) = CreateEmptyState(mainArea);

            // 11. Connect SerializedFields
            ConnectSerializedFields(root, backButton, homeButton, categoryDropdown, sortDropdown,
                settingsButton, itemScrollRect, itemGridContainer, itemCardPrefab,
                emptyStateObject, emptyStateText, itemDetailWidget, tabWidget);

            return root;
        }

        #region Root

        private static GameObject CreateRoot()
        {
            var root = new GameObject("InventoryScreen");

            var rect = root.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            root.AddComponent<CanvasGroup>();
            root.AddComponent<InventoryScreen>();

            return root;
        }

        #endregion

        #region Background

        private static void CreateBackground(GameObject parent)
        {
            var bg = CreateChild(parent, "Background");
            SetStretch(bg);

            var bgImage = bg.AddComponent<Image>();
            bgImage.color = BgLight;
            bgImage.raycastTarget = false;

            // Pattern overlay
            var pattern = CreateChild(bg, "Pattern");
            SetStretch(pattern);

            var patternImage = pattern.AddComponent<Image>();
            patternImage.color = BgPattern;
            patternImage.raycastTarget = false;
        }

        #endregion

        #region SafeArea & Layout

        private static GameObject CreateSafeArea(GameObject parent)
        {
            var safeArea = CreateChild(parent, "SafeArea");
            SetStretch(safeArea);
            return safeArea;
        }

        private static (Button backButton, Button homeButton) CreateHeader(GameObject parent)
        {
            var header = CreateChild(parent, "Header");
            var headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.anchoredPosition = Vector2.zero;
            headerRect.sizeDelta = new Vector2(0, HEADER_HEIGHT);

            // Background
            var headerBg = header.AddComponent<Image>();
            headerBg.color = BgCard;
            headerBg.raycastTarget = false;

            // BackButton
            var backButton = CreateHeaderButton(header, "BackButton", "<", true);

            // TitleText
            CreateTitleText(header, "배낭");

            // CurrencyHUD placeholder
            CreateCurrencyHUD(header);

            // HomeButton
            var homeButton = CreateHeaderButton(header, "HomeButton", "H", false);

            return (backButton, homeButton);
        }

        private static Button CreateHeaderButton(GameObject parent, string name, string text, bool isLeft)
        {
            var btn = CreateChild(parent, name);
            var btnRect = btn.AddComponent<RectTransform>();

            if (isLeft)
            {
                btnRect.anchorMin = new Vector2(0, 0.5f);
                btnRect.anchorMax = new Vector2(0, 0.5f);
                btnRect.pivot = new Vector2(0, 0.5f);
                btnRect.anchoredPosition = new Vector2(10, 0);
            }
            else
            {
                btnRect.anchorMin = new Vector2(1, 0.5f);
                btnRect.anchorMax = new Vector2(1, 0.5f);
                btnRect.pivot = new Vector2(1, 0.5f);
                btnRect.anchoredPosition = new Vector2(-10, 0);
            }

            btnRect.sizeDelta = new Vector2(40, 40);

            var btnImage = btn.AddComponent<Image>();
            btnImage.color = TabNormal;
            btnImage.raycastTarget = true;

            var button = btn.AddComponent<Button>();
            button.targetGraphic = btnImage;

            // Text
            var textObj = CreateChild(btn, "Text");
            SetStretch(textObj);

            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 18;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.raycastTarget = false;
            ApplyFont(tmp);

            return button;
        }

        private static void CreateTitleText(GameObject parent, string title)
        {
            var titleObj = CreateChild(parent, "TitleText");
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(200, 40);

            var tmp = titleObj.AddComponent<TextMeshProUGUI>();
            tmp.text = title;
            tmp.fontSize = 24;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.raycastTarget = false;
            ApplyFont(tmp);
        }

        private static void CreateCurrencyHUD(GameObject parent)
        {
            var hud = CreateChild(parent, "CurrencyHUD");
            var hudRect = hud.AddComponent<RectTransform>();
            hudRect.anchorMin = new Vector2(1, 0.5f);
            hudRect.anchorMax = new Vector2(1, 0.5f);
            hudRect.pivot = new Vector2(1, 0.5f);
            hudRect.anchoredPosition = new Vector2(-60, 0);
            hudRect.sizeDelta = new Vector2(400, 40);

            var hudImage = hud.AddComponent<Image>();
            hudImage.color = FilterBarBg;
            hudImage.raycastTarget = false;

            var hudText = CreateChild(hud, "Text");
            SetStretch(hudText);

            var tmp = hudText.AddComponent<TextMeshProUGUI>();
            tmp.text = "G: 94,572 | S: 102/102 | P: 1,809";
            tmp.fontSize = 12;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
            ApplyFont(tmp);
        }

        private static GameObject CreateContent(GameObject parent)
        {
            var content = CreateChild(parent, "Content");
            var contentRect = SetStretch(content);
            contentRect.offsetMin = new Vector2(0, 0);
            contentRect.offsetMax = new Vector2(0, -HEADER_HEIGHT);

            return content;
        }

        #endregion

        #region LeftSideTab

        private static InventoryTabWidget CreateLeftSideTab(GameObject parent)
        {
            var tabContainer = CreateChild(parent, "LeftSideTab");
            var tabRect = tabContainer.AddComponent<RectTransform>();
            tabRect.anchorMin = new Vector2(0, 0);
            tabRect.anchorMax = new Vector2(0, 1);
            tabRect.pivot = new Vector2(0, 0.5f);
            tabRect.anchoredPosition = Vector2.zero;
            tabRect.sizeDelta = new Vector2(LEFT_TAB_WIDTH, 0);

            var tabBg = tabContainer.AddComponent<Image>();
            tabBg.color = BgCard;
            tabBg.raycastTarget = false;

            // VerticalLayoutGroup
            var layoutGroup = tabContainer.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 8f;
            layoutGroup.padding = new RectOffset(8, 8, 8, 8);
            layoutGroup.childAlignment = TextAnchor.UpperCenter;

            // Tab buttons
            string[] tabNames = { "사용", "성장", "장비", "교단", "연성카드" };
            var tabButtons = new System.Collections.Generic.List<InventoryTabButton>();

            for (int i = 0; i < tabNames.Length; i++)
            {
                var tabButton = CreateTabButton(tabContainer, tabNames[i], i);
                tabButtons.Add(tabButton);
            }

            // Add InventoryTabWidget component
            var tabWidget = tabContainer.AddComponent<InventoryTabWidget>();

            // Connect tab buttons through SerializedObject
            var so = new SerializedObject(tabWidget);
            so.FindProperty("_tabButtons").arraySize = tabButtons.Count;
            for (int i = 0; i < tabButtons.Count; i++)
            {
                so.FindProperty("_tabButtons").GetArrayElementAtIndex(i).objectReferenceValue = tabButtons[i];
            }

            so.FindProperty("_activeColor").colorValue = TabSelected;
            so.FindProperty("_inactiveColor").colorValue = TabNormal;
            so.FindProperty("_activeTextColor").colorValue = TabTextSelected;
            so.FindProperty("_inactiveTextColor").colorValue = TabTextNormal;
            so.ApplyModifiedPropertiesWithoutUndo();

            return tabWidget;
        }

        private static InventoryTabButton CreateTabButton(GameObject parent, string label, int index)
        {
            var btn = CreateChild(parent, $"Tab_{label}");
            var btnRect = btn.AddComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(0, 60);

            var btnImage = btn.AddComponent<Image>();
            btnImage.color = index == 0 ? TabSelected : TabNormal;
            btnImage.raycastTarget = true;

            var button = btn.AddComponent<Button>();
            button.targetGraphic = btnImage;

            // Background
            var background = CreateChild(btn, "Background");
            SetStretch(background);
            var bgImage = background.AddComponent<Image>();
            bgImage.color = Color.clear;
            bgImage.raycastTarget = false;

            // Label
            var labelObj = CreateChild(btn, "Label");
            SetStretch(labelObj);

            var tmp = labelObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 14;
            tmp.color = index == 0 ? TabTextSelected : TabTextNormal;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.raycastTarget = false;
            ApplyFont(tmp);

            // SelectedIndicator
            var indicator = CreateChild(btn, "SelectedIndicator");
            var indicatorRect = indicator.AddComponent<RectTransform>();
            indicatorRect.anchorMin = new Vector2(0, 0.5f);
            indicatorRect.anchorMax = new Vector2(0, 0.5f);
            indicatorRect.pivot = new Vector2(0, 0.5f);
            indicatorRect.anchoredPosition = Vector2.zero;
            indicatorRect.sizeDelta = new Vector2(4, 40);

            var indicatorImage = indicator.AddComponent<Image>();
            indicatorImage.color = TabSelected;
            indicatorImage.raycastTarget = false;
            indicator.SetActive(index == 0);

            // Add InventoryTabButton component
            var tabButton = btn.AddComponent<InventoryTabButton>();

            // Connect fields through SerializedObject
            var so = new SerializedObject(tabButton);
            so.FindProperty("_button").objectReferenceValue = button;
            so.FindProperty("_background").objectReferenceValue = bgImage;
            so.FindProperty("_label").objectReferenceValue = tmp;
            so.FindProperty("_selectedIndicator").objectReferenceValue = indicator;
            so.ApplyModifiedPropertiesWithoutUndo();

            return tabButton;
        }

        #endregion

        #region MainArea

        private static GameObject CreateMainArea(GameObject parent)
        {
            var mainArea = CreateChild(parent, "MainArea");
            var mainRect = mainArea.AddComponent<RectTransform>();
            mainRect.anchorMin = new Vector2(0, 0);
            mainRect.anchorMax = new Vector2(1, 1);
            mainRect.pivot = new Vector2(0.5f, 0.5f);
            mainRect.anchoredPosition = Vector2.zero;
            mainRect.offsetMin = new Vector2(LEFT_TAB_WIDTH, 0);
            mainRect.offsetMax = new Vector2(-ITEM_DETAIL_WIDTH, 0);

            var mainBg = mainArea.AddComponent<Image>();
            mainBg.color = Color.white;
            mainBg.raycastTarget = false;

            return mainArea;
        }

        #endregion

        #region FilterBar

        private static (TMP_Dropdown categoryDropdown, TMP_Dropdown sortDropdown, Button settingsButton)
            CreateFilterBar(GameObject parent)
        {
            var filterBar = CreateChild(parent, "FilterBar");
            var filterRect = filterBar.AddComponent<RectTransform>();
            filterRect.anchorMin = new Vector2(0, 1);
            filterRect.anchorMax = new Vector2(1, 1);
            filterRect.pivot = new Vector2(0.5f, 1);
            filterRect.anchoredPosition = Vector2.zero;
            filterRect.sizeDelta = new Vector2(0, FILTER_BAR_HEIGHT);

            var filterBg = filterBar.AddComponent<Image>();
            filterBg.color = FilterBarBg;
            filterBg.raycastTarget = false;

            // HorizontalLayoutGroup
            var layoutGroup = filterBar.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 8f;
            layoutGroup.padding = new RectOffset(16, 16, 5, 5);
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;

            // CategoryDropdown
            var categoryDropdown = CreateDropdown(filterBar, "CategoryDropdown", "전체", 150f);

            // SortDropdown
            var sortDropdown = CreateDropdown(filterBar, "SortDropdown", "기본", 120f);

            // Spacer
            var spacer = CreateChild(filterBar, "Spacer");
            var spacerLayout = spacer.AddComponent<LayoutElement>();
            spacerLayout.flexibleWidth = 1f;

            // SettingsButton
            var settingsButton = CreateIconButton(filterBar, "SettingsButton", "⚙", 40f);

            return (categoryDropdown, sortDropdown, settingsButton);
        }

        private static TMP_Dropdown CreateDropdown(GameObject parent, string name, string label, float width)
        {
            var dropdown = CreateChild(parent, name);
            var dropdownRect = dropdown.AddComponent<RectTransform>();
            dropdownRect.sizeDelta = new Vector2(width, 40);

            var dropdownLayout = dropdown.AddComponent<LayoutElement>();
            dropdownLayout.preferredWidth = width;
            dropdownLayout.preferredHeight = 40;

            var dropdownBg = dropdown.AddComponent<Image>();
            dropdownBg.color = BgCard;
            dropdownBg.raycastTarget = true;

            var dropdownComponent = dropdown.AddComponent<TMP_Dropdown>();
            dropdownComponent.targetGraphic = dropdownBg;

            // Label
            var labelObj = CreateChild(dropdown, "Label");
            var labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.offsetMin = new Vector2(10, 2);
            labelRect.offsetMax = new Vector2(-30, -2);

            var labelTmp = labelObj.AddComponent<TextMeshProUGUI>();
            labelTmp.text = label;
            labelTmp.fontSize = 14;
            labelTmp.color = TextPrimary;
            labelTmp.alignment = TextAlignmentOptions.Left;
            labelTmp.raycastTarget = false;
            ApplyFont(labelTmp);

            dropdownComponent.captionText = labelTmp;

            // Arrow
            var arrow = CreateChild(dropdown, "Arrow");
            var arrowRect = arrow.AddComponent<RectTransform>();
            arrowRect.anchorMin = new Vector2(1, 0.5f);
            arrowRect.anchorMax = new Vector2(1, 0.5f);
            arrowRect.pivot = new Vector2(1, 0.5f);
            arrowRect.anchoredPosition = new Vector2(-10, 0);
            arrowRect.sizeDelta = new Vector2(16, 16);

            var arrowImage = arrow.AddComponent<Image>();
            arrowImage.color = TextSecondary;
            arrowImage.raycastTarget = false;

            // Template (collapsed)
            var template = CreateChild(dropdown, "Template");
            template.SetActive(false);

            return dropdownComponent;
        }

        private static Button CreateIconButton(GameObject parent, string name, string icon, float size)
        {
            var btn = CreateChild(parent, name);
            var btnRect = btn.AddComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(size, size);

            var btnLayout = btn.AddComponent<LayoutElement>();
            btnLayout.preferredWidth = size;
            btnLayout.preferredHeight = size;

            var btnImage = btn.AddComponent<Image>();
            btnImage.color = BgCard;
            btnImage.raycastTarget = true;

            var button = btn.AddComponent<Button>();
            button.targetGraphic = btnImage;

            // Icon text
            var iconObj = CreateChild(btn, "Icon");
            SetStretch(iconObj);

            var tmp = iconObj.AddComponent<TextMeshProUGUI>();
            tmp.text = icon;
            tmp.fontSize = 18;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
            ApplyFont(tmp);

            return button;
        }

        #endregion

        #region ItemGrid

        private static (ScrollRect scrollRect, Transform gridContainer, ItemCard cardPrefab)
            CreateItemGrid(GameObject parent)
        {
            // ItemGridArea
            var gridArea = CreateChild(parent, "ItemGridArea");
            var gridAreaRect = gridArea.AddComponent<RectTransform>();
            gridAreaRect.anchorMin = new Vector2(0, 0);
            gridAreaRect.anchorMax = new Vector2(1, 1);
            gridAreaRect.pivot = new Vector2(0.5f, 0.5f);
            gridAreaRect.anchoredPosition = Vector2.zero;
            gridAreaRect.offsetMin = new Vector2(0, 0);
            gridAreaRect.offsetMax = new Vector2(0, -FILTER_BAR_HEIGHT);

            // ScrollRect
            var scrollRect = gridArea.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;

            // Viewport
            var viewport = CreateChild(gridArea, "Viewport");
            SetStretch(viewport);

            var viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = Color.clear;
            viewportImage.raycastTarget = true;

            var mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            scrollRect.viewport = viewport.GetComponent<RectTransform>();

            // Content
            var content = CreateChild(viewport, "Content");
            var contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 500);

            scrollRect.content = contentRect;

            // GridLayoutGroup
            var gridLayout = content.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(ITEM_SLOT_SIZE, ITEM_SLOT_SIZE);
            gridLayout.spacing = new Vector2(ITEM_SLOT_SPACING, ITEM_SLOT_SPACING);
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.UpperLeft;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = GRID_COLUMN_COUNT;
            gridLayout.padding = new RectOffset(16, 16, 16, 16);

            // ContentSizeFitter
            var sizeFitter = content.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Create ItemCard prefab placeholder
            var itemCardPrefab = CreateItemCardPrefab();

            return (scrollRect, content.transform, itemCardPrefab);
        }

        private static ItemCard CreateItemCardPrefab()
        {
            // This should be referenced from project assets, but for builder purposes
            // we return null and expect it to be assigned later
            return null;
        }

        #endregion

        #region ItemDetailPanel

        private static ItemDetailWidget CreateItemDetailPanel(GameObject parent)
        {
            var detailPanel = CreateChild(parent, "ItemDetailPanel");
            var detailRect = detailPanel.AddComponent<RectTransform>();
            detailRect.anchorMin = new Vector2(1, 0);
            detailRect.anchorMax = new Vector2(1, 1);
            detailRect.pivot = new Vector2(1, 0.5f);
            detailRect.anchoredPosition = Vector2.zero;
            detailRect.sizeDelta = new Vector2(ITEM_DETAIL_WIDTH, 0);

            var detailBg = detailPanel.AddComponent<Image>();
            detailBg.color = BgCard;
            detailBg.raycastTarget = false;

            // EmptyState
            var emptyState = CreateChild(detailPanel, "EmptyState");
            SetStretch(emptyState);

            var emptyText = CreateChild(emptyState, "Text");
            SetStretch(emptyText);

            var emptyTmp = emptyText.AddComponent<TextMeshProUGUI>();
            emptyTmp.text = "아이템을 선택해주세요";
            emptyTmp.fontSize = 14;
            emptyTmp.color = TextMuted;
            emptyTmp.alignment = TextAlignmentOptions.Center;
            emptyTmp.raycastTarget = false;
            ApplyFont(emptyTmp);

            // DetailView
            var detailView = CreateChild(detailPanel, "DetailView");
            SetStretch(detailView);
            detailView.SetActive(false);

            // VerticalLayoutGroup
            var layoutGroup = detailView.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 12f;
            layoutGroup.padding = new RectOffset(16, 16, 16, 16);
            layoutGroup.childAlignment = TextAnchor.UpperCenter;

            // ItemImage
            var itemImage = CreateChild(detailView, "ItemImage");
            var imageRect = itemImage.AddComponent<RectTransform>();
            imageRect.sizeDelta = new Vector2(120, 120);
            var imageLayout = itemImage.AddComponent<LayoutElement>();
            imageLayout.preferredHeight = 120;

            var itemImg = itemImage.AddComponent<Image>();
            itemImg.color = RarityCommon;
            itemImg.raycastTarget = false;

            // ItemName
            var itemName = CreateChild(detailView, "ItemName");
            var nameLayout = itemName.AddComponent<LayoutElement>();
            nameLayout.preferredHeight = 30;

            var nameTmp = itemName.AddComponent<TextMeshProUGUI>();
            nameTmp.text = "Item Name";
            nameTmp.fontSize = 16;
            nameTmp.color = TextPrimary;
            nameTmp.alignment = TextAlignmentOptions.Center;
            nameTmp.fontStyle = FontStyles.Bold;
            nameTmp.raycastTarget = false;
            ApplyFont(nameTmp);

            // ItemDescription
            var itemDesc = CreateChild(detailView, "ItemDescription");
            var descLayout = itemDesc.AddComponent<LayoutElement>();
            descLayout.preferredHeight = 100;

            var descTmp = itemDesc.AddComponent<TextMeshProUGUI>();
            descTmp.text = "Item description...";
            descTmp.fontSize = 12;
            descTmp.color = TextSecondary;
            descTmp.alignment = TextAlignmentOptions.TopLeft;
            descTmp.raycastTarget = false;
            ApplyFont(descTmp);

            // ActionButtons
            var actionButtons = CreateChild(detailView, "ActionButtons");
            var buttonsLayout = actionButtons.AddComponent<LayoutElement>();
            buttonsLayout.preferredHeight = 80;

            var buttonsVLayout = actionButtons.AddComponent<VerticalLayoutGroup>();
            buttonsVLayout.childControlWidth = true;
            buttonsVLayout.childControlHeight = false;
            buttonsVLayout.childForceExpandWidth = true;
            buttonsVLayout.childForceExpandHeight = false;
            buttonsVLayout.spacing = 8f;

            CreateActionButton(actionButtons, "UseButton", "사용");
            CreateActionButton(actionButtons, "SellButton", "판매");

            // Add ItemDetailWidget component (stub, actual widget may be different)
            var itemDetailWidget = detailPanel.AddComponent<ItemDetailWidget>();

            return itemDetailWidget;
        }

        private static void CreateActionButton(GameObject parent, string name, string label)
        {
            var btn = CreateChild(parent, name);
            var btnLayout = btn.AddComponent<LayoutElement>();
            btnLayout.preferredHeight = 36;

            var btnImage = btn.AddComponent<Image>();
            btnImage.color = TabSelected;
            btnImage.raycastTarget = true;

            var button = btn.AddComponent<Button>();
            button.targetGraphic = btnImage;

            var text = CreateChild(btn, "Text");
            SetStretch(text);

            var tmp = text.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 14;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.raycastTarget = false;
            ApplyFont(tmp);
        }

        #endregion

        #region EmptyState

        private static (GameObject emptyStateObject, TMP_Text emptyStateText) CreateEmptyState(GameObject parent)
        {
            var emptyState = CreateChild(parent, "EmptyState");
            var emptyRect = emptyState.AddComponent<RectTransform>();
            emptyRect.anchorMin = new Vector2(0, 0);
            emptyRect.anchorMax = new Vector2(1, 1);
            emptyRect.pivot = new Vector2(0.5f, 0.5f);
            emptyRect.anchoredPosition = Vector2.zero;
            emptyRect.offsetMin = new Vector2(0, 0);
            emptyRect.offsetMax = new Vector2(0, -FILTER_BAR_HEIGHT);

            emptyState.SetActive(false);

            var text = CreateChild(emptyState, "Text");
            SetStretch(text);

            var tmp = text.AddComponent<TextMeshProUGUI>();
            tmp.text = "보유한 아이템이 없습니다.";
            tmp.fontSize = 16;
            tmp.color = TextMuted;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
            ApplyFont(tmp);

            return (emptyState, tmp);
        }

        #endregion

        #region SerializeField Connections

        private static void ConnectSerializedFields(
            GameObject root,
            Button backButton,
            Button homeButton,
            TMP_Dropdown categoryDropdown,
            TMP_Dropdown sortDropdown,
            Button settingsButton,
            ScrollRect itemScrollRect,
            Transform itemGridContainer,
            ItemCard itemCardPrefab,
            GameObject emptyStateObject,
            TMP_Text emptyStateText,
            ItemDetailWidget itemDetailWidget,
            InventoryTabWidget tabWidget)
        {
            var screen = root.GetComponent<InventoryScreen>();
            var so = new SerializedObject(screen);

            // Tab
            so.FindProperty("_tabWidget").objectReferenceValue = tabWidget;

            // Filter Bar
            so.FindProperty("_categoryDropdown").objectReferenceValue = categoryDropdown;
            so.FindProperty("_sortDropdown").objectReferenceValue = sortDropdown;
            so.FindProperty("_settingsButton").objectReferenceValue = settingsButton;

            // Item Grid
            so.FindProperty("_itemScrollRect").objectReferenceValue = itemScrollRect;
            so.FindProperty("_itemGridContainer").objectReferenceValue = itemGridContainer;
            so.FindProperty("_itemCardPrefab").objectReferenceValue = itemCardPrefab;

            // Detail Panel
            so.FindProperty("_itemDetailWidget").objectReferenceValue = itemDetailWidget;

            // Empty State
            so.FindProperty("_emptyStateObject").objectReferenceValue = emptyStateObject;
            so.FindProperty("_emptyStateText").objectReferenceValue = emptyStateText;

            // Navigation
            so.FindProperty("_backButton").objectReferenceValue = backButton;
            so.FindProperty("_homeButton").objectReferenceValue = homeButton;

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion

        #region Helpers

        private static GameObject CreateChild(GameObject parent, string name)
        {
            var child = new GameObject(name);
            child.transform.SetParent(parent.transform, false);
            return child;
        }

        private static RectTransform SetStretch(GameObject go)
        {
            var rect = go.GetComponent<RectTransform>();
            if (rect == null)
                rect = go.AddComponent<RectTransform>();

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return rect;
        }

        #endregion
    }
}