using Sc.Common.UI;
using Sc.Data;

namespace Sc.Contents.Gacha
{
    /// <summary>
    /// 확률 상세 팝업 상태
    /// </summary>
    public class RateDetailState : IPopupState
    {
        /// <summary>
        /// 가챠 풀 마스터 데이터
        /// </summary>
        public GachaPoolData PoolData { get; set; }
    }
}
