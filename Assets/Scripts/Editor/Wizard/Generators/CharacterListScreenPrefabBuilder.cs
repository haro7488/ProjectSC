using Sc.Contents.Character;
using Sc.Contents.Character.Widgets;
using Sc.Editor.AI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Editor.Wizard.Generators
{
    /// <summary>
    /// CharacterListScreen 전용 프리팹 빌더.
    /// Luminous Dark Fantasy 테마 - 캐릭터 목록 화면 UI 생성.
    /// </summary>
    public static class CharacterListScreenPrefabBuilder
    {
        #region Colors (Luminous Dark Fantasy Theme)

        // Deep cosmic background
        private static readonly Color BgDeep = new Color32(5, 5, 12, 255);

        // Accent colors - ethereal glow
        private static readonly Color AccentCyan = new Color32(0, 212, 255, 255);
        private static readonly Color AccentGreen = new Color32(120, 255, 120, 255);
        private static readonly Color AccentGold = new Color32(255, 200, 100, 255);

        // Text colors
        private static readonly Color TextPrimary = Color.white;
        private static readonly Color TextSecondary = new Color(1f, 1f, 1f, 0.7f);
        private static readonly Color TextMuted = new Color(1f, 1f, 1f, 0.4f);

        // Glass effects
        private static readonly Color GlassOverlay = new Color(1f, 1f, 1f, 0.03f);
        private static readonly Color GlassBorder = new Color(1f, 1f, 1f, 0.08f);

        // Tab colors
        private static readonly Color TabActive = new Color32(120, 255, 120, 60);
        private static readonly Color TabInactive = new Color32(50, 50, 50, 100);

        // Button colors
        private static readonly Color ButtonDefault = new Color32(40, 40, 60, 200);
        private static readonly Color ButtonHighlight = new Color32(60, 60, 80, 220);

        // Card colors
        private static readonly Color CardBackground = new Color32(30, 30, 45, 230);

        #endregion

        #region Constants

        private const float HEADER_HEIGHT = 60f;
        private const float TAB_AREA_HEIGHT = 50f;
        private const float FILTER_AREA_HEIGHT = 40f;
        private const float SAFE_AREA_PADDING = 20f;

        // Grid settings
        private const int GRID_COLUMNS = 6;
        private const float CARD_WIDTH = 150f;
        private const float CARD_HEIGHT = 200f;
        private const float GRID_SPACING = 10f;

        // Icon sizes
        private const float ICON_SIZE_SMALL = 30f;
        private const float ICON_SIZE_MEDIUM = 40f;
        private const float STAR_SIZE = 20f;
        private const int MAX_STARS = 5;

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
        /// CharacterListScreen 프리팹용 GameObject 생성.
        /// </summary>
        public static GameObject Build()
        {
            var root = CreateRoot();

            // 1. Background
            CreateBackground(root);

            // 2. SafeArea with all content
            var safeArea = CreateSafeArea(root);

            // 3. Header (BackButton, TitleText, HomeButton)
            var (backButton, homeButton) = CreateHeader(safeArea);

            // 4. TabArea (AllCharactersTab, FavoritesTab)
            var (allTab, allTabText, favTab, favTabText) = CreateTabArea(safeArea);

            // 5. FilterArea (ExpressionFilter, FilterToggle, SortButton, SortOrderToggle)
            var filterWidget = CreateFilterArea(safeArea);

            // 6. Character Grid (ScrollView + GridLayoutGroup)
            var (scrollRect, gridContainer, cardTemplate) = CreateCharacterGrid(safeArea);

            // 7. OverlayLayer
            CreateOverlayLayer(root);

            // 8. Connect SerializedFields
            ConnectSerializedFields(
                root,
                backButton,
                homeButton,
                allTab,
                allTabText,
                favTab,
                favTabText,
                filterWidget,
                scrollRect,
                gridContainer,
                cardTemplate
            );

            return root;
        }

        #region Root

        private static GameObject CreateRoot()
        {
            var root = new GameObject("CharacterListScreen");

            var rect = root.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            root.AddComponent<CanvasGroup>();
            root.AddComponent<CharacterListScreen>();

            return root;
        }

        #endregion

        #region Background

        private static void CreateBackground(GameObject parent)
        {
            var bg = CreateChild(parent, "Background");
            SetStretch(bg);

            var bgImage = bg.AddComponent<Image>();
            bgImage.color = BgDeep;
            bgImage.raycastTarget = false;
        }

        #endregion

        #region SafeArea

        private static GameObject CreateSafeArea(GameObject parent)
        {
            var safeArea = CreateChild(parent, "SafeArea");
            var rect = SetStretch(safeArea);

            // SafeArea 내부 패딩 적용
            rect.offsetMin = new Vector2(SAFE_AREA_PADDING, SAFE_AREA_PADDING);
            rect.offsetMax = new Vector2(-SAFE_AREA_PADDING, -SAFE_AREA_PADDING);

            // VerticalLayoutGroup for content stacking
            var vlg = safeArea.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 5f;

            return safeArea;
        }

        #endregion

        #region Header

        private static (Button backButton, Button homeButton) CreateHeader(GameObject parent)
        {
            var header = CreateChild(parent, "Header");
            var headerRect = header.AddComponent<RectTransform>();
            headerRect.sizeDelta = new Vector2(0, HEADER_HEIGHT);

            var layoutElement = header.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = HEADER_HEIGHT;
            layoutElement.flexibleWidth = 1;

            var headerImage = header.AddComponent<Image>();
            headerImage.color = GlassOverlay;
            headerImage.raycastTarget = false;

            // HorizontalLayoutGroup for header content
            var hlg = header.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = false;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;
            hlg.spacing = 10f;
            hlg.padding = new RectOffset(10, 10, 5, 5);

            // LeftArea
            var leftArea = CreateChild(header, "LeftArea");
            var leftRect = leftArea.AddComponent<RectTransform>();
            leftRect.sizeDelta = new Vector2(200, 0);

            var leftLayout = leftArea.AddComponent<LayoutElement>();
            leftLayout.preferredWidth = 200;

            var leftHlg = leftArea.AddComponent<HorizontalLayoutGroup>();
            leftHlg.childAlignment = TextAnchor.MiddleLeft;
            leftHlg.childControlWidth = false;
            leftHlg.childControlHeight = true;
            leftHlg.spacing = 10f;

            // BackButton
            var backButton = CreateHeaderButton(leftArea, "BackButton", "<");

            // TitleText
            var titleObj = CreateChild(leftArea, "TitleText");
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(120, 40);

            var titleLayoutElem = titleObj.AddComponent<LayoutElement>();
            titleLayoutElem.preferredWidth = 120;

            var titleTmp = titleObj.AddComponent<TextMeshProUGUI>();
            titleTmp.text = "사도";
            titleTmp.fontSize = 28;
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.color = TextPrimary;
            titleTmp.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(titleTmp);

            // CenterArea (spacer)
            var centerArea = CreateChild(header, "CenterArea");
            var centerLayout = centerArea.AddComponent<LayoutElement>();
            centerLayout.flexibleWidth = 1;

            // RightArea
            var rightArea = CreateChild(header, "RightArea");
            var rightRect = rightArea.AddComponent<RectTransform>();
            rightRect.sizeDelta = new Vector2(150, 0);

            var rightLayout = rightArea.AddComponent<LayoutElement>();
            rightLayout.preferredWidth = 150;

            var rightHlg = rightArea.AddComponent<HorizontalLayoutGroup>();
            rightHlg.childAlignment = TextAnchor.MiddleRight;
            rightHlg.childControlWidth = false;
            rightHlg.childControlHeight = true;
            rightHlg.spacing = 10f;

            // CurrencyHUD placeholder
            var currencyHud = CreateChild(rightArea, "CurrencyHUD");
            var currencyRect = currencyHud.AddComponent<RectTransform>();
            currencyRect.sizeDelta = new Vector2(80, 40);

            var currencyLayout = currencyHud.AddComponent<LayoutElement>();
            currencyLayout.preferredWidth = 80;

            var currencyImage = currencyHud.AddComponent<Image>();
            currencyImage.color = GlassBorder;

            // HomeButton
            var homeButton = CreateHeaderButton(rightArea, "HomeButton", "H");

            return (backButton, homeButton);
        }

        private static Button CreateHeaderButton(GameObject parent, string name, string label)
        {
            var btnObj = CreateChild(parent, name);
            var btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(40, 40);

            var layoutElem = btnObj.AddComponent<LayoutElement>();
            layoutElem.preferredWidth = 40;
            layoutElem.preferredHeight = 40;

            var btnImage = btnObj.AddComponent<Image>();
            btnImage.color = ButtonDefault;

            var button = btnObj.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1, 1, 1, 1.2f);
            colors.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            button.colors = colors;

            // Icon/Label
            var iconObj = CreateChild(btnObj, "Icon");
            SetStretch(iconObj);

            var iconTmp = iconObj.AddComponent<TextMeshProUGUI>();
            iconTmp.text = label;
            iconTmp.fontSize = 20;
            iconTmp.fontStyle = FontStyles.Bold;
            iconTmp.color = TextPrimary;
            iconTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(iconTmp);

            return button;
        }

        #endregion

        #region TabArea

        private static (Button allTab, TMP_Text allTabText, Button favTab, TMP_Text favTabText) CreateTabArea(
            GameObject parent)
        {
            var tabArea = CreateChild(parent, "TabArea");
            var tabRect = tabArea.AddComponent<RectTransform>();
            tabRect.sizeDelta = new Vector2(0, TAB_AREA_HEIGHT);

            var layoutElement = tabArea.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = TAB_AREA_HEIGHT;
            layoutElement.flexibleWidth = 1;

            // TabGroup (HorizontalLayoutGroup)
            var tabGroup = CreateChild(tabArea, "TabGroup");
            SetStretch(tabGroup);

            var hlg = tabGroup.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;
            hlg.spacing = 5f;
            hlg.padding = new RectOffset(5, 5, 5, 5);

            // AllCharactersTab
            var (allTab, allTabText) = CreateTabButton(tabGroup, "AllCharactersTab", "모여라 사도!", true);

            // FavoritesTab
            var (favTab, favTabText) = CreateTabButton(tabGroup, "FavoritesTab", "관심 사도 0/2", false);

            return (allTab, allTabText, favTab, favTabText);
        }

        private static (Button button, TMP_Text text) CreateTabButton(GameObject parent, string name, string label,
            bool isActive)
        {
            var tabObj = CreateChild(parent, name);

            var image = tabObj.AddComponent<Image>();
            image.color = isActive ? TabActive : TabInactive;

            var button = tabObj.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1, 1, 1, 1.1f);
            colors.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            button.colors = colors;

            // Tab Text
            var textObj = CreateChild(tabObj, "Text");
            SetStretch(textObj);

            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 18;
            tmp.fontStyle = isActive ? FontStyles.Bold : FontStyles.Normal;
            tmp.color = isActive ? TextPrimary : TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(tmp);

            return (button, tmp);
        }

        #endregion

        #region FilterArea

        private static CharacterFilterWidget CreateFilterArea(GameObject parent)
        {
            var filterArea = CreateChild(parent, "FilterArea");
            var filterRect = filterArea.AddComponent<RectTransform>();
            filterRect.sizeDelta = new Vector2(0, FILTER_AREA_HEIGHT);

            var layoutElement = filterArea.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = FILTER_AREA_HEIGHT;
            layoutElement.flexibleWidth = 1;

            var filterImage = filterArea.AddComponent<Image>();
            filterImage.color = GlassOverlay;
            filterImage.raycastTarget = false;

            // Add CharacterFilterWidget component
            var filterWidget = filterArea.AddComponent<CharacterFilterWidget>();

            // FilterGroup (HorizontalLayoutGroup)
            var filterGroup = CreateChild(filterArea, "FilterGroup");
            SetStretch(filterGroup);

            var hlg = filterGroup.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = false;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;
            hlg.spacing = 10f;
            hlg.padding = new RectOffset(10, 10, 5, 5);

            // ExpressionFilter
            var expressionFilter = CreateFilterButton(filterGroup, "ExpressionFilter", "감정표현", 80);

            // FilterToggle
            var filterToggle = CreateFilterButton(filterGroup, "FilterToggle", "필터 OFF", 80);

            // Spacer (flexible width)
            var spacer = CreateChild(filterGroup, "Spacer");
            var spacerLayout = spacer.AddComponent<LayoutElement>();
            spacerLayout.flexibleWidth = 1;

            // SortButton
            var sortButton = CreateFilterButton(filterGroup, "SortButton", "정렬", 60);

            // SortOrderToggle
            var sortOrderToggle = CreateFilterButton(filterGroup, "SortOrderToggle", "↓", 40);

            // Connect filter widget fields via SerializedObject
            ConnectFilterWidgetFields(filterWidget, expressionFilter, filterToggle, sortButton, sortOrderToggle);

            return filterWidget;
        }

        private static Button CreateFilterButton(GameObject parent, string name, string label, float width)
        {
            var btnObj = CreateChild(parent, name);

            var layoutElem = btnObj.AddComponent<LayoutElement>();
            layoutElem.preferredWidth = width;

            var image = btnObj.AddComponent<Image>();
            image.color = ButtonDefault;

            var button = btnObj.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1, 1, 1, 1.1f);
            colors.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            button.colors = colors;

            // Label
            var textObj = CreateChild(btnObj, "Text");
            SetStretch(textObj);

            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 14;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(tmp);

            return button;
        }

        private static void ConnectFilterWidgetFields(CharacterFilterWidget widget, Button expressionFilter,
            Button filterToggle, Button sortButton, Button sortOrderToggle)
        {
            var so = new SerializedObject(widget);

            so.FindProperty("_expressionFilterButton").objectReferenceValue = expressionFilter;
            so.FindProperty("_expressionFilterText").objectReferenceValue =
                expressionFilter.GetComponentInChildren<TMP_Text>();

            so.FindProperty("_filterToggleButton").objectReferenceValue = filterToggle;
            so.FindProperty("_filterToggleText").objectReferenceValue =
                filterToggle.GetComponentInChildren<TMP_Text>();

            so.FindProperty("_sortButton").objectReferenceValue = sortButton;
            so.FindProperty("_sortText").objectReferenceValue = sortButton.GetComponentInChildren<TMP_Text>();

            so.FindProperty("_sortOrderButton").objectReferenceValue = sortOrderToggle;
            // SortOrderIcon은 Image가 필요하므로 별도 처리
            var sortOrderIcon = sortOrderToggle.GetComponentInChildren<TMP_Text>();
            if (sortOrderIcon != null)
            {
                // sortOrderIcon이 Image가 아니라 Text이므로 일단 null로 둠
                // 실제로는 Image 아이콘이 필요하면 추가 구현
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion

        #region Character Grid

        private static (ScrollRect scrollRect, Transform gridContainer, GameObject cardTemplate) CreateCharacterGrid(
            GameObject parent)
        {
            var content = CreateChild(parent, "Content");

            var contentLayout = content.AddComponent<LayoutElement>();
            contentLayout.flexibleHeight = 1;
            contentLayout.flexibleWidth = 1;

            // ScrollView
            var scrollView = CreateChild(content, "CharacterGrid");
            SetStretch(scrollView);

            var scrollRect = scrollView.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.elasticity = 0.1f;
            scrollRect.scrollSensitivity = 30f;

            var scrollImage = scrollView.AddComponent<Image>();
            scrollImage.color = new Color(0, 0, 0, 0.1f);

            // Viewport
            var viewport = CreateChild(scrollView, "Viewport");
            SetStretch(viewport);

            var viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = Color.clear;

            var mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            scrollRect.viewport = viewport.GetComponent<RectTransform>();

            // Grid Content
            var gridContent = CreateChild(viewport, "GridContent");
            var gridRect = gridContent.AddComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0, 1);
            gridRect.anchorMax = new Vector2(1, 1);
            gridRect.pivot = new Vector2(0.5f, 1);
            gridRect.anchoredPosition = Vector2.zero;

            var gridLayout = gridContent.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(CARD_WIDTH, CARD_HEIGHT);
            gridLayout.spacing = new Vector2(GRID_SPACING, GRID_SPACING);
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.UpperCenter;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = GRID_COLUMNS;
            gridLayout.padding = new RectOffset(10, 10, 10, 10);

            var contentSizeFitter = gridContent.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.content = gridRect;

            // CharacterCard Template (inactive)
            var cardTemplate = CreateCharacterCardTemplate(gridContent);

            return (scrollRect, gridContent.transform, cardTemplate);
        }

        private static GameObject CreateCharacterCardTemplate(GameObject parent)
        {
            var card = CreateChild(parent, "CharacterCardTemplate");
            var cardRect = card.GetComponent<RectTransform>();
            if (cardRect == null) cardRect = card.AddComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(CARD_WIDTH, CARD_HEIGHT);

            // Add CharacterCard component
            var cardComponent = card.AddComponent<CharacterCard>();

            // CardBackground
            var cardBg = CreateChild(card, "CardBackground");
            SetStretch(cardBg);

            var bgImage = cardBg.AddComponent<Image>();
            bgImage.color = CardBackground;

            // CharacterThumbnail
            var thumbnail = CreateChild(card, "CharacterThumbnail");
            var thumbRect = thumbnail.GetComponent<RectTransform>();
            if (thumbRect == null) thumbRect = thumbnail.AddComponent<RectTransform>();
            thumbRect.anchorMin = new Vector2(0, 0.25f);
            thumbRect.anchorMax = new Vector2(1, 1);
            thumbRect.offsetMin = new Vector2(5, 0);
            thumbRect.offsetMax = new Vector2(-5, -5);

            var thumbImage = thumbnail.AddComponent<Image>();
            thumbImage.color = GlassBorder;
            thumbImage.raycastTarget = false;

            // ElementIcon (top-left)
            var elementIcon = CreateChild(card, "ElementIcon");
            var elemRect = elementIcon.GetComponent<RectTransform>();
            if (elemRect == null) elemRect = elementIcon.AddComponent<RectTransform>();
            elemRect.anchorMin = new Vector2(0, 1);
            elemRect.anchorMax = new Vector2(0, 1);
            elemRect.pivot = new Vector2(0, 1);
            elemRect.sizeDelta = new Vector2(ICON_SIZE_SMALL, ICON_SIZE_SMALL);
            elemRect.anchoredPosition = new Vector2(5, -5);

            var elemImage = elementIcon.AddComponent<Image>();
            elemImage.color = AccentCyan;
            elemImage.raycastTarget = false;

            // RoleIcon (top-right)
            var roleIcon = CreateChild(card, "RoleIcon");
            var roleRect = roleIcon.GetComponent<RectTransform>();
            if (roleRect == null) roleRect = roleIcon.AddComponent<RectTransform>();
            roleRect.anchorMin = new Vector2(1, 1);
            roleRect.anchorMax = new Vector2(1, 1);
            roleRect.pivot = new Vector2(1, 1);
            roleRect.sizeDelta = new Vector2(ICON_SIZE_SMALL, ICON_SIZE_SMALL);
            roleRect.anchoredPosition = new Vector2(-5, -5);

            var roleImage = roleIcon.AddComponent<Image>();
            roleImage.color = AccentGold;
            roleImage.raycastTarget = false;

            // StarRating (bottom area)
            var starRating = CreateChild(card, "StarRating");
            var starRect = starRating.GetComponent<RectTransform>();
            if (starRect == null) starRect = starRating.AddComponent<RectTransform>();
            starRect.anchorMin = new Vector2(0.5f, 0);
            starRect.anchorMax = new Vector2(0.5f, 0);
            starRect.pivot = new Vector2(0.5f, 0);
            starRect.sizeDelta = new Vector2(STAR_SIZE * MAX_STARS + 5 * (MAX_STARS - 1), STAR_SIZE);
            starRect.anchoredPosition = new Vector2(0, 25);

            var starHlg = starRating.AddComponent<HorizontalLayoutGroup>();
            starHlg.childAlignment = TextAnchor.MiddleCenter;
            starHlg.childControlWidth = false;
            starHlg.childControlHeight = false;
            starHlg.spacing = 2f;

            // Create 5 stars
            for (int i = 0; i < MAX_STARS; i++)
            {
                var star = CreateChild(starRating, $"Star_{i}");
                var starObjRect = star.AddComponent<RectTransform>();
                starObjRect.sizeDelta = new Vector2(STAR_SIZE, STAR_SIZE);

                var starLayoutElem = star.AddComponent<LayoutElement>();
                starLayoutElem.preferredWidth = STAR_SIZE;
                starLayoutElem.preferredHeight = STAR_SIZE;

                var starImage = star.AddComponent<Image>();
                starImage.color = AccentGold;
                starImage.raycastTarget = false;
            }

            // NameText (bottom)
            var nameText = CreateChild(card, "NameText");
            var nameRect = nameText.GetComponent<RectTransform>();
            if (nameRect == null) nameRect = nameText.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0);
            nameRect.anchorMax = new Vector2(1, 0);
            nameRect.pivot = new Vector2(0.5f, 0);
            nameRect.sizeDelta = new Vector2(0, 25);
            nameRect.anchoredPosition = new Vector2(0, 0);

            var nameTmp = nameText.AddComponent<TextMeshProUGUI>();
            nameTmp.text = "캐릭터 이름";
            nameTmp.fontSize = 14;
            nameTmp.color = TextPrimary;
            nameTmp.alignment = TextAlignmentOptions.Center;
            nameTmp.enableWordWrapping = false;
            nameTmp.overflowMode = TextOverflowModes.Ellipsis;
            ApplyFont(nameTmp);

            // Button for interaction
            var button = card.AddComponent<Button>();
            var btnImage = card.AddComponent<Image>();
            btnImage.color = Color.clear;

            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1, 1, 1, 1.1f);
            colors.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            button.colors = colors;
            button.targetGraphic = btnImage;

            // Connect CharacterCard fields
            ConnectCharacterCardFields(cardComponent, bgImage, thumbImage, elemImage, roleImage, starRating.transform,
                nameTmp, button);

            // Set inactive (template)
            card.SetActive(false);

            return card;
        }

        private static void ConnectCharacterCardFields(CharacterCard card, Image cardBackground, Image thumbnail,
            Image elementIcon, Image roleIcon, Transform starContainer, TMP_Text nameText, Button button)
        {
            var so = new SerializedObject(card);

            so.FindProperty("_cardBackground").objectReferenceValue = cardBackground;
            so.FindProperty("_characterThumbnail").objectReferenceValue = thumbnail;
            so.FindProperty("_elementIcon").objectReferenceValue = elementIcon;
            so.FindProperty("_roleIcon").objectReferenceValue = roleIcon;
            so.FindProperty("_starContainer").objectReferenceValue = starContainer;
            so.FindProperty("_nameText").objectReferenceValue = nameText;
            so.FindProperty("_button").objectReferenceValue = button;

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion

        #region OverlayLayer

        private static void CreateOverlayLayer(GameObject parent)
        {
            var overlay = CreateChild(parent, "OverlayLayer");
            SetStretch(overlay);

            var canvasGroup = overlay.AddComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        #endregion

        #region SerializeField Connections

        private static void ConnectSerializedFields(
            GameObject root,
            Button backButton,
            Button homeButton,
            Button allCharactersTab,
            TMP_Text allCharactersTabText,
            Button favoritesTab,
            TMP_Text favoritesTabText,
            CharacterFilterWidget filterWidget,
            ScrollRect scrollRect,
            Transform gridContainer,
            GameObject cardTemplate)
        {
            var screen = root.GetComponent<CharacterListScreen>();
            if (screen == null) return;

            var so = new SerializedObject(screen);

            // Tab Area
            so.FindProperty("_allCharactersTab").objectReferenceValue = allCharactersTab;
            so.FindProperty("_allCharactersTabText").objectReferenceValue = allCharactersTabText;
            so.FindProperty("_favoritesTab").objectReferenceValue = favoritesTab;
            so.FindProperty("_favoritesTabText").objectReferenceValue = favoritesTabText;

            // Filter Area
            so.FindProperty("_filterWidget").objectReferenceValue = filterWidget;

            // Character Grid
            so.FindProperty("_characterGridContainer").objectReferenceValue = gridContainer;
            so.FindProperty("_scrollRect").objectReferenceValue = scrollRect;
            so.FindProperty("_characterCardPrefab").objectReferenceValue = cardTemplate;

            // Legacy fields
            so.FindProperty("_listContainer").objectReferenceValue = gridContainer;
            so.FindProperty("_characterItemPrefab").objectReferenceValue = cardTemplate;
            so.FindProperty("_backButton").objectReferenceValue = backButton;
            // _countText는 Header에 없으므로 null

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