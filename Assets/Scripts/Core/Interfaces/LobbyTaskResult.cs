using System;
using Sc.Data;

namespace Sc.Core
{
    /// <summary>
    /// 로비 진입 Task 실행 결과.
    /// 팝업 표시 여부 및 보상 정보 포함.
    /// </summary>
    public class LobbyTaskResult
    {
        /// <summary>팝업 표시 여부</summary>
        public bool ShouldShowPopup { get; init; }

        /// <summary>팝업 타입 (Reward / Notification)</summary>
        public PopupType PopupType { get; init; }

        /// <summary>보상 정보 (RewardPopup용)</summary>
        public RewardInfo[] Rewards { get; init; }

        /// <summary>팝업 제목</summary>
        public string PopupTitle { get; init; }

        /// <summary>팝업 메시지 (알림용)</summary>
        public string PopupMessage { get; init; }

        /// <summary>
        /// 보상 팝업 결과 생성
        /// </summary>
        public static LobbyTaskResult WithRewards(string title, RewardInfo[] rewards)
        {
            if (rewards == null || rewards.Length == 0)
            {
                return Empty();
            }

            return new LobbyTaskResult
            {
                ShouldShowPopup = true,
                PopupType = PopupType.Reward,
                PopupTitle = title,
                Rewards = rewards
            };
        }

        /// <summary>
        /// 알림 팝업 결과 생성
        /// </summary>
        public static LobbyTaskResult WithNotification(string title, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return Empty();
            }

            return new LobbyTaskResult
            {
                ShouldShowPopup = true,
                PopupType = PopupType.Notification,
                PopupTitle = title,
                PopupMessage = message
            };
        }

        /// <summary>
        /// 팝업 없는 빈 결과 생성
        /// </summary>
        public static LobbyTaskResult Empty()
        {
            return new LobbyTaskResult
            {
                ShouldShowPopup = false
            };
        }
    }

    /// <summary>
    /// 팝업 타입
    /// </summary>
    public enum PopupType
    {
        None,
        Reward,
        Notification
    }
}
