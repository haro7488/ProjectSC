using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Sc.Common.UI
{
    /// <summary>
    /// Screen 전환 애니메이션 베이스 클래스.
    /// </summary>
    public abstract class Transition
    {
        public ScreenWidget OutScreen { get; set; }
        public ScreenWidget InScreen { get; set; }

        public abstract float Duration { get; }

        public abstract UniTask Out();
        public abstract UniTask In();

        /// <summary>
        /// CrossFade - Out/In 동시 실행.
        /// </summary>
        public virtual async UniTask CrossFade()
        {
            await UniTask.WhenAll(Out(), In());
        }

        public static Transition Default => new FadeTransition();
    }

    /// <summary>
    /// 페이드 인/아웃 전환.
    /// </summary>
    public class FadeTransition : Transition
    {
        private readonly float _duration;
        private readonly Ease _ease;

        public override float Duration => _duration;

        public FadeTransition(float duration = 0.3f, Ease ease = Ease.InOutQuad)
        {
            _duration = duration;
            _ease = ease;
        }

        public override async UniTask Out()
        {
            if (OutScreen == null) return;

            var canvasGroup = OutScreen.GetOrAddCanvasGroup();
            canvasGroup.alpha = 1f;

            await canvasGroup
                .DOFade(0f, _duration)
                .SetEase(_ease)
                .SetUpdate(true) // TimeScale 무시
                .ToUniTask();

            // Tween 완료 후 객체가 파괴되었을 수 있음 (테스트 환경 등)
            if (OutScreen != null)
                OutScreen.Hide();
        }

        public override async UniTask In()
        {
            if (InScreen == null) return;

            var canvasGroup = InScreen.GetOrAddCanvasGroup();
            canvasGroup.alpha = 0f;

            // CrossFade 시 InScreen은 먼저 보여야 함
            InScreen.Show();

            await canvasGroup
                .DOFade(1f, _duration)
                .SetEase(_ease)
                .SetUpdate(true)
                .ToUniTask();
        }
    }

    /// <summary>
    /// 슬라이드 전환 (오른쪽에서 들어오고, 왼쪽으로 나감).
    /// </summary>
    public class SlideTransition : Transition
    {
        private readonly float _duration;
        private readonly Ease _ease;

        public override float Duration => _duration;

        public SlideTransition(float duration = 0.3f, Ease ease = Ease.OutQuad)
        {
            _duration = duration;
            _ease = ease;
        }

        public override async UniTask Out()
        {
            if (OutScreen == null) return;

            var rectTransform = OutScreen.GetComponent<RectTransform>();
            if (rectTransform == null) return;

            var screenWidth = Screen.width;
            var startPos = rectTransform.anchoredPosition;
            var endPos = new Vector2(-screenWidth, startPos.y);

            await rectTransform
                .DOAnchorPos(endPos, _duration)
                .SetEase(_ease)
                .SetUpdate(true)
                .ToUniTask();

            // Tween 완료 후 객체가 파괴되었을 수 있음 (테스트 환경 등)
            if (OutScreen != null)
            {
                OutScreen.Hide();
                rectTransform.anchoredPosition = startPos; // 위치 복원
            }
        }

        public override async UniTask In()
        {
            if (InScreen == null) return;

            var rectTransform = InScreen.GetComponent<RectTransform>();
            if (rectTransform == null) return;

            var screenWidth = Screen.width;
            var endPos = rectTransform.anchoredPosition;
            var startPos = new Vector2(screenWidth, endPos.y);

            rectTransform.anchoredPosition = startPos;
            InScreen.Show();

            await rectTransform
                .DOAnchorPos(endPos, _duration)
                .SetEase(_ease)
                .SetUpdate(true)
                .ToUniTask();
        }
    }

    /// <summary>
    /// 즉시 전환 (애니메이션 없음).
    /// </summary>
    public class EmptyTransition : Transition
    {
        public override float Duration => 0f;

        public override UniTask Out()
        {
            OutScreen?.Hide();
            return UniTask.CompletedTask;
        }

        public override UniTask In()
        {
            InScreen?.Show();
            return UniTask.CompletedTask;
        }
    }
}
