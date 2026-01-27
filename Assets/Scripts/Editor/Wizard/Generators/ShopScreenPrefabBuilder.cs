using Sc.Common.UI.Widgets;
using Sc.Contents.Shop;
using Sc.Contents.Shop.Widgets;
using Sc.Editor.AI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Editor.Wizard.Generators
{
    /// <summary>
    /// ShopScreen 전용 프리팹 빌더.
    /// 상점 NPC와 세로 탭, 상품 그리드로 구성된 상점 화면 UI 생성.
    /// 스펙: Docs/Specs/Shop.md - UI 레이아웃 구조
    /// 레퍼런스: Docs/Design/Reference/Shop.jpg
    /// </summary>
    public static class ShopScreenPrefabBuilder
    {
        #region Colors (Warm Wood Theme)

        // Background - Wooden crate theme
        private static readonly Color BgWood = new Color32(80, 55, 40, 255);
        private static readonly Color BgWoodDark = new Color32(50, 35, 25, 255);
        private static readonly Color BgHeader = new Color32(30, 45, 60, 240);

        // Tab colors
        private static readonly Color TabNormal = new Color32(60, 60, 80, 255);
        private static readonly Color TabSelected = new Color32(255, 150, 100, 255);

        // Text colors
        private static readonly Color TextPrimary = Color.white;
        private static readonly Color TextDark = new Color32(50, 50, 50, 255);
        private static readonly Color TextMuted = new Color(1f, 1f, 1f, 0.5f);

        // Accent colors
        private static readonly Color AccentGold = new Color32(255, 200, 100, 255);
        private static readonly Color AccentGreen = new Color32(100, 180, 100, 255);
        private static readonly Color AccentOrange = new Color32(255, 150, 100, 255);
        private static readonly Color AccentBlue = new Color32(100, 150, 255, 255);

        // Button colors
        private static readonly Color ButtonSecondary = new Color32(100, 100, 120, 255);

        // Product card colors
        private static readonly Color ProductCardBg = new Color32(250, 245, 235, 255);
        private static readonly Color ProductTagBg = new Color32(255, 100, 80, 220);

        // Dialogue box
        private static readonly Color DialogueBg = new Color32(255, 255, 240, 240);

        #endregion

        #region Constants

        private const float HEADER_HEIGHT = 60f;
        private const float FOOTER_HEIGHT = 50f;
        private const float LEFT_AREA_WIDTH = 280f;
        private const float TAB_BUTTON_HEIGHT = 55f;
        private const float TAB_BUTTON_SPACING = 5f;
        private const float PRODUCT_CELL_WIDTH = 180f;
        private const float PRODUCT_CELL_HEIGHT = 220f;
        private const float PRODUCT_SPACING = 15f;
        private const int GRID_COLUMNS = 3;
        private const int GRID_ROWS = 2;
        private const float SHOPKEEPER_HEIGHT = 200f;
        private const float CATEGORY_SHORTCUT_HEIGHT = 60f;

        #endregion

        #region Tab Labels

        private static readonly string[] TAB_LABELS = new[]
        {
            "데일리",
            "잡화",
            "전투석",
            "인증서",
            "추천",
            "프론티어",
            "여우"
        };

        #endregion

        #region Font Helper

        private static void ApplyFont(TextMeshProUGUI tmp)
        {
            var font = EditorUIHelpers.GetProjectFont();
            if (font != null) tmp.font = font;
        }

        #endregion

        /// <summary>
        /// ShopScreen 프리팹용 GameObject 생성.
        /// </summary>
        public static GameObject Build()
        {
            var root = CreateRoot();

            // 1. Background
            CreateBackground(root);

            // 2. SafeArea
            CreateSafeArea(root);

            // 3. Empty State (비활성)
            CreateEmptyState(root);

            // 4. Overlay Layer
            CreateOverlayLayer(root);

            // 5. Add main component
            root.AddComponent<ShopScreen>();

            // 6. Connect SerializedFields
            ConnectSerializedFields(root);

            return root;
        }

        #region Root

        private static GameObject CreateRoot()
        {
            var root = new GameObject("ShopScreen");
            var rect = root.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            root.AddComponent<CanvasGroup>();

            return root;
        }

        #endregion

        #region Background

        private static void CreateBackground(GameObject parent)
        {
            var bg = CreateChild(parent, "Background");
            SetStretch(bg);

            var bgImage = bg.AddComponent<Image>();
            bgImage.color = BgWood;
            bgImage.raycastTarget = true;

            // Wooden crate overlay pattern
            var pattern = CreateChild(bg, "WoodPattern");
            SetStretch(pattern);
            var patternImage = pattern.AddComponent<Image>();
            patternImage.color = BgWoodDark;
            patternImage.raycastTarget = false;

            // Vignette
            var vignette = CreateChild(bg, "Vignette");
            SetStretch(vignette);
            var vignetteImage = vignette.AddComponent<Image>();
            vignetteImage.color = new Color(0, 0, 0, 0.3f);
            vignetteImage.raycastTarget = false;
        }

        #endregion

        #region SafeArea

        private static GameObject CreateSafeArea(GameObject parent)
        {
            var safeArea = CreateChild(parent, "SafeArea");
            SetStretch(safeArea);

            CreateHeader(safeArea);
            CreateContent(safeArea);
            CreateFooter(safeArea);

            return safeArea;
        }

        #endregion

        #region Header

        private static void CreateHeader(GameObject parent)
        {
            var header = CreateChild(parent, "Header");
            SetAnchorTop(header, HEADER_HEIGHT);

            var bg = header.AddComponent<Image>();
            bg.color = BgHeader;

            var layout = header.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            layout.padding = new RectOffset(20, 20, 0, 0);
            layout.spacing = 15;

            // BackButton Group
            var backGroup = CreateChild(header, "BackButtonGroup");
            var backGroupLayout = backGroup.AddComponent<HorizontalLayoutGroup>();
            backGroupLayout.spacing = 10;
            backGroupLayout.childAlignment = TextAnchor.MiddleLeft;

            // Back Button
            CreateButton(backGroup, "BackButton", "<", 50, 50, ButtonSecondary);

            // Title Text
            CreateLabel(backGroup, "TitleText", "상점", 24, TextPrimary, TextAlignmentOptions.Left);

            // Spacer
            var spacer = CreateChild(header, "Spacer");
            var spacerLayout = spacer.AddComponent<LayoutElement>();
            spacerLayout.flexibleWidth = 1;

            // Right Group (Currency + Home)
            var rightGroup = CreateChild(header, "RightGroup");
            var rightLayout = rightGroup.AddComponent<HorizontalLayoutGroup>();
            rightLayout.spacing = 15;
            rightLayout.childAlignment = TextAnchor.MiddleRight;

            // Currency HUD
            var currencyHUD = CreateChild(rightGroup, "CurrencyHUD");
            var currencyLayout = currencyHUD.AddComponent<HorizontalLayoutGroup>();
            currencyLayout.spacing = 20;
            currencyLayout.childAlignment = TextAnchor.MiddleCenter;

            // Gold Display
            CreateCurrencyDisplay(currencyHUD, "GoldDisplay", "GoldText", "549,061", AccentGold);

            // Gem Display
            CreateCurrencyDisplay(currencyHUD, "GemDisplay", "GemText", "1,809", AccentBlue);

            // Home Button
            CreateButton(rightGroup, "HomeButton", "H", 40, 40, AccentGreen);
        }

        private static void CreateCurrencyDisplay(GameObject parent, string name, string textName, string value,
            Color iconColor)
        {
            var display = CreateChild(parent, name);
            var layout = display.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 5;
            layout.childAlignment = TextAnchor.MiddleCenter;

            // Icon
            var icon = CreateChild(display, "Icon");
            var iconLayout = icon.AddComponent<LayoutElement>();
            iconLayout.preferredWidth = 24;
            iconLayout.preferredHeight = 24;
            var iconImage = icon.AddComponent<Image>();
            iconImage.color = iconColor;

            // Value Text
            var valueLabel = CreateChild(display, textName);
            var valueLayoutElem = valueLabel.AddComponent<LayoutElement>();
            valueLayoutElem.minWidth = 80;
            var valueTmp = valueLabel.AddComponent<TextMeshProUGUI>();
            valueTmp.text = value;
            valueTmp.fontSize = 16;
            valueTmp.color = TextPrimary;
            valueTmp.alignment = TextAlignmentOptions.Left;
            ApplyFont(valueTmp);
        }

        #endregion

        #region Content

        private static void CreateContent(GameObject parent)
        {
            var content = CreateChild(parent, "Content");
            var rect = content.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(0, FOOTER_HEIGHT);
            rect.offsetMax = new Vector2(0, -HEADER_HEIGHT);

            CreateLeftArea(content);
            CreateRightArea(content);
        }

        #region Left Area

        private static void CreateLeftArea(GameObject parent)
        {
            var leftArea = CreateChild(parent, "LeftArea");
            var rect = leftArea.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 0.5f);
            rect.sizeDelta = new Vector2(LEFT_AREA_WIDTH, 0);
            rect.anchoredPosition = Vector2.zero;

            var layout = leftArea.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateShopkeeperDisplay(leftArea);
            CreateTabList(leftArea);
        }

        private static void CreateShopkeeperDisplay(GameObject parent)
        {
            var shopkeeperDisplay = CreateChild(parent, "ShopkeeperDisplay");
            var layoutElem = shopkeeperDisplay.AddComponent<LayoutElement>();
            layoutElem.preferredHeight = SHOPKEEPER_HEIGHT;
            layoutElem.flexibleWidth = 1;

            var layout = shopkeeperDisplay.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 5;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.padding = new RectOffset(5, 5, 5, 5);

            // Character Image
            var characterImage = CreateChild(shopkeeperDisplay, "CharacterImage");
            var charLayoutElem = characterImage.AddComponent<LayoutElement>();
            charLayoutElem.preferredHeight = 120;
            charLayoutElem.preferredWidth = 120;

            var charImage = characterImage.AddComponent<Image>();
            charImage.color = new Color32(200, 150, 200, 255); // Purple placeholder
            charImage.raycastTarget = false;

            // Dialogue Box
            var dialogueBox = CreateChild(shopkeeperDisplay, "DialogueBox");
            var dialogueLayoutElem = dialogueBox.AddComponent<LayoutElement>();
            dialogueLayoutElem.preferredHeight = 50;
            dialogueLayoutElem.flexibleWidth = 1;

            var dialogueBgImage = dialogueBox.AddComponent<Image>();
            dialogueBgImage.color = DialogueBg;

            var dialogueLayout = dialogueBox.AddComponent<HorizontalLayoutGroup>();
            dialogueLayout.padding = new RectOffset(10, 10, 5, 5);
            dialogueLayout.childAlignment = TextAnchor.MiddleCenter;

            // Dialogue Text
            var dialogueText = CreateChild(dialogueBox, "DialogueText");
            var dialogueTmp = dialogueText.AddComponent<TextMeshProUGUI>();
            dialogueTmp.text = "오늘은 뭘 보여드릴까요?";
            dialogueTmp.fontSize = 14;
            dialogueTmp.color = TextDark;
            dialogueTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(dialogueTmp);
        }

        private static void CreateTabList(GameObject parent)
        {
            var tabList = CreateChild(parent, "TabList");
            var layoutElem = tabList.AddComponent<LayoutElement>();
            layoutElem.flexibleHeight = 1;
            layoutElem.flexibleWidth = 1;

            // TabGroupWidget component
            tabList.AddComponent<TabGroupWidget>();

            var layout = tabList.AddComponent<VerticalLayoutGroup>();
            layout.spacing = TAB_BUTTON_SPACING;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.padding = new RectOffset(5, 5, 5, 5);

            // Create 7 tab buttons
            for (int i = 0; i < TAB_LABELS.Length; i++)
            {
                CreateShopTabButton(tabList, i, TAB_LABELS[i], i == 0);
            }
        }

        private static GameObject CreateShopTabButton(GameObject parent, int index, string label, bool isSelected)
        {
            var tabBtn = CreateChild(parent, $"TabButton_{index}");

            var layoutElem = tabBtn.AddComponent<LayoutElement>();
            layoutElem.preferredHeight = TAB_BUTTON_HEIGHT;
            layoutElem.flexibleWidth = 1;

            // Add ShopTabButton component
            tabBtn.AddComponent<ShopTabButton>();

            // Background
            var bgImage = tabBtn.AddComponent<Image>();
            bgImage.color = isSelected ? TabSelected : TabNormal;

            // Button
            var button = tabBtn.AddComponent<Button>();
            button.targetGraphic = bgImage;

            // Layout for icon + label
            var tabLayout = tabBtn.AddComponent<HorizontalLayoutGroup>();
            tabLayout.padding = new RectOffset(10, 10, 5, 5);
            tabLayout.spacing = 8;
            tabLayout.childAlignment = TextAnchor.MiddleLeft;
            tabLayout.childForceExpandWidth = false;
            tabLayout.childForceExpandHeight = false;

            // Icon placeholder
            var icon = CreateChild(tabBtn, "Icon");
            var iconLayoutElem = icon.AddComponent<LayoutElement>();
            iconLayoutElem.preferredWidth = 30;
            iconLayoutElem.preferredHeight = 30;
            var iconImage = icon.AddComponent<Image>();
            iconImage.color = isSelected ? Color.white : new Color(1, 1, 1, 0.6f);

            // Label
            var labelObj = CreateChild(tabBtn, "Label");
            var labelLayoutElem = labelObj.AddComponent<LayoutElement>();
            labelLayoutElem.flexibleWidth = 1;
            var labelTmp = labelObj.AddComponent<TextMeshProUGUI>();
            labelTmp.text = label;
            labelTmp.fontSize = 16;
            labelTmp.color = TextPrimary;
            labelTmp.alignment = TextAlignmentOptions.Left;
            ApplyFont(labelTmp);

            // Selected Indicator
            var indicator = CreateChild(tabBtn, "SelectedIndicator");
            var indicatorRect = indicator.GetComponent<RectTransform>();
            indicatorRect.anchorMin = new Vector2(0, 0);
            indicatorRect.anchorMax = new Vector2(0, 1);
            indicatorRect.pivot = new Vector2(0, 0.5f);
            indicatorRect.sizeDelta = new Vector2(4, 0);
            indicatorRect.anchoredPosition = Vector2.zero;
            var indicatorImage = indicator.AddComponent<Image>();
            indicatorImage.color = AccentOrange;
            indicator.SetActive(isSelected);

            // Badge (hidden by default)
            var badge = CreateChild(tabBtn, "Badge");
            var badgeRect = badge.GetComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(1, 1);
            badgeRect.anchorMax = new Vector2(1, 1);
            badgeRect.pivot = new Vector2(1, 1);
            badgeRect.sizeDelta = new Vector2(20, 20);
            badgeRect.anchoredPosition = new Vector2(-5, -5);
            var badgeBg = badge.AddComponent<Image>();
            badgeBg.color = Color.red;
            badge.SetActive(false);

            var badgeCount = CreateChild(badge, "BadgeCount");
            SetStretch(badgeCount);
            var badgeTmp = badgeCount.AddComponent<TextMeshProUGUI>();
            badgeTmp.text = "1";
            badgeTmp.fontSize = 12;
            badgeTmp.color = Color.white;
            badgeTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(badgeTmp);

            return tabBtn;
        }

        #endregion

        #region Right Area

        private static void CreateRightArea(GameObject parent)
        {
            var rightArea = CreateChild(parent, "RightArea");
            var rect = rightArea.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(LEFT_AREA_WIDTH, 0);
            rect.offsetMax = Vector2.zero;

            var layout = rightArea.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(15, 15, 10, 10);
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateProductContainer(rightArea);
        }

        private static void CreateProductContainer(GameObject parent)
        {
            var productContainer = CreateChild(parent, "ProductContainer");
            var containerLayoutElem = productContainer.AddComponent<LayoutElement>();
            containerLayoutElem.flexibleHeight = 1;
            containerLayoutElem.flexibleWidth = 1;

            var containerLayout = productContainer.AddComponent<VerticalLayoutGroup>();
            containerLayout.spacing = 10;
            containerLayout.childAlignment = TextAnchor.UpperCenter;
            containerLayout.childForceExpandWidth = true;
            containerLayout.childForceExpandHeight = false;

            // Scroll View
            var scrollView = CreateChild(productContainer, "ScrollView");
            var scrollLayoutElem = scrollView.AddComponent<LayoutElement>();
            scrollLayoutElem.flexibleHeight = 1;
            scrollLayoutElem.flexibleWidth = 1;

            var scrollRect = scrollView.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;

            var scrollBg = scrollView.AddComponent<Image>();
            scrollBg.color = new Color(0, 0, 0, 0.1f);

            // Viewport
            var viewport = CreateChild(scrollView, "Viewport");
            SetStretch(viewport);
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            var viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = Color.white;

            scrollRect.viewport = viewport.GetComponent<RectTransform>();

            // Content (ProductGrid)
            var productGrid = CreateChild(viewport, "ProductGrid");
            var gridRect = productGrid.GetComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0, 1);
            gridRect.anchorMax = new Vector2(1, 1);
            gridRect.pivot = new Vector2(0.5f, 1);
            gridRect.sizeDelta =
                new Vector2(0, PRODUCT_CELL_HEIGHT * GRID_ROWS + PRODUCT_SPACING * (GRID_ROWS - 1) + 20);

            var gridLayout = productGrid.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(PRODUCT_CELL_WIDTH, PRODUCT_CELL_HEIGHT);
            gridLayout.spacing = new Vector2(PRODUCT_SPACING, PRODUCT_SPACING);
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.UpperCenter;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = GRID_COLUMNS;
            gridLayout.padding = new RectOffset(10, 10, 10, 10);

            var contentSizeFitter = productGrid.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = gridRect;

            // Create product item templates (6 items for 3x2 grid)
            for (int i = 0; i < 6; i++)
            {
                CreateProductItemTemplate(productGrid, i);
            }

            // Product Grid Footer (Category Shortcuts)
            CreateProductGridFooter(productContainer);
        }

        private static void CreateProductItemTemplate(GameObject parent, int index)
        {
            var productItem = CreateChild(parent, $"ProductItem_{index}");

            // Add ShopProductItem component
            productItem.AddComponent<ShopProductItem>();

            // Canvas Group for state management
            productItem.AddComponent<CanvasGroup>();

            // Card Background
            var cardBg = productItem.AddComponent<Image>();
            cardBg.color = ProductCardBg;

            // Button
            var button = productItem.AddComponent<Button>();
            button.targetGraphic = cardBg;

            // Internal layout
            var layout = productItem.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(8, 8, 8, 8);
            layout.spacing = 5;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // Tag Label (top-left)
            var tagLabel = CreateChild(productItem, "TagLabel");
            var tagRect = tagLabel.GetComponent<RectTransform>();
            tagRect.anchorMin = new Vector2(0, 1);
            tagRect.anchorMax = new Vector2(0, 1);
            tagRect.pivot = new Vector2(0, 1);
            tagRect.sizeDelta = new Vector2(60, 20);
            tagRect.anchoredPosition = new Vector2(5, -5);

            var tagBg = tagLabel.AddComponent<Image>();
            tagBg.color = ProductTagBg;

            var tagTmp = CreateChild(tagLabel, "Text");
            SetStretch(tagTmp);
            var tagText = tagTmp.AddComponent<TextMeshProUGUI>();
            tagText.text = "일일갱신";
            tagText.fontSize = 10;
            tagText.color = Color.white;
            tagText.alignment = TextAlignmentOptions.Center;
            ApplyFont(tagText);

            // Product Icon (center)
            var productIcon = CreateChild(productItem, "ProductIcon");
            var iconLayoutElem = productIcon.AddComponent<LayoutElement>();
            iconLayoutElem.preferredHeight = 100;
            iconLayoutElem.preferredWidth = 100;
            iconLayoutElem.flexibleWidth = 0;

            var iconImage = productIcon.AddComponent<Image>();
            iconImage.color = new Color32(150, 150, 150, 200);
            iconImage.raycastTarget = false;

            // Product Name
            var productName = CreateChild(productItem, "ProductName");
            var nameLayoutElem = productName.AddComponent<LayoutElement>();
            nameLayoutElem.preferredHeight = 20;

            var nameTmp = productName.AddComponent<TextMeshProUGUI>();
            nameTmp.text = "상품명";
            nameTmp.fontSize = 14;
            nameTmp.fontStyle = FontStyles.Bold;
            nameTmp.color = TextDark;
            nameTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(nameTmp);

            // Purchase Limit
            var purchaseLimit = CreateChild(productItem, "PurchaseLimit");
            var limitLayoutElem = purchaseLimit.AddComponent<LayoutElement>();
            limitLayoutElem.preferredHeight = 16;

            var limitTmp = purchaseLimit.AddComponent<TextMeshProUGUI>();
            limitTmp.text = "구매 가능 1/1";
            limitTmp.fontSize = 11;
            limitTmp.color = TextDark;
            limitTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(limitTmp);

            // Price Group
            var priceGroup = CreateChild(productItem, "PriceGroup");
            var priceGroupLayoutElem = priceGroup.AddComponent<LayoutElement>();
            priceGroupLayoutElem.preferredHeight = 25;

            var priceLayout = priceGroup.AddComponent<HorizontalLayoutGroup>();
            priceLayout.spacing = 5;
            priceLayout.childAlignment = TextAnchor.MiddleCenter;
            priceLayout.childForceExpandWidth = false;

            // Currency Icon
            var currencyIcon = CreateChild(priceGroup, "CurrencyIcon");
            var currIconLayoutElem = currencyIcon.AddComponent<LayoutElement>();
            currIconLayoutElem.preferredWidth = 18;
            currIconLayoutElem.preferredHeight = 18;
            var currIconImage = currencyIcon.AddComponent<Image>();
            currIconImage.color = AccentGold;

            // Price Text
            var priceText = CreateChild(priceGroup, "PriceText");
            var priceTmp = priceText.AddComponent<TextMeshProUGUI>();
            priceTmp.text = "1";
            priceTmp.fontSize = 14;
            priceTmp.fontStyle = FontStyles.Bold;
            priceTmp.color = TextDark;
            priceTmp.alignment = TextAlignmentOptions.Left;
            ApplyFont(priceTmp);

            // Sold Out Overlay (hidden)
            var soldOut = CreateChild(productItem, "SoldOutOverlay");
            SetStretch(soldOut);
            var soldOutImage = soldOut.AddComponent<Image>();
            soldOutImage.color = new Color(0, 0, 0, 0.5f);
            soldOut.SetActive(false);

            var soldOutText = CreateChild(soldOut, "SoldOutText");
            SetStretch(soldOutText);
            var soldOutTmp = soldOutText.AddComponent<TextMeshProUGUI>();
            soldOutTmp.text = "품절";
            soldOutTmp.fontSize = 20;
            soldOutTmp.fontStyle = FontStyles.Bold;
            soldOutTmp.color = Color.white;
            soldOutTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(soldOutTmp);

            // Initially inactive (template)
            productItem.SetActive(false);
        }

        private static void CreateProductGridFooter(GameObject parent)
        {
            var footer = CreateChild(parent, "ProductGridFooter");
            var layoutElem = footer.AddComponent<LayoutElement>();
            layoutElem.preferredHeight = CATEGORY_SHORTCUT_HEIGHT;
            layoutElem.flexibleWidth = 1;

            var layout = footer.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 15;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            layout.padding = new RectOffset(10, 10, 5, 5);

            // Create 3 category shortcuts
            string[] shortcuts = { "고단 성장 재료 상자", "교주의방 꾸미기", "고단 요리 상자" };
            for (int i = 0; i < shortcuts.Length; i++)
            {
                CreateCategoryShortcut(footer, i, shortcuts[i]);
            }
        }

        private static void CreateCategoryShortcut(GameObject parent, int index, string label)
        {
            var shortcut = CreateChild(parent, $"CategoryShortcut_{index}");

            // Add CategoryShortcut component
            shortcut.AddComponent<CategoryShortcut>();

            var layoutElem = shortcut.AddComponent<LayoutElement>();
            layoutElem.flexibleWidth = 1;

            var bg = shortcut.AddComponent<Image>();
            bg.color = new Color32(220, 210, 190, 255);

            var button = shortcut.AddComponent<Button>();
            button.targetGraphic = bg;

            var layout = shortcut.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 5, 5);
            layout.spacing = 5;
            layout.childAlignment = TextAnchor.MiddleCenter;

            // Icon
            var icon = CreateChild(shortcut, "Icon");
            var iconLayoutElem = icon.AddComponent<LayoutElement>();
            iconLayoutElem.preferredWidth = 30;
            iconLayoutElem.preferredHeight = 30;
            var iconImage = icon.AddComponent<Image>();
            iconImage.color = AccentGold;

            // Label
            var labelObj = CreateChild(shortcut, "Label");
            var labelTmp = labelObj.AddComponent<TextMeshProUGUI>();
            labelTmp.text = label;
            labelTmp.fontSize = 12;
            labelTmp.color = TextDark;
            labelTmp.alignment = TextAlignmentOptions.Left;
            labelTmp.enableAutoSizing = true;
            labelTmp.fontSizeMin = 10;
            labelTmp.fontSizeMax = 12;
            ApplyFont(labelTmp);
        }

        #endregion

        #endregion

        #region Footer

        private static void CreateFooter(GameObject parent)
        {
            var footer = CreateChild(parent, "Footer");
            SetAnchorBottom(footer, FOOTER_HEIGHT);

            var bg = footer.AddComponent<Image>();
            bg.color = BgHeader;

            var layout = footer.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 5, 5);
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            // Refresh Timer Group
            var refreshGroup = CreateChild(footer, "RefreshTimerGroup");
            var refreshLayout = refreshGroup.AddComponent<HorizontalLayoutGroup>();
            refreshLayout.spacing = 8;
            refreshLayout.childAlignment = TextAnchor.MiddleLeft;

            // Refresh Icon
            var refreshIcon = CreateChild(refreshGroup, "RefreshIcon");
            var refreshIconLayoutElem = refreshIcon.AddComponent<LayoutElement>();
            refreshIconLayoutElem.preferredWidth = 20;
            refreshIconLayoutElem.preferredHeight = 20;
            var refreshIconImage = refreshIcon.AddComponent<Image>();
            refreshIconImage.color = AccentGreen;

            // Refresh Timer Text
            var refreshText = CreateChild(refreshGroup, "RefreshTimerText");
            var refreshTmp = refreshText.AddComponent<TextMeshProUGUI>();
            refreshTmp.text = "갱신까지 10시간 12분";
            refreshTmp.fontSize = 14;
            refreshTmp.color = TextPrimary;
            refreshTmp.alignment = TextAlignmentOptions.Left;
            ApplyFont(refreshTmp);

            // Spacer
            var spacer = CreateChild(footer, "Spacer");
            var spacerLayout = spacer.AddComponent<LayoutElement>();
            spacerLayout.flexibleWidth = 1;

            // Select All Toggle
            var selectAllToggle = CreateChild(footer, "SelectAllToggle");
            var toggleLayoutElem = selectAllToggle.AddComponent<LayoutElement>();
            toggleLayoutElem.preferredWidth = 140;

            var toggleLayout = selectAllToggle.AddComponent<HorizontalLayoutGroup>();
            toggleLayout.spacing = 8;
            toggleLayout.childAlignment = TextAnchor.MiddleCenter;
            toggleLayout.childForceExpandWidth = false;

            // Toggle Background
            var toggleBg = CreateChild(selectAllToggle, "Background");
            var toggleBgLayoutElem = toggleBg.AddComponent<LayoutElement>();
            toggleBgLayoutElem.preferredWidth = 30;
            toggleBgLayoutElem.preferredHeight = 30;
            var toggleBgImage = toggleBg.AddComponent<Image>();
            toggleBgImage.color = ButtonSecondary;

            // Toggle Checkmark
            var checkmark = CreateChild(toggleBg, "Checkmark");
            SetStretch(checkmark);
            var checkmarkPadding = checkmark.GetComponent<RectTransform>();
            checkmarkPadding.offsetMin = new Vector2(4, 4);
            checkmarkPadding.offsetMax = new Vector2(-4, -4);
            var checkmarkImage = checkmark.AddComponent<Image>();
            checkmarkImage.color = AccentGreen;

            // Toggle Component
            var toggle = selectAllToggle.AddComponent<Toggle>();
            toggle.targetGraphic = toggleBgImage;
            toggle.graphic = checkmarkImage;
            toggle.isOn = false;

            // Toggle Text
            var selectAllText = CreateChild(selectAllToggle, "SelectAllText");
            var selectAllTmp = selectAllText.AddComponent<TextMeshProUGUI>();
            selectAllTmp.text = "모두 선택 OFF";
            selectAllTmp.fontSize = 14;
            selectAllTmp.color = TextPrimary;
            selectAllTmp.alignment = TextAlignmentOptions.Left;
            ApplyFont(selectAllTmp);

            // Bulk Purchase Button
            var bulkPurchaseBtn = CreateChild(footer, "BulkPurchaseButton");
            var bulkLayoutElem = bulkPurchaseBtn.AddComponent<LayoutElement>();
            bulkLayoutElem.preferredWidth = 100;
            bulkLayoutElem.preferredHeight = 40;

            var bulkBg = bulkPurchaseBtn.AddComponent<Image>();
            bulkBg.color = AccentOrange;

            var bulkButton = bulkPurchaseBtn.AddComponent<Button>();
            bulkButton.targetGraphic = bulkBg;

            var bulkLabel = CreateChild(bulkPurchaseBtn, "Label");
            SetStretch(bulkLabel);
            var bulkTmp = bulkLabel.AddComponent<TextMeshProUGUI>();
            bulkTmp.text = "일괄 구매";
            bulkTmp.fontSize = 16;
            bulkTmp.fontStyle = FontStyles.Bold;
            bulkTmp.color = Color.white;
            bulkTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(bulkTmp);
        }

        #endregion

        #region Empty State

        private static void CreateEmptyState(GameObject parent)
        {
            var emptyState = CreateChild(parent, "EmptyState");
            SetStretch(emptyState);

            var bg = emptyState.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.5f);

            var layout = emptyState.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // Empty Text
            var emptyText = CreateChild(emptyState, "EmptyStateText");
            var emptyTmp = emptyText.AddComponent<TextMeshProUGUI>();
            emptyTmp.text = "상품이 없습니다.";
            emptyTmp.fontSize = 24;
            emptyTmp.color = TextMuted;
            emptyTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(emptyTmp);

            // Initially hidden
            emptyState.SetActive(false);
        }

        #endregion

        #region Overlay Layer

        private static void CreateOverlayLayer(GameObject parent)
        {
            var overlay = CreateChild(parent, "OverlayLayer");
            SetStretch(overlay);
        }

        #endregion

        #region Helpers

        private static GameObject CreateChild(GameObject parent, string name)
        {
            var child = new GameObject(name);
            child.transform.SetParent(parent.transform, false);
            child.AddComponent<RectTransform>();
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

        private static void SetAnchorTop(GameObject go, float height)
        {
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, height);
            rect.anchoredPosition = Vector2.zero;
        }

        private static void SetAnchorBottom(GameObject go, float height)
        {
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(0, height);
            rect.anchoredPosition = Vector2.zero;
        }

        private static GameObject CreateButton(GameObject parent, string name, string text, float width, float height,
            Color bgColor)
        {
            var go = CreateChild(parent, name);

            var layout = go.AddComponent<LayoutElement>();
            layout.preferredWidth = width;
            layout.preferredHeight = height;

            var image = go.AddComponent<Image>();
            image.color = bgColor;

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;

            var label = CreateChild(go, "Label");
            SetStretch(label);
            var tmp = label.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 16;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(tmp);

            return go;
        }

        private static GameObject CreateLabel(GameObject parent, string name, string text, int fontSize, Color color,
            TextAlignmentOptions alignment)
        {
            var go = CreateChild(parent, name);

            var layout = go.AddComponent<LayoutElement>();
            layout.preferredHeight = fontSize + 10;

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region Connect Serialized Fields

        private static void ConnectSerializedFields(GameObject root)
        {
            var screen = root.GetComponent<ShopScreen>();
            if (screen == null) return;

            var so = new SerializedObject(screen);

            // Shopkeeper Display
            ConnectField(so, "_shopkeeperImage", root, "SafeArea/Content/LeftArea/ShopkeeperDisplay/CharacterImage");
            ConnectField(so, "_dialogueText", root,
                "SafeArea/Content/LeftArea/ShopkeeperDisplay/DialogueBox/DialogueText");

            // Tab
            ConnectField(so, "_tabGroup", root, "SafeArea/Content/LeftArea/TabList");
            ConnectTabButtons(so, root);

            // Product Grid
            ConnectField(so, "_productContainer", root,
                "SafeArea/Content/RightArea/ProductContainer/ScrollView/Viewport/ProductGrid");
            // Note: _productItemPrefab and _productCardPrefab should be assigned from project assets
            ConnectField(so, "_scrollRect", root, "SafeArea/Content/RightArea/ProductContainer/ScrollView");

            // Category Shortcuts
            ConnectField(so, "_categoryShortcutContainer", root,
                "SafeArea/Content/RightArea/ProductContainer/ProductGridFooter");
            ConnectCategoryShortcuts(so, root);

            // Currency Display
            ConnectField(so, "_goldText", root, "SafeArea/Header/RightGroup/CurrencyHUD/GoldDisplay/GoldText");
            ConnectField(so, "_gemText", root, "SafeArea/Header/RightGroup/CurrencyHUD/GemDisplay/GemText");

            // Footer
            ConnectField(so, "_refreshTimerText", root, "SafeArea/Footer/RefreshTimerGroup/RefreshTimerText");
            ConnectField(so, "_selectAllToggle", root, "SafeArea/Footer/SelectAllToggle");
            ConnectField(so, "_selectAllText", root, "SafeArea/Footer/SelectAllToggle/SelectAllText");
            ConnectField(so, "_bulkPurchaseButton", root, "SafeArea/Footer/BulkPurchaseButton");

            // Empty State
            ConnectField(so, "_emptyStateObject", root, "EmptyState");
            ConnectField(so, "_emptyStateText", root, "EmptyState/EmptyStateText");

            // Navigation
            ConnectField(so, "_backButton", root, "SafeArea/Header/BackButtonGroup/BackButton");
            ConnectField(so, "_homeButton", root, "SafeArea/Header/RightGroup/HomeButton");

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConnectTabButtons(SerializedObject so, GameObject root)
        {
            var prop = so.FindProperty("_tabButtons");
            if (prop == null) return;

            prop.arraySize = TAB_LABELS.Length;

            for (int i = 0; i < TAB_LABELS.Length; i++)
            {
                var target = root.transform.Find($"SafeArea/Content/LeftArea/TabList/TabButton_{i}");
                if (target != null)
                {
                    var element = prop.GetArrayElementAtIndex(i);
                    element.objectReferenceValue = target.GetComponent<ShopTabButton>();
                }
            }
        }

        private static void ConnectCategoryShortcuts(SerializedObject so, GameObject root)
        {
            var prop = so.FindProperty("_categoryShortcuts");
            if (prop == null) return;

            prop.arraySize = 3;

            for (int i = 0; i < 3; i++)
            {
                var target =
                    root.transform.Find(
                        $"SafeArea/Content/RightArea/ProductContainer/ProductGridFooter/CategoryShortcut_{i}");
                if (target != null)
                {
                    var element = prop.GetArrayElementAtIndex(i);
                    element.objectReferenceValue = target.GetComponent<CategoryShortcut>();
                }
            }
        }

        private static void ConnectField(SerializedObject so, string fieldName, GameObject root, string path)
        {
            var prop = so.FindProperty(fieldName);
            if (prop == null) return;

            var target = root.transform.Find(path);
            if (target == null) return;

            if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                var fieldType = prop.type;

                if (fieldType.Contains("Button"))
                    prop.objectReferenceValue = target.GetComponent<Button>();
                else if (fieldType.Contains("TMP_Text") || fieldType.Contains("TextMeshProUGUI"))
                    prop.objectReferenceValue = target.GetComponent<TextMeshProUGUI>();
                else if (fieldType.Contains("Image"))
                    prop.objectReferenceValue = target.GetComponent<Image>();
                else if (fieldType.Contains("Toggle"))
                    prop.objectReferenceValue = target.GetComponent<Toggle>();
                else if (fieldType.Contains("ScrollRect"))
                    prop.objectReferenceValue = target.GetComponent<ScrollRect>();
                else if (fieldType.Contains("TabGroupWidget"))
                    prop.objectReferenceValue = target.GetComponent<TabGroupWidget>();
                else if (fieldType.Contains("Transform"))
                    prop.objectReferenceValue = target;
                else if (fieldType.Contains("GameObject"))
                    prop.objectReferenceValue = target.gameObject;
                else
                    prop.objectReferenceValue = target.gameObject;
            }
        }

        #endregion
    }
}