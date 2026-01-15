using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sc.Common.UI
{
    /// <summary>
    /// ScrollRect를 래핑하는 Widget.
    /// 스크롤 컨테이너 기능 제공.
    /// </summary>
    public class ScrollViewWidget : Widget
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _content;

        /// <summary>
        /// 스크롤 위치 변경 시 발생하는 이벤트.
        /// </summary>
        public event Action<Vector2> OnScrollChanged;

        public Vector2 NormalizedPosition => _scrollRect != null ? _scrollRect.normalizedPosition : Vector2.zero;
        public RectTransform Content => _content;

        #region Lifecycle

        protected override void OnInitialize()
        {
            if (_scrollRect != null)
            {
                _scrollRect.onValueChanged.AddListener(HandleScrollChanged);

                if (_content == null)
                {
                    _content = _scrollRect.content;
                }
            }
        }

        protected override void OnRelease()
        {
            OnScrollChanged = null;
        }

        #endregion

        #region Public API

        /// <summary>
        /// 맨 위로 스크롤.
        /// </summary>
        public void ScrollToTop()
        {
            if (_scrollRect != null)
            {
                _scrollRect.normalizedPosition = new Vector2(
                    _scrollRect.normalizedPosition.x,
                    1f
                );
            }
        }

        /// <summary>
        /// 맨 아래로 스크롤.
        /// </summary>
        public void ScrollToBottom()
        {
            if (_scrollRect != null)
            {
                _scrollRect.normalizedPosition = new Vector2(
                    _scrollRect.normalizedPosition.x,
                    0f
                );
            }
        }

        /// <summary>
        /// 맨 왼쪽으로 스크롤.
        /// </summary>
        public void ScrollToLeft()
        {
            if (_scrollRect != null)
            {
                _scrollRect.normalizedPosition = new Vector2(
                    0f,
                    _scrollRect.normalizedPosition.y
                );
            }
        }

        /// <summary>
        /// 맨 오른쪽으로 스크롤.
        /// </summary>
        public void ScrollToRight()
        {
            if (_scrollRect != null)
            {
                _scrollRect.normalizedPosition = new Vector2(
                    1f,
                    _scrollRect.normalizedPosition.y
                );
            }
        }

        /// <summary>
        /// 특정 위치로 스크롤 (정규화된 값 0~1).
        /// </summary>
        public void ScrollTo(Vector2 normalizedPosition)
        {
            if (_scrollRect != null)
            {
                _scrollRect.normalizedPosition = new Vector2(
                    Mathf.Clamp01(normalizedPosition.x),
                    Mathf.Clamp01(normalizedPosition.y)
                );
            }
        }

        /// <summary>
        /// 수직 위치로 스크롤 (0=아래, 1=위).
        /// </summary>
        public void ScrollToVertical(float normalizedY)
        {
            if (_scrollRect != null)
            {
                _scrollRect.normalizedPosition = new Vector2(
                    _scrollRect.normalizedPosition.x,
                    Mathf.Clamp01(normalizedY)
                );
            }
        }

        /// <summary>
        /// 수평 위치로 스크롤 (0=왼쪽, 1=오른쪽).
        /// </summary>
        public void ScrollToHorizontal(float normalizedX)
        {
            if (_scrollRect != null)
            {
                _scrollRect.normalizedPosition = new Vector2(
                    Mathf.Clamp01(normalizedX),
                    _scrollRect.normalizedPosition.y
                );
            }
        }

        /// <summary>
        /// 스크롤 활성화/비활성화.
        /// </summary>
        public void SetScrollEnabled(bool horizontal, bool vertical)
        {
            if (_scrollRect != null)
            {
                _scrollRect.horizontal = horizontal;
                _scrollRect.vertical = vertical;
            }
        }

        /// <summary>
        /// 관성 설정.
        /// </summary>
        public void SetInertia(bool enabled, float decelerationRate = 0.135f)
        {
            if (_scrollRect != null)
            {
                _scrollRect.inertia = enabled;
                _scrollRect.decelerationRate = decelerationRate;
            }
        }

        /// <summary>
        /// 콘텐츠 크기 반환.
        /// </summary>
        public Vector2 GetContentSize()
        {
            return _content != null ? _content.sizeDelta : Vector2.zero;
        }

        /// <summary>
        /// 뷰포트 크기 반환.
        /// </summary>
        public Vector2 GetViewportSize()
        {
            if (_scrollRect != null && _scrollRect.viewport != null)
            {
                return _scrollRect.viewport.rect.size;
            }
            return Vector2.zero;
        }

        /// <summary>
        /// 현재 정규화된 스크롤 위치 반환.
        /// </summary>
        public Vector2 GetNormalizedPosition()
        {
            return _scrollRect != null ? _scrollRect.normalizedPosition : Vector2.zero;
        }

        #endregion

        #region Private

        private void HandleScrollChanged(Vector2 position)
        {
            OnScrollChanged?.Invoke(position);
        }

        #endregion
    }
}
