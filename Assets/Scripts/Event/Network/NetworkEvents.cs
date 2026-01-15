namespace Sc.Event.Network
{
    /// <summary>
    /// 네트워크 연결 완료 이벤트
    /// </summary>
    public readonly struct NetworkConnectedEvent { }

    /// <summary>
    /// 네트워크 연결 해제 이벤트
    /// </summary>
    public readonly struct NetworkDisconnectedEvent
    {
        /// <summary>
        /// 연결 해제 사유
        /// </summary>
        public string Reason { get; init; }

        /// <summary>
        /// 의도된 연결 해제 여부
        /// </summary>
        public bool IsIntentional { get; init; }
    }

    /// <summary>
    /// 네트워크 재연결 시도 이벤트
    /// </summary>
    public readonly struct NetworkReconnectingEvent
    {
        /// <summary>
        /// 재연결 시도 횟수
        /// </summary>
        public int AttemptCount { get; init; }

        /// <summary>
        /// 최대 재연결 시도 횟수
        /// </summary>
        public int MaxAttempts { get; init; }
    }

    /// <summary>
    /// 요청 시작 이벤트 (로딩 UI용)
    /// </summary>
    public readonly struct RequestStartedEvent
    {
        /// <summary>
        /// 요청 ID
        /// </summary>
        public string RequestId { get; init; }

        /// <summary>
        /// 요청 타입 이름 (예: "Login", "Gacha")
        /// </summary>
        public string RequestType { get; init; }
    }

    /// <summary>
    /// 요청 완료 이벤트
    /// </summary>
    public readonly struct RequestCompletedEvent
    {
        /// <summary>
        /// 요청 ID
        /// </summary>
        public string RequestId { get; init; }

        /// <summary>
        /// 요청 타입 이름
        /// </summary>
        public string RequestType { get; init; }

        /// <summary>
        /// 성공 여부
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// 에러 코드 (성공 시 0)
        /// </summary>
        public int ErrorCode { get; init; }

        /// <summary>
        /// 에러 메시지
        /// </summary>
        public string ErrorMessage { get; init; }
    }

    /// <summary>
    /// 네트워크 에러 이벤트
    /// </summary>
    public readonly struct NetworkErrorEvent
    {
        /// <summary>
        /// 요청 ID
        /// </summary>
        public string RequestId { get; init; }

        /// <summary>
        /// 요청 타입 이름
        /// </summary>
        public string RequestType { get; init; }

        /// <summary>
        /// 에러 코드
        /// </summary>
        public int ErrorCode { get; init; }

        /// <summary>
        /// 에러 메시지
        /// </summary>
        public string ErrorMessage { get; init; }

        /// <summary>
        /// 복구 가능 여부
        /// </summary>
        public bool IsRecoverable { get; init; }
    }

    /// <summary>
    /// 서버 푸시 수신 이벤트
    /// </summary>
    public readonly struct ServerPushReceivedEvent
    {
        /// <summary>
        /// 푸시 메시지 타입
        /// </summary>
        public Sc.Packet.PushMessageType Type { get; init; }

        /// <summary>
        /// JSON 페이로드
        /// </summary>
        public string Payload { get; init; }
    }
}
