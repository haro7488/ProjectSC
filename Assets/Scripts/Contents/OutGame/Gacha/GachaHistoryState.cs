using Sc.Common.UI;

namespace Sc.Contents.Gacha
{
    /// <summary>
    /// 가챠 히스토리 화면 상태
    /// </summary>
    public class GachaHistoryState : IScreenState
    {
        /// <summary>
        /// 필터링할 풀 ID (null이면 전체)
        /// </summary>
        public string FilterPoolId { get; set; }
    }
}
