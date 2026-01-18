using System;
using Sc.Data;

namespace Sc.Core
{
    /// <summary>
    /// 시간 서비스 인터페이스.
    /// 서버 시간 기준으로 설계, 로컬 구현 시 클라이언트 시간 사용.
    /// 서버 전환 시 구현체만 교체하면 됨.
    /// </summary>
    public interface ITimeService
    {
        /// <summary>
        /// 현재 서버 시간 (UTC Unix timestamp, 초 단위)
        /// - 로컬: 클라이언트 UTC 시간 반환
        /// - 서버: 동기화된 서버 시간 반환
        /// </summary>
        long ServerTimeUtc { get; }

        /// <summary>
        /// 현재 서버 시간 (DateTime UTC)
        /// </summary>
        DateTime ServerDateTime { get; }

        /// <summary>
        /// 클라이언트-서버 시간 오프셋 (초 단위)
        /// - 로컬: 항상 0
        /// - 서버: 서버시간 - 클라이언트시간
        /// </summary>
        long TimeOffset { get; }

        /// <summary>
        /// 서버 시간 동기화.
        /// 서버 응답의 ServerTime으로 호출하여 오프셋 보정.
        /// - 로컬: 무시 또는 오프셋만 계산
        /// - 서버: 오프셋 저장하여 이후 시간 계산에 반영
        /// </summary>
        /// <param name="serverTimestamp">서버에서 받은 UTC timestamp</param>
        void SyncServerTime(long serverTimestamp);

        /// <summary>
        /// 다음 리셋 시간 계산 (UTC Unix timestamp)
        /// </summary>
        /// <param name="limitType">리셋 타입</param>
        /// <returns>다음 리셋 시각의 Unix timestamp (None/Permanent는 0 반환)</returns>
        long GetNextResetTime(LimitType limitType);

        /// <summary>
        /// 마지막 기록 이후 리셋이 발생했는지 확인.
        /// 예: 일일 상점 구매 기록의 리셋 여부 판단
        /// </summary>
        /// <param name="lastTimestamp">마지막 기록 시각 (UTC Unix timestamp)</param>
        /// <param name="limitType">리셋 타입</param>
        /// <returns>리셋 발생 여부</returns>
        bool HasResetOccurred(long lastTimestamp, LimitType limitType);

        /// <summary>
        /// 현재 시간이 지정된 기간 내인지 확인.
        /// 이벤트 기간, 배너 기간 등에 사용.
        /// </summary>
        /// <param name="startTime">시작 시각 (UTC Unix timestamp)</param>
        /// <param name="endTime">종료 시각 (UTC Unix timestamp)</param>
        /// <returns>기간 내 여부</returns>
        bool IsWithinPeriod(long startTime, long endTime);

        /// <summary>
        /// 지정된 시간까지 남은 시간 (초 단위)
        /// </summary>
        /// <param name="targetTime">목표 시각 (UTC Unix timestamp)</param>
        /// <returns>남은 초 (이미 지났으면 0)</returns>
        long GetRemainingSeconds(long targetTime);
    }
}
