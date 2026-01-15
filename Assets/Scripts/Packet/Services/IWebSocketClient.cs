using System;
using Cysharp.Threading.Tasks;

namespace Sc.Packet
{
    /// <summary>
    /// WebSocket 클라이언트 인터페이스
    /// 실시간 양방향 통신 (서버 푸시)
    /// </summary>
    public interface IWebSocketClient
    {
        /// <summary>
        /// 서버 연결
        /// </summary>
        /// <param name="url">WebSocket 서버 URL</param>
        UniTask ConnectAsync(string url);

        /// <summary>
        /// 연결 해제
        /// </summary>
        UniTask DisconnectAsync();

        /// <summary>
        /// 메시지 전송
        /// </summary>
        /// <param name="message">전송할 메시지</param>
        void Send(string message);

        /// <summary>
        /// 연결 상태
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 연결 상태 상세
        /// </summary>
        WebSocketState State { get; }

        /// <summary>
        /// 서버 메시지 수신 이벤트
        /// </summary>
        event Action<string> OnMessageReceived;

        /// <summary>
        /// 연결 완료 이벤트
        /// </summary>
        event Action OnConnected;

        /// <summary>
        /// 연결 해제 이벤트
        /// </summary>
        event Action<string> OnDisconnected;

        /// <summary>
        /// 에러 발생 이벤트
        /// </summary>
        event Action<Exception> OnError;
    }

    /// <summary>
    /// WebSocket 연결 상태
    /// </summary>
    public enum WebSocketState
    {
        Disconnected,
        Connecting,
        Connected,
        Reconnecting,
        Error
    }

    /// <summary>
    /// 서버 푸시 메시지 타입
    /// </summary>
    public enum PushMessageType
    {
        /// <summary>
        /// 메일 수신
        /// </summary>
        Mail,

        /// <summary>
        /// 점검 알림
        /// </summary>
        Maintenance,

        /// <summary>
        /// 이벤트 배너 갱신
        /// </summary>
        EventBanner,

        /// <summary>
        /// 친구 요청
        /// </summary>
        FriendRequest,

        /// <summary>
        /// 길드 알림
        /// </summary>
        GuildNotice,

        /// <summary>
        /// 실시간 PvP 매칭
        /// </summary>
        PvpMatch,

        /// <summary>
        /// 기타
        /// </summary>
        Other
    }

    /// <summary>
    /// 서버 푸시 메시지
    /// </summary>
    [Serializable]
    public class PushMessage
    {
        /// <summary>
        /// 메시지 타입
        /// </summary>
        public PushMessageType Type;

        /// <summary>
        /// JSON 페이로드
        /// </summary>
        public string Payload;

        /// <summary>
        /// 서버 타임스탬프
        /// </summary>
        public long Timestamp;
    }
}
