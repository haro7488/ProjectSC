using System;

namespace Sc.Data
{
    /// <summary>
    /// 스테이지 입장 기록
    /// </summary>
    [Serializable]
    public struct StageEntryRecord
    {
        /// <summary>
        /// 스테이지 ID
        /// </summary>
        public string StageId;

        /// <summary>
        /// 입장 횟수
        /// </summary>
        public int EntryCount;

        /// <summary>
        /// 마지막 입장 시간 (Unix Timestamp)
        /// </summary>
        public long LastEntryTime;

        /// <summary>
        /// 다음 리셋 시간 (Unix Timestamp, 0 = 리셋 없음)
        /// </summary>
        public long ResetTime;

        /// <summary>
        /// 생성자
        /// </summary>
        public StageEntryRecord(string stageId, int entryCount, long lastEntryTime, long resetTime)
        {
            StageId = stageId;
            EntryCount = entryCount;
            LastEntryTime = lastEntryTime;
            ResetTime = resetTime;
        }

        /// <summary>
        /// 새 입장 기록 생성
        /// </summary>
        public static StageEntryRecord Create(string stageId, long entryTime, long resetTime)
        {
            return new StageEntryRecord(stageId, 1, entryTime, resetTime);
        }

        /// <summary>
        /// 리셋이 필요한지 확인
        /// </summary>
        public bool NeedsReset(long currentTime)
        {
            return ResetTime > 0 && currentTime >= ResetTime;
        }

        /// <summary>
        /// 입장 횟수 증가
        /// </summary>
        public StageEntryRecord IncrementEntry(long entryTime, long newResetTime)
        {
            return new StageEntryRecord(
                StageId,
                EntryCount + 1,
                entryTime,
                newResetTime > 0 ? newResetTime : ResetTime
            );
        }

        /// <summary>
        /// 리셋 후 새 입장
        /// </summary>
        public StageEntryRecord ResetAndEnter(long entryTime, long newResetTime)
        {
            return new StageEntryRecord(StageId, 1, entryTime, newResetTime);
        }

        public override string ToString()
        {
            return $"[{StageId}] Count: {EntryCount}, LastEntry: {LastEntryTime}, Reset: {ResetTime}";
        }
    }
}