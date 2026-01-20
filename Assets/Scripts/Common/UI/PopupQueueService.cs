using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sc.Core;
using Sc.Data;
using UnityEngine;

namespace Sc.Common.UI
{
    /// <summary>
    /// 팝업 순차 표시 서비스.
    /// 여러 시스템에서 재사용 가능 (로비, 상점, 가챠 등).
    /// </summary>
    public class PopupQueueService : IPopupQueueService
    {
        private readonly Queue<IPopupRequest> _queue = new();
        private bool _isProcessing;

        /// <summary>대기 중인 팝업 수</summary>
        public int PendingCount => _queue.Count;

        /// <summary>처리 중 여부</summary>
        public bool IsProcessing => _isProcessing;

        /// <summary>
        /// 보상 팝업 큐에 추가
        /// </summary>
        public void EnqueueReward(string title, RewardInfo[] rewards)
        {
            if (rewards == null || rewards.Length == 0)
            {
                Debug.LogWarning("[PopupQueueService] 빈 보상 배열, 스킵");
                return;
            }

            _queue.Enqueue(new RewardPopupRequest
            {
                Title = title,
                Rewards = rewards
            });

            Debug.Log($"[PopupQueueService] 보상 팝업 큐 추가: {title} ({rewards.Length}개 보상)");
        }

        /// <summary>
        /// 알림 팝업 큐에 추가
        /// </summary>
        public void EnqueueNotification(string title, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogWarning("[PopupQueueService] 빈 메시지, 스킵");
                return;
            }

            _queue.Enqueue(new NotificationPopupRequest
            {
                Title = title,
                Message = message
            });

            Debug.Log($"[PopupQueueService] 알림 팝업 큐 추가: {title}");
        }

        /// <summary>
        /// 큐의 모든 팝업 순차 표시
        /// </summary>
        public async UniTask ProcessQueueAsync()
        {
            if (_isProcessing)
            {
                Debug.LogWarning("[PopupQueueService] 이미 처리 중");
                return;
            }

            if (_queue.Count == 0)
            {
                Debug.Log("[PopupQueueService] 큐 비어있음");
                return;
            }

            _isProcessing = true;

            try
            {
                while (_queue.Count > 0)
                {
                    var request = _queue.Dequeue();
                    await ShowPopupAsync(request);
                }
            }
            finally
            {
                _isProcessing = false;
            }

            Debug.Log("[PopupQueueService] 모든 팝업 처리 완료");
        }

        /// <summary>
        /// 대기 중인 팝업 모두 제거
        /// </summary>
        public void Clear()
        {
            _queue.Clear();
            Debug.Log("[PopupQueueService] 큐 초기화");
        }

        private async UniTask ShowPopupAsync(IPopupRequest request)
        {
            var tcs = new UniTaskCompletionSource();

            switch (request)
            {
                case RewardPopupRequest rewardRequest:
                    ShowRewardPopup(rewardRequest, tcs);
                    break;

                case NotificationPopupRequest notificationRequest:
                    ShowNotificationPopup(notificationRequest, tcs);
                    break;

                default:
                    Debug.LogWarning($"[PopupQueueService] 알 수 없는 팝업 타입: {request.GetType()}");
                    tcs.TrySetResult();
                    break;
            }

            await tcs.Task;
        }

        private void ShowRewardPopup(RewardPopupRequest request, UniTaskCompletionSource tcs)
        {
            var state = new RewardPopup.State
            {
                Title = request.Title,
                Rewards = request.Rewards,
                OnClose = () => tcs.TrySetResult()
            };

            RewardPopup.Open(state);
        }

        private void ShowNotificationPopup(NotificationPopupRequest request, UniTaskCompletionSource tcs)
        {
            var state = new ConfirmState
            {
                Title = request.Title,
                Message = request.Message,
                ConfirmText = "확인",
                ShowCancelButton = false,
                OnConfirm = () => tcs.TrySetResult()
            };

            ConfirmPopup.Open(state);
        }
    }

    #region Internal Request Types

    /// <summary>
    /// 팝업 요청 인터페이스 (내부용)
    /// </summary>
    internal interface IPopupRequest { }

    /// <summary>
    /// 보상 팝업 요청
    /// </summary>
    internal class RewardPopupRequest : IPopupRequest
    {
        public string Title { get; init; }
        public RewardInfo[] Rewards { get; init; }
    }

    /// <summary>
    /// 알림 팝업 요청
    /// </summary>
    internal class NotificationPopupRequest : IPopupRequest
    {
        public string Title { get; init; }
        public string Message { get; init; }
    }

    #endregion
}
