using UnityEngine;
using Sc.Common.UI;
using TMPro;

namespace Sc.Tests
{
    /// <summary>
    /// 테스트용 간단한 Popup State
    /// </summary>
    public class SimpleTestPopupState : IPopupState
    {
        public string PopupName;
        public int Index;
    }

    /// <summary>
    /// 테스트용 간단한 Popup.
    /// 동적 생성 가능하도록 설계.
    /// </summary>
    public class SimpleTestPopup : PopupWidget<SimpleTestPopup, SimpleTestPopupState>
    {
        private TextMeshProUGUI _nameText;
        private SimpleTestPopupState _state;

        protected override void OnInitialize()
        {
            _nameText = GetComponentInChildren<TextMeshProUGUI>();
        }

        protected override void OnBind(SimpleTestPopupState state)
        {
            _state = state ?? new SimpleTestPopupState
            {
                PopupName = "SimpleTestPopup",
                Index = 0
            };

            UpdateUI();
        }

        protected override void OnShow()
        {
            Debug.Log($"[SimpleTestPopup] Show: {_state?.PopupName}");
        }

        protected override void OnHide()
        {
            Debug.Log($"[SimpleTestPopup] Hide: {_state?.PopupName}");
        }

        public override SimpleTestPopupState GetState() => _state;

        public override bool OnEscape() => true;

        private void UpdateUI()
        {
            if (_nameText != null)
            {
                _nameText.text = $"{_state.PopupName} (#{_state.Index})";
            }
        }

        /// <summary>
        /// 테스트용 Popup 인스턴스 동적 생성
        /// </summary>
        public static SimpleTestPopup CreateInstance(Transform parent, string name = "TestPopup")
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);

            // RectTransform 설정
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(500, 300);

            // Canvas Group for visibility
            go.AddComponent<CanvasGroup>();

            // Popup 컴포넌트
            var popup = go.AddComponent<SimpleTestPopup>();

            // 배경
            var bgGO = new GameObject("Background");
            bgGO.transform.SetParent(go.transform);
            var bgRect = bgGO.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var bgImage = bgGO.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);

            // 이름 텍스트
            var textGO = new GameObject("NameText");
            textGO.transform.SetParent(go.transform);
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.sizeDelta = new Vector2(400, 60);

            var text = textGO.AddComponent<TextMeshProUGUI>();
            text.text = name;
            text.fontSize = 24;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;

            return popup;
        }
    }
}
