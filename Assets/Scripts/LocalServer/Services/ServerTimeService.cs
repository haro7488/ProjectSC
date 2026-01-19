using System;
using Sc.Data;

namespace Sc.LocalServer
{
    /// <summary>
    /// 서버측 시간 서비스
    /// 실제 서버에서는 서버 시간 기준, 로컬에서는 UTC 시간 사용
    /// </summary>
    public class ServerTimeService
    {
        /// <summary>
        /// 현재 서버 시간 (UTC Unix timestamp)
        /// </summary>
        public long ServerTimeUtc => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        /// <summary>
        /// 현재 서버 시간 (DateTime UTC)
        /// </summary>
        public DateTime ServerDateTime => DateTimeOffset.FromUnixTimeSeconds(ServerTimeUtc).UtcDateTime;

        /// <summary>
        /// 다음 리셋 시간 계산
        /// </summary>
        public long GetNextResetTime(LimitType limitType)
        {
            var now = ServerDateTime;

            return limitType switch
            {
                LimitType.None or LimitType.Permanent or LimitType.EventPeriod => 0,
                LimitType.Daily => GetNextDayReset(now),
                LimitType.Weekly => GetNextWeekReset(now),
                LimitType.Monthly => GetNextMonthReset(now),
                _ => 0
            };
        }

        /// <summary>
        /// 마지막 기록 이후 리셋이 발생했는지 확인
        /// </summary>
        public bool HasResetOccurred(long lastTimestamp, LimitType limitType)
        {
            if (lastTimestamp <= 0) return false;

            return limitType switch
            {
                LimitType.None or LimitType.Permanent => false,
                LimitType.EventPeriod => false, // 별도 로직
                LimitType.Daily or LimitType.Weekly or LimitType.Monthly =>
                    ServerTimeUtc >= GetResetTimeAfter(lastTimestamp, limitType),
                _ => false
            };
        }

        /// <summary>
        /// 현재 시간이 지정된 기간 내인지 확인
        /// </summary>
        public bool IsWithinPeriod(long startTime, long endTime)
        {
            var now = ServerTimeUtc;
            return now >= startTime && now < endTime;
        }

        private long GetNextDayReset(DateTime now)
        {
            var nextDay = now.Date.AddDays(1);
            return new DateTimeOffset(nextDay, TimeSpan.Zero).ToUnixTimeSeconds();
        }

        private long GetNextWeekReset(DateTime now)
        {
            var daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
            if (daysUntilMonday == 0) daysUntilMonday = 7;
            var nextMonday = now.Date.AddDays(daysUntilMonday);
            return new DateTimeOffset(nextMonday, TimeSpan.Zero).ToUnixTimeSeconds();
        }

        private long GetNextMonthReset(DateTime now)
        {
            var nextMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1);
            return new DateTimeOffset(nextMonth, TimeSpan.Zero).ToUnixTimeSeconds();
        }

        private long GetResetTimeAfter(long timestamp, LimitType limitType)
        {
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;

            return limitType switch
            {
                LimitType.Daily => GetNextDayReset(dateTime),
                LimitType.Weekly => GetNextWeekReset(dateTime),
                LimitType.Monthly => GetNextMonthReset(dateTime),
                _ => 0
            };
        }
    }
}
