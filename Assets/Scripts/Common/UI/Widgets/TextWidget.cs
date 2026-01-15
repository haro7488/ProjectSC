using TMPro;
using UnityEngine;

namespace Sc.Common.UI
{
    /// <summary>
    /// TMP_Text를 래핑하는 Widget.
    /// 텍스트 표시 및 스타일링 기능 제공.
    /// </summary>
    public class TextWidget : Widget
    {
        [SerializeField] private TMP_Text _text;

        private string _currentText;

        public string Text => _currentText;

        #region Lifecycle

        protected override void OnInitialize()
        {
            if (_text != null)
            {
                _currentText = _text.text;
            }
        }

        protected override void OnBind(object data)
        {
            if (data is string text)
            {
                SetText(text);
            }
        }

        protected override void OnRelease()
        {
            _currentText = null;
        }

        #endregion

        #region Public API

        /// <summary>
        /// 텍스트 설정.
        /// </summary>
        public void SetText(string content)
        {
            _currentText = content;
            if (_text != null)
            {
                _text.text = _currentText ?? string.Empty;
            }
        }

        /// <summary>
        /// 텍스트 색상 설정.
        /// </summary>
        public void SetColor(Color color)
        {
            if (_text != null)
            {
                _text.color = color;
            }
        }

        /// <summary>
        /// 폰트 크기 설정.
        /// </summary>
        public void SetFontSize(float size)
        {
            if (_text != null)
            {
                _text.fontSize = size;
            }
        }

        /// <summary>
        /// 텍스트 정렬 설정.
        /// </summary>
        public void SetAlignment(TextAlignmentOptions alignment)
        {
            if (_text != null)
            {
                _text.alignment = alignment;
            }
        }

        /// <summary>
        /// 현재 텍스트 반환.
        /// </summary>
        public string GetText()
        {
            return _currentText;
        }

        #endregion
    }
}
