using System.Collections.Generic;
using Sc.Editor.AI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Sc.Contents.Lobby;
using Sc.Contents.Lobby.Widgets;

namespace Sc.Editor.Wizard.Generators
{
    /// <summary>
    /// LobbyScreen ?�리??빌더 (?�동 ?�성??.
    /// Generated from: Assets/Prefabs/UI/Screens/LobbyScreen.prefab
    /// Generated at: 2026-01-26 23:09:59
    /// </summary>
    public static class LobbyScreenPrefabBuilder_Generated
    {
        #region Theme Colors

        private static readonly Color BgDeep = new Color32(10, 10, 18, 255);
        private static readonly Color BgCard = new Color32(25, 25, 45, 217);
        private static readonly Color BgOverlay = new Color32(0, 0, 0, 200);
        private static readonly Color TextPrimary = Color.white;
        private static readonly Color TextSecondary = new Color(1f, 1f, 1f, 0.7f);
        private static readonly Color TextMuted = new Color(1f, 1f, 1f, 0.5f);
        private static readonly Color AccentPrimary = new Color32(100, 200, 255, 255);
        private static readonly Color AccentGold = new Color32(255, 215, 100, 255);
        private static readonly Color Transparent = Color.clear;

        // Extracted from prefab
        private static readonly Color AccentPurple = new Color32(168, 85, 247, 255);
        private static readonly Color AccentSecondary = new Color32(255, 107, 157, 255);
        private static readonly Color BgGlass = new Color32(255, 255, 255, 26);
        private static readonly Color Blue = new Color32(100, 200, 255, 255);
        private static readonly Color Color = new Color32(25, 25, 45, 217);

        #endregion

        #region Constants

        private const float ADVENTURE_BUTTON_HEIGHT = 100f;
        private const float BADGE_HEIGHT = 20f;
        private const float BADGE_WIDTH = 20f;
        private const float BANNER_CONTAINER_HEIGHT = 20f;
        private const float BOTTOM_NAV_HEIGHT = 193.9596f;
        private const float CHARACTER_IMAGE_HEIGHT = 615.8789f;
        private const float CHARACTER_IMAGE_WIDTH = 350f;
        private const float CONTENT_HEIGHT = 180f;
        private const float CONTENT_NAV_BUTTON__CHARACTER_LIST_SCREEN_WIDTH = 100f;
        private const float CONTENT_NAV_BUTTON__GACHA_SCREEN_WIDTH = 100f;
        private const float CONTENT_NAV_BUTTON__SHOP_SCREEN_WIDTH = 100f;
        private const float DIALOGUE_BOX_HEIGHT = 60f;
        private const float DIALOGUE_BOX_WIDTH = 300f;
        private const float EVENT_BANNER_CAROUSEL_HEIGHT = 150f;
        private const float GLOW_EFFECT_HEIGHT = 4f;
        private const float GLOW_EFFECT_WIDTH = 60f;
        private const float HEADER_HEIGHT = 80f;
        private const float ICON_HEIGHT = 100f;
        private const float INDICATOR_PREFAB_HEIGHT = 8f;
        private const float INDICATOR_PREFAB_WIDTH = 8f;
        private const float INDICATORS_HEIGHT = 16f;
        private const float INDICATORS_WIDTH = 100f;
        private const float LABEL_HEIGHT = 50f;
        private const float LEFT_ARROW_HEIGHT = 80f;
        private const float LEFT_ARROW_WIDTH = 40f;
        private const float LEFT_TOP_AREA_HEIGHT = 350f;
        private const float LEFT_TOP_AREA_WIDTH = 400f;
        private const float NEW_BADGE_HEIGHT = 14f;
        private const float NEW_BADGE_WIDTH = 24f;
        private const float PASS_BUTTON_GROUP_HEIGHT = 100f;
        private const float QUICK_MENU_GRID_HEIGHT = 180f;
        private const float RIGHT_ARROW_HEIGHT = 80f;
        private const float RIGHT_ARROW_WIDTH = 40f;
        private const float RIGHT_BOTTOM_AREA_HEIGHT = 231.8349f;
        private const float RIGHT_BOTTOM_AREA_WIDTH = 220f;
        private const float RIGHT_TOP_AREA_HEIGHT = 250f;
        private const float RIGHT_TOP_AREA_WIDTH = 350f;
        private const float STAGE_LABEL_HEIGHT = 50f;
        private const float STAGE_NAME_HEIGHT = 50f;
        private const float STAGE_PROGRESS_WIDGET_HEIGHT = 50f;
        private const float STAGE_SHORTCUT_BUTTON_HEIGHT = 100f;
        private const float VIEWPORT_HEIGHT = 20f;

        #endregion

        #region Font Helper

        private static void ApplyFont(TextMeshProUGUI tmp)
        {
            var font = EditorUIHelpers.GetProjectFont();
            if (font != null) tmp.font = font;
        }

        #endregion

        /// <summary>
        /// LobbyScreen ?�리?�용 GameObject ?�성.
        /// </summary>
        public static GameObject Build()
        {
            var root = CreateRoot("LobbyScreen");

            var background = CreateBackground(root);
            var safeArea = CreateSafeArea(root);
            var overlayLayer = CreateOverlayLayer(root);

            // Add main component
            root.AddComponent<LobbyScreen>();

            // Connect serialized fields
            ConnectSerializedFields(root);

            return root;
        }

        #region Background

        private static GameObject CreateBackground(GameObject parent)
        {
            var go = CreateChild(parent, "Background");
            SetStretch(go);


            var image = go.AddComponent<Image>();
            image.color = BgDeep;
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region SafeArea

        private static GameObject CreateSafeArea(GameObject parent)
        {
            var go = CreateChild(parent, "SafeArea");
            SetStretch(go);

            CreateHeader(go);
            CreateContent_1(go);

            return go;
        }

        #endregion

        #region Header

        private static GameObject CreateHeader(GameObject parent)
        {
            var go = CreateChild(parent, "Header");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, 80f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            CreateScreenHeaderPlaceholder(go);

            return go;
        }

        #endregion

        #region ScreenHeaderPlaceholder

        private static GameObject CreateScreenHeaderPlaceholder(GameObject parent)
        {
            var go = CreateChild(parent, "ScreenHeaderPlaceholder");
            SetStretch(go);


            var image = go.AddComponent<Image>();
            image.color = new Color32(0, 0, 0, 128);
            image.raycastTarget = true;

            CreateText_1(go);

            return go;
        }

        #endregion

        #region Text

        private static GameObject CreateText_1(GameObject parent)
        {
            var go = CreateChild(parent, "Text");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "[ScreenHeader]";
            tmp.fontSize = 14f;
            tmp.color = new Color32(255, 255, 255, 128);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region Content

        private static GameObject CreateContent_1(GameObject parent)
        {
            var go = CreateChild(parent, "Content");
            SetStretch(go);
            var rect = go.GetComponent<RectTransform>();
            rect.offsetMin = new Vector2(0f, 100f);
            rect.offsetMax = new Vector2(0f, -80f);

            CreateLeftTopArea(go);
            CreateRightTopArea(go);
            CreateCenterArea(go);
            CreateBottomNav(go);
            CreateRightBottomArea(go);

            return go;
        }

        #endregion

        #region LeftTopArea

        private static GameObject CreateLeftTopArea(GameObject parent)
        {
            var go = CreateChild(parent, "LeftTopArea");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.sizeDelta = new Vector2(400f, 350f);
            rect.anchoredPosition = new Vector2(20f, -20f);

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 16f;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateEventBannerCarousel(go);
            CreatePassButtonGroup(go);

            return go;
        }

        #endregion

        #region EventBannerCarousel

        private static GameObject CreateEventBannerCarousel(GameObject parent)
        {
            var go = CreateChild(parent, "EventBannerCarousel");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 150f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 150f;


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            CreateBannerContainer(go);
            CreateIndicators(go);
            CreateIndicatorPrefab(go);

            return go;
        }

        #endregion

        #region BannerContainer

        private static GameObject CreateBannerContainer(GameObject parent)
        {
            var go = CreateChild(parent, "BannerContainer");
            SetStretch(go);
            var rect = go.GetComponent<RectTransform>();
            rect.offsetMin = new Vector2(0f, 20f);
            rect.offsetMax = new Vector2(0f, 0f);

            return go;
        }

        #endregion

        #region Indicators

        private static GameObject CreateIndicators(GameObject parent)
        {
            var go = CreateChild(parent, "Indicators");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(100f, 16f);
            rect.anchoredPosition = new Vector2(0f, 4f);

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            return go;
        }

        #endregion

        #region IndicatorPrefab

        private static GameObject CreateIndicatorPrefab(GameObject parent)
        {
            var go = CreateChild(parent, "IndicatorPrefab");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(8f, 8f);
            rect.anchoredPosition = new Vector2(0f, 0f);


            var image = go.AddComponent<Image>();
            image.color = new Color32(255, 255, 255, 128);
            image.raycastTarget = true;
            go.SetActive(false);

            return go;
        }

        #endregion

        #region PassButtonGroup

        private static GameObject CreatePassButtonGroup(GameObject parent)
        {
            var go = CreateChild(parent, "PassButtonGroup");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 100f;

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreatePassButton_LevelPass(go);
            CreatePassButton_StoryPass(go);
            CreatePassButton_TrialPass(go);
            CreatePassButton_StepUpPackage(go);

            return go;
        }

        #endregion

        #region PassButton_LevelPass

        private static GameObject CreatePassButton_LevelPass(GameObject parent)
        {
            var go = CreateChild(parent, "PassButton_LevelPass");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(4, 4, 8, 4);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_1(go);
            CreateLabel_1(go);
            CreateNewBadge_1(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_1(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 40f;
            layoutElement.preferredHeight = 40f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(255, 215, 100, 255);
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_1(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 20f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "?�벨?�스";
            tmp.fontSize = 10f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region NewBadge

        private static GameObject CreateNewBadge_1(GameObject parent)
        {
            var go = CreateChild(parent, "NewBadge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(24f, 14f);
            rect.anchoredPosition = new Vector2(-2f, -2f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateText_2(go);

            return go;
        }

        #endregion

        #region Text

        private static GameObject CreateText_2(GameObject parent)
        {
            var go = CreateChild(parent, "Text");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "NEW";
            tmp.fontSize = 8f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region PassButton_StoryPass

        private static GameObject CreatePassButton_StoryPass(GameObject parent)
        {
            var go = CreateChild(parent, "PassButton_StoryPass");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(4, 4, 8, 4);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_2(go);
            CreateLabel_2(go);
            CreateNewBadge_2(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_2(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 40f;
            layoutElement.preferredHeight = 40f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(255, 215, 100, 255);
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_2(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 20f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "?�록?�스";
            tmp.fontSize = 10f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region NewBadge

        private static GameObject CreateNewBadge_2(GameObject parent)
        {
            var go = CreateChild(parent, "NewBadge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(24f, 14f);
            rect.anchoredPosition = new Vector2(-2f, -2f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateText_3(go);

            return go;
        }

        #endregion

        #region Text

        private static GameObject CreateText_3(GameObject parent)
        {
            var go = CreateChild(parent, "Text");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "NEW";
            tmp.fontSize = 8f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region PassButton_TrialPass

        private static GameObject CreatePassButton_TrialPass(GameObject parent)
        {
            var go = CreateChild(parent, "PassButton_TrialPass");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(4, 4, 8, 4);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_3(go);
            CreateLabel_3(go);
            CreateNewBadge_3(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_3(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 40f;
            layoutElement.preferredHeight = 40f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(255, 215, 100, 255);
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_3(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 20f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "?�라?�얼?�스";
            tmp.fontSize = 10f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region NewBadge

        private static GameObject CreateNewBadge_3(GameObject parent)
        {
            var go = CreateChild(parent, "NewBadge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(24f, 14f);
            rect.anchoredPosition = new Vector2(-2f, -2f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateText_4(go);

            return go;
        }

        #endregion

        #region Text

        private static GameObject CreateText_4(GameObject parent)
        {
            var go = CreateChild(parent, "Text");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "NEW";
            tmp.fontSize = 8f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region PassButton_StepUpPackage

        private static GameObject CreatePassButton_StepUpPackage(GameObject parent)
        {
            var go = CreateChild(parent, "PassButton_StepUpPackage");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(4, 4, 8, 4);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_4(go);
            CreateLabel_4(go);
            CreateNewBadge_4(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_4(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 40f;
            layoutElement.preferredHeight = 40f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(255, 215, 100, 255);
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_4(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 20f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "?�텝?�패?��?";
            tmp.fontSize = 10f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region NewBadge

        private static GameObject CreateNewBadge_4(GameObject parent)
        {
            var go = CreateChild(parent, "NewBadge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(24f, 14f);
            rect.anchoredPosition = new Vector2(-2f, -2f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateText_5(go);

            return go;
        }

        #endregion

        #region Text

        private static GameObject CreateText_5(GameObject parent)
        {
            var go = CreateChild(parent, "Text");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "NEW";
            tmp.fontSize = 8f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region RightTopArea

        private static GameObject CreateRightTopArea(GameObject parent)
        {
            var go = CreateChild(parent, "RightTopArea");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(350f, 250f);
            rect.anchoredPosition = new Vector2(-20f, -20f);

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 12f;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateStageProgressWidget(go);
            CreateQuickMenuGrid(go);

            return go;
        }

        #endregion

        #region StageProgressWidget

        private static GameObject CreateStageProgressWidget(GameObject parent)
        {
            var go = CreateChild(parent, "StageProgressWidget");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 50f;


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(12, 12, 8, 8);
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateStageLabel(go);
            CreateStageName(go);

            return go;
        }

        #endregion

        #region StageLabel

        private static GameObject CreateStageLabel(GameObject parent)
        {
            var go = CreateChild(parent, "StageLabel");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 16f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "11-10";
            tmp.fontSize = 12f;
            tmp.color = new Color32(100, 200, 255, 255);
            tmp.alignment = TextAlignmentOptions.TopLeft;
            tmp.fontStyle = FontStyles.Bold;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region StageName

        private static GameObject CreateStageName(GameObject parent)
        {
            var go = CreateChild(parent, "StageName");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 18f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "최후??방어??";
            tmp.fontSize = 14f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.TopLeft;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region QuickMenuGrid

        private static GameObject CreateQuickMenuGrid(GameObject parent)
        {
            var go = CreateChild(parent, "QuickMenuGrid");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 180f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 180f;
            layoutElement.flexibleHeight = 1f;

            var grid = go.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(80f, 80f);
            grid.spacing = new Vector2(8f, 8f);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperLeft;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;
            grid.padding = new RectOffset(0, 0, 0, 0);

            CreateQuickMenuButton_LiveEventScreen(go);
            CreateQuickMenuButton_FarmScreen(go);
            CreateQuickMenuButton_FriendScreen(go);
            CreateQuickMenuButton_QuestScreen(go);
            CreateQuickMenuButton_PowerUpScreen(go);
            CreateQuickMenuButton_MonthlyScreen(go);
            CreateQuickMenuButton_ReturnScreen(go);
            CreateQuickMenuButton_MissionScreen(go);

            return go;
        }

        #endregion

        #region QuickMenuButton_LiveEventScreen

        private static GameObject CreateQuickMenuButton_LiveEventScreen(GameObject parent)
        {
            var go = CreateChild(parent, "QuickMenuButton_LiveEventScreen");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(4, 4, 8, 4);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_5(go);
            CreateLabel_5(go);
            CreateBadge_1(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_5(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 32f;
            layoutElement.preferredHeight = 32f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(100, 200, 255, 255);
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_5(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 14f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "?�벤??;
            tmp.fontSize = 10f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region Badge

        private static GameObject CreateBadge_1(GameObject parent)
        {
            var go = CreateChild(parent, "Badge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(20f, 20f);
            rect.anchoredPosition = new Vector2(-2f, -2f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateCount_1(go);

            return go;
        }

        #endregion

        #region Count

        private static GameObject CreateCount_1(GameObject parent)
        {
            var go = CreateChild(parent, "Count");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 10f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region QuickMenuButton_FarmScreen

        private static GameObject CreateQuickMenuButton_FarmScreen(GameObject parent)
        {
            var go = CreateChild(parent, "QuickMenuButton_FarmScreen");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(4, 4, 8, 4);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_6(go);
            CreateLabel_6(go);
            CreateBadge_2(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_6(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 32f;
            layoutElement.preferredHeight = 32f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(100, 200, 255, 255);
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_6(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 14f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "?�일?�장";
            tmp.fontSize = 10f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region Badge

        private static GameObject CreateBadge_2(GameObject parent)
        {
            var go = CreateChild(parent, "Badge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(20f, 20f);
            rect.anchoredPosition = new Vector2(-2f, -2f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateCount_2(go);

            return go;
        }

        #endregion

        #region Count

        private static GameObject CreateCount_2(GameObject parent)
        {
            var go = CreateChild(parent, "Count");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 10f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region QuickMenuButton_FriendScreen

        private static GameObject CreateQuickMenuButton_FriendScreen(GameObject parent)
        {
            var go = CreateChild(parent, "QuickMenuButton_FriendScreen");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(4, 4, 8, 4);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_7(go);
            CreateLabel_7(go);
            CreateBadge_3(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_7(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 32f;
            layoutElement.preferredHeight = 32f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(100, 200, 255, 255);
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_7(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 14f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "친구";
            tmp.fontSize = 10f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region Badge

        private static GameObject CreateBadge_3(GameObject parent)
        {
            var go = CreateChild(parent, "Badge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(20f, 20f);
            rect.anchoredPosition = new Vector2(-2f, -2f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateCount_3(go);

            return go;
        }

        #endregion

        #region Count

        private static GameObject CreateCount_3(GameObject parent)
        {
            var go = CreateChild(parent, "Count");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 10f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region QuickMenuButton_QuestScreen

        private static GameObject CreateQuickMenuButton_QuestScreen(GameObject parent)
        {
            var go = CreateChild(parent, "QuickMenuButton_QuestScreen");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(4, 4, 8, 4);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_8(go);
            CreateLabel_8(go);
            CreateBadge_4(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_8(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 32f;
            layoutElement.preferredHeight = 32f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(100, 200, 255, 255);
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_8(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 14f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "?�스??;
            tmp.fontSize = 10f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region Badge

        private static GameObject CreateBadge_4(GameObject parent)
        {
            var go = CreateChild(parent, "Badge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(20f, 20f);
            rect.anchoredPosition = new Vector2(-2f, -2f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateCount_4(go);

            return go;
        }

        #endregion

        #region Count

        private static GameObject CreateCount_4(GameObject parent)
        {
            var go = CreateChild(parent, "Count");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 10f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region QuickMenuButton_PowerUpScreen

        private static GameObject CreateQuickMenuButton_PowerUpScreen(GameObject parent)
        {
            var go = CreateChild(parent, "QuickMenuButton_PowerUpScreen");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(4, 4, 8, 4);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_9(go);
            CreateLabel_9(go);
            CreateBadge_5(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_9(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 32f;
            layoutElement.preferredHeight = 32f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(100, 200, 255, 255);
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_9(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 14f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "강해지�?;
            tmp.fontSize = 10f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region Badge

        private static GameObject CreateBadge_5(GameObject parent)
        {
            var go = CreateChild(parent, "Badge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(20f, 20f);
            rect.anchoredPosition = new Vector2(-2f, -2f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateCount_5(go);

            return go;
        }

        #endregion

        #region Count

        private static GameObject CreateCount_5(GameObject parent)
        {
            var go = CreateChild(parent, "Count");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 10f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region QuickMenuButton_MonthlyScreen

        private static GameObject CreateQuickMenuButton_MonthlyScreen(GameObject parent)
        {
            var go = CreateChild(parent, "QuickMenuButton_MonthlyScreen");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(4, 4, 8, 4);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_10(go);
            CreateLabel_10(go);
            CreateBadge_6(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_10(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 32f;
            layoutElement.preferredHeight = 32f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(100, 200, 255, 255);
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_10(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 14f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "?�간??;
            tmp.fontSize = 10f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region Badge

        private static GameObject CreateBadge_6(GameObject parent)
        {
            var go = CreateChild(parent, "Badge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(20f, 20f);
            rect.anchoredPosition = new Vector2(-2f, -2f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateCount_6(go);

            return go;
        }

        #endregion

        #region Count

        private static GameObject CreateCount_6(GameObject parent)
        {
            var go = CreateChild(parent, "Count");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 10f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region QuickMenuButton_ReturnScreen

        private static GameObject CreateQuickMenuButton_ReturnScreen(GameObject parent)
        {
            var go = CreateChild(parent, "QuickMenuButton_ReturnScreen");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(4, 4, 8, 4);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_11(go);
            CreateLabel_11(go);
            CreateBadge_7(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_11(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 32f;
            layoutElement.preferredHeight = 32f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(100, 200, 255, 255);
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_11(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 14f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "복�??�영";
            tmp.fontSize = 10f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region Badge

        private static GameObject CreateBadge_7(GameObject parent)
        {
            var go = CreateChild(parent, "Badge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(20f, 20f);
            rect.anchoredPosition = new Vector2(-2f, -2f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateCount_7(go);

            return go;
        }

        #endregion

        #region Count

        private static GameObject CreateCount_7(GameObject parent)
        {
            var go = CreateChild(parent, "Count");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 10f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region QuickMenuButton_MissionScreen

        private static GameObject CreateQuickMenuButton_MissionScreen(GameObject parent)
        {
            var go = CreateChild(parent, "QuickMenuButton_MissionScreen");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(4, 4, 8, 4);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_12(go);
            CreateLabel_12(go);
            CreateBadge_8(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_12(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 32f;
            layoutElement.preferredHeight = 32f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(100, 200, 255, 255);
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_12(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 14f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "미션";
            tmp.fontSize = 10f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region Badge

        private static GameObject CreateBadge_8(GameObject parent)
        {
            var go = CreateChild(parent, "Badge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(20f, 20f);
            rect.anchoredPosition = new Vector2(-2f, -2f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateCount_8(go);

            return go;
        }

        #endregion

        #region Count

        private static GameObject CreateCount_8(GameObject parent)
        {
            var go = CreateChild(parent, "Count");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 10f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region CenterArea

        private static GameObject CreateCenterArea(GameObject parent)
        {
            var go = CreateChild(parent, "CenterArea");
            SetStretch(go);

            CreateCharacterDisplay(go);

            return go;
        }

        #endregion

        #region CharacterDisplay

        private static GameObject CreateCharacterDisplay(GameObject parent)
        {
            var go = CreateChild(parent, "CharacterDisplay");
            SetStretch(go);

            CreateContent_2(go);
            CreateLeftArrow(go);
            CreateRightArrow(go);

            return go;
        }

        #endregion

        #region Content

        private static GameObject CreateContent_2(GameObject parent)
        {
            var go = CreateChild(parent, "Content");
            SetStretch(go);

            CreateCharacterImage(go);
            CreateDialogueBox(go);

            return go;
        }

        #endregion

        #region CharacterImage

        private static GameObject CreateCharacterImage(GameObject parent)
        {
            var go = CreateChild(parent, "CharacterImage");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(350f, 615.8789f);
            rect.anchoredPosition = new Vector2(0f, 127.2311f);


            var image = go.AddComponent<Image>();
            image.color = new Color32(255, 255, 255, 26);
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            CreatePlaceholder(go);

            return go;
        }

        #endregion

        #region Placeholder

        private static GameObject CreatePlaceholder(GameObject parent)
        {
            var go = CreateChild(parent, "Placeholder");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "CHARACTER";
            tmp.fontSize = 24f;
            tmp.color = new Color32(255, 255, 255, 128);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region DialogueBox

        private static GameObject CreateDialogueBox(GameObject parent)
        {
            var go = CreateChild(parent, "DialogueBox");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(300f, 60f);
            rect.anchoredPosition = new Vector2(0f, 762f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            CreateDialogueText(go);

            return go;
        }

        #endregion

        #region DialogueText

        private static GameObject CreateDialogueText(GameObject parent)
        {
            var go = CreateChild(parent, "DialogueText");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "?��? ?��??�을 ???�???�고...";
            tmp.fontSize = 14f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region LeftArrow

        private static GameObject CreateLeftArrow(GameObject parent)
        {
            var go = CreateChild(parent, "LeftArrow");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = new Vector2(40f, 80f);
            rect.anchoredPosition = new Vector2(10f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            CreateText_6(go);

            return go;
        }

        #endregion

        #region Text

        private static GameObject CreateText_6(GameObject parent)
        {
            var go = CreateChild(parent, "Text");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "<";
            tmp.fontSize = 24f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region RightArrow

        private static GameObject CreateRightArrow(GameObject parent)
        {
            var go = CreateChild(parent, "RightArrow");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.sizeDelta = new Vector2(40f, 80f);
            rect.anchoredPosition = new Vector2(-10f, 0f);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            CreateText_7(go);

            return go;
        }

        #endregion

        #region Text

        private static GameObject CreateText_7(GameObject parent)
        {
            var go = CreateChild(parent, "Text");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = ">";
            tmp.fontSize = 24f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region BottomNav

        private static GameObject CreateBottomNav(GameObject parent)
        {
            var go = CreateChild(parent, "BottomNav");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(0f, 193.9596f);
            rect.anchoredPosition = new Vector2(0f, -100f);


            var image = go.AddComponent<Image>();
            image.color = new Color32(0, 0, 0, 230);
            image.raycastTarget = true;

            var scrollRect = go.AddComponent<ScrollRect>();
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;

            CreateViewport(go);

            return go;
        }

        #endregion

        #region Viewport

        private static GameObject CreateViewport(GameObject parent)
        {
            var go = CreateChild(parent, "Viewport");
            SetStretch(go);
            var rect = go.GetComponent<RectTransform>();
            rect.offsetMin = new Vector2(10f, 10f);
            rect.offsetMax = new Vector2(-10f, -10f);

            var mask = go.AddComponent<Mask>();
            mask.showMaskGraphic = false;


            var image = go.AddComponent<Image>();
            image.color = TextPrimary;
            image.raycastTarget = true;

            CreateContent_3(go);

            return go;
        }

        #endregion

        #region Content

        private static GameObject CreateContent_3(GameObject parent)
        {
            var go = CreateChild(parent, "Content");
            SetStretch(go);

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            var fitter = go.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;

            CreateContentNavButton_GachaScreen(go);
            CreateContentNavButton_ShopScreen(go);
            CreateContentNavButton_CharacterListScreen(go);

            return go;
        }

        #endregion

        #region ContentNavButton_GachaScreen

        private static GameObject CreateContentNavButton_GachaScreen(GameObject parent)
        {
            var go = CreateChild(parent, "ContentNavButton_GachaScreen");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(100f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 100f;


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(8, 8, 12, 8);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_13(go);
            CreateLabel_13(go);
            CreateBadge_9(go);
            CreateGlowEffect_1(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_13(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 32f;
            layoutElement.preferredHeight = 32f;


            var image = go.AddComponent<Image>();
            image.color = AccentPurple;
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_13(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 18f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "모집";
            tmp.fontSize = 12f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region Badge

        private static GameObject CreateBadge_9(GameObject parent)
        {
            var go = CreateChild(parent, "Badge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(20f, 20f);
            rect.anchoredPosition = new Vector2(-4f, -4f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateCount_9(go);

            return go;
        }

        #endregion

        #region Count

        private static GameObject CreateCount_9(GameObject parent)
        {
            var go = CreateChild(parent, "Count");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 10f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region GlowEffect

        private static GameObject CreateGlowEffect_1(GameObject parent)
        {
            var go = CreateChild(parent, "GlowEffect");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(60f, 4f);
            rect.anchoredPosition = new Vector2(0f, 2f);


            var image = go.AddComponent<Image>();
            image.color = AccentPurple;
            image.raycastTarget = true;
            go.SetActive(false);

            return go;
        }

        #endregion

        #region ContentNavButton_ShopScreen

        private static GameObject CreateContentNavButton_ShopScreen(GameObject parent)
        {
            var go = CreateChild(parent, "ContentNavButton_ShopScreen");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(100f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 100f;


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(8, 8, 12, 8);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_14(go);
            CreateLabel_14(go);
            CreateBadge_10(go);
            CreateGlowEffect_2(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_14(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 32f;
            layoutElement.preferredHeight = 32f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(100, 200, 255, 255);
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_14(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 18f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "?�점";
            tmp.fontSize = 12f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region Badge

        private static GameObject CreateBadge_10(GameObject parent)
        {
            var go = CreateChild(parent, "Badge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(20f, 20f);
            rect.anchoredPosition = new Vector2(-4f, -4f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateCount_10(go);

            return go;
        }

        #endregion

        #region Count

        private static GameObject CreateCount_10(GameObject parent)
        {
            var go = CreateChild(parent, "Count");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 10f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region GlowEffect

        private static GameObject CreateGlowEffect_2(GameObject parent)
        {
            var go = CreateChild(parent, "GlowEffect");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(60f, 4f);
            rect.anchoredPosition = new Vector2(0f, 2f);


            var image = go.AddComponent<Image>();
            image.color = new Color32(100, 200, 255, 255);
            image.raycastTarget = true;
            go.SetActive(false);

            return go;
        }

        #endregion

        #region ContentNavButton_CharacterListScreen

        private static GameObject CreateContentNavButton_CharacterListScreen(GameObject parent)
        {
            var go = CreateChild(parent, "ContentNavButton_CharacterListScreen");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(100f, 0f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 100f;


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.padding = new RectOffset(8, 8, 12, 8);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            CreateIcon_15(go);
            CreateLabel_15(go);
            CreateBadge_11(go);
            CreateGlowEffect_3(go);

            return go;
        }

        #endregion

        #region Icon

        private static GameObject CreateIcon_15(GameObject parent)
        {
            var go = CreateChild(parent, "Icon");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 32f;
            layoutElement.preferredHeight = 32f;


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_15(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 50f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 18f;


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "?�도";
            tmp.fontSize = 12f;
            tmp.color = TextSecondary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region Badge

        private static GameObject CreateBadge_11(GameObject parent)
        {
            var go = CreateChild(parent, "Badge");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(20f, 20f);
            rect.anchoredPosition = new Vector2(-4f, -4f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            CreateCount_11(go);

            return go;
        }

        #endregion

        #region Count

        private static GameObject CreateCount_11(GameObject parent)
        {
            var go = CreateChild(parent, "Count");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 10f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region GlowEffect

        private static GameObject CreateGlowEffect_3(GameObject parent)
        {
            var go = CreateChild(parent, "GlowEffect");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(60f, 4f);
            rect.anchoredPosition = new Vector2(0f, 2f);


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;
            go.SetActive(false);

            return go;
        }

        #endregion

        #region RightBottomArea

        private static GameObject CreateRightBottomArea(GameObject parent)
        {
            var go = CreateChild(parent, "RightBottomArea");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.sizeDelta = new Vector2(220f, 231.8349f);
            rect.anchoredPosition = new Vector2(0f, -100f);

            CreateInGameContentDashboard(go);

            return go;
        }

        #endregion

        #region InGameContentDashboard

        private static GameObject CreateInGameContentDashboard(GameObject parent)
        {
            var go = CreateChild(parent, "InGameContentDashboard");
            SetStretch(go);


            var image = go.AddComponent<Image>();
            image.color = BgCard;
            image.raycastTarget = true;

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 8f;
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            CreateStageShortcutButton(go);
            CreateAdventureButton(go);

            return go;
        }

        #endregion

        #region StageShortcutButton

        private static GameObject CreateStageShortcutButton(GameObject parent)
        {
            var go = CreateChild(parent, "StageShortcutButton");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 50f;


            var image = go.AddComponent<Image>();
            image.color = new Color32(100, 200, 255, 255);
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            CreateLabel_16(go);

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_16(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "11-1 바로 가??";
            tmp.fontSize = 14f;
            tmp.color = BgDeep;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region AdventureButton

        private static GameObject CreateAdventureButton(GameObject parent)
        {
            var go = CreateChild(parent, "AdventureButton");
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 100f);
            rect.anchoredPosition = new Vector2(0f, 0f);

            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 50f;


            var image = go.AddComponent<Image>();
            image.color = AccentSecondary;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            var img = go.GetComponent<Image>();
            if (img != null) button.targetGraphic = img;

            CreateLabel_17(go);

            return go;
        }

        #endregion

        #region Label

        private static GameObject CreateLabel_17(GameObject parent)
        {
            var go = CreateChild(parent, "Label");
            SetStretch(go);


            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = "모험";
            tmp.fontSize = 16f;
            tmp.color = TextPrimary;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.raycastTarget = true;
            ApplyFont(tmp);

            return go;
        }

        #endregion

        #region OverlayLayer

        private static GameObject CreateOverlayLayer(GameObject parent)
        {
            var go = CreateChild(parent, "OverlayLayer");
            SetStretch(go);

            return go;
        }

        #endregion

        #region SerializedField Connection

        private static void ConnectSerializedFields(GameObject root)
        {
            var component = root.GetComponent<LobbyScreen>();
            if (component == null) return;

            var so = new SerializedObject(component);

            // _eventBannerCarousel
            so.FindProperty("_eventBannerCarousel").objectReferenceValue = FindChild(root, "SafeArea/Content/LeftTopArea/EventBannerCarousel")?.GetComponent<EventBannerCarousel>();

            // _passButtons (array)
            var passButtonsProp = so.FindProperty("_passButtons");
            passButtonsProp.arraySize = 4;
            passButtonsProp.GetArrayElementAtIndex(0).objectReferenceValue = FindChild(root, "SafeArea/Content/LeftTopArea/PassButtonGroup/PassButton_LevelPass")?.GetComponent<PassButton>();
            passButtonsProp.GetArrayElementAtIndex(1).objectReferenceValue = FindChild(root, "SafeArea/Content/LeftTopArea/PassButtonGroup/PassButton_StoryPass")?.GetComponent<PassButton>();
            passButtonsProp.GetArrayElementAtIndex(2).objectReferenceValue = FindChild(root, "SafeArea/Content/LeftTopArea/PassButtonGroup/PassButton_TrialPass")?.GetComponent<PassButton>();
            passButtonsProp.GetArrayElementAtIndex(3).objectReferenceValue = FindChild(root, "SafeArea/Content/LeftTopArea/PassButtonGroup/PassButton_StepUpPackage")?.GetComponent<PassButton>();

            // _stageProgressWidget
            so.FindProperty("_stageProgressWidget").objectReferenceValue = FindChild(root, "SafeArea/Content/RightTopArea/StageProgressWidget")?.GetComponent<StageProgressWidget>();

            // _quickMenuButtons (array)
            var quickMenuButtonsProp = so.FindProperty("_quickMenuButtons");
            quickMenuButtonsProp.arraySize = 8;
            quickMenuButtonsProp.GetArrayElementAtIndex(0).objectReferenceValue = FindChild(root, "SafeArea/Content/RightTopArea/QuickMenuGrid/QuickMenuButton_LiveEventScreen")?.GetComponent<QuickMenuButton>();
            quickMenuButtonsProp.GetArrayElementAtIndex(1).objectReferenceValue = FindChild(root, "SafeArea/Content/RightTopArea/QuickMenuGrid/QuickMenuButton_FarmScreen")?.GetComponent<QuickMenuButton>();
            quickMenuButtonsProp.GetArrayElementAtIndex(2).objectReferenceValue = FindChild(root, "SafeArea/Content/RightTopArea/QuickMenuGrid/QuickMenuButton_FriendScreen")?.GetComponent<QuickMenuButton>();
            quickMenuButtonsProp.GetArrayElementAtIndex(3).objectReferenceValue = FindChild(root, "SafeArea/Content/RightTopArea/QuickMenuGrid/QuickMenuButton_QuestScreen")?.GetComponent<QuickMenuButton>();
            quickMenuButtonsProp.GetArrayElementAtIndex(4).objectReferenceValue = FindChild(root, "SafeArea/Content/RightTopArea/QuickMenuGrid/QuickMenuButton_PowerUpScreen")?.GetComponent<QuickMenuButton>();
            quickMenuButtonsProp.GetArrayElementAtIndex(5).objectReferenceValue = FindChild(root, "SafeArea/Content/RightTopArea/QuickMenuGrid/QuickMenuButton_MonthlyScreen")?.GetComponent<QuickMenuButton>();
            quickMenuButtonsProp.GetArrayElementAtIndex(6).objectReferenceValue = FindChild(root, "SafeArea/Content/RightTopArea/QuickMenuGrid/QuickMenuButton_ReturnScreen")?.GetComponent<QuickMenuButton>();
            quickMenuButtonsProp.GetArrayElementAtIndex(7).objectReferenceValue = FindChild(root, "SafeArea/Content/RightTopArea/QuickMenuGrid/QuickMenuButton_MissionScreen")?.GetComponent<QuickMenuButton>();

            // _characterDisplay
            so.FindProperty("_characterDisplay").objectReferenceValue = FindChild(root, "SafeArea/Content/CenterArea/CharacterDisplay")?.GetComponent<CharacterDisplayWidget>();

            // _inGameDashboard
            so.FindProperty("_inGameDashboard").objectReferenceValue = FindChild(root, "SafeArea/Content/RightBottomArea/InGameContentDashboard");

            // _stageShortcutButton
            so.FindProperty("_stageShortcutButton").objectReferenceValue = FindChild(root, "SafeArea/Content/RightBottomArea/InGameContentDashboard/StageShortcutButton")?.GetComponent<Button>();

            // _stageShortcutLabel
            so.FindProperty("_stageShortcutLabel").objectReferenceValue = FindChild(root, "SafeArea/Content/RightBottomArea/InGameContentDashboard/StageShortcutButton/Label")?.GetComponent<TextMeshProUGUI>();

            // _adventureButton
            so.FindProperty("_adventureButton").objectReferenceValue = FindChild(root, "SafeArea/Content/RightBottomArea/InGameContentDashboard/AdventureButton")?.GetComponent<Button>();

            // _contentNavButtons (array)
            var contentNavButtonsProp = so.FindProperty("_contentNavButtons");
            contentNavButtonsProp.arraySize = 3;
            contentNavButtonsProp.GetArrayElementAtIndex(0).objectReferenceValue = FindChild(root, "SafeArea/Content/BottomNav/Viewport/Content/ContentNavButton_GachaScreen")?.GetComponent<ContentNavButton>();
            contentNavButtonsProp.GetArrayElementAtIndex(1).objectReferenceValue = FindChild(root, "SafeArea/Content/BottomNav/Viewport/Content/ContentNavButton_ShopScreen")?.GetComponent<ContentNavButton>();
            contentNavButtonsProp.GetArrayElementAtIndex(2).objectReferenceValue = FindChild(root, "SafeArea/Content/BottomNav/Viewport/Content/ContentNavButton_CharacterListScreen")?.GetComponent<ContentNavButton>();

            // _bottomNavScroll
            so.FindProperty("_bottomNavScroll").objectReferenceValue = FindChild(root, "SafeArea/Content/BottomNav")?.GetComponent<ScrollRect>();

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject FindChild(GameObject root, string path)
        {
            if (string.IsNullOrEmpty(path)) return root;
            var t = root.transform.Find(path);
            return t?.gameObject;
        }

        #endregion

        #region Helpers

        private static GameObject CreateRoot(string name)
        {
            var root = new GameObject(name);
            var rect = root.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            root.AddComponent<CanvasGroup>();
            return root;
        }

        private static GameObject CreateChild(GameObject parent, string name)
        {
            var child = new GameObject(name);
            child.transform.SetParent(parent.transform, false);
            return child;
        }

        private static RectTransform SetStretch(GameObject go)
        {
            var rect = go.GetComponent<RectTransform>();
            if (rect == null) rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        #endregion
    }
}
