using Cysharp.Threading.Tasks;
using Sc.Data;

namespace Sc.Core
{
    /// <summary>
    /// 팝업 큐잉 서비스 인터페이스.
    /// 순환 참조 방지를 위해 Core에 정의.
    /// </summary>
    public interface IPopupQueueService
    {
        /// <summary>대기 중인 팝업 수</summary>
        int PendingCount { get; }

        /// <summary>처리 중 여부</summary>
        bool IsProcessing { get; }

        /// <summary>보상 팝업 큐에 추가</summary>
        void EnqueueReward(string title, RewardInfo[] rewards);

        /// <summary>알림 팝업 큐에 추가</summary>
        void EnqueueNotification(string title, string message);

        /// <summary>큐의 모든 팝업 순차 표시</summary>
        UniTask ProcessQueueAsync();

        /// <summary>대기 중인 팝업 모두 제거</summary>
        void Clear();
    }
}
