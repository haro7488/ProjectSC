using UnityEngine;

namespace Sc.Core
{
    /// <summary>
    /// 네트워크 설정 ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "NetworkConfig", menuName = "SC/Network Config")]
    public class NetworkConfig : ScriptableObject
    {
        [Header("Mode")]
        [Tooltip("네트워크 모드 (Local: 로컬 시뮬레이션, Server: 실제 서버)")]
        public NetworkMode Mode = NetworkMode.Local;

        [Header("Server Settings")]
        [Tooltip("REST API 서버 URL")]
        public string BaseUrl = "https://api.example.com";

        [Tooltip("WebSocket 서버 URL")]
        public string WebSocketUrl = "wss://ws.example.com";

        [Header("Timeouts")]
        [Tooltip("요청 타임아웃 (밀리초)")]
        public int RequestTimeoutMs = 30000;

        [Tooltip("연결 타임아웃 (밀리초)")]
        public int ConnectionTimeoutMs = 10000;

        [Header("Retry")]
        [Tooltip("자동 재연결 시도 횟수")]
        public int MaxReconnectAttempts = 3;

        [Tooltip("재연결 간격 (밀리초)")]
        public int ReconnectIntervalMs = 2000;

        [Header("Local Simulation")]
        [Tooltip("시뮬레이션 지연 시간 (밀리초)")]
        public int SimulatedLatencyMs = 100;

        [Tooltip("시뮬레이션 실패 확률 (0-1)")]
        [Range(0f, 1f)]
        public float SimulatedFailureRate = 0f;
    }

    /// <summary>
    /// 네트워크 모드
    /// </summary>
    public enum NetworkMode
    {
        /// <summary>
        /// 로컬 시뮬레이션 (서버 없이 테스트)
        /// </summary>
        Local,

        /// <summary>
        /// 실제 서버 연결
        /// </summary>
        Server
    }

    /// <summary>
    /// 네트워크 연결 상태
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// 연결 안됨
        /// </summary>
        Disconnected,

        /// <summary>
        /// 연결 중
        /// </summary>
        Connecting,

        /// <summary>
        /// 연결됨
        /// </summary>
        Connected,

        /// <summary>
        /// 재연결 중
        /// </summary>
        Reconnecting,

        /// <summary>
        /// 에러
        /// </summary>
        Error
    }
}
