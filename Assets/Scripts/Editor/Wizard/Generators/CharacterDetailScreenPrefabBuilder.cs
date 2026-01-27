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
    /// CharacterDetailScreen 전용 프리팹 빌더.
    /// Luminous Dark Fantasy 테마 - 캐릭터 상세 정보 화면 UI 생성.
    /// </summary>
    public static class CharacterDetailScreenPrefabBuilder
    {
        #region Colors (Luminous Dark Fantasy Theme)

        // Deep cosmic background
        private static readonly Color BgDeep = new Color32(10, 10, 18, 255);
        private static readonly Color BgCard = new Color32(25, 25, 45, 230);
        private static readonly Color BgPanel = new Color32(30, 30, 50, 200);
        private static readonly Color BgOverlay = new Color32(0, 0, 0, 180);

        // Accent colors
        private static readonly Color AccentCyan = new Color32(0, 212, 255, 255);
        private static readonly Color AccentGold = new Color32(255, 200, 100, 255);
        private static readonly Color AccentGreen = new Color32(34, 197, 94, 255);
        private static readonly Color AccentLime = new Color32(200, 220, 100, 255);
        private static readonly Color AccentPink = new Color32(255, 150, 200, 255);
        private static readonly Color AccentOrange = new Color32(255, 165, 80, 255);
        private static readonly Color AccentRed = new Color32(255, 100, 100, 255);

        // Text colors
        private static readonly Color TextPrimary = Color.white;
        private static readonly Color TextSecondary = new Color(1f, 1f, 1f, 0.7f);
        private static readonly Color TextMuted = new Color(1f, 1f, 1f, 0.5f);
        private static readonly Color TextDark = new Color32(30, 30, 30, 255);

        // Menu button colors
        private static readonly Color MenuNormalBg = new Color32(80, 80, 80, 200);
        private static readonly Color MenuSelectedBg = new Color32(150, 230, 150, 255);

        // Glass effects
        private static readonly Color GlassOverlay = new Color(1f, 1f, 1f, 0.05f);

        // Tag colors
        private static readonly Color TagPersonality = new Color32(255, 165, 80, 255); // 주황색 (성격)
        private static readonly Color TagRole = new Color32(255, 150, 200, 255); // 분홍색 (역할)
        private static readonly Color TagAttack = new Color32(255, 220, 100, 255); // 노란색 (공격타입)
        private static readonly Color TagPosition = new Color32(255, 100, 100, 255); // 빨간색 (배치)

        #endregion

        #region Constants

        private const float HEADER_HEIGHT = 60f;
        private const float LEFT_MENU_WIDTH = 100f;
        private const float MENU_BUTTON_HEIGHT = 50f;
        private const float MENU_BUTTON_SPACING = 8f;
        private const float RIGHT_PANEL_WIDTH = 320f;
        private const float BOTTOM_INFO_HEIGHT = 100f;
        private const float RIGHT_TOP_HEIGHT = 120f;
        private const float RIGHT_BOTTOM_HEIGHT = 80f;
        private const float STAT_ROW_HEIGHT = 36f;
        private const float STAR_SIZE = 24f;

        #endregion

        #region Font Helper

        private static void ApplyFont(TextMeshProUGUI tmp)
        {
            var font = EditorUIHelpers.GetProjectFont();
            if (font != null) tmp.font = font;
        }

        #endregion

        /// <summary>
        /// CharacterDetailScreen 프리팹용 GameObject 생성.
        /// </summary>
        public static GameObject Build()
        {
            var root = CreateRoot();

            // 1. Background
            CreateBackground(root);

            // 2. SafeArea with all content
            var safeArea = CreateSafeArea(root);

            // 3. Header
            var (backButton, titleText, homeButton) = CreateHeader(safeArea);

            // 4. LeftMenuArea
            var (menuContainer, infoMenu, levelUpMenu, equipmentMenu, skillMenu, promotionMenu, boardMenu, asideMenu)
                = CreateLeftMenuArea(safeArea);

            // 5. CenterArea
            var (characterImage, companionImage, characterSwitchButton, dogamButton) = CreateCenterArea(safeArea);

            // 6. BottomInfoArea
            var (characterInfoWidget, rarityBadge, rarityText, nameText) = CreateBottomInfoArea(safeArea);

            // 7. RightTopArea
            var (levelText, starRatingContainer, combatPowerWidget) = CreateRightTopArea(safeArea);

            // 8. RightCenterArea
            var characterStatWidget = CreateRightCenterArea(safeArea);

            // 9. RightBottomArea
            var costumeWidget = CreateRightBottomArea(safeArea);

            // 10. OverlayLayer
            CreateOverlayLayer(root);

            // 11. Connect SerializedFields
            ConnectSerializedFields(root,
                backButton, titleText, homeButton,
                menuContainer, infoMenu, levelUpMenu, equipmentMenu, skillMenu, promotionMenu, boardMenu, asideMenu,
                characterImage, companionImage, characterSwitchButton, dogamButton,
                characterInfoWidget, rarityBadge, rarityText, nameText,
                levelText, starRatingContainer, combatPowerWidget,
                characterStatWidget, costumeWidget);

            return root;
        }

        #region Root

        private static GameObject CreateRoot()
        {
            var root = new GameObject("CharacterDetailScreen");

            var rect = root.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            root.AddComponent<CanvasGroup>();
            root.AddComponent<CharacterDetailScreen>();

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

            // Gradient overlay for depth
            var gradient = CreateChild(bg, "GradientOverlay");
            SetStretch(gradient);
            var gradientImage = gradient.AddComponent<Image>();
            gradientImage.color = new Color(0, 0, 0, 0.3f);
            gradientImage.raycastTarget = false;
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

        private static (Button backButton, TMP_Text titleText, Button homeButton) CreateHeader(GameObject parent)
        {
            var header = CreateChild(parent, "Header");
            var rect = header.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, HEADER_HEIGHT);
            rect.anchoredPosition = Vector2.zero;

            // Header background
            var headerBg = header.AddComponent<Image>();
            headerBg.color = BgCard;
            headerBg.raycastTarget = true;

            // Back Button (left)
            var backBtn = CreateChild(header, "BackButton");
            var backRect = backBtn.AddComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0f, 0.5f);
            backRect.anchorMax = new Vector2(0f, 0.5f);
            backRect.pivot = new Vector2(0f, 0.5f);
            backRect.sizeDelta = new Vector2(50f, 40f);
            backRect.anchoredPosition = new Vector2(16f, 0f);

            var backBg = backBtn.AddComponent<Image>();
            backBg.color = GlassOverlay;
            var backButton = backBtn.AddComponent<Button>();

            var backIcon = CreateChild(backBtn, "Icon");
            SetStretch(backIcon);
            var backTmp = backIcon.AddComponent<TextMeshProUGUI>();
            backTmp.text = "<";
            backTmp.fontSize = 24;
            backTmp.color = TextPrimary;
            backTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(backTmp);

            // Title Text (center)
            var title = CreateChild(header, "TitleText");
            var titleRect = title.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(300f, 40f);

            var titleText = title.AddComponent<TextMeshProUGUI>();
            titleText.text = "Character Name";
            titleText.fontSize = 24;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = TextPrimary;
            titleText.alignment = TextAlignmentOptions.Center;
            ApplyFont(titleText);

            // Home Button (right)
            var homeBtn = CreateChild(header, "HomeButton");
            var homeRect = homeBtn.AddComponent<RectTransform>();
            homeRect.anchorMin = new Vector2(1f, 0.5f);
            homeRect.anchorMax = new Vector2(1f, 0.5f);
            homeRect.pivot = new Vector2(1f, 0.5f);
            homeRect.sizeDelta = new Vector2(50f, 40f);
            homeRect.anchoredPosition = new Vector2(-16f, 0f);

            var homeBg = homeBtn.AddComponent<Image>();
            homeBg.color = GlassOverlay;
            var homeButton = homeBtn.AddComponent<Button>();

            var homeIcon = CreateChild(homeBtn, "Icon");
            SetStretch(homeIcon);
            var homeTmp = homeIcon.AddComponent<TextMeshProUGUI>();
            homeTmp.text = "H";
            homeTmp.fontSize = 20;
            homeTmp.color = TextPrimary;
            homeTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(homeTmp);

            return (backButton, titleText, homeButton);
        }

        #endregion

        #region LeftMenuArea

        private static (Transform container, MenuButtonWidget info, MenuButtonWidget levelUp, MenuButtonWidget equipment
            ,
            MenuButtonWidget skill, MenuButtonWidget promotion, MenuButtonWidget board, MenuButtonWidget aside)
            CreateLeftMenuArea(GameObject parent)
        {
            var leftMenu = CreateChild(parent, "LeftMenuArea");
            var rect = leftMenu.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = new Vector2(LEFT_MENU_WIDTH, 0f);
            rect.offsetMin = new Vector2(0f, 0f);
            rect.offsetMax = new Vector2(LEFT_MENU_WIDTH, -HEADER_HEIGHT);

            // Menu List Container
            var menuList = CreateChild(leftMenu, "MenuList");
            var menuListRect = menuList.AddComponent<RectTransform>();
            menuListRect.anchorMin = new Vector2(0f, 1f);
            menuListRect.anchorMax = new Vector2(1f, 1f);
            menuListRect.pivot = new Vector2(0.5f, 1f);
            menuListRect.anchoredPosition = new Vector2(0f, -16f);
            menuListRect.sizeDelta = new Vector2(0f, 7 * MENU_BUTTON_HEIGHT + 6 * MENU_BUTTON_SPACING);

            var vlg = menuList.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = MENU_BUTTON_SPACING;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.padding = new RectOffset(8, 8, 0, 0);

            // Create 7 menu buttons
            var infoMenu = CreateMenuButton(menuList, "InfoMenuButton", "정보", MenuButtonType.Info);
            var levelUpMenu = CreateMenuButton(menuList, "LevelUpMenuButton", "레벨업", MenuButtonType.LevelUp);
            var equipmentMenu = CreateMenuButton(menuList, "EquipmentMenuButton", "장비", MenuButtonType.Equipment);
            var skillMenu = CreateMenuButton(menuList, "SkillMenuButton", "스킬", MenuButtonType.Skill);
            var promotionMenu = CreateMenuButton(menuList, "PromotionMenuButton", "승급", MenuButtonType.Promotion);
            var boardMenu = CreateMenuButton(menuList, "BoardMenuButton", "보드", MenuButtonType.Board);
            var asideMenu = CreateMenuButton(menuList, "AsideMenuButton", "어사이드", MenuButtonType.Aside);

            return (menuList.transform, infoMenu, levelUpMenu, equipmentMenu, skillMenu, promotionMenu, boardMenu,
                asideMenu);
        }

        private static MenuButtonWidget CreateMenuButton(GameObject parent, string name, string label,
            MenuButtonType type)
        {
            var btn = CreateChild(parent, name);
            var rect = btn.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, MENU_BUTTON_HEIGHT);

            var layoutElement = btn.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = MENU_BUTTON_HEIGHT;

            // Background
            var bg = btn.AddComponent<Image>();
            bg.color = MenuNormalBg;

            // Button component
            var button = btn.AddComponent<Button>();

            // Label
            var labelObj = CreateChild(btn, "Label");
            SetStretch(labelObj);
            var labelTmp = labelObj.AddComponent<TextMeshProUGUI>();
            labelTmp.text = label;
            labelTmp.fontSize = 14;
            labelTmp.color = TextPrimary;
            labelTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(labelTmp);

            // MenuButtonWidget
            var widget = btn.AddComponent<MenuButtonWidget>();

            // Connect serialized fields via SerializedObject
            var so = new SerializedObject(widget);
            so.FindProperty("_button").objectReferenceValue = button;
            so.FindProperty("_background").objectReferenceValue = bg;
            so.FindProperty("_labelText").objectReferenceValue = labelTmp;
            so.FindProperty("_buttonType").enumValueIndex = (int)type;
            so.ApplyModifiedPropertiesWithoutUndo();

            return widget;
        }

        #endregion

        #region CenterArea

        private static (Image characterImage, Image companionImage, Button switchButton, Button dogamButton)
            CreateCenterArea(GameObject parent)
        {
            var center = CreateChild(parent, "CenterArea");
            var rect = center.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.offsetMin = new Vector2(LEFT_MENU_WIDTH, BOTTOM_INFO_HEIGHT);
            rect.offsetMax = new Vector2(-RIGHT_PANEL_WIDTH, -HEADER_HEIGHT);

            // Character Display
            var charDisplay = CreateChild(center, "CharacterDisplay");
            SetStretch(charDisplay);

            // Character Image (full illustration)
            var charImg = CreateChild(charDisplay, "CharacterImage");
            var charImgRect = charImg.AddComponent<RectTransform>();
            charImgRect.anchorMin = new Vector2(0.5f, 0f);
            charImgRect.anchorMax = new Vector2(0.5f, 1f);
            charImgRect.pivot = new Vector2(0.5f, 0.5f);
            charImgRect.sizeDelta = new Vector2(400f, 0f);

            var characterImage = charImg.AddComponent<Image>();
            characterImage.color = new Color(1f, 1f, 1f, 0.1f); // Placeholder
            characterImage.raycastTarget = false;

            // Companion Image (top-right, smaller)
            var companionImg = CreateChild(charDisplay, "CompanionImage");
            var companionRect = companionImg.AddComponent<RectTransform>();
            companionRect.anchorMin = new Vector2(1f, 1f);
            companionRect.anchorMax = new Vector2(1f, 1f);
            companionRect.pivot = new Vector2(1f, 1f);
            companionRect.sizeDelta = new Vector2(80f, 80f);
            companionRect.anchoredPosition = new Vector2(-16f, -16f);

            var companionImage = companionImg.AddComponent<Image>();
            companionImage.color = new Color(1f, 1f, 1f, 0.1f); // Placeholder
            companionImage.raycastTarget = false;

            // Character Switch Button (right side arrow)
            var switchBtn = CreateChild(center, "CharacterSwitchButton");
            var switchRect = switchBtn.AddComponent<RectTransform>();
            switchRect.anchorMin = new Vector2(1f, 0.5f);
            switchRect.anchorMax = new Vector2(1f, 0.5f);
            switchRect.pivot = new Vector2(1f, 0.5f);
            switchRect.sizeDelta = new Vector2(40f, 80f);
            switchRect.anchoredPosition = new Vector2(-8f, 0f);

            var switchBg = switchBtn.AddComponent<Image>();
            switchBg.color = GlassOverlay;
            var characterSwitchButton = switchBtn.AddComponent<Button>();

            var switchIcon = CreateChild(switchBtn, "Icon");
            SetStretch(switchIcon);
            var switchTmp = switchIcon.AddComponent<TextMeshProUGUI>();
            switchTmp.text = ">";
            switchTmp.fontSize = 24;
            switchTmp.color = TextPrimary;
            switchTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(switchTmp);

            // Dogam Button (bottom-left)
            var dogamBtn = CreateChild(center, "DogamButton");
            var dogamRect = dogamBtn.AddComponent<RectTransform>();
            dogamRect.anchorMin = new Vector2(0f, 0f);
            dogamRect.anchorMax = new Vector2(0f, 0f);
            dogamRect.pivot = new Vector2(0f, 0f);
            dogamRect.sizeDelta = new Vector2(80f, 32f);
            dogamRect.anchoredPosition = new Vector2(16f, 16f);

            var dogamBg = dogamBtn.AddComponent<Image>();
            dogamBg.color = BgPanel;
            var dogamButton = dogamBtn.AddComponent<Button>();

            var dogamText = CreateChild(dogamBtn, "Text");
            SetStretch(dogamText);
            var dogamTmp = dogamText.AddComponent<TextMeshProUGUI>();
            dogamTmp.text = "도감";
            dogamTmp.fontSize = 14;
            dogamTmp.color = TextPrimary;
            dogamTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(dogamTmp);

            return (characterImage, companionImage, characterSwitchButton, dogamButton);
        }

        #endregion

        #region BottomInfoArea

        private static (CharacterInfoWidget widget, Image rarityBadge, TMP_Text rarityText, TMP_Text nameText)
            CreateBottomInfoArea(GameObject parent)
        {
            var bottomInfo = CreateChild(parent, "BottomInfoArea");
            var rect = bottomInfo.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0f, 0f);
            rect.sizeDelta = new Vector2(0f, BOTTOM_INFO_HEIGHT);
            rect.offsetMin = new Vector2(LEFT_MENU_WIDTH, 0f);
            rect.offsetMax = new Vector2(-RIGHT_PANEL_WIDTH, BOTTOM_INFO_HEIGHT);

            var infoBg = bottomInfo.AddComponent<Image>();
            infoBg.color = BgPanel;
            infoBg.raycastTarget = false;

            // Rarity Badge
            var rarityBadgeObj = CreateChild(bottomInfo, "RarityBadge");
            var rarityBadgeRect = rarityBadgeObj.AddComponent<RectTransform>();
            rarityBadgeRect.anchorMin = new Vector2(0f, 1f);
            rarityBadgeRect.anchorMax = new Vector2(0f, 1f);
            rarityBadgeRect.pivot = new Vector2(0f, 1f);
            rarityBadgeRect.sizeDelta = new Vector2(32f, 32f);
            rarityBadgeRect.anchoredPosition = new Vector2(16f, -16f);

            var rarityBadge = rarityBadgeObj.AddComponent<Image>();
            rarityBadge.color = AccentGold;

            var rarityTextObj = CreateChild(rarityBadgeObj, "Text");
            SetStretch(rarityTextObj);
            var rarityText = rarityTextObj.AddComponent<TextMeshProUGUI>();
            rarityText.text = "5";
            rarityText.fontSize = 18;
            rarityText.fontStyle = FontStyles.Bold;
            rarityText.color = TextDark;
            rarityText.alignment = TextAlignmentOptions.Center;
            ApplyFont(rarityText);

            // Name Text
            var nameObj = CreateChild(bottomInfo, "NameText");
            var nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0f, 1f);
            nameRect.anchorMax = new Vector2(0f, 1f);
            nameRect.pivot = new Vector2(0f, 1f);
            nameRect.sizeDelta = new Vector2(200f, 32f);
            nameRect.anchoredPosition = new Vector2(56f, -16f);

            var nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "Character Name";
            nameText.fontSize = 22;
            nameText.fontStyle = FontStyles.Bold;
            nameText.color = TextPrimary;
            nameText.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(nameText);

            // Tag Group
            var tagGroup = CreateChild(bottomInfo, "TagGroup");
            var tagGroupRect = tagGroup.AddComponent<RectTransform>();
            tagGroupRect.anchorMin = new Vector2(0f, 0f);
            tagGroupRect.anchorMax = new Vector2(1f, 0f);
            tagGroupRect.pivot = new Vector2(0f, 0f);
            tagGroupRect.sizeDelta = new Vector2(0f, 28f);
            tagGroupRect.anchoredPosition = new Vector2(16f, 16f);

            var hlg = tagGroup.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 8f;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;

            // Create 4 tags
            var personalityTag = CreateTag(tagGroup, "PersonalityTag", "활발", TagPersonality);
            var roleTag = CreateTag(tagGroup, "RoleTag", "서포터", TagRole);
            var attackTag = CreateTag(tagGroup, "AttackTypeTag", "물리", TagAttack);
            var positionTag = CreateTag(tagGroup, "PositionTag", "후열", TagPosition);

            // CharacterInfoWidget
            var infoWidget = bottomInfo.AddComponent<CharacterInfoWidget>();

            // Connect serialized fields
            var so = new SerializedObject(infoWidget);
            so.FindProperty("_rarityBadge").objectReferenceValue = rarityBadge;
            so.FindProperty("_rarityText").objectReferenceValue = rarityText;
            so.FindProperty("_nameText").objectReferenceValue = nameText;
            so.FindProperty("_tagContainer").objectReferenceValue = tagGroup.transform;
            so.FindProperty("_personalityTag").objectReferenceValue = personalityTag.GetComponent<Image>();
            so.FindProperty("_personalityText").objectReferenceValue =
                personalityTag.GetComponentInChildren<TextMeshProUGUI>();
            so.FindProperty("_roleTag").objectReferenceValue = roleTag.GetComponent<Image>();
            so.FindProperty("_roleText").objectReferenceValue = roleTag.GetComponentInChildren<TextMeshProUGUI>();
            so.FindProperty("_attackTypeTag").objectReferenceValue = attackTag.GetComponent<Image>();
            so.FindProperty("_attackTypeText").objectReferenceValue =
                attackTag.GetComponentInChildren<TextMeshProUGUI>();
            so.FindProperty("_positionTag").objectReferenceValue = positionTag.GetComponent<Image>();
            so.FindProperty("_positionText").objectReferenceValue =
                positionTag.GetComponentInChildren<TextMeshProUGUI>();
            so.ApplyModifiedPropertiesWithoutUndo();

            return (infoWidget, rarityBadge, rarityText, nameText);
        }

        private static GameObject CreateTag(GameObject parent, string name, string text, Color bgColor)
        {
            var tag = CreateChild(parent, name);
            var rect = tag.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(60f, 24f);

            var bg = tag.AddComponent<Image>();
            bg.color = bgColor;

            var textObj = CreateChild(tag, "Text");
            SetStretch(textObj);
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 12;
            tmp.color = TextDark;
            tmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(tmp);

            return tag;
        }

        #endregion

        #region RightTopArea

        private static (TMP_Text levelText, Transform starContainer, CombatPowerWidget combatPower)
            CreateRightTopArea(GameObject parent)
        {
            var rightTop = CreateChild(parent, "RightTopArea");
            var rect = rightTop.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(RIGHT_PANEL_WIDTH, RIGHT_TOP_HEIGHT);
            rect.anchoredPosition = new Vector2(0f, -HEADER_HEIGHT);

            var topBg = rightTop.AddComponent<Image>();
            topBg.color = BgPanel;
            topBg.raycastTarget = false;

            // Level Text
            var levelObj = CreateChild(rightTop, "LevelText");
            var levelRect = levelObj.AddComponent<RectTransform>();
            levelRect.anchorMin = new Vector2(0f, 1f);
            levelRect.anchorMax = new Vector2(0f, 1f);
            levelRect.pivot = new Vector2(0f, 1f);
            levelRect.sizeDelta = new Vector2(100f, 32f);
            levelRect.anchoredPosition = new Vector2(16f, -12f);

            var levelText = levelObj.AddComponent<TextMeshProUGUI>();
            levelText.text = "Lv. 52";
            levelText.fontSize = 20;
            levelText.fontStyle = FontStyles.Bold;
            levelText.color = TextPrimary;
            levelText.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(levelText);

            // Star Rating Container
            var starContainer = CreateChild(rightTop, "StarRatingContainer");
            var starRect = starContainer.AddComponent<RectTransform>();
            starRect.anchorMin = new Vector2(0f, 1f);
            starRect.anchorMax = new Vector2(0f, 1f);
            starRect.pivot = new Vector2(0f, 1f);
            starRect.sizeDelta = new Vector2(5 * STAR_SIZE + 4 * 4f, STAR_SIZE);
            starRect.anchoredPosition = new Vector2(120f, -16f);

            var starHlg = starContainer.AddComponent<HorizontalLayoutGroup>();
            starHlg.spacing = 4f;
            starHlg.childAlignment = TextAnchor.MiddleLeft;
            starHlg.childForceExpandWidth = false;
            starHlg.childForceExpandHeight = false;

            // Create 5 stars
            for (int i = 0; i < 5; i++)
            {
                var star = CreateChild(starContainer, $"Star_{i}");
                var starObjRect = star.AddComponent<RectTransform>();
                starObjRect.sizeDelta = new Vector2(STAR_SIZE, STAR_SIZE);

                var starImage = star.AddComponent<Image>();
                starImage.color = i < 3 ? AccentGold : TextMuted; // Default 3 filled stars
            }

            // Combat Power Widget
            var combatPowerObj = CreateChild(rightTop, "CombatPowerWidget");
            var cpRect = combatPowerObj.AddComponent<RectTransform>();
            cpRect.anchorMin = new Vector2(0f, 0f);
            cpRect.anchorMax = new Vector2(1f, 0f);
            cpRect.pivot = new Vector2(0.5f, 0f);
            cpRect.sizeDelta = new Vector2(-32f, 48f);
            cpRect.anchoredPosition = new Vector2(0f, 12f);

            var cpBg = combatPowerObj.AddComponent<Image>();
            cpBg.color = AccentGreen;

            // Label
            var cpLabel = CreateChild(combatPowerObj, "LabelText");
            var cpLabelRect = cpLabel.AddComponent<RectTransform>();
            cpLabelRect.anchorMin = new Vector2(0f, 0.5f);
            cpLabelRect.anchorMax = new Vector2(0f, 0.5f);
            cpLabelRect.pivot = new Vector2(0f, 0.5f);
            cpLabelRect.sizeDelta = new Vector2(60f, 24f);
            cpLabelRect.anchoredPosition = new Vector2(12f, 0f);

            var cpLabelText = cpLabel.AddComponent<TextMeshProUGUI>();
            cpLabelText.text = "전투력";
            cpLabelText.fontSize = 14;
            cpLabelText.color = TextPrimary;
            cpLabelText.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(cpLabelText);

            // Value
            var cpValue = CreateChild(combatPowerObj, "ValueText");
            var cpValueRect = cpValue.AddComponent<RectTransform>();
            cpValueRect.anchorMin = new Vector2(1f, 0.5f);
            cpValueRect.anchorMax = new Vector2(1f, 0.5f);
            cpValueRect.pivot = new Vector2(1f, 0.5f);
            cpValueRect.sizeDelta = new Vector2(120f, 32f);
            cpValueRect.anchoredPosition = new Vector2(-12f, 0f);

            var cpValueText = cpValue.AddComponent<TextMeshProUGUI>();
            cpValueText.text = "25,555";
            cpValueText.fontSize = 24;
            cpValueText.fontStyle = FontStyles.Bold;
            cpValueText.color = TextPrimary;
            cpValueText.alignment = TextAlignmentOptions.MidlineRight;
            ApplyFont(cpValueText);

            // CombatPowerWidget component
            var combatPowerWidget = combatPowerObj.AddComponent<CombatPowerWidget>();
            var cpSo = new SerializedObject(combatPowerWidget);
            cpSo.FindProperty("_background").objectReferenceValue = cpBg;
            cpSo.FindProperty("_labelText").objectReferenceValue = cpLabelText;
            cpSo.FindProperty("_valueText").objectReferenceValue = cpValueText;
            cpSo.ApplyModifiedPropertiesWithoutUndo();

            return (levelText, starContainer.transform, combatPowerWidget);
        }

        #endregion

        #region RightCenterArea

        private static CharacterStatWidget CreateRightCenterArea(GameObject parent)
        {
            var rightCenter = CreateChild(parent, "RightCenterArea");
            var rect = rightCenter.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.sizeDelta = new Vector2(RIGHT_PANEL_WIDTH, 0f);
            rect.offsetMin = new Vector2(-RIGHT_PANEL_WIDTH, RIGHT_BOTTOM_HEIGHT);
            rect.offsetMax = new Vector2(0f, -(HEADER_HEIGHT + RIGHT_TOP_HEIGHT));

            var centerBg = rightCenter.AddComponent<Image>();
            centerBg.color = BgCard;
            centerBg.raycastTarget = false;

            // Tab Group
            var tabGroup = CreateChild(rightCenter, "StatTabGroup");
            var tabGroupRect = tabGroup.AddComponent<RectTransform>();
            tabGroupRect.anchorMin = new Vector2(0f, 1f);
            tabGroupRect.anchorMax = new Vector2(1f, 1f);
            tabGroupRect.pivot = new Vector2(0.5f, 1f);
            tabGroupRect.sizeDelta = new Vector2(-16f, 40f);
            tabGroupRect.anchoredPosition = new Vector2(0f, -8f);

            var tabHlg = tabGroup.AddComponent<HorizontalLayoutGroup>();
            tabHlg.spacing = 4f;
            tabHlg.childAlignment = TextAnchor.MiddleCenter;
            tabHlg.childForceExpandWidth = true;
            tabHlg.childForceExpandHeight = true;

            // Status Tab
            var (statusTabBtn, statusTabBg, statusTabText) = CreateTabButton(tabGroup, "StatusTab", "스테이터스", true);

            // Trait Tab
            var (traitTabBtn, traitTabBg, traitTabText) = CreateTabButton(tabGroup, "TraitTab", "특성", false);

            // Stat List
            var statList = CreateChild(rightCenter, "StatList");
            var statListRect = statList.AddComponent<RectTransform>();
            statListRect.anchorMin = new Vector2(0f, 0f);
            statListRect.anchorMax = new Vector2(1f, 1f);
            statListRect.offsetMin = new Vector2(8f, 80f);
            statListRect.offsetMax = new Vector2(-8f, -56f);

            var statVlg = statList.AddComponent<VerticalLayoutGroup>();
            statVlg.spacing = 4f;
            statVlg.childAlignment = TextAnchor.UpperCenter;
            statVlg.childForceExpandWidth = true;
            statVlg.childForceExpandHeight = false;
            statVlg.childControlWidth = true;
            statVlg.childControlHeight = false;

            // Create 6 stat rows
            var (_, hpValue) = CreateStatRow(statList, "HP", "7,321");
            var (_, spValue) = CreateStatRow(statList, "SP", "400");
            var (_, physAtkValue) = CreateStatRow(statList, "물리 공격력", "1,234");
            var (_, magAtkValue) = CreateStatRow(statList, "마법 공격력", "567");
            var (_, physDefValue) = CreateStatRow(statList, "물리 방어력", "890");
            var (_, magDefValue) = CreateStatRow(statList, "마법 방어력", "678");

            // Action Buttons
            var actionBtns = CreateChild(rightCenter, "ActionButtons");
            var actionRect = actionBtns.AddComponent<RectTransform>();
            actionRect.anchorMin = new Vector2(0f, 0f);
            actionRect.anchorMax = new Vector2(1f, 0f);
            actionRect.pivot = new Vector2(0.5f, 0f);
            actionRect.sizeDelta = new Vector2(-16f, 32f);
            actionRect.anchoredPosition = new Vector2(0f, 44f);

            var actionHlg = actionBtns.AddComponent<HorizontalLayoutGroup>();
            actionHlg.spacing = 8f;
            actionHlg.childAlignment = TextAnchor.MiddleLeft;
            actionHlg.childForceExpandWidth = false;
            actionHlg.childForceExpandHeight = true;

            // Favorite Button
            var favoriteBtn = CreateActionButton(actionBtns, "FavoriteButton", "H", 32f);

            // Info Button
            var infoBtn = CreateActionButton(actionBtns, "InfoButton", "i", 32f);

            // Detail Button
            var detailBtn = CreateChild(rightCenter, "DetailButton");
            var detailRect = detailBtn.AddComponent<RectTransform>();
            detailRect.anchorMin = new Vector2(0f, 0f);
            detailRect.anchorMax = new Vector2(1f, 0f);
            detailRect.pivot = new Vector2(0.5f, 0f);
            detailRect.sizeDelta = new Vector2(-16f, 36f);
            detailRect.anchoredPosition = new Vector2(0f, 8f);

            var detailBg = detailBtn.AddComponent<Image>();
            detailBg.color = BgPanel;
            var detailButton = detailBtn.AddComponent<Button>();

            var detailTextObj = CreateChild(detailBtn, "Text");
            SetStretch(detailTextObj);
            var detailText = detailTextObj.AddComponent<TextMeshProUGUI>();
            detailText.text = "상세 보기";
            detailText.fontSize = 14;
            detailText.color = TextPrimary;
            detailText.alignment = TextAlignmentOptions.Center;
            ApplyFont(detailText);

            // CharacterStatWidget
            var statWidget = rightCenter.AddComponent<CharacterStatWidget>();
            var so = new SerializedObject(statWidget);
            so.FindProperty("_statusTab").objectReferenceValue = statusTabBtn;
            so.FindProperty("_statusTabText").objectReferenceValue = statusTabText;
            so.FindProperty("_statusTabBg").objectReferenceValue = statusTabBg;
            so.FindProperty("_traitTab").objectReferenceValue = traitTabBtn;
            so.FindProperty("_traitTabText").objectReferenceValue = traitTabText;
            so.FindProperty("_traitTabBg").objectReferenceValue = traitTabBg;
            so.FindProperty("_statListContainer").objectReferenceValue = statList.transform;
            so.FindProperty("_hpValue").objectReferenceValue = hpValue;
            so.FindProperty("_spValue").objectReferenceValue = spValue;
            so.FindProperty("_physicalAttackValue").objectReferenceValue = physAtkValue;
            so.FindProperty("_magicAttackValue").objectReferenceValue = magAtkValue;
            so.FindProperty("_physicalDefenseValue").objectReferenceValue = physDefValue;
            so.FindProperty("_magicDefenseValue").objectReferenceValue = magDefValue;
            so.FindProperty("_favoriteButton").objectReferenceValue = favoriteBtn.GetComponent<Button>();
            so.FindProperty("_favoriteIcon").objectReferenceValue = favoriteBtn.GetComponent<Image>();
            so.FindProperty("_infoButton").objectReferenceValue = infoBtn.GetComponent<Button>();
            so.FindProperty("_detailButton").objectReferenceValue = detailButton;
            so.FindProperty("_detailButtonText").objectReferenceValue = detailText;
            so.ApplyModifiedPropertiesWithoutUndo();

            return statWidget;
        }

        private static (Button button, Image bg, TMP_Text text) CreateTabButton(GameObject parent, string name,
            string label, bool isActive)
        {
            var tab = CreateChild(parent, name);
            var rect = tab.AddComponent<RectTransform>();

            var bg = tab.AddComponent<Image>();
            bg.color = isActive ? new Color32(240, 240, 240, 255) : new Color32(100, 100, 100, 200);

            var button = tab.AddComponent<Button>();

            var textObj = CreateChild(tab, "Text");
            SetStretch(textObj);
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 14;
            tmp.color = isActive ? TextDark : TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(tmp);

            return (button, bg, tmp);
        }

        private static (GameObject row, TMP_Text valueText) CreateStatRow(GameObject parent, string label, string value)
        {
            var row = CreateChild(parent, $"StatRow_{label}");
            var rect = row.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, STAT_ROW_HEIGHT);

            var layoutElement = row.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = STAT_ROW_HEIGHT;

            var rowBg = row.AddComponent<Image>();
            rowBg.color = new Color(0f, 0f, 0f, 0.2f);

            // Label
            var labelObj = CreateChild(row, "Label");
            var labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(0.5f, 1f);
            labelRect.offsetMin = new Vector2(8f, 0f);
            labelRect.offsetMax = new Vector2(0f, 0f);

            var labelTmp = labelObj.AddComponent<TextMeshProUGUI>();
            labelTmp.text = label;
            labelTmp.fontSize = 14;
            labelTmp.color = TextSecondary;
            labelTmp.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(labelTmp);

            // Value
            var valueObj = CreateChild(row, "Value");
            var valueRect = valueObj.AddComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0.5f, 0f);
            valueRect.anchorMax = new Vector2(1f, 1f);
            valueRect.offsetMin = new Vector2(0f, 0f);
            valueRect.offsetMax = new Vector2(-8f, 0f);

            var valueTmp = valueObj.AddComponent<TextMeshProUGUI>();
            valueTmp.text = value;
            valueTmp.fontSize = 16;
            valueTmp.fontStyle = FontStyles.Bold;
            valueTmp.color = TextPrimary;
            valueTmp.alignment = TextAlignmentOptions.MidlineRight;
            ApplyFont(valueTmp);

            return (row, valueTmp);
        }

        private static GameObject CreateActionButton(GameObject parent, string name, string icon, float size)
        {
            var btn = CreateChild(parent, name);
            var rect = btn.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(size, size);

            var bg = btn.AddComponent<Image>();
            bg.color = GlassOverlay;

            btn.AddComponent<Button>();

            var iconObj = CreateChild(btn, "Icon");
            SetStretch(iconObj);
            var tmp = iconObj.AddComponent<TextMeshProUGUI>();
            tmp.text = icon;
            tmp.fontSize = 16;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(tmp);

            return btn;
        }

        #endregion

        #region RightBottomArea

        private static CostumeWidget CreateRightBottomArea(GameObject parent)
        {
            var rightBottom = CreateChild(parent, "RightBottomArea");
            var rect = rightBottom.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.sizeDelta = new Vector2(RIGHT_PANEL_WIDTH, RIGHT_BOTTOM_HEIGHT);
            rect.anchoredPosition = Vector2.zero;

            var bottomBg = rightBottom.AddComponent<Image>();
            bottomBg.color = AccentLime;

            var button = rightBottom.AddComponent<Button>();

            // Costume Icon
            var iconObj = CreateChild(rightBottom, "CostumeIcon");
            var iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0f, 0.5f);
            iconRect.anchorMax = new Vector2(0f, 0.5f);
            iconRect.pivot = new Vector2(0f, 0.5f);
            iconRect.sizeDelta = new Vector2(48f, 48f);
            iconRect.anchoredPosition = new Vector2(16f, 0f);

            var iconImage = iconObj.AddComponent<Image>();
            iconImage.color = new Color(1f, 1f, 1f, 0.5f);

            // Title Text
            var titleObj = CreateChild(rightBottom, "TitleText");
            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 0.5f);
            titleRect.anchorMax = new Vector2(1f, 0.5f);
            titleRect.pivot = new Vector2(0f, 0.5f);
            titleRect.sizeDelta = new Vector2(-80f, 32f);
            titleRect.anchoredPosition = new Vector2(72f, 0f);

            var titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "캐릭터의 옷장";
            titleText.fontSize = 16;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color32(50, 80, 20, 255);
            titleText.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(titleText);

            // CostumeWidget
            var costumeWidget = rightBottom.AddComponent<CostumeWidget>();
            var so = new SerializedObject(costumeWidget);
            so.FindProperty("_background").objectReferenceValue = bottomBg;
            so.FindProperty("_button").objectReferenceValue = button;
            so.FindProperty("_costumeIcon").objectReferenceValue = iconImage;
            so.FindProperty("_titleText").objectReferenceValue = titleText;
            so.ApplyModifiedPropertiesWithoutUndo();

            return costumeWidget;
        }

        #endregion

        #region OverlayLayer

        private static void CreateOverlayLayer(GameObject parent)
        {
            var overlay = CreateChild(parent, "OverlayLayer");
            SetStretch(overlay);

            var overlayGroup = overlay.AddComponent<CanvasGroup>();
            overlayGroup.alpha = 0f;
            overlayGroup.blocksRaycasts = false;
            overlayGroup.interactable = false;
        }

        #endregion

        #region ConnectSerializedFields

        private static void ConnectSerializedFields(GameObject root,
            Button backButton, TMP_Text titleText, Button homeButton,
            Transform menuContainer, MenuButtonWidget infoMenu, MenuButtonWidget levelUpMenu,
            MenuButtonWidget equipmentMenu, MenuButtonWidget skillMenu, MenuButtonWidget promotionMenu,
            MenuButtonWidget boardMenu, MenuButtonWidget asideMenu,
            Image characterImage, Image companionImage, Button characterSwitchButton, Button dogamButton,
            CharacterInfoWidget characterInfoWidget, Image rarityBadge, TMP_Text rarityText, TMP_Text nameText,
            TMP_Text levelText, Transform starRatingContainer, CombatPowerWidget combatPowerWidget,
            CharacterStatWidget characterStatWidget, CostumeWidget costumeWidget)
        {
            var screen = root.GetComponent<CharacterDetailScreen>();
            var so = new SerializedObject(screen);

            // Header
            so.FindProperty("_backButton").objectReferenceValue = backButton;
            so.FindProperty("_titleText").objectReferenceValue = titleText;
            so.FindProperty("_homeButton").objectReferenceValue = homeButton;

            // Left Menu Area
            so.FindProperty("_menuButtonContainer").objectReferenceValue = menuContainer;
            so.FindProperty("_infoMenuButton").objectReferenceValue = infoMenu;
            so.FindProperty("_levelUpMenuButton").objectReferenceValue = levelUpMenu;
            so.FindProperty("_equipmentMenuButton").objectReferenceValue = equipmentMenu;
            so.FindProperty("_skillMenuButton").objectReferenceValue = skillMenu;
            so.FindProperty("_promotionMenuButton").objectReferenceValue = promotionMenu;
            so.FindProperty("_boardMenuButton").objectReferenceValue = boardMenu;
            so.FindProperty("_asideMenuButton").objectReferenceValue = asideMenu;

            // Center Area
            so.FindProperty("_characterImage").objectReferenceValue = characterImage;
            so.FindProperty("_companionImage").objectReferenceValue = companionImage;
            so.FindProperty("_characterSwitchButton").objectReferenceValue = characterSwitchButton;
            so.FindProperty("_dogamButton").objectReferenceValue = dogamButton;

            // Bottom Info Area
            so.FindProperty("_characterInfoWidget").objectReferenceValue = characterInfoWidget;
            so.FindProperty("_rarityBadge").objectReferenceValue = rarityBadge;
            so.FindProperty("_rarityText").objectReferenceValue = rarityText;
            so.FindProperty("_nameText").objectReferenceValue = nameText;

            // Right Top Area
            so.FindProperty("_levelText").objectReferenceValue = levelText;
            so.FindProperty("_starRatingContainer").objectReferenceValue = starRatingContainer;
            so.FindProperty("_combatPowerWidget").objectReferenceValue = combatPowerWidget;

            // Right Center Area
            so.FindProperty("_characterStatWidget").objectReferenceValue = characterStatWidget;

            // Right Bottom Area
            so.FindProperty("_costumeWidget").objectReferenceValue = costumeWidget;

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