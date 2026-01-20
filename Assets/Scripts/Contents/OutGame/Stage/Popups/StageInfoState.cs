using System;
using Sc.Common.UI;
using Sc.Data;

namespace Sc.Contents.Stage
{
    /// <summary>
    /// StageInfoPopup 상태.
    /// 스테이지 상세 정보, 유저 진행 상황, 콜백을 포함.
    /// </summary>
    public class StageInfoState : IPopupState
    {
        /// <summary>
        /// 스테이지 마스터 데이터
        /// </summary>
        public StageData StageData { get; set; }

        /// <summary>
        /// 유저의 스테이지 클리어 정보 (null이면 미클리어)
        /// </summary>
        public StageClearInfo? ClearInfo { get; set; }

        /// <summary>
        /// 입장 가능 여부
        /// </summary>
        public bool CanEnter { get; set; } = true;

        /// <summary>
        /// 남은 입장 횟수 (-1이면 무제한)
        /// </summary>
        public int RemainingEntryCount { get; set; } = -1;

        /// <summary>
        /// 최대 입장 횟수 (-1이면 무제한)
        /// </summary>
        public int MaxEntryCount { get; set; } = -1;

        /// <summary>
        /// 입장 버튼 클릭 시 콜백
        /// </summary>
        public Action OnEnter { get; set; }

        /// <summary>
        /// 닫기 버튼 클릭 시 콜백
        /// </summary>
        public Action OnClose { get; set; }

        /// <summary>
        /// 배경 터치로 닫기 허용
        /// </summary>
        public bool AllowBackgroundDismiss => true;
    }
}