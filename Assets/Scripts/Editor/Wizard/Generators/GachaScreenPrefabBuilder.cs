using Sc.Contents.Gacha;
using Sc.Editor.AI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Editor.Wizard.Generators
{
    /// <summary>
    /// GachaScreen 전용 프리팹 빌더.
    /// Luminous Dark Fantasy 테마 - 가챠 화면 UI 생성.
    /// </summary>
    public static class GachaScreenPrefabBuilder
    {
        #region Colors (Luminous Dark Fantasy Theme)

        // Deep background
        private static readonly Color BgDeep = new Color32(10, 10, 18, 255);
        private static readonly Color BgCard = new Color32(25, 25, 45, 217);
        private static readonly Color BgGlass = new Color32(255, 255, 255, 8);
        private static readonly Color BgGlassLight = new Color32(255, 255, 255, 15);

        // Accent colors
        private static readonly Color AccentPrimary = new Color32(0, 212, 255, 255);
        private static readonly Color AccentSecondary = new Color32(255, 107, 157, 255);
        private static readonly Color AccentGold = new Color32(255, 215, 0, 255);
        private static readonly Color AccentPurple = new Color32(168, 85, 247, 255);
        private static readonly Color AccentGreen = new Color32(76, 217, 100, 255);

        // Text colors
        private static readonly Color TextPrimary = Color.white;
        private static readonly Color TextSecondary = new Color(1f, 1f, 1f, 0.7f);
        private static readonly Color TextMuted = new Color(1f, 1f, 1f, 0.4f);
        private static readonly Color TextOnButton = new Color32(10, 10, 18, 255);

        // Button colors
        private static readonly Color ButtonPrimary = new Color32(0, 212, 255, 255);
        private static readonly Color ButtonSecondary = new Color32(80, 80, 120, 255);
        private static readonly Color ButtonFree = new Color32(76, 217, 100, 255);
        private static readonly Color ButtonGold = new Color32(255, 195, 0, 255);

        // Tab/Menu colors
        private static readonly Color TabActive = new Color32(0, 212, 255, 50);
        private static readonly Color TabInactive = new Color32(255, 255, 255, 8);

        #endregion

        #region Constants

        private const float HEADER_HEIGHT = 80f;
        private const float FOOTER_HEIGHT = 80f;
        private const float LEFT_AREA_WIDTH = 350f;
        private const float MENU_BUTTON_HEIGHT = 70f;
        private const float MENU_BUTTON_SPACING = 12f;
        private const float BANNER_CAROUSEL_HEIGHT = 120f;
        private const float BANNER_SLOT_WIDTH = 180f;
        private const float PULL_BUTTON_HEIGHT = 100f;
        private const float PULL_BUTTON_WIDTH = 200f;
        private const float INFO_BUTTON_SIZE = 80f;

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
        /// GachaScreen 프리팹용 GameObject 생성.
        /// </summary>
        public static GameObject Build()
        {
            var root = CreateRoot();

            // 1. Background
            CreateBackground(root);

            // 2. SafeArea
            var safeArea = CreateSafeArea(root);

            // 3. Header
            var header = CreateHeader(safeArea);

            // 4. Content (LeftArea + RightArea)
            var content = CreateContent(safeArea);
            var leftArea = CreateLeftArea(content);
            var rightArea = CreateRightArea(content);

            // 5. LeftArea Components
            var menuButtonGroup = CreateMenuButtonGroup(leftArea);
            var characterDisplay = CreateCharacterDisplay(leftArea);

            // 6. RightArea Components
            var bannerCarousel = CreateBannerCarousel(rightArea);
            var bannerInfoArea = CreateBannerInfoArea(rightArea);
            var pityInfoArea = CreatePityInfoArea(rightArea);
            var pullButtonGroup = CreatePullButtonGroup(rightArea);

            // 7. Footer
            var footer = CreateFooter(safeArea);
            var infoButtonGroup = CreateInfoButtonGroup(footer);

            // 8. OverlayLayer
            CreateOverlayLayer(root);

            // 9. Connect SerializedFields
            ConnectSerializedFields(root, header, menuButtonGroup, characterDisplay,
                bannerCarousel, bannerInfoArea, pityInfoArea, pullButtonGroup, infoButtonGroup);

            return root;
        }

        #region Root & Background

        private static GameObject CreateRoot()
        {
            var root = new GameObject("GachaScreen");

            var rect = root.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            root.AddComponent<CanvasGroup>();
            root.AddComponent<GachaScreen>();

            return root;
        }

        private static void CreateBackground(GameObject parent)
        {
            var bg = CreateChild(parent, "Background");
            SetStretch(bg);

            var bgImage = bg.AddComponent<Image>();
            bgImage.color = BgDeep;
            bgImage.raycastTarget = false;
        }

        #endregion

        #region SafeArea & Layout

        private static GameObject CreateSafeArea(GameObject parent)
        {
            var safeArea = CreateChild(parent, "SafeArea");
            SetStretch(safeArea);
            return safeArea;
        }

        private static GameObject CreateHeader(GameObject parent)
        {
            var header = CreateChild(parent, "Header");
            var rect = header.GetComponent<RectTransform>();

            // Top anchored, stretch horizontal
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0, HEADER_HEIGHT);

            // Header Background
            var headerBg = CreateChild(header, "HeaderBg");
            SetStretch(headerBg);
            var headerBgImage = headerBg.AddComponent<Image>();
            headerBgImage.color = BgGlass;
            headerBgImage.raycastTarget = false;

            // Back Button
            var backButton = CreateBackButton(header);

            // Title
            var title = CreateChild(header, "Title");
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.5f);
            titleRect.anchorMax = new Vector2(0, 0.5f);
            titleRect.pivot = new Vector2(0, 0.5f);
            titleRect.anchoredPosition = new Vector2(70, 0);
            titleRect.sizeDelta = new Vector2(150, 40);

            var titleText = title.AddComponent<TextMeshProUGUI>();
            titleText.text = "사도 모집";
            titleText.fontSize = 24;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = TextPrimary;
            titleText.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(titleText);

            // Currency HUD (5개)
            CreateCurrencyHUD(header);

            return header;
        }

        private static Button CreateBackButton(GameObject parent)
        {
            var backBtn = CreateChild(parent, "BackButton");
            var rect = backBtn.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = new Vector2(16, 0);
            rect.sizeDelta = new Vector2(44, 44);

            var image = backBtn.AddComponent<Image>();
            image.color = BgGlassLight;
            image.raycastTarget = true;

            var button = backBtn.AddComponent<Button>();
            button.targetGraphic = image;

            // Back Icon
            var icon = CreateChild(backBtn, "Icon");
            SetStretch(icon);

            var iconText = icon.AddComponent<TextMeshProUGUI>();
            iconText.text = "<";
            iconText.fontSize = 24;
            iconText.color = TextPrimary;
            iconText.alignment = TextAlignmentOptions.Center;
            iconText.fontStyle = FontStyles.Bold;
            ApplyFont(iconText);

            return button;
        }

        private static void CreateCurrencyHUD(GameObject parent)
        {
            var currencyHUD = CreateChild(parent, "CurrencyHUD");
            var rect = currencyHUD.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(1, 0.5f);
            rect.anchorMax = new Vector2(1, 0.5f);
            rect.pivot = new Vector2(1, 0.5f);
            rect.anchoredPosition = new Vector2(-16, 0);
            rect.sizeDelta = new Vector2(600, 50);

            var layoutGroup = currencyHUD.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 16;
            layoutGroup.childAlignment = TextAnchor.MiddleRight;

            // Currency Items (유료, 골드, 무료, 프리미엄, 신앙심)
            string[] currencies = { "유료", "골드", "무료", "프리미엄", "신앙심" };
            string[] values = { "52", "549,061", "0", "1,809", "4" };
            Color[] colors = { AccentPurple, AccentGold, AccentGreen, AccentSecondary, AccentPrimary };

            for (int i = 0; i < currencies.Length; i++)
            {
                CreateCurrencyItem(currencyHUD, currencies[i], values[i], colors[i]);
            }
        }

        private static void CreateCurrencyItem(GameObject parent, string label, string value, Color iconColor)
        {
            var item = CreateChild(parent, $"Currency_{label}");
            var rect = item.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 40);

            var layoutGroup = item.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 4;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;

            // Icon
            var icon = CreateChild(item, "Icon");
            var iconRect = icon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(20, 20);

            var iconImage = icon.AddComponent<Image>();
            iconImage.color = iconColor;
            iconImage.raycastTarget = false;

            // Value
            var valueObj = CreateChild(item, "Value");
            var valueRect = valueObj.GetComponent<RectTransform>();
            valueRect.sizeDelta = new Vector2(70, 30);

            var valueText = valueObj.AddComponent<TextMeshProUGUI>();
            valueText.text = value;
            valueText.fontSize = 14;
            valueText.color = TextPrimary;
            valueText.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(valueText);
        }

        private static GameObject CreateContent(GameObject parent)
        {
            var content = CreateChild(parent, "Content");
            var rect = SetStretch(content);

            rect.offsetMin = new Vector2(0, FOOTER_HEIGHT);
            rect.offsetMax = new Vector2(0, -HEADER_HEIGHT);

            return content;
        }

        private static GameObject CreateLeftArea(GameObject parent)
        {
            var leftArea = CreateChild(parent, "LeftArea");
            var rect = leftArea.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(LEFT_AREA_WIDTH, 0);

            return leftArea;
        }

        private static GameObject CreateRightArea(GameObject parent)
        {
            var rightArea = CreateChild(parent, "RightArea");
            var rect = SetStretch(rightArea);

            rect.offsetMin = new Vector2(LEFT_AREA_WIDTH, 0);
            rect.offsetMax = Vector2.zero;

            return rightArea;
        }

        #endregion

        #region LeftArea Components

        private static GameObject CreateMenuButtonGroup(GameObject parent)
        {
            var menuGroup = CreateChild(parent, "MenuButtonGroup");
            var rect = menuGroup.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -20);
            rect.sizeDelta = new Vector2(-32, 240);

            var layoutGroup = menuGroup.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = MENU_BUTTON_SPACING;
            layoutGroup.padding = new RectOffset(16, 16, 0, 0);
            layoutGroup.childAlignment = TextAnchor.UpperCenter;

            // Menu Buttons
            CreateMenuButton(menuGroup, "GachaButton", "사도 모집", true);
            CreateMenuButton(menuGroup, "SpecialButton", "특별 모집", false);
            CreateMenuButton(menuGroup, "CardButton", "카드 뽑기", false);

            return menuGroup;
        }

        private static Button CreateMenuButton(GameObject parent, string name, string label, bool isActive)
        {
            var btn = CreateChild(parent, name);
            var rect = btn.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, MENU_BUTTON_HEIGHT);

            var layoutElement = btn.AddComponent<LayoutElement>();
            layoutElement.minHeight = MENU_BUTTON_HEIGHT;
            layoutElement.preferredHeight = MENU_BUTTON_HEIGHT;

            var image = btn.AddComponent<Image>();
            image.color = isActive ? TabActive : TabInactive;
            image.raycastTarget = true;

            var button = btn.AddComponent<Button>();
            button.targetGraphic = image;

            // Icon (placeholder)
            var icon = CreateChild(btn, "Icon");
            var iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0.5f);
            iconRect.anchorMax = new Vector2(0, 0.5f);
            iconRect.pivot = new Vector2(0, 0.5f);
            iconRect.anchoredPosition = new Vector2(16, 0);
            iconRect.sizeDelta = new Vector2(40, 40);

            var iconImage = icon.AddComponent<Image>();
            iconImage.color = isActive ? AccentPrimary : TextMuted;
            iconImage.raycastTarget = false;

            // Label
            var labelObj = CreateChild(btn, "Label");
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.offsetMin = new Vector2(70, 0);
            labelRect.offsetMax = new Vector2(-16, 0);

            var labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 18;
            labelText.fontStyle = FontStyles.Bold;
            labelText.color = isActive ? TextPrimary : TextSecondary;
            labelText.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(labelText);

            return button;
        }

        private static GameObject CreateCharacterDisplay(GameObject parent)
        {
            var display = CreateChild(parent, "CharacterDisplay");
            var rect = display.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(0, 0);
            rect.offsetMax = new Vector2(0, -280);

            // Character Image (placeholder)
            var charImage = CreateChild(display, "CharacterImage");
            SetStretch(charImage);

            var image = charImage.AddComponent<Image>();
            image.color = new Color(1, 1, 1, 0.1f);
            image.raycastTarget = false;

            return display;
        }

        #endregion

        #region RightArea Components

        private static GameObject CreateBannerCarousel(GameObject parent)
        {
            var carousel = CreateChild(parent, "BannerCarousel");
            var rect = carousel.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -20);
            rect.sizeDelta = new Vector2(-32, BANNER_CAROUSEL_HEIGHT);

            // ScrollRect
            var scrollRect = carousel.AddComponent<ScrollRect>();
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.elasticity = 0.1f;
            scrollRect.inertia = true;
            scrollRect.decelerationRate = 0.135f;

            // Viewport
            var viewport = CreateChild(carousel, "Viewport");
            SetStretch(viewport);

            var viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = Color.clear;
            viewportImage.raycastTarget = true;

            var mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            scrollRect.viewport = viewport.GetComponent<RectTransform>();

            // Content (BannerContainer)
            var content = CreateChild(viewport, "BannerContainer");
            var contentRect = content.GetComponent<RectTransform>();

            contentRect.anchorMin = new Vector2(0, 0);
            contentRect.anchorMax = new Vector2(0, 1);
            contentRect.pivot = new Vector2(0, 0.5f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(BANNER_SLOT_WIDTH * 4 + 48, 0);

            var layoutGroup = content.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.spacing = 12;
            layoutGroup.padding = new RectOffset(0, 0, 8, 8);

            scrollRect.content = contentRect;

            // Banner Slots (4개)
            for (int i = 0; i < 4; i++)
            {
                bool isPickup = i < 3;
                CreateBannerSlot(content, $"BannerSlot_{i}", isPickup ? "픽업 사도 모집" : "사도 모집", i == 0);
            }

            // Indicators
            CreateBannerIndicators(carousel, 4);

            return carousel;
        }

        private static void CreateBannerSlot(GameObject parent, string name, string label, bool isSelected)
        {
            var slot = CreateChild(parent, name);
            var rect = slot.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(BANNER_SLOT_WIDTH, 0);

            var image = slot.AddComponent<Image>();
            image.color = isSelected ? new Color32(0, 212, 255, 40) : BgCard;
            image.raycastTarget = true;

            var button = slot.AddComponent<Button>();
            button.targetGraphic = image;

            // Thumbnail (placeholder)
            var thumb = CreateChild(slot, "Thumbnail");
            var thumbRect = thumb.GetComponent<RectTransform>();
            thumbRect.anchorMin = new Vector2(0, 0.5f);
            thumbRect.anchorMax = new Vector2(0, 0.5f);
            thumbRect.pivot = new Vector2(0, 0.5f);
            thumbRect.anchoredPosition = new Vector2(8, 0);
            thumbRect.sizeDelta = new Vector2(60, 60);

            var thumbImage = thumb.AddComponent<Image>();
            thumbImage.color = new Color(1, 1, 1, 0.3f);
            thumbImage.raycastTarget = false;

            // Label
            var labelObj = CreateChild(slot, "Label");
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.offsetMin = new Vector2(75, 0);
            labelRect.offsetMax = new Vector2(-8, 0);

            var labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 14;
            labelText.color = TextPrimary;
            labelText.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(labelText);
        }

        private static void CreateBannerIndicators(GameObject parent, int count)
        {
            var indicators = CreateChild(parent, "Indicators");
            var rect = indicators.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = new Vector2(0, 4);
            rect.sizeDelta = new Vector2(count * 16, 8);

            var layoutGroup = indicators.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.spacing = 8;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;

            for (int i = 0; i < count; i++)
            {
                var dot = CreateChild(indicators, $"Dot_{i}");
                var dotRect = dot.GetComponent<RectTransform>();
                dotRect.sizeDelta = new Vector2(8, 8);

                var dotImage = dot.AddComponent<Image>();
                dotImage.color = i == 0 ? AccentPrimary : TextMuted;
                dotImage.raycastTarget = false;
            }
        }

        private static GameObject CreateBannerInfoArea(GameObject parent)
        {
            var infoArea = CreateChild(parent, "BannerInfoArea");
            var rect = infoArea.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -160);
            rect.sizeDelta = new Vector2(-32, 180);

            var layoutGroup = infoArea.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 8;
            layoutGroup.padding = new RectOffset(16, 16, 8, 8);
            layoutGroup.childAlignment = TextAnchor.UpperLeft;

            // Banner Title
            var title = CreateChild(infoArea, "BannerTitle");
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(0, 36);

            var titleElement = title.AddComponent<LayoutElement>();
            titleElement.minHeight = 36;
            titleElement.preferredHeight = 36;

            var titleText = title.AddComponent<TextMeshProUGUI>();
            titleText.text = "용족 성골 귀족 비비";
            titleText.fontSize = 28;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = TextPrimary;
            titleText.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(titleText);

            // Banner Period
            var period = CreateChild(infoArea, "BannerPeriod");
            var periodRect = period.GetComponent<RectTransform>();
            periodRect.sizeDelta = new Vector2(0, 24);

            var periodElement = period.AddComponent<LayoutElement>();
            periodElement.minHeight = 24;
            periodElement.preferredHeight = 24;

            var periodText = period.AddComponent<TextMeshProUGUI>();
            periodText.text = "2026-01-22 11:00 ~ 2026-01-29 10:59";
            periodText.fontSize = 14;
            periodText.color = TextMuted;
            periodText.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(periodText);

            // Banner Description
            var desc = CreateChild(infoArea, "BannerDescription");
            var descRect = desc.GetComponent<RectTransform>();
            descRect.sizeDelta = new Vector2(0, 100);

            var descElement = desc.AddComponent<LayoutElement>();
            descElement.minHeight = 100;
            descElement.preferredHeight = 100;

            var descText = desc.AddComponent<TextMeshProUGUI>();
            descText.text = "기간 중 픽업된 사도 등장 시 비비 3판 출현!\n10회 모집 시 ★2 이상 사도 1인 안정\n\n※모집 시 신앙심을 함께 획득할 수 있습니다.\n※일특한 신앙심으로 캔서드로 교환 수도 있습니다.\n※이미 얻고있는 사도는 해당 사도 돌파석으로 변환됩니다.";
            descText.fontSize = 12;
            descText.color = TextSecondary;
            descText.alignment = TextAlignmentOptions.TopLeft;
            descText.enableWordWrapping = true;
            ApplyFont(descText);

            return infoArea;
        }

        private static GameObject CreatePityInfoArea(GameObject parent)
        {
            var pityArea = CreateChild(parent, "PityInfoArea");
            var rect = pityArea.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -350);
            rect.sizeDelta = new Vector2(-32, 60);

            var layoutGroup = pityArea.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.spacing = 16;
            layoutGroup.padding = new RectOffset(16, 16, 8, 8);
            layoutGroup.childAlignment = TextAnchor.MiddleRight;

            // Pity Icon
            var pityIcon = CreateChild(pityArea, "PityIcon");
            var pityIconRect = pityIcon.GetComponent<RectTransform>();
            pityIconRect.sizeDelta = new Vector2(24, 24);

            var pityIconImage = pityIcon.AddComponent<Image>();
            pityIconImage.color = AccentSecondary;
            pityIconImage.raycastTarget = false;

            // Pity Label
            var pityLabel = CreateChild(pityArea, "PityLabel");
            var pityLabelRect = pityLabel.GetComponent<RectTransform>();
            pityLabelRect.sizeDelta = new Vector2(80, 30);

            var pityLabelText = pityLabel.AddComponent<TextMeshProUGUI>();
            pityLabelText.text = "신앙심 110";
            pityLabelText.fontSize = 16;
            pityLabelText.fontStyle = FontStyles.Bold;
            pityLabelText.color = TextPrimary;
            pityLabelText.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(pityLabelText);

            // Exchange Button
            var exchangeBtn = CreateChild(pityArea, "ExchangeButton");
            var exchangeBtnRect = exchangeBtn.GetComponent<RectTransform>();
            exchangeBtnRect.sizeDelta = new Vector2(70, 36);

            var exchangeBtnImage = exchangeBtn.AddComponent<Image>();
            exchangeBtnImage.color = ButtonSecondary;
            exchangeBtnImage.raycastTarget = true;

            var exchangeButton = exchangeBtn.AddComponent<Button>();
            exchangeButton.targetGraphic = exchangeBtnImage;

            var exchangeText = CreateChild(exchangeBtn, "Text");
            SetStretch(exchangeText);

            var exchangeTextTmp = exchangeText.AddComponent<TextMeshProUGUI>();
            exchangeTextTmp.text = "교환";
            exchangeTextTmp.fontSize = 14;
            exchangeTextTmp.fontStyle = FontStyles.Bold;
            exchangeTextTmp.color = TextPrimary;
            exchangeTextTmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(exchangeTextTmp);

            // Pity Description
            var pityDesc = CreateChild(pityArea, "PityDescription");
            var pityDescRect = pityDesc.GetComponent<RectTransform>();
            pityDescRect.sizeDelta = new Vector2(300, 30);

            var pityDescText = pityDesc.AddComponent<TextMeshProUGUI>();
            pityDescText.text = "사용하지 않은 신앙심은 다음 픽업 모집에 이월됩니다.";
            pityDescText.fontSize = 11;
            pityDescText.color = TextMuted;
            pityDescText.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(pityDescText);

            return pityArea;
        }

        private static GameObject CreatePullButtonGroup(GameObject parent)
        {
            var pullGroup = CreateChild(parent, "PullButtonGroup");
            var rect = pullGroup.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = new Vector2(0, 20);
            rect.sizeDelta = new Vector2(-32, PULL_BUTTON_HEIGHT);

            var layoutGroup = pullGroup.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.spacing = 16;
            layoutGroup.padding = new RectOffset(16, 16, 0, 0);
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;

            // Free Pull Button
            CreatePullButton(pullGroup, "FreePullButton", "1일 1회 모집", "30", ButtonFree, true, "1회 남음");

            // Single Pull Button
            CreatePullButton(pullGroup, "SinglePullButton", "1회 모집", "1", ButtonPrimary, false, null);

            // Multi Pull Button
            CreatePullButton(pullGroup, "MultiPullButton", "10회 모집", "10", ButtonGold, false, null, "★2 확정");

            return pullGroup;
        }

        private static Button CreatePullButton(GameObject parent, string name, string label, string cost,
            Color buttonColor, bool isFree, string remainingText, string badgeText = null)
        {
            var btn = CreateChild(parent, name);
            var rect = btn.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(PULL_BUTTON_WIDTH, 0);

            var image = btn.AddComponent<Image>();
            image.color = buttonColor;
            image.raycastTarget = true;

            var button = btn.AddComponent<Button>();
            button.targetGraphic = image;

            var layoutGroup = btn.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 4;
            layoutGroup.padding = new RectOffset(8, 8, 8, 8);
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;

            // Remaining Label (for free pull)
            if (!string.IsNullOrEmpty(remainingText))
            {
                var remaining = CreateChild(btn, "RemainingLabel");
                var remainingRect = remaining.GetComponent<RectTransform>();
                remainingRect.sizeDelta = new Vector2(0, 18);

                var remainingElement = remaining.AddComponent<LayoutElement>();
                remainingElement.minHeight = 18;
                remainingElement.preferredHeight = 18;

                var remainingTextTmp = remaining.AddComponent<TextMeshProUGUI>();
                remainingTextTmp.text = remainingText;
                remainingTextTmp.fontSize = 12;
                remainingTextTmp.color = TextOnButton;
                remainingTextTmp.alignment = TextAlignmentOptions.Center;
                ApplyFont(remainingTextTmp);
            }

            // Button Label
            var labelObj = CreateChild(btn, "ButtonLabel");
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(0, 24);

            var labelElement = labelObj.AddComponent<LayoutElement>();
            labelElement.minHeight = 24;
            labelElement.preferredHeight = 24;

            var labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 16;
            labelText.fontStyle = FontStyles.Bold;
            labelText.color = TextOnButton;
            labelText.alignment = TextAlignmentOptions.Center;
            ApplyFont(labelText);

            // Cost Group
            var costGroup = CreateChild(btn, "CostGroup");
            var costGroupRect = costGroup.GetComponent<RectTransform>();
            costGroupRect.sizeDelta = new Vector2(0, 24);

            var costGroupElement = costGroup.AddComponent<LayoutElement>();
            costGroupElement.minHeight = 24;
            costGroupElement.preferredHeight = 24;

            var costLayout = costGroup.AddComponent<HorizontalLayoutGroup>();
            costLayout.childControlWidth = false;
            costLayout.childControlHeight = false;
            costLayout.spacing = 4;
            costLayout.childAlignment = TextAnchor.MiddleCenter;

            // Cost Icon
            var costIcon = CreateChild(costGroup, "CostIcon");
            var costIconRect = costIcon.GetComponent<RectTransform>();
            costIconRect.sizeDelta = new Vector2(18, 18);

            var costIconImage = costIcon.AddComponent<Image>();
            costIconImage.color = isFree ? AccentGreen : AccentSecondary;
            costIconImage.raycastTarget = false;

            // Cost Value
            var costValue = CreateChild(costGroup, "CostValue");
            var costValueRect = costValue.GetComponent<RectTransform>();
            costValueRect.sizeDelta = new Vector2(40, 20);

            var costValueText = costValue.AddComponent<TextMeshProUGUI>();
            costValueText.text = cost;
            costValueText.fontSize = 16;
            costValueText.fontStyle = FontStyles.Bold;
            costValueText.color = TextOnButton;
            costValueText.alignment = TextAlignmentOptions.MidlineLeft;
            ApplyFont(costValueText);

            // Guarantee Badge (for multi pull)
            if (!string.IsNullOrEmpty(badgeText))
            {
                var badge = CreateChild(btn, "GuaranteeBadge");
                var badgeRect = badge.GetComponent<RectTransform>();

                badgeRect.anchorMin = new Vector2(1, 1);
                badgeRect.anchorMax = new Vector2(1, 1);
                badgeRect.pivot = new Vector2(1, 1);
                badgeRect.anchoredPosition = new Vector2(8, 8);
                badgeRect.sizeDelta = new Vector2(70, 24);

                var badgeImage = badge.AddComponent<Image>();
                badgeImage.color = AccentSecondary;
                badgeImage.raycastTarget = false;

                var badgeTextObj = CreateChild(badge, "Text");
                SetStretch(badgeTextObj);

                var badgeTextTmp = badgeTextObj.AddComponent<TextMeshProUGUI>();
                badgeTextTmp.text = badgeText;
                badgeTextTmp.fontSize = 11;
                badgeTextTmp.fontStyle = FontStyles.Bold;
                badgeTextTmp.color = TextPrimary;
                badgeTextTmp.alignment = TextAlignmentOptions.Center;
                ApplyFont(badgeTextTmp);
            }

            return button;
        }

        #endregion

        #region Footer

        private static GameObject CreateFooter(GameObject parent)
        {
            var footer = CreateChild(parent, "Footer");
            var rect = footer.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0, FOOTER_HEIGHT);

            // Footer Background
            var footerBg = CreateChild(footer, "FooterBg");
            SetStretch(footerBg);
            var footerBgImage = footerBg.AddComponent<Image>();
            footerBgImage.color = BgGlass;
            footerBgImage.raycastTarget = false;

            return footer;
        }

        private static GameObject CreateInfoButtonGroup(GameObject parent)
        {
            var infoGroup = CreateChild(parent, "InfoButtonGroup");
            var rect = infoGroup.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = new Vector2(16, 0);
            rect.sizeDelta = new Vector2(300, 60);

            var layoutGroup = infoGroup.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.spacing = 16;
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;

            // Info Buttons
            CreateInfoButton(infoGroup, "CharacterInfoButton", "사도 정보");
            CreateInfoButton(infoGroup, "RateInfoButton", "확률 정보");
            CreateInfoButton(infoGroup, "HistoryButton", "기록");

            return infoGroup;
        }

        private static Button CreateInfoButton(GameObject parent, string name, string label)
        {
            var btn = CreateChild(parent, name);
            var rect = btn.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(INFO_BUTTON_SIZE, 0);

            var image = btn.AddComponent<Image>();
            image.color = BgGlassLight;
            image.raycastTarget = true;

            var button = btn.AddComponent<Button>();
            button.targetGraphic = image;

            var layoutGroup = btn.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 4;
            layoutGroup.padding = new RectOffset(4, 4, 8, 4);
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;

            // Icon
            var icon = CreateChild(btn, "Icon");
            var iconRect = icon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(24, 24);

            var iconElement = icon.AddComponent<LayoutElement>();
            iconElement.minWidth = 24;
            iconElement.minHeight = 24;
            iconElement.preferredWidth = 24;
            iconElement.preferredHeight = 24;

            var iconImage = icon.AddComponent<Image>();
            iconImage.color = TextMuted;
            iconImage.raycastTarget = false;

            // Label
            var labelObj = CreateChild(btn, "Label");
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(0, 16);

            var labelElement = labelObj.AddComponent<LayoutElement>();
            labelElement.minHeight = 16;
            labelElement.preferredHeight = 16;

            var labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = label;
            labelText.fontSize = 11;
            labelText.color = TextSecondary;
            labelText.alignment = TextAlignmentOptions.Center;
            ApplyFont(labelText);

            return button;
        }

        #endregion

        #region OverlayLayer

        private static void CreateOverlayLayer(GameObject parent)
        {
            var overlay = CreateChild(parent, "OverlayLayer");
            SetStretch(overlay);

            // CanvasGroup for fade effects
            var canvasGroup = overlay.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = false;
        }

        #endregion

        #region SerializeField Connections

        private static void ConnectSerializedFields(
            GameObject root,
            GameObject header,
            GameObject menuButtonGroup,
            GameObject characterDisplay,
            GameObject bannerCarousel,
            GameObject bannerInfoArea,
            GameObject pityInfoArea,
            GameObject pullButtonGroup,
            GameObject infoButtonGroup)
        {
            var gachaScreen = root.GetComponent<GachaScreen>();
            var so = new SerializedObject(gachaScreen);

            // Header
            so.FindProperty("_backButton").objectReferenceValue =
                header.transform.Find("BackButton")?.GetComponent<Button>();

            // Menu Buttons
            so.FindProperty("_gachaMenuButton").objectReferenceValue =
                menuButtonGroup.transform.Find("GachaButton")?.GetComponent<Button>();
            so.FindProperty("_specialMenuButton").objectReferenceValue =
                menuButtonGroup.transform.Find("SpecialButton")?.GetComponent<Button>();
            so.FindProperty("_cardMenuButton").objectReferenceValue =
                menuButtonGroup.transform.Find("CardButton")?.GetComponent<Button>();

            // Character Display
            so.FindProperty("_characterDisplay").objectReferenceValue =
                characterDisplay.transform;

            // Banner
            so.FindProperty("_bannerScrollRect").objectReferenceValue =
                bannerCarousel.GetComponent<ScrollRect>();
            so.FindProperty("_bannerContainer").objectReferenceValue =
                bannerCarousel.transform.Find("Viewport/BannerContainer");

            // Banner Info
            so.FindProperty("_bannerTitleText").objectReferenceValue =
                bannerInfoArea.transform.Find("BannerTitle")?.GetComponent<TMP_Text>();
            so.FindProperty("_bannerPeriodText").objectReferenceValue =
                bannerInfoArea.transform.Find("BannerPeriod")?.GetComponent<TMP_Text>();
            so.FindProperty("_bannerDescriptionText").objectReferenceValue =
                bannerInfoArea.transform.Find("BannerDescription")?.GetComponent<TMP_Text>();

            // Pity Info
            so.FindProperty("_pityLabel").objectReferenceValue =
                pityInfoArea.transform.Find("PityLabel")?.GetComponent<TMP_Text>();
            so.FindProperty("_exchangeButton").objectReferenceValue =
                pityInfoArea.transform.Find("ExchangeButton")?.GetComponent<Button>();

            // Pull Buttons
            so.FindProperty("_singlePullButton").objectReferenceValue =
                pullButtonGroup.transform.Find("SinglePullButton")?.GetComponent<Button>();
            so.FindProperty("_multiPullButton").objectReferenceValue =
                pullButtonGroup.transform.Find("MultiPullButton")?.GetComponent<Button>();

            // Info Buttons
            so.FindProperty("_characterInfoButton").objectReferenceValue =
                infoButtonGroup.transform.Find("CharacterInfoButton")?.GetComponent<Button>();
            so.FindProperty("_rateDetailButton").objectReferenceValue =
                infoButtonGroup.transform.Find("RateInfoButton")?.GetComponent<Button>();
            so.FindProperty("_historyButton").objectReferenceValue =
                infoButtonGroup.transform.Find("HistoryButton")?.GetComponent<Button>();

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion

        #region Helpers

        private static GameObject CreateChild(GameObject parent, string name)
        {
            var child = new GameObject(name);
            child.AddComponent<RectTransform>();
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
