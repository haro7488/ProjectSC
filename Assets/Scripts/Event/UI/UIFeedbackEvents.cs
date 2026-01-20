using System;

namespace Sc.Event.UI
{
    #region Loading Events

    /// <summary>
    /// 로딩 진행률 업데이트 이벤트
    /// </summary>
    public readonly struct LoadingProgressEvent
    {
        /// <summary>
        /// 진행률 (0~1)
        /// </summary>
        public float Progress { get; init; }

        /// <summary>
        /// 현재 단계 이름
        /// </summary>
        public string StepName { get; init; }
    }

    /// <summary>
    /// 로딩 표시 요청 이벤트
    /// </summary>
    public readonly struct ShowLoadingEvent
    {
        /// <summary>
        /// 초기 진행률
        /// </summary>
        public float InitialProgress { get; init; }

        /// <summary>
        /// 초기 메시지
        /// </summary>
        public string Message { get; init; }
    }

    /// <summary>
    /// 로딩 숨김 요청 이벤트
    /// </summary>
    public readonly struct HideLoadingEvent
    {
    }

    #endregion

    #region Confirmation Events

    /// <summary>
    /// 확인 팝업 표시 요청 이벤트
    /// </summary>
    public readonly struct ShowConfirmationEvent
    {
        /// <summary>
        /// 팝업 제목
        /// </summary>
        public string Title { get; init; }

        /// <summary>
        /// 팝업 메시지
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// 확인 버튼 텍스트
        /// </summary>
        public string ConfirmText { get; init; }

        /// <summary>
        /// 취소 버튼 텍스트
        /// </summary>
        public string CancelText { get; init; }

        /// <summary>
        /// 취소 버튼 표시 여부
        /// </summary>
        public bool ShowCancelButton { get; init; }

        /// <summary>
        /// 확인 콜백
        /// </summary>
        public Action OnConfirm { get; init; }

        /// <summary>
        /// 취소 콜백
        /// </summary>
        public Action OnCancel { get; init; }
    }

    #endregion
}