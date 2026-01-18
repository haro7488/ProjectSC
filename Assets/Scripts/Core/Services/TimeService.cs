using System;
using Sc.Data;

namespace Sc.Core
{
    /// <summary>
    /// 로컬 시간 서비스 구현.
    /// 클라이언트 시간을 서버 시간으로 사용 (오프셋 0).
    /// 실제 서버 연동 시 ServerTimeService로 교체.
    /// </summary>
    public class TimeService : ITimeService
    {
        private long _offset;

        /// <summary>
        /// 현재 서버 시간 (UTC Unix timestamp)
        /// 로컬 환경에서는 클라이언트 시간 + 오프셋
        /// </summary>
        public long ServerTimeUtc => DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _offset;

        /// <summary>
        /// 현재 서버 시간 (DateTime UTC)
        /// </summary>
        public DateTime ServerDateTime => DateTimeOffset.FromUnixTimeSeconds(ServerTimeUtc).UtcDateTime;

        /// <summary>
        /// 클라이언트-서버 시간 오프셋 (초)
        /// 로컬 환경에서는 기본 0
        /// </summary>
        public long TimeOffset => _offset;

        /// <summary>
        /// 서버 시간 동기화.
        /// 로컬 환경에서는 오프셋만 계산하여 저장.
        /// </summary>
        public void SyncServerTime(long serverTimestamp)
        {
            _offset = serverTimestamp - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// 다음 리셋 시간 계산
        /// </summary>
        public long GetNextResetTime(LimitType limitType)
        {
            var now = ServerDateTime;

            switch (limitType)
            {
                case LimitType.None:
                case LimitType.Permanent:
                case LimitType.EventPeriod:
                    return 0; // 리셋 없음 또는 별도 관리

                case LimitType.Daily:
                    // 다음 날 UTC 00:00
                    var nextDay = now.Date.AddDays(1);
                    return new DateTimeOffset(nextDay, TimeSpan.Zero).ToUnixTimeSeconds();

                case LimitType.Weekly:
                    // 다음 월요일 UTC 00:00
                    var daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
                    if (daysUntilMonday == 0) daysUntilMonday = 7; // 오늘이 월요일이면 다음 주
                    var nextMonday = now.Date.AddDays(daysUntilMonday);
                    return new DateTimeOffset(nextMonday, TimeSpan.Zero).ToUnixTimeSeconds();

                case LimitType.Monthly:
                    // 다음 달 1일 UTC 00:00
                    var nextMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1);
                    return new DateTimeOffset(nextMonth, TimeSpan.Zero).ToUnixTimeSeconds();

                default:
                    return 0;
            }
        }

        /// <summary>
        /// 마지막 기록 이후 리셋이 발생했는지 확인
        /// </summary>
        public bool HasResetOccurred(long lastTimestamp, LimitType limitType)
        {
            if (lastTimestamp <= 0) return false;

            switch (limitType)
            {
                case LimitType.None:
                case LimitType.Permanent:
                    return false; // 리셋 없음

                case LimitType.EventPeriod:
                    return false; // 별도 로직으로 처리 (이벤트 종료 시간과 비교)

                case LimitType.Daily:
                case LimitType.Weekly:
                case LimitType.Monthly:
                    // 마지막 기록 시점의 "다음 리셋 시간"이 현재보다 과거인지 확인
                    var lastResetTime = GetResetTimeAfter(lastTimestamp, limitType);
                    return ServerTimeUtc >= lastResetTime;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 현재 시간이 지정된 기간 내인지 확인
        /// </summary>
        public bool IsWithinPeriod(long startTime, long endTime)
        {
            var now = ServerTimeUtc;
            return now >= startTime && now < endTime;
        }

        /// <summary>
        /// 지정된 시간까지 남은 시간 (초)
        /// </summary>
        public long GetRemainingSeconds(long targetTime)
        {
            var remaining = targetTime - ServerTimeUtc;
            return remaining > 0 ? remaining : 0;
        }

        /// <summary>
        /// 특정 시점 이후의 첫 리셋 시간 계산 (내부 헬퍼)
        /// </summary>
        private long GetResetTimeAfter(long timestamp, LimitType limitType)
        {
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;

            switch (limitType)
            {
                case LimitType.Daily:
                    // 해당 날짜의 다음 날 UTC 00:00
                    var nextDay = dateTime.Date.AddDays(1);
                    return new DateTimeOffset(nextDay, TimeSpan.Zero).ToUnixTimeSeconds();

                case LimitType.Weekly:
                    // 해당 날짜 이후 첫 월요일 UTC 00:00
                    var daysUntilMonday = ((int)DayOfWeek.Monday - (int)dateTime.DayOfWeek + 7) % 7;
                    if (daysUntilMonday == 0) daysUntilMonday = 7;
                    var nextMonday = dateTime.Date.AddDays(daysUntilMonday);
                    return new DateTimeOffset(nextMonday, TimeSpan.Zero).ToUnixTimeSeconds();

                case LimitType.Monthly:
                    // 해당 날짜 이후 첫 1일 UTC 00:00
                    var nextMonth = new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1);
                    return new DateTimeOffset(nextMonth, TimeSpan.Zero).ToUnixTimeSeconds();

                default:
                    return 0;
            }
        }
    }
}
