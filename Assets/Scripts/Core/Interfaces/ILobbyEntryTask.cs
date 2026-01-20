using Cysharp.Threading.Tasks;
using Sc.Foundation;

namespace Sc.Core
{
    /// <summary>
    /// 로비 진입 후 자동 실행되는 Task 인터페이스.
    /// 출석체크, 이벤트 재화 전환, 신규 이벤트 알림 등.
    /// </summary>
    public interface ILobbyEntryTask
    {
        /// <summary>Task 이름 (로깅용)</summary>
        string TaskName { get; }

        /// <summary>실행 우선순위 (낮을수록 먼저 실행)</summary>
        int Priority { get; }

        /// <summary>
        /// 이 Task가 실행 필요한지 체크.
        /// false 반환 시 스킵 (예: 오늘 이미 출석체크 완료)
        /// </summary>
        UniTask<bool> CheckRequiredAsync();

        /// <summary>
        /// Task 실행.
        /// 실패해도 다음 Task는 계속 실행됨.
        /// </summary>
        UniTask<Result<LobbyTaskResult>> ExecuteAsync();
    }
}
