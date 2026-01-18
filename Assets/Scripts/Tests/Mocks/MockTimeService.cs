using System;

namespace Sc.Tests
{
    /// <summary>
    /// 테스트용 시간 서비스 Mock.
    /// 고정된 시간을 반환하거나 시간 조작이 가능.
    /// </summary>
    public class MockTimeService : ITimeService
    {
        private long _fixedTimeUtc;
        private bool _useFixedTime;

        /// <summary>
        /// 기본 생성자 (현재 시간 사용)
        /// </summary>
        public MockTimeService()
        {
            _useFixedTime = false;
        }

        /// <summary>
        /// 고정 시간 생성자
        /// </summary>
        public MockTimeService(long fixedTimeUtc)
        {
            _fixedTimeUtc = fixedTimeUtc;
            _useFixedTime = true;
        }

        /// <summary>
        /// 고정 시간 설정
        /// </summary>
        public void SetFixedTime(long utcTimestamp)
        {
            _fixedTimeUtc = utcTimestamp;
            _useFixedTime = true;
        }

        /// <summary>
        /// 고정 시간 설정 (DateTime)
        /// </summary>
        public void SetFixedTime(DateTime dateTime)
        {
            _fixedTimeUtc = new DateTimeOffset(dateTime, TimeSpan.Zero).ToUnixTimeSeconds();
            _useFixedTime = true;
        }

        /// <summary>
        /// 시간 경과 시뮬레이션
        /// </summary>
        public void AdvanceTime(TimeSpan duration)
        {
            if (!_useFixedTime)
            {
                _fixedTimeUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                _useFixedTime = true;
            }
            _fixedTimeUtc += (long)duration.TotalSeconds;
        }

        /// <summary>
        /// 실시간 모드로 전환
        /// </summary>
        public void UseRealTime()
        {
            _useFixedTime = false;
        }

        public long ServerTimeUtc => _useFixedTime
            ? _fixedTimeUtc
            : DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public DateTime ServerDateTime => DateTimeOffset
            .FromUnixTimeSeconds(ServerTimeUtc)
            .UtcDateTime;
    }
}
