using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Common.UI
{
    /// <summary>
    /// 진행률 표시용 Widget.
    /// Slider를 읽기 전용으로 사용하여 진행 상태 표시.
    /// </summary>
    public class ProgressBarWidget : Widget
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _valueText;
        [SerializeField] private Image _fillImage;
        [SerializeField] private string _valueFormat = "{0:P0}";

        private float _currentProgress;

        public float Progress => _currentProgress;

        #region Lifecycle

        protected override void OnInitialize()
        {
            if (_slider != null)
            {
                _slider.interactable = false;
                _slider.minValue = 0f;
                _slider.maxValue = 1f;
            }
        }

        protected override void OnBind(object data)
        {
            if (data is float progress)
            {
                SetProgress(progress);
            }
        }

        protected override void OnRelease()
        {
            _currentProgress = 0f;
        }

        #endregion

        #region Public API

        /// <summary>
        /// 진행률 설정 (0~1).
        /// </summary>
        public void SetProgress(float progress)
        {
            _currentProgress = Mathf.Clamp01(progress);

            if (_slider != null)
            {
                _slider.value = _currentProgress;
            }

            UpdateValueText();
        }

        /// <summary>
        /// 진행률과 텍스트 동시 설정.
        /// </summary>
        public void SetProgressWithText(float progress, string text)
        {
            _currentProgress = Mathf.Clamp01(progress);

            if (_slider != null)
            {
                _slider.value = _currentProgress;
            }

            if (_valueText != null)
            {
                _valueText.text = text ?? string.Empty;
            }
        }

        /// <summary>
        /// 값 텍스트 포맷 설정.
        /// </summary>
        public void SetValueFormat(string format)
        {
            _valueFormat = format;
            UpdateValueText();
        }

        /// <summary>
        /// Fill 이미지 색상 설정.
        /// </summary>
        public void SetFillColor(Color color)
        {
            if (_fillImage != null)
            {
                _fillImage.color = color;
            }
        }

        /// <summary>
        /// 배경 색상 설정.
        /// </summary>
        public void SetBackgroundColor(Color color)
        {
            if (_slider != null)
            {
                var background = _slider.GetComponentInChildren<Image>();
                if (background != null && background != _fillImage)
                {
                    background.color = color;
                }
            }
        }

        /// <summary>
        /// 현재 진행률 반환.
        /// </summary>
        public float GetProgress()
        {
            return _currentProgress;
        }

        #endregion

        #region Private

        private void UpdateValueText()
        {
            if (_valueText != null && !string.IsNullOrEmpty(_valueFormat))
            {
                _valueText.text = string.Format(_valueFormat, _currentProgress);
            }
        }

        #endregion
    }
}
