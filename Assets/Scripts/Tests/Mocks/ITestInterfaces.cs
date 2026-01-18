using System;

namespace Sc.Tests
{
    /// <summary>
    /// 테스트용 시간 서비스 인터페이스.
    /// Phase 0 구현 시 Core로 이동 예정.
    /// </summary>
    public interface ITimeService
    {
        /// <summary>
        /// 현재 서버 시간 (UTC Unix timestamp)
        /// </summary>
        long ServerTimeUtc { get; }

        /// <summary>
        /// 현재 서버 시간 (DateTime)
        /// </summary>
        DateTime ServerDateTime { get; }
    }

    /// <summary>
    /// 테스트용 저장소 인터페이스.
    /// Phase 0 구현 시 Core로 이동 예정.
    /// </summary>
    public interface ISaveStorage
    {
        /// <summary>
        /// 데이터 저장
        /// </summary>
        bool Save(string key, string data);

        /// <summary>
        /// 데이터 로드
        /// </summary>
        string Load(string key);

        /// <summary>
        /// 데이터 존재 여부
        /// </summary>
        bool Exists(string key);

        /// <summary>
        /// 데이터 삭제
        /// </summary>
        bool Delete(string key);
    }
}
