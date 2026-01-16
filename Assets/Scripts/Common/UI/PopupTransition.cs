using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Sc.Common.UI
{
    /// <summary>
    /// Popup 전용 전환 애니메이션 베이스 클래스.
    /// </summary>
    public abstract class PopupTransition
    {
        public PopupWidget Target { get; set; }

        public abstract float Duration { get; }

        public abstract UniTask Show();
        public abstract UniTask Hide();

        public static PopupTransition Default => new PopupFadeTransition();
    }

    /// <summary>
    /// Popup 페이드 전환.
    /// </summary>
    public class PopupFadeTransition : PopupTransition
    {
        private readonly float _duration;
        private readonly Ease _ease;

        public override float Duration => _duration;

        public PopupFadeTransition(float duration = 0.25f, Ease ease = Ease.OutQuad)
        {
            _duration = duration;
            _ease = ease;
        }

        public override async UniTask Show()
        {
            if (Target == null) return;

            var canvasGroup = Target.GetOrAddCanvasGroup();
            canvasGroup.alpha = 0f;
            Target.Show();

            await canvasGroup
                .DOFade(1f, _duration)
                .SetEase(_ease)
                .SetUpdate(true)
                .ToUniTask();
        }

        public override async UniTask Hide()
        {
            if (Target == null) return;

            var canvasGroup = Target.GetOrAddCanvasGroup();
            canvasGroup.alpha = 1f;

            await canvasGroup
                .DOFade(0f, _duration)
                .SetEase(Ease.InQuad)
                .SetUpdate(true)
                .ToUniTask();

            Target.Hide();
        }
    }

    /// <summary>
    /// Popup 스케일 + 페이드 전환 (팝업 특유의 효과).
    /// </summary>
    public class PopupScaleTransition : PopupTransition
    {
        private readonly float _duration;
        private readonly float _fromScale;
        private readonly Ease _ease;

        public override float Duration => _duration;

        public PopupScaleTransition(float duration = 0.25f, float fromScale = 0.8f, Ease ease = Ease.OutBack)
        {
            _duration = duration;
            _fromScale = fromScale;
            _ease = ease;
        }

        public override async UniTask Show()
        {
            if (Target == null) return;

            var canvasGroup = Target.GetOrAddCanvasGroup();
            var transform = Target.transform;

            canvasGroup.alpha = 0f;
            transform.localScale = Vector3.one * _fromScale;
            Target.Show();

            await UniTask.WhenAll(
                canvasGroup
                    .DOFade(1f, _duration)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true)
                    .ToUniTask(),
                transform
                    .DOScale(1f, _duration)
                    .SetEase(_ease)
                    .SetUpdate(true)
                    .ToUniTask()
            );
        }

        public override async UniTask Hide()
        {
            if (Target == null) return;

            var canvasGroup = Target.GetOrAddCanvasGroup();
            var transform = Target.transform;

            canvasGroup.alpha = 1f;
            transform.localScale = Vector3.one;

            await UniTask.WhenAll(
                canvasGroup
                    .DOFade(0f, _duration * 0.8f)
                    .SetEase(Ease.InQuad)
                    .SetUpdate(true)
                    .ToUniTask(),
                transform
                    .DOScale(_fromScale, _duration * 0.8f)
                    .SetEase(Ease.InBack)
                    .SetUpdate(true)
                    .ToUniTask()
            );

            Target.Hide();
            transform.localScale = Vector3.one; // 복원
        }
    }

    /// <summary>
    /// Popup 슬라이드 전환 (아래에서 위로).
    /// </summary>
    public class PopupSlideTransition : PopupTransition
    {
        private readonly float _duration;
        private readonly Ease _ease;

        public override float Duration => _duration;

        public PopupSlideTransition(float duration = 0.3f, Ease ease = Ease.OutQuad)
        {
            _duration = duration;
            _ease = ease;
        }

        public override async UniTask Show()
        {
            if (Target == null) return;

            var rectTransform = Target.GetComponent<RectTransform>();
            if (rectTransform == null) return;

            var canvasGroup = Target.GetOrAddCanvasGroup();
            var endPos = rectTransform.anchoredPosition;
            var startPos = new Vector2(endPos.x, endPos.y - Screen.height * 0.3f);

            canvasGroup.alpha = 0f;
            rectTransform.anchoredPosition = startPos;
            Target.Show();

            await UniTask.WhenAll(
                canvasGroup
                    .DOFade(1f, _duration * 0.5f)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true)
                    .ToUniTask(),
                rectTransform
                    .DOAnchorPos(endPos, _duration)
                    .SetEase(_ease)
                    .SetUpdate(true)
                    .ToUniTask()
            );
        }

        public override async UniTask Hide()
        {
            if (Target == null) return;

            var rectTransform = Target.GetComponent<RectTransform>();
            if (rectTransform == null) return;

            var canvasGroup = Target.GetOrAddCanvasGroup();
            var startPos = rectTransform.anchoredPosition;
            var endPos = new Vector2(startPos.x, startPos.y - Screen.height * 0.3f);

            await UniTask.WhenAll(
                canvasGroup
                    .DOFade(0f, _duration * 0.5f)
                    .SetDelay(_duration * 0.5f)
                    .SetEase(Ease.InQuad)
                    .SetUpdate(true)
                    .ToUniTask(),
                rectTransform
                    .DOAnchorPos(endPos, _duration)
                    .SetEase(Ease.InQuad)
                    .SetUpdate(true)
                    .ToUniTask()
            );

            Target.Hide();
            rectTransform.anchoredPosition = startPos; // 복원
        }
    }

    /// <summary>
    /// Popup 즉시 전환 (애니메이션 없음).
    /// </summary>
    public class EmptyPopupTransition : PopupTransition
    {
        public override float Duration => 0f;

        public override UniTask Show()
        {
            Target?.Show();
            return UniTask.CompletedTask;
        }

        public override UniTask Hide()
        {
            Target?.Hide();
            return UniTask.CompletedTask;
        }
    }
}
