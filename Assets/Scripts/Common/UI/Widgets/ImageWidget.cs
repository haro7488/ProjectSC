using UnityEngine;
using UnityEngine.UI;

namespace Sc.Common.UI
{
    /// <summary>
    /// Image를 래핑하는 Widget.
    /// Sprite 표시 및 색상/투명도 조절 기능 제공.
    /// </summary>
    public class ImageWidget : Widget
    {
        [SerializeField] private Image _image;

        private Sprite _currentSprite;

        public Sprite Sprite => _currentSprite;

        #region Lifecycle

        protected override void OnInitialize()
        {
            if (_image != null)
            {
                _currentSprite = _image.sprite;
            }
        }

        protected override void OnBind(object data)
        {
            if (data is Sprite sprite)
            {
                SetSprite(sprite);
            }
        }

        protected override void OnRelease()
        {
            _currentSprite = null;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Sprite 설정.
        /// </summary>
        public void SetSprite(Sprite sprite)
        {
            _currentSprite = sprite;
            if (_image != null)
            {
                _image.sprite = _currentSprite;
            }
        }

        /// <summary>
        /// 이미지 색상 설정.
        /// </summary>
        public void SetColor(Color color)
        {
            if (_image != null)
            {
                _image.color = color;
            }
        }

        /// <summary>
        /// 이미지 투명도 설정 (0~1).
        /// </summary>
        public void SetAlpha(float alpha)
        {
            if (_image != null)
            {
                var color = _image.color;
                color.a = Mathf.Clamp01(alpha);
                _image.color = color;
            }
        }

        /// <summary>
        /// Sprite의 원본 크기로 설정.
        /// </summary>
        public void SetNativeSize()
        {
            if (_image != null)
            {
                _image.SetNativeSize();
            }
        }

        /// <summary>
        /// Fill Amount 설정 (Filled 타입일 때).
        /// </summary>
        public void SetFillAmount(float amount)
        {
            if (_image != null)
            {
                _image.fillAmount = Mathf.Clamp01(amount);
            }
        }

        /// <summary>
        /// 이미지 활성화/비활성화.
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            if (_image != null)
            {
                _image.enabled = enabled;
            }
        }

        #endregion
    }
}
