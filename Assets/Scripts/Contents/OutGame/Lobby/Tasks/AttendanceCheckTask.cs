using Cysharp.Threading.Tasks;
using Sc.Core;
using Sc.Foundation;
using UnityEngine;

namespace Sc.Contents.Lobby
{
    /// <summary>
    /// 출석 체크 Task (Stub).
    /// 추후 실제 출석 시스템 구현 시 완성.
    /// </summary>
    public class AttendanceCheckTask : ILobbyEntryTask
    {
        public string TaskName => "출석 체크";
        public int Priority => 10;

        public UniTask<bool> CheckRequiredAsync()
        {
            // TODO[FUTURE]: 오늘 이미 출석체크했는지 확인
            // UserSaveData.LastAttendanceDate 비교
            Debug.Log("[AttendanceCheckTask] CheckRequired: Stub - 항상 스킵");
            return UniTask.FromResult(false);
        }

        public UniTask<Result<LobbyTaskResult>> ExecuteAsync()
        {
            // TODO[FUTURE]: 실제 출석 로직 구현
            // 1. 서버에 출석 요청
            // 2. 보상 수령
            // 3. UserSaveData 업데이트
            Debug.Log("[AttendanceCheckTask] Execute: Stub - 빈 결과 반환");
            return UniTask.FromResult(Result<LobbyTaskResult>.Success(LobbyTaskResult.Empty()));
        }
    }
}
