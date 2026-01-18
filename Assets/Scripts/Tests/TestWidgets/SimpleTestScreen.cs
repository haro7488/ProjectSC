using UnityEngine;
using Sc.Common.UI;
using TMPro;

namespace Sc.Tests
{
    /// <summary>
    /// 테스트용 간단한 Screen State
    /// </summary>
    public class SimpleTestScreenState : IScreenState
    {
        public string ScreenName;
        public int Index;
    }

    /// <summary>
    /// 테스트용 간단한 Screen.
    /// 동적 생성 가능하도록 설계.
    /// </summary>
    public class SimpleTestScreen : ScreenWidget<SimpleTestScreen, SimpleTestScreenState>
    {
        private TextMeshProUGUI _nameText;
        private SimpleTestScreenState _state;

        protected override void OnInitialize()
        {
            // UI 요소 찾기 또는 생성
            _nameText = GetComponentInChildren<TextMeshProUGUI>();
        }

        protected override void OnBind(SimpleTestScreenState state)
        {
            _state = state ?? new SimpleTestScreenState
            {
                ScreenName = "SimpleTestScreen",
                Index = 0
            };

            UpdateUI();
        }

        protected override void OnShow()
        {
            Debug.Log($"[SimpleTestScreen] Show: {_state?.ScreenName}");
        }

        protected override void OnHide()
        {
            Debug.Log($"[SimpleTestScreen] Hide: {_state?.ScreenName}");
        }

        public override SimpleTestScreenState GetState() => _state;

        private void UpdateUI()
        {
            if (_nameText != null)
            {
                _nameText.text = $"{_state.ScreenName} (#{_state.Index})";
            }
        }

        /// <summary>
        /// 테스트용 Screen 인스턴스 동적 생성
        /// </summary>
        public static SimpleTestScreen CreateInstance(Transform parent, string name = "TestScreen")
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);

            // RectTransform 설정
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // Canvas Group for visibility
            go.AddComponent<CanvasGroup>();

            // Screen 컴포넌트
            var screen = go.AddComponent<SimpleTestScreen>();

            // 배경
            var bgGO = new GameObject("Background");
            bgGO.transform.SetParent(go.transform);
            var bgRect = bgGO.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var bgImage = bgGO.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(
                Random.Range(0.2f, 0.4f),
                Random.Range(0.2f, 0.4f),
                Random.Range(0.2f, 0.4f),
                1f
            );

            // 이름 텍스트
            var textGO = new GameObject("NameText");
            textGO.transform.SetParent(go.transform);
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.sizeDelta = new Vector2(400, 100);

            var text = textGO.AddComponent<TextMeshProUGUI>();
            text.text = name;
            text.fontSize = 36;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;

            return screen;
        }
    }
}
