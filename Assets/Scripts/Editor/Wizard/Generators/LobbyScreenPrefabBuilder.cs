using Sc.Contents.Lobby;
using Sc.Contents.Lobby.Widgets;
using Sc.Editor.AI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Editor.Wizard.Generators
{
    /// <summary>
    /// LobbyScreen 전용 프리팹 빌더.
    /// Luminous Dark Fantasy 테마 - 로비 화면 UI 생성.
    /// </summary>
    public static class LobbyScreenPrefabBuilder
    {
        #region Colors (Luminous Dark Fantasy Theme)

        // Deep cosmic background
        private static readonly Color BgDeep = new Color32(10, 10, 18, 255);
        private static readonly Color BgCard = new Color32(25, 25, 45, 217);
        private static readonly Color BgOverlay = new Color32(0, 0, 0, 200);

        // Glass effects
        private static readonly Color BgGlass = new Color32(255, 255, 255, 26);
        private static readonly Color GlassBorder = new Color32(255, 255, 255, 128);

        // Accent colors - ethereal glow
        private static readonly Color AccentPrimary = new Color32(100, 200, 255, 255);
        private static readonly Color AccentSecondary = new Color32(255, 107, 157, 255);
        private static readonly Color AccentGold = new Color32(255, 215, 100, 255);
        private static readonly Color AccentPurple = new Color32(168, 85, 247, 255);

        // Text colors
        private static readonly Color TextPrimary = Color.white;
        private static readonly Color TextSecondary = new Color(1f, 1f, 1f, 0.7f);
        private static readonly Color TextMuted = new Color(1f, 1f, 1f, 0.5f);
        private static readonly Color TextPlaceholder = new Color32(255, 255, 255, 128);

        // Glow effects
        private static readonly Color GlowCyan = new Color32(100, 200, 255, 100);

        #endregion

        #region Constants

        // Layout
        private const float HEADER_HEIGHT = 80f;
        private const float SAFE_AREA_PADDING = 20f;
        private const float BOTTOM_NAV_HEIGHT = 194f;

        // Left Top Area
        private const float LEFT_TOP_AREA_WIDTH = 400f;
        private const float LEFT_TOP_AREA_HEIGHT = 350f;
        private const float EVENT_BANNER_HEIGHT = 150f;
        private const float PASS_BUTTON_GROUP_HEIGHT = 100f;
        private const float INDICATOR_SIZE = 8f;

        // Right Top Area
        private const float RIGHT_TOP_AREA_WIDTH = 350f;
        private const float RIGHT_TOP_AREA_HEIGHT = 250f;
        private const float STAGE_PROGRESS_HEIGHT = 50f;
        private const float QUICK_MENU_GRID_HEIGHT = 180f;
        private const int QUICK_MENU_COLUMNS = 4;
        private const float QUICK_MENU_CELL_SIZE = 80f;

        // Center Area
        private const float CHARACTER_IMAGE_WIDTH = 350f;
        private const float CHARACTER_IMAGE_HEIGHT = 616f;
        private const float DIALOGUE_BOX_WIDTH = 300f;
        private const float DIALOGUE_BOX_HEIGHT = 60f;
        private const float ARROW_WIDTH = 40f;
        private const float ARROW_HEIGHT = 80f;

        // Right Bottom Area
        private const float RIGHT_BOTTOM_AREA_WIDTH = 220f;
        private const float RIGHT_BOTTOM_AREA_HEIGHT = 232f;
        private const float SHORTCUT_BUTTON_HEIGHT = 100f;
        private const float ADVENTURE_BUTTON_HEIGHT = 100f;

        // Bottom Nav
        private const float CONTENT_NAV_BUTTON_WIDTH = 100f;
        private const float NAV_ICON_SIZE = 48f;
        private const float NAV_LABEL_HEIGHT = 20f;

        // Common
        private const float ICON_SIZE_SMALL = 32f;
        private const float ICON_SIZE_MEDIUM = 40f;
        private const float BADGE_SIZE = 20f;
        private const float GLOW_HEIGHT = 4f;

        #endregion

        #region Config Data

        private static readonly (string type, string label)[] PassButtonConfigs =
        {
            ("LevelPass", "레벨패스"),
            ("StoryPass", "사록패스"),
            ("TrialPass", "트라이얼패스"),
            ("StepUpPackage", "스텝업패키지")
        };

        private static readonly (string target, string label)[] QuickMenuConfigs =
        {
            ("LiveEventScreen", "이벤트"),
            ("FarmScreen", "데일리농장"),
            ("FriendScreen", "친구"),
            ("QuestScreen", "퀘스트"),
            ("PowerUpScreen", "강화"),
            ("MonthlyScreen", "월정액"),
            ("ReturnScreen", "복귀"),
            ("MissionScreen", "미션")
        };

        private static readonly (string target, string label)[] ContentNavConfigs =
        {
            ("GachaScreen", "모집"),
            ("ShopScreen", "상점"),
            ("CharacterListScreen", "사도"),
            ("CardScreen", "카드"),
            ("TheaterScreen", "극장"),
            ("GuildScreen", "고단"),
            ("InventoryScreen", "가방")
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
        /// LobbyScreen 프리팹용 GameObject 생성.
        /// </summary>
        public static GameObject Build()
        {
            var root = CreateRoot();

            // 1. Background
            CreateBackground(root);

            // 2. SafeArea with all content
            var safeArea = CreateSafeArea(root);

            // 3. Header
            CreateHeader(safeArea);

            // 4. Content Area
            var content = CreateContent(safeArea);

            // 4.1 Left Top Area (EventBanner + PassButtons)
            var (eventBannerCarousel, passButtons) = CreateLeftTopArea(content);

            // 4.2 Right Top Area (StageProgress + QuickMenu)
            var (stageProgressWidget, quickMenuButtons) = CreateRightTopArea(content);

            // 4.3 Center Area (CharacterDisplay)
            var characterDisplay = CreateCenterArea(content);

            // 4.4 Right Bottom Area (InGameDashboard)
            var (inGameDashboard, stageShortcutBtn, stageShortcutLabel, adventureBtn) = CreateRightBottomArea(content);

            // 4.5 Bottom Nav
            var (contentNavButtons, bottomNavScroll) = CreateBottomNav(safeArea);

            // 5. OverlayLayer
            CreateOverlayLayer(root);

            // 6. Connect SerializedFields
            ConnectSerializedFields(
                root, eventBannerCarousel, passButtons, stageProgressWidget,
                quickMenuButtons, characterDisplay, inGameDashboard,
                stageShortcutBtn, stageShortcutLabel, adventureBtn,
                contentNavButtons, bottomNavScroll
            );

            return root;
        }

        #region Root

        private static GameObject CreateRoot()
        {
            var root = new GameObject("LobbyScreen");

            var rect = root.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            root.AddComponent<CanvasGroup>();
            root.AddComponent<LobbyScreen>();

            return root;
        }

        #endregion

        #region Background

        private static void CreateBackground(GameObject parent)
        {
            var bg = CreateChild(parent, "Background");
            SetStretch(bg);

            var image = bg.AddComponent<Image>();
            image.color = BgDeep;
            image.raycastTarget = true;
        }

        #endregion

        #region SafeArea

        private static GameObject CreateSafeArea(GameObject parent)
        {
            var safeArea = CreateChild(parent, "SafeArea");
            SetStretch(safeArea);
            return safeArea;
        }

        #endregion

        #region Header

        private static void CreateHeader(GameObject parent)
        {
            var header = CreateChild(parent, "Header");
            var rect = AddRectTransform(header);
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, HEADER_HEIGHT);
            rect.anchoredPosition = Vector2.zero;

            // ScreenHeaderPlaceholder
            var placeholder = CreateChild(header, "ScreenHeaderPlaceholder");
            SetStretch(placeholder);

            var image = placeholder.AddComponent<Image>();
            image.color = new Color32(0, 0, 0, 128);
            image.raycastTarget = true;

            var textGo = CreateChild(placeholder, "Text");
            SetStretch(textGo);

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "[ScreenHeader]";
            tmp.fontSize = 14f;
            tmp.color = TextPlaceholder;
            tmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(tmp);
        }

        #endregion

        #region Content

        private static GameObject CreateContent(GameObject parent)
        {
            var content = CreateChild(parent, "Content");
            var rect = AddRectTransform(content);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(0f, BOTTOM_NAV_HEIGHT);
            rect.offsetMax = new Vector2(0f, -HEADER_HEIGHT);
            return content;
        }

        #endregion

        #region Left Top Area

        private static (EventBannerCarousel, PassButton[]) CreateLeftTopArea(GameObject parent)
        {
            var area = CreateChild(parent, "LeftTopArea");
            var rect = AddRectTransform(area);
            SetAnchorTopLeft(rect, new Vector2(LEFT_TOP_AREA_WIDTH, LEFT_TOP_AREA_HEIGHT),
                new Vector2(SAFE_AREA_PADDING, -SAFE_AREA_PADDING));

            var layout = area.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 10f;

            var carousel = CreateEventBannerCarousel(area);
            var passButtons = CreatePassButtonGroup(area);

            return (carousel, passButtons);
        }

        private static EventBannerCarousel CreateEventBannerCarousel(GameObject parent)
        {
            var carouselGo = CreateChild(parent, "EventBannerCarousel");
            var rect = AddRectTransform(carouselGo);

            var layoutElem = carouselGo.AddComponent<LayoutElement>();
            layoutElem.preferredHeight = EVENT_BANNER_HEIGHT;

            var carousel = carouselGo.AddComponent<EventBannerCarousel>();

            // Banner Container
            var bannerContainer = CreateChild(carouselGo, "BannerContainer");
            var bannerRect = SetStretch(bannerContainer);
            bannerRect.offsetMax = new Vector2(0f, -20f);

            var bannerImage = bannerContainer.AddComponent<Image>();
            bannerImage.color = BgCard;
            bannerImage.raycastTarget = true;

            // Indicators
            var indicators = CreateChild(carouselGo, "Indicators");
            var indRect = AddRectTransform(indicators);
            indRect.anchorMin = new Vector2(0.5f, 0f);
            indRect.anchorMax = new Vector2(0.5f, 0f);
            indRect.pivot = new Vector2(0.5f, 0f);
            indRect.sizeDelta = new Vector2(100f, 16f);
            indRect.anchoredPosition = new Vector2(0f, 4f);

            var hlg = indicators.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.spacing = 8f;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;

            // Indicator Prefab (template)
            var indicatorPrefab = CreateChild(indicators, "IndicatorPrefab");
            var indPrefabRect = AddRectTransform(indicatorPrefab);
            indPrefabRect.sizeDelta = new Vector2(INDICATOR_SIZE, INDICATOR_SIZE);

            var indImage = indicatorPrefab.AddComponent<Image>();
            indImage.color = TextSecondary;
            indicatorPrefab.SetActive(false);

            // Connect EventBannerCarousel fields
            ConnectEventBannerCarouselFields(carousel, bannerContainer.transform, indicators.transform,
                indicatorPrefab);

            return carousel;
        }

        private static PassButton[] CreatePassButtonGroup(GameObject parent)
        {
            var group = CreateChild(parent, "PassButtonGroup");
            var rect = AddRectTransform(group);

            var layoutElem = group.AddComponent<LayoutElement>();
            layoutElem.preferredHeight = PASS_BUTTON_GROUP_HEIGHT;

            var hlg = group.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.spacing = 8f;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;

            var passButtons = new PassButton[PassButtonConfigs.Length];
            for (int i = 0; i < PassButtonConfigs.Length; i++)
            {
                passButtons[i] = CreatePassButton(group, PassButtonConfigs[i].type, PassButtonConfigs[i].label);
            }

            return passButtons;
        }

        private static PassButton CreatePassButton(GameObject parent, string passType, string label)
        {
            var buttonGo = CreateChild(parent, $"PassButton_{passType}");
            var rect = AddRectTransform(buttonGo);

            var vlg = buttonGo.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.spacing = 4f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.padding = new RectOffset(4, 4, 4, 4);

            var bgImage = buttonGo.AddComponent<Image>();
            bgImage.color = BgCard;
            bgImage.raycastTarget = true;

            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = bgImage;

            var widget = buttonGo.AddComponent<PassButton>();

            // Icon
            var iconGo = CreateChild(buttonGo, "Icon");
            var iconRect = AddRectTransform(iconGo);
            var iconLayoutElem = iconGo.AddComponent<LayoutElement>();
            iconLayoutElem.preferredHeight = 50f;

            var iconImage = iconGo.AddComponent<Image>();
            iconImage.color = AccentGold;

            // Label
            var labelGo = CreateChild(buttonGo, "Label");
            var labelRect = AddRectTransform(labelGo);
            var labelLayoutElem = labelGo.AddComponent<LayoutElement>();
            labelLayoutElem.preferredHeight = 20f;

            var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
            labelTmp.text = label;
            labelTmp.fontSize = 12f;
            labelTmp.color = TextPrimary;
            labelTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(labelTmp);

            // NewBadge
            var newBadge = CreateChild(buttonGo, "NewBadge");
            var badgeRect = AddRectTransform(newBadge);
            badgeRect.anchorMin = new Vector2(1f, 1f);
            badgeRect.anchorMax = new Vector2(1f, 1f);
            badgeRect.pivot = new Vector2(1f, 1f);
            badgeRect.sizeDelta = new Vector2(24f, 14f);
            badgeRect.anchoredPosition = new Vector2(-4f, -4f);

            var badgeImage = newBadge.AddComponent<Image>();
            badgeImage.color = AccentSecondary;

            var badgeText = CreateChild(newBadge, "Text");
            SetStretch(badgeText);
            var badgeTmp = badgeText.AddComponent<TextMeshProUGUI>();
            badgeTmp.text = "NEW";
            badgeTmp.fontSize = 10f;
            badgeTmp.color = TextPrimary;
            badgeTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(badgeTmp);

            newBadge.SetActive(false);

            // Connect PassButton fields
            ConnectPassButtonFields(widget, button, iconImage, labelTmp, newBadge, passType);

            return widget;
        }

        #endregion

        #region Right Top Area

        private static (StageProgressWidget, QuickMenuButton[]) CreateRightTopArea(GameObject parent)
        {
            var area = CreateChild(parent, "RightTopArea");
            var rect = AddRectTransform(area);
            SetAnchorTopRight(rect, new Vector2(RIGHT_TOP_AREA_WIDTH, RIGHT_TOP_AREA_HEIGHT),
                new Vector2(-SAFE_AREA_PADDING, -SAFE_AREA_PADDING));

            var layout = area.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperRight;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 10f;

            var stageProgress = CreateStageProgressWidget(area);
            var quickMenuButtons = CreateQuickMenuGrid(area);

            return (stageProgress, quickMenuButtons);
        }

        private static StageProgressWidget CreateStageProgressWidget(GameObject parent)
        {
            var widgetGo = CreateChild(parent, "StageProgressWidget");
            var rect = AddRectTransform(widgetGo);

            var layoutElem = widgetGo.AddComponent<LayoutElement>();
            layoutElem.preferredHeight = STAGE_PROGRESS_HEIGHT;

            var hlg = widgetGo.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.spacing = 10f;
            hlg.childControlWidth = false;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;

            var widget = widgetGo.AddComponent<StageProgressWidget>();

            // Stage Label
            var stageLabelGo = CreateChild(widgetGo, "StageLabel");
            var stageLabelRect =
                AddRectTransform(stageLabelGo);
            var stageLabelLayout = stageLabelGo.AddComponent<LayoutElement>();
            stageLabelLayout.preferredWidth = 60f;

            var stageLabelTmp = stageLabelGo.AddComponent<TextMeshProUGUI>();
            stageLabelTmp.text = "11-1";
            stageLabelTmp.fontSize = 18f;
            stageLabelTmp.fontStyle = FontStyles.Bold;
            stageLabelTmp.color = AccentPrimary;
            stageLabelTmp.alignment = TextAlignmentOptions.Left;
            ApplyFont(stageLabelTmp);

            // Stage Name
            var stageNameGo = CreateChild(widgetGo, "StageName");
            var stageNameRect = AddRectTransform(stageNameGo);
            var stageNameLayout = stageNameGo.AddComponent<LayoutElement>();
            stageNameLayout.flexibleWidth = 1f;

            var stageNameTmp = stageNameGo.AddComponent<TextMeshProUGUI>();
            stageNameTmp.text = "최후의 방어선!";
            stageNameTmp.fontSize = 14f;
            stageNameTmp.color = TextSecondary;
            stageNameTmp.alignment = TextAlignmentOptions.Left;
            ApplyFont(stageNameTmp);

            // Connect StageProgressWidget fields (no progress bar/text in this simplified version)
            ConnectStageProgressWidgetFields(widget, stageLabelTmp, stageNameTmp, null, null);

            return widget;
        }

        private static QuickMenuButton[] CreateQuickMenuGrid(GameObject parent)
        {
            var grid = CreateChild(parent, "QuickMenuGrid");
            var rect = AddRectTransform(grid);

            var layoutElem = grid.AddComponent<LayoutElement>();
            layoutElem.preferredHeight = QUICK_MENU_GRID_HEIGHT;

            var glg = grid.AddComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(QUICK_MENU_CELL_SIZE, QUICK_MENU_CELL_SIZE);
            glg.spacing = new Vector2(8f, 8f);
            glg.startCorner = GridLayoutGroup.Corner.UpperLeft;
            glg.startAxis = GridLayoutGroup.Axis.Horizontal;
            glg.childAlignment = TextAnchor.UpperLeft;
            glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            glg.constraintCount = QUICK_MENU_COLUMNS;

            var buttons = new QuickMenuButton[QuickMenuConfigs.Length];
            for (int i = 0; i < QuickMenuConfigs.Length; i++)
            {
                buttons[i] = CreateQuickMenuButton(grid, QuickMenuConfigs[i].target, QuickMenuConfigs[i].label);
            }

            return buttons;
        }

        private static QuickMenuButton CreateQuickMenuButton(GameObject parent, string targetScreen, string label)
        {
            var buttonGo = CreateChild(parent, $"QuickMenuButton_{targetScreen}");
            var rect = AddRectTransform(buttonGo);

            var vlg = buttonGo.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.spacing = 4f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.padding = new RectOffset(4, 4, 8, 4);

            var bgImage = buttonGo.AddComponent<Image>();
            bgImage.color = BgGlass;
            bgImage.raycastTarget = true;

            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = bgImage;

            var widget = buttonGo.AddComponent<QuickMenuButton>();

            // Icon
            var iconGo = CreateChild(buttonGo, "Icon");
            var iconRect = AddRectTransform(iconGo);
            var iconLayoutElem = iconGo.AddComponent<LayoutElement>();
            iconLayoutElem.preferredHeight = 32f;

            var iconImage = iconGo.AddComponent<Image>();
            iconImage.color = AccentPrimary;

            // Label
            var labelGo = CreateChild(buttonGo, "Label");
            var labelRect = AddRectTransform(labelGo);
            var labelLayoutElem = labelGo.AddComponent<LayoutElement>();
            labelLayoutElem.preferredHeight = 16f;

            var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
            labelTmp.text = label;
            labelTmp.fontSize = 11f;
            labelTmp.color = TextSecondary;
            labelTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(labelTmp);

            // Badge
            var badge = CreateChild(buttonGo, "Badge");
            var badgeRect = AddRectTransform(badge);
            badgeRect.anchorMin = new Vector2(1f, 1f);
            badgeRect.anchorMax = new Vector2(1f, 1f);
            badgeRect.pivot = new Vector2(1f, 1f);
            badgeRect.sizeDelta = new Vector2(BADGE_SIZE, BADGE_SIZE);
            badgeRect.anchoredPosition = new Vector2(-4f, -4f);

            var badgeBgImage = badge.AddComponent<Image>();
            badgeBgImage.color = AccentSecondary;

            var badgeCountGo = CreateChild(badge, "Count");
            SetStretch(badgeCountGo);
            var badgeCountTmp = badgeCountGo.AddComponent<TextMeshProUGUI>();
            badgeCountTmp.text = "0";
            badgeCountTmp.fontSize = 10f;
            badgeCountTmp.color = TextPrimary;
            badgeCountTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(badgeCountTmp);

            badge.SetActive(false);

            // Connect QuickMenuButton fields
            ConnectQuickMenuButtonFields(widget, button, iconImage, labelTmp, badge, badgeCountTmp, targetScreen);

            return widget;
        }

        #endregion

        #region Center Area

        private static CharacterDisplayWidget CreateCenterArea(GameObject parent)
        {
            var area = CreateChild(parent, "CenterArea");
            SetStretch(area);

            var widget = CreateCharacterDisplayWidget(area);
            return widget;
        }

        private static CharacterDisplayWidget CreateCharacterDisplayWidget(GameObject parent)
        {
            var widgetGo = CreateChild(parent, "CharacterDisplay");
            var rect = AddRectTransform(widgetGo);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(CHARACTER_IMAGE_WIDTH + ARROW_WIDTH * 2 + 20f, CHARACTER_IMAGE_HEIGHT);
            rect.anchoredPosition = Vector2.zero;

            var widget = widgetGo.AddComponent<CharacterDisplayWidget>();

            // Content (Character Image + Dialogue)
            var content = CreateChild(widgetGo, "Content");
            var contentRect = AddRectTransform(content);
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.pivot = new Vector2(0.5f, 0.5f);
            contentRect.sizeDelta = new Vector2(CHARACTER_IMAGE_WIDTH, CHARACTER_IMAGE_HEIGHT);
            contentRect.anchoredPosition = Vector2.zero;

            // Character Image (Button)
            var charImageGo = CreateChild(content, "CharacterImage");
            SetStretch(charImageGo);

            var charImage = charImageGo.AddComponent<Image>();
            charImage.color = BgCard;
            charImage.raycastTarget = true;

            var charButton = charImageGo.AddComponent<Button>();
            charButton.targetGraphic = charImage;

            // Glow Effect
            var glowGo = CreateChild(content, "GlowEffect");
            var glowRect = AddRectTransform(glowGo);
            glowRect.anchorMin = new Vector2(0f, 0f);
            glowRect.anchorMax = new Vector2(1f, 0f);
            glowRect.pivot = new Vector2(0.5f, 0f);
            glowRect.sizeDelta = new Vector2(0f, GLOW_HEIGHT);
            glowRect.anchoredPosition = Vector2.zero;

            var glowImage = glowGo.AddComponent<Image>();
            glowImage.color = GlowCyan;

            // Dialogue Box
            var dialogueBox = CreateChild(content, "DialogueBox");
            var dialogueRect = AddRectTransform(dialogueBox);
            dialogueRect.anchorMin = new Vector2(0.5f, 0f);
            dialogueRect.anchorMax = new Vector2(0.5f, 0f);
            dialogueRect.pivot = new Vector2(0.5f, 0f);
            dialogueRect.sizeDelta = new Vector2(DIALOGUE_BOX_WIDTH, DIALOGUE_BOX_HEIGHT);
            dialogueRect.anchoredPosition = new Vector2(0f, 20f);

            var dialogueBg = dialogueBox.AddComponent<Image>();
            dialogueBg.color = BgOverlay;

            var dialogueTextGo = CreateChild(dialogueBox, "Text");
            SetStretch(dialogueTextGo);
            var dialogueTmp = dialogueTextGo.AddComponent<TextMeshProUGUI>();
            dialogueTmp.text = "오늘도 좋은 하루!";
            dialogueTmp.fontSize = 14f;
            dialogueTmp.color = TextPrimary;
            dialogueTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(dialogueTmp);

            // Left Arrow
            var leftArrow = CreateChild(widgetGo, "LeftArrow");
            var leftArrowRect = AddRectTransform(leftArrow);
            leftArrowRect.anchorMin = new Vector2(0f, 0.5f);
            leftArrowRect.anchorMax = new Vector2(0f, 0.5f);
            leftArrowRect.pivot = new Vector2(0f, 0.5f);
            leftArrowRect.sizeDelta = new Vector2(ARROW_WIDTH, ARROW_HEIGHT);
            leftArrowRect.anchoredPosition = Vector2.zero;

            var leftArrowImage = leftArrow.AddComponent<Image>();
            leftArrowImage.color = BgGlass;

            var leftArrowBtn = leftArrow.AddComponent<Button>();
            leftArrowBtn.targetGraphic = leftArrowImage;

            var leftArrowText = CreateChild(leftArrow, "Text");
            SetStretch(leftArrowText);
            var leftArrowTmp = leftArrowText.AddComponent<TextMeshProUGUI>();
            leftArrowTmp.text = "<";
            leftArrowTmp.fontSize = 24f;
            leftArrowTmp.color = TextPrimary;
            leftArrowTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(leftArrowTmp);

            // Right Arrow
            var rightArrow = CreateChild(widgetGo, "RightArrow");
            var rightArrowRect = AddRectTransform(rightArrow);
            rightArrowRect.anchorMin = new Vector2(1f, 0.5f);
            rightArrowRect.anchorMax = new Vector2(1f, 0.5f);
            rightArrowRect.pivot = new Vector2(1f, 0.5f);
            rightArrowRect.sizeDelta = new Vector2(ARROW_WIDTH, ARROW_HEIGHT);
            rightArrowRect.anchoredPosition = Vector2.zero;

            var rightArrowImage = rightArrow.AddComponent<Image>();
            rightArrowImage.color = BgGlass;

            var rightArrowBtn = rightArrow.AddComponent<Button>();
            rightArrowBtn.targetGraphic = rightArrowImage;

            var rightArrowText = CreateChild(rightArrow, "Text");
            SetStretch(rightArrowText);
            var rightArrowTmp = rightArrowText.AddComponent<TextMeshProUGUI>();
            rightArrowTmp.text = ">";
            rightArrowTmp.fontSize = 24f;
            rightArrowTmp.color = TextPrimary;
            rightArrowTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(rightArrowTmp);

            // Connect CharacterDisplayWidget fields
            ConnectCharacterDisplayWidgetFields(widget, charImage, dialogueTmp, charButton, leftArrowBtn, rightArrowBtn,
                glowImage);

            return widget;
        }

        #endregion

        #region Right Bottom Area

        private static (GameObject, Button, TMP_Text, Button) CreateRightBottomArea(GameObject parent)
        {
            var area = CreateChild(parent, "RightBottomArea");
            var rect = AddRectTransform(area);
            SetAnchorBottomRight(rect, new Vector2(RIGHT_BOTTOM_AREA_WIDTH, RIGHT_BOTTOM_AREA_HEIGHT),
                new Vector2(-SAFE_AREA_PADDING, SAFE_AREA_PADDING));

            return CreateInGameDashboard(area);
        }

        private static (GameObject, Button, TMP_Text, Button) CreateInGameDashboard(GameObject parent)
        {
            var dashboard = CreateChild(parent, "InGameContentDashboard");
            SetStretch(dashboard);

            var vlg = dashboard.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.spacing = 16f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            var (shortcutBtn, shortcutLabel) = CreateStageShortcutButton(dashboard);
            var adventureBtn = CreateAdventureButton(dashboard);

            return (dashboard, shortcutBtn, shortcutLabel, adventureBtn);
        }

        private static (Button, TMP_Text) CreateStageShortcutButton(GameObject parent)
        {
            var buttonGo = CreateChild(parent, "StageShortcutButton");
            var rect = AddRectTransform(buttonGo);

            var layoutElem = buttonGo.AddComponent<LayoutElement>();
            layoutElem.preferredHeight = SHORTCUT_BUTTON_HEIGHT;

            var vlg = buttonGo.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.spacing = 4f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.padding = new RectOffset(8, 8, 8, 8);

            var bgImage = buttonGo.AddComponent<Image>();
            bgImage.color = AccentPrimary;
            bgImage.raycastTarget = true;

            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = bgImage;

            // Icon
            var iconGo = CreateChild(buttonGo, "Icon");
            var iconRect = AddRectTransform(iconGo);
            var iconLayoutElem = iconGo.AddComponent<LayoutElement>();
            iconLayoutElem.preferredHeight = 40f;

            var iconImage = iconGo.AddComponent<Image>();
            iconImage.color = BgDeep;

            // Label
            var labelGo = CreateChild(buttonGo, "Label");
            var labelRect = AddRectTransform(labelGo);
            var labelLayoutElem = labelGo.AddComponent<LayoutElement>();
            labelLayoutElem.preferredHeight = 20f;

            var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
            labelTmp.text = "11-1 바로 가자!";
            labelTmp.fontSize = 14f;
            labelTmp.fontStyle = FontStyles.Bold;
            labelTmp.color = BgDeep;
            labelTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(labelTmp);

            return (button, labelTmp);
        }

        private static Button CreateAdventureButton(GameObject parent)
        {
            var buttonGo = CreateChild(parent, "AdventureButton");
            var rect = AddRectTransform(buttonGo);

            var layoutElem = buttonGo.AddComponent<LayoutElement>();
            layoutElem.preferredHeight = ADVENTURE_BUTTON_HEIGHT;

            var vlg = buttonGo.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.spacing = 4f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.padding = new RectOffset(8, 8, 8, 8);

            var bgImage = buttonGo.AddComponent<Image>();
            bgImage.color = AccentSecondary;
            bgImage.raycastTarget = true;

            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = bgImage;

            // Icon
            var iconGo = CreateChild(buttonGo, "Icon");
            var iconRect = AddRectTransform(iconGo);
            var iconLayoutElem = iconGo.AddComponent<LayoutElement>();
            iconLayoutElem.preferredHeight = 40f;

            var iconImage = iconGo.AddComponent<Image>();
            iconImage.color = TextPrimary;

            // Label
            var labelGo = CreateChild(buttonGo, "Label");
            var labelRect = AddRectTransform(labelGo);
            var labelLayoutElem = labelGo.AddComponent<LayoutElement>();
            labelLayoutElem.preferredHeight = 20f;

            var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
            labelTmp.text = "모험";
            labelTmp.fontSize = 16f;
            labelTmp.fontStyle = FontStyles.Bold;
            labelTmp.color = TextPrimary;
            labelTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(labelTmp);

            return button;
        }

        #endregion

        #region Bottom Nav

        private static (ContentNavButton[], ScrollRect) CreateBottomNav(GameObject parent)
        {
            var bottomNav = CreateChild(parent, "BottomNav");
            var rect = AddRectTransform(bottomNav);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(0f, BOTTOM_NAV_HEIGHT);
            rect.anchoredPosition = Vector2.zero;

            var bgImage = bottomNav.AddComponent<Image>();
            bgImage.color = BgDeep;

            // ScrollRect
            var scrollRect = bottomNav.AddComponent<ScrollRect>();
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.elasticity = 0.1f;

            // Viewport
            var viewport = CreateChild(bottomNav, "Viewport");
            var viewportRect = SetStretch(viewport);
            viewportRect.offsetMin = new Vector2(SAFE_AREA_PADDING, 0f);
            viewportRect.offsetMax = new Vector2(-SAFE_AREA_PADDING, 0f);

            var mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            var viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = Color.white;

            scrollRect.viewport = viewportRect;

            // Content
            var content = CreateChild(viewport, "Content");
            var contentRect = AddRectTransform(content);
            contentRect.anchorMin = new Vector2(0f, 0f);
            contentRect.anchorMax = new Vector2(0f, 1f);
            contentRect.pivot = new Vector2(0f, 0.5f);
            contentRect.anchoredPosition = Vector2.zero;

            var hlg = content.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.spacing = 10f;
            hlg.childControlWidth = false;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;
            hlg.padding = new RectOffset(0, 0, 10, 10);

            var csf = content.AddComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            scrollRect.content = contentRect;

            var buttons = new ContentNavButton[ContentNavConfigs.Length];
            for (int i = 0; i < ContentNavConfigs.Length; i++)
            {
                buttons[i] = CreateContentNavButton(content, ContentNavConfigs[i].target, ContentNavConfigs[i].label);
            }

            return (buttons, scrollRect);
        }

        private static ContentNavButton CreateContentNavButton(GameObject parent, string targetScreen, string label)
        {
            var buttonGo = CreateChild(parent, $"ContentNavButton_{targetScreen}");
            var rect = AddRectTransform(buttonGo);

            var layoutElem = buttonGo.AddComponent<LayoutElement>();
            layoutElem.preferredWidth = CONTENT_NAV_BUTTON_WIDTH;

            var vlg = buttonGo.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.spacing = 4f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.padding = new RectOffset(8, 8, 8, 8);

            var bgImage = buttonGo.AddComponent<Image>();
            bgImage.color = BgCard;
            bgImage.raycastTarget = true;

            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = bgImage;

            var widget = buttonGo.AddComponent<ContentNavButton>();

            // Icon
            var iconGo = CreateChild(buttonGo, "Icon");
            var iconRect = AddRectTransform(iconGo);
            var iconLayoutElem = iconGo.AddComponent<LayoutElement>();
            iconLayoutElem.preferredHeight = NAV_ICON_SIZE;

            var iconImage = iconGo.AddComponent<Image>();
            iconImage.color = AccentPrimary;

            // Label
            var labelGo = CreateChild(buttonGo, "Label");
            var labelRect = AddRectTransform(labelGo);
            var labelLayoutElem = labelGo.AddComponent<LayoutElement>();
            labelLayoutElem.preferredHeight = NAV_LABEL_HEIGHT;

            var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
            labelTmp.text = label;
            labelTmp.fontSize = 12f;
            labelTmp.color = TextSecondary;
            labelTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(labelTmp);

            // Badge
            var badge = CreateChild(buttonGo, "Badge");
            var badgeRect = AddRectTransform(badge);
            badgeRect.anchorMin = new Vector2(1f, 1f);
            badgeRect.anchorMax = new Vector2(1f, 1f);
            badgeRect.pivot = new Vector2(1f, 1f);
            badgeRect.sizeDelta = new Vector2(BADGE_SIZE, BADGE_SIZE);
            badgeRect.anchoredPosition = new Vector2(-4f, -4f);

            var badgeBgImage = badge.AddComponent<Image>();
            badgeBgImage.color = AccentSecondary;

            var badgeCountGo = CreateChild(badge, "Count");
            SetStretch(badgeCountGo);
            var badgeCountTmp = badgeCountGo.AddComponent<TextMeshProUGUI>();
            badgeCountTmp.text = "0";
            badgeCountTmp.fontSize = 10f;
            badgeCountTmp.color = TextPrimary;
            badgeCountTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(badgeCountTmp);

            badge.SetActive(false);

            // Glow Effect
            var glow = CreateChild(buttonGo, "GlowEffect");
            var glowRect = AddRectTransform(glow);
            glowRect.anchorMin = new Vector2(0.5f, 0f);
            glowRect.anchorMax = new Vector2(0.5f, 0f);
            glowRect.pivot = new Vector2(0.5f, 0f);
            glowRect.sizeDelta = new Vector2(60f, GLOW_HEIGHT);
            glowRect.anchoredPosition = Vector2.zero;

            var glowImage = glow.AddComponent<Image>();
            glowImage.color = GlowCyan;
            glow.SetActive(false);

            // Connect ContentNavButton fields
            ConnectContentNavButtonFields(widget, button, iconImage, labelTmp, badge, badgeCountTmp, glowImage,
                targetScreen);

            return widget;
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
            EventBannerCarousel eventBannerCarousel,
            PassButton[] passButtons,
            StageProgressWidget stageProgressWidget,
            QuickMenuButton[] quickMenuButtons,
            CharacterDisplayWidget characterDisplay,
            GameObject inGameDashboard,
            Button stageShortcutButton,
            TMP_Text stageShortcutLabel,
            Button adventureButton,
            ContentNavButton[] contentNavButtons,
            ScrollRect bottomNavScroll)
        {
            var screen = root.GetComponent<LobbyScreen>();
            if (screen == null) return;

            var so = new SerializedObject(screen);

            // Left Top Area
            so.FindProperty("_eventBannerCarousel").objectReferenceValue = eventBannerCarousel;

            var passButtonsProp = so.FindProperty("_passButtons");
            passButtonsProp.arraySize = passButtons.Length;
            for (int i = 0; i < passButtons.Length; i++)
            {
                passButtonsProp.GetArrayElementAtIndex(i).objectReferenceValue = passButtons[i];
            }

            // Right Top Area
            so.FindProperty("_stageProgressWidget").objectReferenceValue = stageProgressWidget;

            var quickMenuProp = so.FindProperty("_quickMenuButtons");
            quickMenuProp.arraySize = quickMenuButtons.Length;
            for (int i = 0; i < quickMenuButtons.Length; i++)
            {
                quickMenuProp.GetArrayElementAtIndex(i).objectReferenceValue = quickMenuButtons[i];
            }

            // Center Area
            so.FindProperty("_characterDisplay").objectReferenceValue = characterDisplay;

            // Right Bottom Area
            so.FindProperty("_inGameDashboard").objectReferenceValue = inGameDashboard;
            so.FindProperty("_stageShortcutButton").objectReferenceValue = stageShortcutButton;
            so.FindProperty("_stageShortcutLabel").objectReferenceValue = stageShortcutLabel;
            so.FindProperty("_adventureButton").objectReferenceValue = adventureButton;

            // Bottom Nav
            var contentNavProp = so.FindProperty("_contentNavButtons");
            contentNavProp.arraySize = contentNavButtons.Length;
            for (int i = 0; i < contentNavButtons.Length; i++)
            {
                contentNavProp.GetArrayElementAtIndex(i).objectReferenceValue = contentNavButtons[i];
            }

            so.FindProperty("_bottomNavScroll").objectReferenceValue = bottomNavScroll;

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConnectEventBannerCarouselFields(
            EventBannerCarousel widget,
            Transform bannerContainer,
            Transform indicatorContainer,
            GameObject indicatorPrefab)
        {
            var so = new SerializedObject(widget);

            so.FindProperty("_bannerContainer").objectReferenceValue = bannerContainer;
            so.FindProperty("_indicatorContainer").objectReferenceValue = indicatorContainer;
            so.FindProperty("_indicatorPrefab").objectReferenceValue = indicatorPrefab;

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConnectPassButtonFields(
            PassButton widget,
            Button button,
            Image icon,
            TMP_Text label,
            GameObject newBadge,
            string passType)
        {
            var so = new SerializedObject(widget);

            so.FindProperty("_button").objectReferenceValue = button;
            so.FindProperty("_icon").objectReferenceValue = icon;
            so.FindProperty("_label").objectReferenceValue = label;
            so.FindProperty("_newBadge").objectReferenceValue = newBadge;
            so.FindProperty("_passType").stringValue = passType;

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConnectStageProgressWidgetFields(
            StageProgressWidget widget,
            TMP_Text stageLabel,
            TMP_Text stageName,
            Slider progressBar,
            TMP_Text progressText)
        {
            var so = new SerializedObject(widget);

            so.FindProperty("_stageLabel").objectReferenceValue = stageLabel;
            so.FindProperty("_stageName").objectReferenceValue = stageName;
            if (progressBar != null)
                so.FindProperty("_progressBar").objectReferenceValue = progressBar;
            if (progressText != null)
                so.FindProperty("_progressText").objectReferenceValue = progressText;

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConnectQuickMenuButtonFields(
            QuickMenuButton widget,
            Button button,
            Image icon,
            TMP_Text label,
            GameObject badge,
            TMP_Text badgeCount,
            string targetScreen)
        {
            var so = new SerializedObject(widget);

            so.FindProperty("_button").objectReferenceValue = button;
            so.FindProperty("_icon").objectReferenceValue = icon;
            so.FindProperty("_label").objectReferenceValue = label;
            so.FindProperty("_badge").objectReferenceValue = badge;
            so.FindProperty("_badgeCount").objectReferenceValue = badgeCount;
            so.FindProperty("_targetScreen").stringValue = targetScreen;

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConnectCharacterDisplayWidgetFields(
            CharacterDisplayWidget widget,
            Image characterImage,
            TMP_Text dialogueText,
            Button characterButton,
            Button leftArrow,
            Button rightArrow,
            Image glowEffect)
        {
            var so = new SerializedObject(widget);

            so.FindProperty("_characterImage").objectReferenceValue = characterImage;
            so.FindProperty("_dialogueText").objectReferenceValue = dialogueText;
            so.FindProperty("_characterButton").objectReferenceValue = characterButton;
            so.FindProperty("_leftArrow").objectReferenceValue = leftArrow;
            so.FindProperty("_rightArrow").objectReferenceValue = rightArrow;
            so.FindProperty("_glowEffect").objectReferenceValue = glowEffect;

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConnectContentNavButtonFields(
            ContentNavButton widget,
            Button button,
            Image icon,
            TMP_Text label,
            GameObject badge,
            TMP_Text badgeCount,
            Image glowEffect,
            string targetScreen)
        {
            var so = new SerializedObject(widget);

            so.FindProperty("_button").objectReferenceValue = button;
            so.FindProperty("_icon").objectReferenceValue = icon;
            so.FindProperty("_label").objectReferenceValue = label;
            so.FindProperty("_badge").objectReferenceValue = badge;
            so.FindProperty("_badgeCount").objectReferenceValue = badgeCount;
            so.FindProperty("_glowEffect").objectReferenceValue = glowEffect;
            so.FindProperty("_targetScreen").stringValue = targetScreen;

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

        private static RectTransform AddRectTransform(GameObject go)
        {
            var rect = go.GetComponent<RectTransform>();
            if (rect == null)
                rect = go.AddComponent<RectTransform>();
            return rect;
        }

        private static RectTransform SetStretch(GameObject go)
        {
            var rect = AddRectTransform(go);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return rect;
        }

        private static void SetAnchorTopLeft(RectTransform rect, Vector2 size, Vector2 position)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
        }

        private static void SetAnchorTopRight(RectTransform rect, Vector2 size, Vector2 position)
        {
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
        }

        private static void SetAnchorBottomRight(RectTransform rect, Vector2 size, Vector2 position)
        {
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
        }

        #endregion
    }
}