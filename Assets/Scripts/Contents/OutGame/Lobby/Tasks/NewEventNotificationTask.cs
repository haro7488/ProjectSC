using Cysharp.Threading.Tasks;
using Sc.Core;
using Sc.Foundation;
using UnityEngine;

namespace Sc.Contents.Lobby
{
    /// <summary>
    /// 신규 이벤트 알림 Task (Stub).
    /// 마지막 로그인 이후 시작된 새 이벤트 알림.
    /// </summary>
    public class NewEventNotificationTask : ILobbyEntryTask
    {
        public string TaskName => "신규 이벤트 알림";
        public int Priority => 30;

        public UniTask<bool> CheckRequiredAsync()
        {
            // TODO: 마지막 로그인 이후 시작된 새 이벤트 있는지 확인
            Debug.Log("[NewEventNotificationTask] CheckRequired: Stub - 항상 스킵");
            return UniTask.FromResult(false);
        }

        public UniTask<Result<LobbyTaskResult>> ExecuteAsync()
        {
            // TODO: 새 이벤트 목록 조회 및 알림
            Debug.Log("[NewEventNotificationTask] Execute: Stub - 빈 결과 반환");
            return UniTask.FromResult(Result<LobbyTaskResult>.Success(LobbyTaskResult.Empty()));
        }
    }
}
