using System;
using Sc.Data;

namespace Sc.Core
{
    /// <summary>
    /// 시간 관련 UI 표시 헬퍼.
    /// 정적 유틸리티 메서드 제공.
    /// </summary>
    public static class TimeHelper
    {
        private const int SecondsPerMinute = 60;
        private const int SecondsPerHour = 3600;
        private const int SecondsPerDay = 86400;

        /// <summary>
        /// UTC timestamp를 로컬 시간 문자열로 변환
        /// </summary>
        /// <param name="utcTimestamp">UTC Unix timestamp</param>
        /// <param name="format">날짜 포맷 (기본: yyyy-MM-dd HH:mm)</param>
        /// <returns>로컬 시간 문자열</returns>
        public static string ToLocalTimeString(long utcTimestamp, string format = "yyyy-MM-dd HH:mm")
        {
            if (utcTimestamp <= 0) return string.Empty;

            var dateTime = DateTimeOffset.FromUnixTimeSeconds(utcTimestamp).LocalDateTime;
            return dateTime.ToString(format);
        }

        /// <summary>
        /// UTC timestamp를 로컬 DateTime으로 변환
        /// </summary>
        public static DateTime ToLocalDateTime(long utcTimestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(utcTimestamp).LocalDateTime;
        }

        /// <summary>
        /// 남은 시간을 읽기 쉬운 문자열로 포맷
        /// 예: "2시간 30분", "1일 5시간", "30초"
        /// </summary>
        /// <param name="remainingSeconds">남은 시간 (초)</param>
        /// <returns>포맷된 문자열</returns>
        public static string FormatRemainingTime(long remainingSeconds)
        {
            if (remainingSeconds <= 0) return "0초";

            if (remainingSeconds >= SecondsPerDay)
            {
                var days = remainingSeconds / SecondsPerDay;
                var hours = (remainingSeconds % SecondsPerDay) / SecondsPerHour;
                return hours > 0 ? $"{days}일 {hours}시간" : $"{days}일";
            }

            if (remainingSeconds >= SecondsPerHour)
            {
                var hours = remainingSeconds / SecondsPerHour;
                var minutes = (remainingSeconds % SecondsPerHour) / SecondsPerMinute;
                return minutes > 0 ? $"{hours}시간 {minutes}분" : $"{hours}시간";
            }

            if (remainingSeconds >= SecondsPerMinute)
            {
                var minutes = remainingSeconds / SecondsPerMinute;
                var seconds = remainingSeconds % SecondsPerMinute;
                return seconds > 0 ? $"{minutes}분 {seconds}초" : $"{minutes}분";
            }

            return $"{remainingSeconds}초";
        }

        /// <summary>
        /// 목표 시간까지 남은 시간을 포맷
        /// </summary>
        /// <param name="targetUtcTimestamp">목표 시간 (UTC Unix timestamp)</param>
        /// <param name="timeService">시간 서비스</param>
        /// <returns>포맷된 문자열 (이미 지났으면 빈 문자열)</returns>
        public static string FormatTimeUntil(long targetUtcTimestamp, ITimeService timeService)
        {
            var remaining = timeService.GetRemainingSeconds(targetUtcTimestamp);
            return remaining > 0 ? FormatRemainingTime(remaining) : string.Empty;
        }

        /// <summary>
        /// 리셋까지 남은 시간을 포맷
        /// </summary>
        /// <param name="limitType">리셋 타입</param>
        /// <param name="timeService">시간 서비스</param>
        /// <returns>포맷된 문자열</returns>
        public static string FormatTimeUntilReset(LimitType limitType, ITimeService timeService)
        {
            var nextReset = timeService.GetNextResetTime(limitType);
            if (nextReset <= 0) return string.Empty;

            return FormatTimeUntil(nextReset, timeService);
        }

        /// <summary>
        /// 남은 시간을 짧은 형식으로 포맷 (타이머 표시용)
        /// 예: "02:30:00", "23:59:59"
        /// </summary>
        /// <param name="remainingSeconds">남은 시간 (초)</param>
        /// <returns>HH:mm:ss 형식</returns>
        public static string FormatRemainingTimeShort(long remainingSeconds)
        {
            if (remainingSeconds <= 0) return "00:00:00";

            var hours = remainingSeconds / SecondsPerHour;
            var minutes = (remainingSeconds % SecondsPerHour) / SecondsPerMinute;
            var seconds = remainingSeconds % SecondsPerMinute;

            if (hours > 99)
            {
                var days = remainingSeconds / SecondsPerDay;
                return $"{days}일";
            }

            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        /// <summary>
        /// 상대 시간 표현 (SNS 스타일)
        /// 예: "방금 전", "5분 전", "3시간 전", "2일 전", "2025-01-15"
        /// </summary>
        /// <param name="utcTimestamp">과거 시간 (UTC Unix timestamp)</param>
        /// <param name="timeService">시간 서비스</param>
        /// <returns>상대 시간 문자열</returns>
        public static string FormatRelativeTime(long utcTimestamp, ITimeService timeService)
        {
            var elapsed = timeService.ServerTimeUtc - utcTimestamp;

            if (elapsed < 0) return "방금 전";
            if (elapsed < SecondsPerMinute) return "방금 전";
            if (elapsed < SecondsPerHour) return $"{elapsed / SecondsPerMinute}분 전";
            if (elapsed < SecondsPerDay) return $"{elapsed / SecondsPerHour}시간 전";
            if (elapsed < SecondsPerDay * 7) return $"{elapsed / SecondsPerDay}일 전";

            // 7일 이상이면 날짜 표시
            return ToLocalTimeString(utcTimestamp, "yyyy-MM-dd");
        }

        /// <summary>
        /// 리셋 타입에 해당하는 표시 문자열
        /// </summary>
        public static string GetLimitTypeDisplayName(LimitType limitType)
        {
            switch (limitType)
            {
                case LimitType.None: return "무제한";
                case LimitType.Daily: return "일일";
                case LimitType.Weekly: return "주간";
                case LimitType.Monthly: return "월간";
                case LimitType.Permanent: return "1회";
                case LimitType.EventPeriod: return "이벤트";
                default: return string.Empty;
            }
        }
    }
}
