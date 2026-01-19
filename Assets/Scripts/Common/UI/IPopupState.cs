namespace Sc.Common.UI
{
    /// <summary>
    /// Popup 상태를 나타내는 마커 인터페이스.
    /// State는 class로 구현.
    /// </summary>
    public interface IPopupState
    {
        /// <summary>
        /// 배경 터치로 Popup 닫기 허용 여부.
        /// 기본값 true. false로 오버라이드하면 배경 터치 무시.
        /// </summary>
        bool AllowBackgroundDismiss => true;
    }
}
