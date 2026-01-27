using Sc.Contents.Stage;
using Sc.Contents.Stage.Widgets;
using Sc.Editor.AI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Editor.Wizard.Generators
{
    /// <summary>
    /// StageSelectScreen 프리팹 빌더.
    /// 스펙: Docs/Specs/Stage.md - StageSelectScreen UI 레이아웃 구조
    /// 레퍼런스: Docs/Design/Reference/StageSelectScreen.jpg
    /// </summary>
    public static class StageSelectScreenPrefabBuilder
    {
        #region Theme Colors

        private static readonly Color BgMap = new Color32(80, 120, 100, 255);
        private static readonly Color BgHeader = new Color32(20, 35, 50, 240);
        private static readonly Color BgCard = new Color32(30, 45, 60, 230);
        private static readonly Color BgInfoBubble = new Color32(250, 250, 250, 240);
        private static readonly Color BgStarProgress = new Color32(50, 60, 50, 220);
        private static readonly Color BgDifficultyTab = new Color32(60, 80, 60, 220);
        private static readonly Color TextPrimary = Color.white;
        private static readonly Color TextSecondary = new Color(1f, 1f, 1f, 0.7f);
        private static readonly Color TextDark = new Color32(40, 40, 40, 255);
        private static readonly Color AccentGreen = new Color32(100, 180, 100, 255);
        private static readonly Color AccentYellow = new Color32(255, 215, 100, 255);
        private static readonly Color AccentBlue = new Color32(100, 180, 255, 255);
        private static readonly Color NodeBgNormal = new Color32(100, 160, 100, 255);
        private static readonly Color NodeBgSelected = new Color32(120, 200, 120, 255);
        private static readonly Color ChapterNavBg = new Color32(255, 255, 255, 180);

        #endregion

        #region Constants

        private const float HEADER_HEIGHT = 70f;
        private const float FOOTER_HEIGHT = 100f;
        private const float RIGHT_TOP_WIDTH = 320f;
        private const float RIGHT_TOP_HEIGHT = 50f;
        private const float STAR_PROGRESS_WIDTH = 350f;
        private const float STAR_PROGRESS_HEIGHT = 70f;
        private const float INFO_BUBBLE_WIDTH = 280f;
        private const float INFO_BUBBLE_HEIGHT = 180f;
        private const float CHAPTER_NAV_SIZE = 60f;
        private const float NODE_SIZE = 100f;

        #endregion

        #region Font Helper

        private static void ApplyFont(TextMeshProUGUI tmp)
        {
            var font = EditorUIHelpers.GetProjectFont();
            if (font != null) tmp.font = font;
        }

        #endregion

        /// <summary>
        /// StageSelectScreen 프리팹용 GameObject 생성.
        /// </summary>
        public static GameObject Build()
        {
            var root = CreateRoot("StageSelectScreen");

            CreateBackground(root);
            CreateSafeArea(root);
            CreateOverlayLayer(root);

            // Add main component
            root.AddComponent<StageSelectScreen>();

            // Connect serialized fields
            ConnectSerializedFields(root);

            return root;
        }

        #region Root

        private static GameObject CreateRoot(string name)
        {
            var go = new GameObject(name);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            go.AddComponent<CanvasGroup>();
            return go;
        }

        #endregion

        #region Background

        private static GameObject CreateBackground(GameObject parent)
        {
            var go = CreateChild(parent, "Background");
            SetStretch(go);

            // Map Background Image (챕터별 배경)
            var mapBg = CreateChild(go, "MapBackground");
            SetStretch(mapBg);
            var mapImage = mapBg.AddComponent<Image>();
            mapImage.color = BgMap;
            mapImage.raycastTarget = true;

            return go;
        }

        #endregion

        #region SafeArea

        private static GameObject CreateSafeArea(GameObject parent)
        {
            var go = CreateChild(parent, "SafeArea");
            SetStretch(go);

            CreateHeader(go);
            CreateContent(go);
            CreateFooter(go);

            return go;
        }

        #endregion

        #region Header

        private static GameObject CreateHeader(GameObject parent)
        {
            var go = CreateChild(parent, "Header");
            SetAnchorTop(go, HEADER_HEIGHT);

            var bg = go.AddComponent<Image>();
            bg.color = BgHeader;

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            layout.padding = new RectOffset(20, 20, 0, 0);
            layout.spacing = 15;

            // Back Button
            CreateButton(go, "BackButton", "<", 50, 50, BgCard);

            // Title
            CreateLabel(go, "TitleText", "스테이지 리스트", 24, TextPrimary, TextAlignmentOptions.Left);

            // Entry Limit (입장 제한)
            CreateLabel(go, "EntryLimitText", "", 14, TextSecondary, TextAlignmentOptions.Left);

            // Spacer
            var spacer = CreateChild(go, "Spacer");
            var spacerLayout = spacer.AddComponent<LayoutElement>();
            spacerLayout.flexibleWidth = 1;

            // Currency HUD placeholder
            CreateCurrencyHUD(go);

            return go;
        }

        private static void CreateCurrencyHUD(GameObject parent)
        {
            var currencyArea = CreateChild(parent, "CurrencyHUD");
            var currencyLayout = currencyArea.AddComponent<HorizontalLayoutGroup>();
            currencyLayout.spacing = 20;
            currencyLayout.childAlignment = TextAnchor.MiddleRight;

            CreateCurrencyItem(currencyArea, "Stamina", "102/102");
            CreateCurrencyItem(currencyArea, "Gold", "549,061");
            CreateCurrencyItem(currencyArea, "Premium", "1,809");
        }

        private static void CreateCurrencyItem(GameObject parent, string name, string value)
        {
            var item = CreateChild(parent, name);
            var layout = item.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 5;
            layout.childAlignment = TextAnchor.MiddleCenter;

            // Icon placeholder
            var icon = CreateChild(item, "Icon");
            var iconLayout = icon.AddComponent<LayoutElement>();
            iconLayout.preferredWidth = 24;
            iconLayout.preferredHeight = 24;
            var iconImage = icon.AddComponent<Image>();
            iconImage.color = AccentYellow;

            // Value text
            CreateLabel(item, "Value", value, 14, TextPrimary, TextAlignmentOptions.Left);

            // Plus button
            var plusBtn = CreateChild(item, "PlusButton");
            var plusLayout = plusBtn.AddComponent<LayoutElement>();
            plusLayout.preferredWidth = 20;
            plusLayout.preferredHeight = 20;
            var plusImage = plusBtn.AddComponent<Image>();
            plusImage.color = AccentGreen;
            plusBtn.AddComponent<Button>();
        }

        #endregion

        #region Content

        private static GameObject CreateContent(GameObject parent)
        {
            var go = CreateChild(parent, "Content");
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(0, FOOTER_HEIGHT);
            rect.offsetMax = new Vector2(0, -HEADER_HEIGHT);

            // Content Module Area (위젯 확장 영역)
            CreateModuleContainer(go);

            // Right Top Area - StageProgressWidget
            CreateRightTopArea(go);

            // Stage Map Area (중앙)
            CreateStageMapArea(go);

            // Star Progress Bar (좌하단)
            CreateStarProgressBar(go);

            return go;
        }

        private static GameObject CreateModuleContainer(GameObject parent)
        {
            var go = CreateChild(parent, "ModuleContainer");
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, 60);
            rect.anchoredPosition = Vector2.zero;

            return go;
        }

        private static GameObject CreateRightTopArea(GameObject parent)
        {
            var go = CreateChild(parent, "RightTopArea");
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.sizeDelta = new Vector2(RIGHT_TOP_WIDTH, RIGHT_TOP_HEIGHT);
            rect.anchoredPosition = new Vector2(-20, -10);

            var bg = go.AddComponent<Image>();
            bg.color = BgCard;

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(15, 15, 10, 10);
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleLeft;

            // Stage Progress Text (11-10 최후의 방어선! 알프트반선!)
            CreateLabel(go, "StageProgressText", "11-10 최후의 방어선!", 14, TextPrimary, TextAlignmentOptions.Left);

            // Navigate Button
            CreateButton(go, "StageProgressNavigateButton", ">>", 30, 30, AccentGreen);

            return go;
        }

        private static GameObject CreateStageMapArea(GameObject parent)
        {
            var go = CreateChild(parent, "StageMapArea");
            SetStretch(go);

            // StageMapWidget 컴포넌트
            var mapWidget = go.AddComponent<StageMapWidget>();

            // Map Scroll View
            var scrollView = CreateChild(go, "MapScrollView");
            SetStretch(scrollView);

            var scrollRect = scrollView.AddComponent<ScrollRect>();
            scrollRect.horizontal = true;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.scrollSensitivity = 10f;

            // Viewport
            var viewport = CreateChild(scrollView, "Viewport");
            SetStretch(viewport);
            var viewportMask = viewport.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            var viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = new Color(1, 1, 1, 0.01f);

            // Map Content
            var mapContent = CreateChild(viewport, "MapContent");
            var mapContentRect = mapContent.GetComponent<RectTransform>();
            mapContentRect.anchorMin = new Vector2(0, 1);
            mapContentRect.anchorMax = new Vector2(0, 1);
            mapContentRect.pivot = new Vector2(0, 1);
            mapContentRect.sizeDelta = new Vector2(1200, 800);

            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.content = mapContent.GetComponent<RectTransform>();

            // Stage Node Container
            var nodeContainer = CreateChild(mapContent, "StageNodeContainer");
            SetStretch(nodeContainer);

            // Create sample stage nodes
            CreateSampleStageNodes(nodeContainer);

            // Stage Info Bubble
            CreateStageInfoBubble(mapContent);

            // Chapter Navigation
            CreateChapterNavigation(go);

            return go;
        }

        private static void CreateSampleStageNodes(GameObject parent)
        {
            // 샘플 스테이지 노드들 (레퍼런스 이미지 기반)
            var nodePositions = new[]
            {
                new Vector2(100, -150), // 10-3
                new Vector2(300, -100), // 10-4
                new Vector2(500, -150), // 10-5
                new Vector2(400, -300), // 10-6
                new Vector2(600, -350), // 10-7 (선택됨)
                new Vector2(800, -300), // 10-8
                new Vector2(700, -150), // 10-9
                new Vector2(900, -100), // 10-10
            };

            var nodeLabels = new[] { "10-3", "10-4", "10-5", "10-6", "10-7", "10-8", "10-9", "10-10" };
            var nodeStars = new[] { 2, 2, 2, 3, 3, 2, 3, 2 };

            for (int i = 0; i < nodePositions.Length; i++)
            {
                CreateStageNode(parent, $"StageNode_{nodeLabels[i]}", nodeLabels[i], nodeStars[i], nodePositions[i],
                    i == 4);
            }
        }

        private static void CreateStageNode(GameObject parent, string name, string label, int stars, Vector2 position,
            bool isSelected)
        {
            var node = CreateChild(parent, name);
            var rect = node.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(NODE_SIZE, NODE_SIZE);
            rect.anchoredPosition = position;

            // Background
            var bg = node.AddComponent<Image>();
            bg.color = isSelected ? NodeBgSelected : NodeBgNormal;

            var button = node.AddComponent<Button>();
            button.targetGraphic = bg;

            // Layout
            var layout = node.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(5, 5, 10, 5);
            layout.spacing = 5;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // Stage Number
            CreateLabel(node, "StageNumber", label, 18, TextPrimary, TextAlignmentOptions.Center);

            // Stars
            var starContainer = CreateChild(node, "Stars");
            var starLayout = starContainer.AddComponent<HorizontalLayoutGroup>();
            starLayout.spacing = 2;
            starLayout.childAlignment = TextAnchor.MiddleCenter;
            starLayout.childForceExpandWidth = false;
            var starLayoutElem = starContainer.AddComponent<LayoutElement>();
            starLayoutElem.preferredHeight = 20;

            string starText = "";
            for (int i = 0; i < 3; i++)
            {
                starText += i < stars ? "★" : "☆";
            }

            CreateLabel(starContainer, "StarText", starText, 14, AccentYellow, TextAlignmentOptions.Center);
        }

        private static void CreateStageInfoBubble(GameObject parent)
        {
            var bubble = CreateChild(parent, "StageInfoBubble");
            var rect = bubble.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(INFO_BUBBLE_WIDTH, INFO_BUBBLE_HEIGHT);
            rect.anchoredPosition = new Vector2(600, -200); // 선택된 노드 위

            // Background
            var bg = bubble.AddComponent<Image>();
            bg.color = BgInfoBubble;

            var button = bubble.AddComponent<Button>();
            button.targetGraphic = bg;

            var layout = bubble.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(15, 15, 15, 15);
            layout.spacing = 8;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // Recommended Power
            CreateLabel(bubble, "RecommendedPowerText", "권장 전투력: 117,660", 14, TextDark, TextAlignmentOptions.Center);

            // Stage Name
            CreateLabel(bubble, "BubbleStageNameText", "깜빡이는 터널!", 18, TextDark, TextAlignmentOptions.Center);

            // Enemy Preview Container
            var enemyPreview = CreateChild(bubble, "EnemyPreviewContainer");
            var enemyLayout = enemyPreview.AddComponent<HorizontalLayoutGroup>();
            enemyLayout.spacing = 5;
            enemyLayout.childAlignment = TextAnchor.MiddleCenter;
            var enemyLayoutElem = enemyPreview.AddComponent<LayoutElement>();
            enemyLayoutElem.preferredHeight = 50;

            // Sample enemy icon
            var enemyIcon = CreateChild(enemyPreview, "EnemyIcon");
            var enemyIconLayout = enemyIcon.AddComponent<LayoutElement>();
            enemyIconLayout.preferredWidth = 40;
            enemyIconLayout.preferredHeight = 40;
            var enemyImage = enemyIcon.AddComponent<Image>();
            enemyImage.color = new Color32(150, 80, 80, 255);

            // Party Preview Container
            var partyPreview = CreateChild(bubble, "PartyPreviewContainer");
            var partyLayout = partyPreview.AddComponent<HorizontalLayoutGroup>();
            partyLayout.spacing = 5;
            partyLayout.childAlignment = TextAnchor.MiddleCenter;
            var partyLayoutElem = partyPreview.AddComponent<LayoutElement>();
            partyLayoutElem.preferredHeight = 40;

            // Sample party icons
            for (int i = 0; i < 4; i++)
            {
                var partyIcon = CreateChild(partyPreview, $"PartyIcon_{i}");
                var iconLayout = partyIcon.AddComponent<LayoutElement>();
                iconLayout.preferredWidth = 35;
                iconLayout.preferredHeight = 35;
                var iconImage = partyIcon.AddComponent<Image>();
                iconImage.color = new Color32(100, 150, 180, 255);
            }
        }

        private static void CreateChapterNavigation(GameObject parent)
        {
            // ChapterSelectWidget 컴포넌트
            var navGo = CreateChild(parent, "ChapterNavigation");
            SetStretch(navGo);
            var chapterWidget = navGo.AddComponent<ChapterSelectWidget>();

            // Prev Chapter Button (좌측)
            var prevBtn = CreateChild(navGo, "PrevChapterButton");
            var prevRect = prevBtn.GetComponent<RectTransform>();
            prevRect.anchorMin = new Vector2(0, 0.5f);
            prevRect.anchorMax = new Vector2(0, 0.5f);
            prevRect.pivot = new Vector2(0, 0.5f);
            prevRect.sizeDelta = new Vector2(CHAPTER_NAV_SIZE, CHAPTER_NAV_SIZE * 2);
            prevRect.anchoredPosition = new Vector2(10, 0);

            var prevBg = prevBtn.AddComponent<Image>();
            prevBg.color = ChapterNavBg;
            var prevButton = prevBtn.AddComponent<Button>();
            prevButton.targetGraphic = prevBg;

            var prevLayout = prevBtn.AddComponent<VerticalLayoutGroup>();
            prevLayout.padding = new RectOffset(5, 5, 10, 10);
            prevLayout.spacing = 5;
            prevLayout.childAlignment = TextAnchor.MiddleCenter;

            CreateLabel(prevBtn, "PrevArrow", "<", 24, TextDark, TextAlignmentOptions.Center);
            CreateLabel(prevBtn, "PrevChapterText", "이전 월드", 12, TextDark, TextAlignmentOptions.Center);

            // Next Chapter Button (우측)
            var nextBtn = CreateChild(navGo, "NextChapterButton");
            var nextRect = nextBtn.GetComponent<RectTransform>();
            nextRect.anchorMin = new Vector2(1, 0.5f);
            nextRect.anchorMax = new Vector2(1, 0.5f);
            nextRect.pivot = new Vector2(1, 0.5f);
            nextRect.sizeDelta = new Vector2(CHAPTER_NAV_SIZE, CHAPTER_NAV_SIZE * 2);
            nextRect.anchoredPosition = new Vector2(-10, 0);

            var nextBg = nextBtn.AddComponent<Image>();
            nextBg.color = ChapterNavBg;
            var nextButton = nextBtn.AddComponent<Button>();
            nextButton.targetGraphic = nextBg;

            var nextLayout = nextBtn.AddComponent<VerticalLayoutGroup>();
            nextLayout.padding = new RectOffset(5, 5, 10, 10);
            nextLayout.spacing = 5;
            nextLayout.childAlignment = TextAnchor.MiddleCenter;

            CreateLabel(nextBtn, "NextArrow", ">", 24, TextDark, TextAlignmentOptions.Center);
            CreateLabel(nextBtn, "NextChapterText", "다음 월드", 12, TextDark, TextAlignmentOptions.Center);
        }

        private static void CreateStarProgressBar(GameObject parent)
        {
            var go = CreateChild(parent, "StarProgressBar");
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 0);
            rect.sizeDelta = new Vector2(STAR_PROGRESS_WIDTH, STAR_PROGRESS_HEIGHT);
            rect.anchoredPosition = new Vector2(20, 20);

            var bg = go.AddComponent<Image>();
            bg.color = BgStarProgress;

            go.AddComponent<StarProgressBarWidget>();

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(15, 15, 10, 10);
            layout.spacing = 15;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            // Star Icon
            var starIcon = CreateChild(go, "StarIcon");
            var starIconLayout = starIcon.AddComponent<LayoutElement>();
            starIconLayout.preferredWidth = 30;
            starIconLayout.preferredHeight = 30;
            var starIconImage = starIcon.AddComponent<Image>();
            starIconImage.color = AccentYellow;

            // Progress Text (14/30)
            CreateLabel(go, "ProgressText", "14/30", 16, TextPrimary, TextAlignmentOptions.Left);

            // Progress Slider Area
            var sliderArea = CreateChild(go, "SliderArea");
            var sliderAreaLayout = sliderArea.AddComponent<LayoutElement>();
            sliderAreaLayout.flexibleWidth = 1;
            sliderAreaLayout.preferredHeight = 30;

            // Progress Slider
            var slider = CreateChild(sliderArea, "ProgressSlider");
            SetStretch(slider);
            var sliderRect = slider.GetComponent<RectTransform>();
            sliderRect.offsetMin = new Vector2(0, 10);
            sliderRect.offsetMax = new Vector2(0, -10);

            var sliderComponent = slider.AddComponent<Slider>();
            sliderComponent.minValue = 0;
            sliderComponent.maxValue = 30;
            sliderComponent.value = 14;
            sliderComponent.interactable = false;

            // Slider Background
            var sliderBg = CreateChild(slider, "Background");
            SetStretch(sliderBg);
            var sliderBgImage = sliderBg.AddComponent<Image>();
            sliderBgImage.color = new Color32(60, 60, 60, 200);

            // Slider Fill
            var fillArea = CreateChild(slider, "Fill Area");
            var fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = Vector2.zero;

            var fill = CreateChild(fillArea, "Fill");
            SetStretch(fill);
            var fillImage = fill.AddComponent<Image>();
            fillImage.color = AccentGreen;

            sliderComponent.fillRect = fill.GetComponent<RectTransform>();

            // Milestone Container
            var milestoneContainer = CreateChild(sliderArea, "MilestoneContainer");
            SetStretch(milestoneContainer);

            // Sample milestones (10->25, 20->50, 30->100)
            CreateMilestoneMarker(milestoneContainer, 10, 30, "25");
            CreateMilestoneMarker(milestoneContainer, 20, 30, "50");
            CreateMilestoneMarker(milestoneContainer, 30, 30, "100");
        }

        private static void CreateMilestoneMarker(GameObject parent, int position, int max, string reward)
        {
            var marker = CreateChild(parent, $"Milestone_{position}");
            var rect = marker.GetComponent<RectTransform>();
            float normalizedPos = (float)position / max;
            rect.anchorMin = new Vector2(normalizedPos, 0.5f);
            rect.anchorMax = new Vector2(normalizedPos, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(40, 35);
            rect.anchoredPosition = new Vector2(0, 15);

            var layout = marker.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 2;
            layout.childAlignment = TextAnchor.MiddleCenter;

            // Milestone Icon
            var icon = CreateChild(marker, "Icon");
            var iconLayout = icon.AddComponent<LayoutElement>();
            iconLayout.preferredWidth = 20;
            iconLayout.preferredHeight = 20;
            var iconImage = icon.AddComponent<Image>();
            iconImage.color = AccentGreen;

            // Reward Amount
            CreateLabel(marker, "RewardText", reward, 10, TextPrimary, TextAlignmentOptions.Center);
        }

        #endregion

        #region Footer

        private static GameObject CreateFooter(GameObject parent)
        {
            var go = CreateChild(parent, "Footer");
            SetAnchorBottom(go, FOOTER_HEIGHT);

            var bg = go.AddComponent<Image>();
            bg.color = BgCard;

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 15, 15);
            layout.spacing = 15;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            // Difficulty Tabs (순한맛/매운맛/핵불맛)
            CreateDifficultyTabs(go);

            // Spacer
            var spacer = CreateChild(go, "Spacer");
            var spacerLayout = spacer.AddComponent<LayoutElement>();
            spacerLayout.flexibleWidth = 1;

            // World Map Button
            CreateButton(go, "WorldMapButton", "세계지도", 100, 50, AccentBlue);

            return go;
        }

        private static void CreateDifficultyTabs(GameObject parent)
        {
            var tabGroup = CreateChild(parent, "DifficultyTabs");
            var tabGroupLayout = tabGroup.AddComponent<HorizontalLayoutGroup>();
            tabGroupLayout.spacing = 10;
            tabGroupLayout.childAlignment = TextAnchor.MiddleLeft;
            tabGroupLayout.childForceExpandWidth = false;

            tabGroup.AddComponent<DifficultyTabWidget>();

            // Normal Tab (순한맛)
            CreateDifficultyTab(tabGroup, "NormalTab", "순한맛", true);

            // Hard Tab (매운맛)
            CreateDifficultyTab(tabGroup, "HardTab", "매운맛", false);

            // Hell Tab (핵불맛)
            CreateDifficultyTab(tabGroup, "HellTab", "핵불맛", false);
        }

        private static void CreateDifficultyTab(GameObject parent, string name, string label, bool isActive)
        {
            var tab = CreateChild(parent, name);
            var tabLayout = tab.AddComponent<LayoutElement>();
            tabLayout.preferredWidth = 80;
            tabLayout.preferredHeight = 50;

            var bg = tab.AddComponent<Image>();
            bg.color = isActive ? AccentGreen : BgDifficultyTab;

            var button = tab.AddComponent<Button>();
            button.targetGraphic = bg;

            var textGo = CreateChild(tab, "Label");
            SetStretch(textGo);
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 16;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            ApplyFont(tmp);

            // Selection Indicator
            var indicator = CreateChild(tab, "SelectionIndicator");
            var indicatorRect = indicator.GetComponent<RectTransform>();
            indicatorRect.anchorMin = new Vector2(0, 0);
            indicatorRect.anchorMax = new Vector2(1, 0);
            indicatorRect.pivot = new Vector2(0.5f, 0);
            indicatorRect.sizeDelta = new Vector2(0, 4);
            indicatorRect.anchoredPosition = Vector2.zero;
            var indicatorImage = indicator.AddComponent<Image>();
            indicatorImage.color = AccentYellow;
            indicator.SetActive(isActive);
        }

        #endregion

        #region Overlay Layer

        private static GameObject CreateOverlayLayer(GameObject parent)
        {
            var go = CreateChild(parent, "OverlayLayer");
            SetStretch(go);
            return go;
        }

        #endregion

        #region Helpers

        private static GameObject CreateChild(GameObject parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            go.AddComponent<RectTransform>();
            return go;
        }

        private static void SetStretch(GameObject go)
        {
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
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
            var screen = root.GetComponent<StageSelectScreen>();
            if (screen == null) return;

            var so = new SerializedObject(screen);

            // Header
            ConnectField(so, "_titleText", root, "SafeArea/Header/TitleText");
            ConnectField(so, "_entryLimitText", root, "SafeArea/Header/EntryLimitText");
            ConnectField(so, "_backButton", root, "SafeArea/Header/BackButton");

            // Content Module Area
            ConnectField(so, "_moduleContainer", root, "SafeArea/Content/ModuleContainer");

            // Map View
            ConnectField(so, "_chapterSelectWidget", root, "SafeArea/Content/StageMapArea/ChapterNavigation");
            ConnectField(so, "_stageMapWidget", root, "SafeArea/Content/StageMapArea");
            ConnectField(so, "_stageProgressText", root, "SafeArea/Content/RightTopArea/StageProgressText");

            // Footer Widgets
            ConnectField(so, "_starProgressBar", root, "SafeArea/Content/StarProgressBar");
            ConnectField(so, "_difficultyTabWidget", root, "SafeArea/Footer/DifficultyTabs");
            ConnectField(so, "_worldMapButton", root, "SafeArea/Footer/WorldMapButton");

            // Stage Detail (InfoBubble)
            ConnectField(so, "_detailPanel", root,
                "SafeArea/Content/StageMapArea/MapScrollView/Viewport/MapContent/StageInfoBubble");
            ConnectField(so, "_stageNameText", root,
                "SafeArea/Content/StageMapArea/MapScrollView/Viewport/MapContent/StageInfoBubble/BubbleStageNameText");
            ConnectField(so, "_recommendedPowerText", root,
                "SafeArea/Content/StageMapArea/MapScrollView/Viewport/MapContent/StageInfoBubble/RecommendedPowerText");

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConnectField(SerializedObject so, string fieldName, GameObject root, string path)
        {
            var prop = so.FindProperty(fieldName);
            if (prop == null)
            {
                Debug.LogWarning($"[StageSelectScreenPrefabBuilder] Field not found: {fieldName}");
                return;
            }

            var target = root.transform.Find(path);
            if (target == null)
            {
                Debug.LogWarning($"[StageSelectScreenPrefabBuilder] Path not found: {path}");
                return;
            }

            if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                var fieldType = prop.type;

                if (fieldType.Contains("Button"))
                    prop.objectReferenceValue = target.GetComponent<Button>();
                else if (fieldType.Contains("TMP_Text") || fieldType.Contains("TextMeshProUGUI"))
                    prop.objectReferenceValue = target.GetComponent<TextMeshProUGUI>();
                else if (fieldType.Contains("ChapterSelectWidget"))
                    prop.objectReferenceValue = target.GetComponent<ChapterSelectWidget>();
                else if (fieldType.Contains("StageMapWidget"))
                    prop.objectReferenceValue = target.GetComponent<StageMapWidget>();
                else if (fieldType.Contains("StarProgressBarWidget"))
                    prop.objectReferenceValue = target.GetComponent<StarProgressBarWidget>();
                else if (fieldType.Contains("DifficultyTabWidget"))
                    prop.objectReferenceValue = target.GetComponent<DifficultyTabWidget>();
                else if (fieldType.Contains("Transform"))
                    prop.objectReferenceValue = target;
                else
                    prop.objectReferenceValue = target.gameObject;
            }
        }

        #endregion
    }
}