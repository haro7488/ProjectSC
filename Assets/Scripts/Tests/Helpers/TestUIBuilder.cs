using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Sc.Tests
{
    /// <summary>
    /// 테스트 컨트롤 패널 UI 빌더
    /// </summary>
    public static class TestUIBuilder
    {
        private const float BUTTON_HEIGHT = 40f;
        private const float LABEL_HEIGHT = 30f;
        private const float SPACING = 5f;
        private const float PADDING = 10f;

        /// <summary>
        /// 컨트롤 패널 생성
        /// </summary>
        public static GameObject CreatePanel(RectTransform parent, string title)
        {
            // Panel 배경
            var panelGO = new GameObject($"Panel_{title}");
            panelGO.transform.SetParent(parent);

            var panelRect = panelGO.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.5f);
            panelRect.anchorMax = new Vector2(0, 0.5f);
            panelRect.pivot = new Vector2(0, 0.5f);
            panelRect.anchoredPosition = new Vector2(PADDING, 0);
            panelRect.sizeDelta = new Vector2(250, 400);

            var panelImage = panelGO.AddComponent<Image>();
            panelImage.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

            // Vertical Layout Group
            var layout = panelGO.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.spacing = SPACING;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = true;
            layout.childControlHeight = false;

            var fitter = panelGO.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Title
            AddLabel(panelGO, $"== {title} ==", FontStyles.Bold, TextAlignmentOptions.Center);

            return panelGO;
        }

        /// <summary>
        /// 버튼 추가
        /// </summary>
        public static Button AddButton(GameObject panel, string text, Action onClick)
        {
            var buttonGO = new GameObject($"Button_{text}");
            buttonGO.transform.SetParent(panel.transform);

            var rect = buttonGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, BUTTON_HEIGHT);

            var image = buttonGO.AddComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.3f, 1f);

            var button = buttonGO.AddComponent<Button>();
            button.targetGraphic = image;

            // Button colors
            var colors = button.colors;
            colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            button.colors = colors;

            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick());
            }

            // Text
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform);

            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var tmpText = textGO.AddComponent<TextMeshProUGUI>();
            tmpText.text = text;
            tmpText.fontSize = 16;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.color = Color.white;

            return button;
        }

        /// <summary>
        /// 라벨 추가
        /// </summary>
        public static TextMeshProUGUI AddLabel(
            GameObject panel,
            string text,
            FontStyles fontStyle = FontStyles.Normal,
            TextAlignmentOptions alignment = TextAlignmentOptions.Left)
        {
            var labelGO = new GameObject($"Label_{text}");
            labelGO.transform.SetParent(panel.transform);

            var rect = labelGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, LABEL_HEIGHT);

            var tmpText = labelGO.AddComponent<TextMeshProUGUI>();
            tmpText.text = text;
            tmpText.fontSize = 14;
            tmpText.fontStyle = fontStyle;
            tmpText.alignment = alignment;
            tmpText.color = Color.white;

            return tmpText;
        }

        /// <summary>
        /// 구분선 추가
        /// </summary>
        public static void AddSeparator(GameObject panel, float height = 2f)
        {
            var separatorGO = new GameObject("Separator");
            separatorGO.transform.SetParent(panel.transform);

            var rect = separatorGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, height + 10f);

            // 실제 선
            var lineGO = new GameObject("Line");
            lineGO.transform.SetParent(separatorGO.transform);

            var lineRect = lineGO.AddComponent<RectTransform>();
            lineRect.anchorMin = new Vector2(0, 0.5f);
            lineRect.anchorMax = new Vector2(1, 0.5f);
            lineRect.sizeDelta = new Vector2(0, height);
            lineRect.offsetMin = new Vector2(5, -height / 2);
            lineRect.offsetMax = new Vector2(-5, height / 2);

            var image = lineGO.AddComponent<Image>();
            image.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        /// <summary>
        /// 토글 추가
        /// </summary>
        public static Toggle AddToggle(GameObject panel, string text, bool initialValue, Action<bool> onValueChanged)
        {
            var toggleGO = new GameObject($"Toggle_{text}");
            toggleGO.transform.SetParent(panel.transform);

            var rect = toggleGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, BUTTON_HEIGHT);

            var layout = toggleGO.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childForceExpandWidth = false;
            layout.childControlWidth = false;
            layout.childAlignment = TextAnchor.MiddleLeft;

            // Checkbox background
            var checkboxGO = new GameObject("Checkbox");
            checkboxGO.transform.SetParent(toggleGO.transform);

            var checkboxRect = checkboxGO.AddComponent<RectTransform>();
            checkboxRect.sizeDelta = new Vector2(24, 24);

            var checkboxImage = checkboxGO.AddComponent<Image>();
            checkboxImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);

            // Checkmark
            var checkmarkGO = new GameObject("Checkmark");
            checkmarkGO.transform.SetParent(checkboxGO.transform);

            var checkmarkRect = checkmarkGO.AddComponent<RectTransform>();
            checkmarkRect.anchorMin = new Vector2(0.15f, 0.15f);
            checkmarkRect.anchorMax = new Vector2(0.85f, 0.85f);
            checkmarkRect.offsetMin = Vector2.zero;
            checkmarkRect.offsetMax = Vector2.zero;

            var checkmarkImage = checkmarkGO.AddComponent<Image>();
            checkmarkImage.color = Color.green;

            // Toggle component
            var toggle = toggleGO.AddComponent<Toggle>();
            toggle.targetGraphic = checkboxImage;
            toggle.graphic = checkmarkImage;
            toggle.isOn = initialValue;

            if (onValueChanged != null)
            {
                toggle.onValueChanged.AddListener((value) => onValueChanged(value));
            }

            // Label
            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(toggleGO.transform);

            var labelRect = labelGO.AddComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(180, 24);

            var label = labelGO.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = 14;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.color = Color.white;

            return toggle;
        }

        /// <summary>
        /// 결과 표시 영역 추가
        /// </summary>
        public static TextMeshProUGUI AddResultArea(GameObject panel, string initialText = "")
        {
            var areaGO = new GameObject("ResultArea");
            areaGO.transform.SetParent(panel.transform);

            var rect = areaGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 100);

            var bgImage = areaGO.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);

            // Text
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(areaGO.transform);

            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 5);
            textRect.offsetMax = new Vector2(-5, -5);

            var tmpText = textGO.AddComponent<TextMeshProUGUI>();
            tmpText.text = initialText;
            tmpText.fontSize = 12;
            tmpText.alignment = TextAlignmentOptions.TopLeft;
            tmpText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            tmpText.enableWordWrapping = true;
            tmpText.overflowMode = TextOverflowModes.Truncate;

            return tmpText;
        }
    }
}
