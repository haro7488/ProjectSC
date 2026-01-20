namespace Sc.Event
{
    /// <summary>
    /// 로비 진입 후처리 Task 전체 완료 이벤트
    /// </summary>
    public readonly struct LobbyEntryTasksCompletedEvent
    {
        /// <summary>전체 Task 수</summary>
        public int TotalTasks { get; init; }

        /// <summary>실행된 Task 수</summary>
        public int ExecutedTasks { get; init; }

        /// <summary>실패한 Task 수</summary>
        public int FailedTasks { get; init; }
    }

    /// <summary>
    /// 개별 Task 완료 이벤트 (로깅/분석용)
    /// </summary>
    public readonly struct LobbyEntryTaskCompletedEvent
    {
        /// <summary>Task 이름</summary>
        public string TaskName { get; init; }

        /// <summary>성공 여부</summary>
        public bool IsSuccess { get; init; }

        /// <summary>실행 시간 (ms)</summary>
        public long ExecutionTimeMs { get; init; }
    }
}
