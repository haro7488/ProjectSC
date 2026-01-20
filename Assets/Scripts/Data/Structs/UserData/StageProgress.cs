using System;
using System.Collections.Generic;

namespace Sc.Data
{
    /// <summary>
    /// 개별 스테이지 클리어 데이터
    /// </summary>
    [Serializable]
    public struct StageClearInfo
    {
        /// <summary>
        /// 스테이지 ID
        /// </summary>
        public string StageId;

        /// <summary>
        /// 클리어 여부
        /// </summary>
        public bool IsCleared;

        /// <summary>
        /// 획득 별 수 (0~3)
        /// </summary>
        public int Stars;

        /// <summary>
        /// 개별 별 달성 여부 [star1, star2, star3]
        /// </summary>
        public bool[] StarAchieved;

        /// <summary>
        /// 최고 턴 수 기록 (낮을수록 좋음)
        /// </summary>
        public int BestTurnCount;

        /// <summary>
        /// 클리어 횟수
        /// </summary>
        public int ClearCount;

        /// <summary>
        /// 첫 클리어 시간 (Unix Timestamp)
        /// </summary>
        public long FirstClearedAt;

        /// <summary>
        /// 마지막 클리어 시간 (Unix Timestamp)
        /// </summary>
        public long LastClearedAt;

        /// <summary>
        /// 기본값 생성
        /// </summary>
        public static StageClearInfo CreateDefault(string stageId)
        {
            return new StageClearInfo
            {
                StageId = stageId,
                IsCleared = false,
                Stars = 0,
                StarAchieved = new bool[3],
                BestTurnCount = int.MaxValue,
                ClearCount = 0,
                FirstClearedAt = 0,
                LastClearedAt = 0
            };
        }

        /// <summary>
        /// 클리어 정보 업데이트
        /// </summary>
        public StageClearInfo UpdateWithClear(
            bool[] newStarAchieved,
            int turnCount,
            long clearedAt)
        {
            var info = this;

            // 첫 클리어 처리
            if (!info.IsCleared)
            {
                info.IsCleared = true;
                info.FirstClearedAt = clearedAt;
            }

            // 클리어 횟수 증가
            info.ClearCount++;
            info.LastClearedAt = clearedAt;

            // 최고 턴 수 갱신
            if (turnCount < info.BestTurnCount)
            {
                info.BestTurnCount = turnCount;
            }

            // 별 달성 병합 (한번 달성하면 유지)
            info.StarAchieved ??= new bool[3];
            for (int i = 0; i < 3 && i < newStarAchieved.Length; i++)
            {
                if (newStarAchieved[i])
                {
                    info.StarAchieved[i] = true;
                }
            }

            // 별 개수 계산
            info.Stars = 0;
            foreach (var achieved in info.StarAchieved)
            {
                if (achieved) info.Stars++;
            }

            return info;
        }
    }

    /// <summary>
    /// 스테이지 진행 데이터
    /// </summary>
    [Serializable]
    public struct StageProgress
    {
        /// <summary>
        /// 현재 진행 중인 챕터
        /// </summary>
        public int CurrentChapter;

        /// <summary>
        /// 현재 진행 중인 스테이지 번호
        /// </summary>
        public int CurrentStageNumber;

        /// <summary>
        /// 클리어한 스테이지 목록
        /// </summary>
        public List<StageClearInfo> ClearedStages;

        /// <summary>
        /// 스테이지 클리어 여부 확인
        /// </summary>
        public bool IsStageCleared(string stageId)
        {
            if (ClearedStages == null) return false;
            foreach (var info in ClearedStages)
            {
                if (info.StageId == stageId && info.IsCleared)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 스테이지 별 수 조회
        /// </summary>
        public int GetStageStars(string stageId)
        {
            if (ClearedStages == null) return 0;
            foreach (var info in ClearedStages)
            {
                if (info.StageId == stageId)
                    return info.Stars;
            }
            return 0;
        }

        /// <summary>
        /// 스테이지 클리어 정보 조회
        /// </summary>
        public StageClearInfo? FindClearInfo(string stageId)
        {
            if (ClearedStages == null) return null;
            foreach (var info in ClearedStages)
            {
                if (info.StageId == stageId)
                    return info;
            }
            return null;
        }

        /// <summary>
        /// 스테이지 클리어 정보 업데이트 또는 추가
        /// </summary>
        public void UpdateClearInfo(string stageId, StageClearInfo clearInfo)
        {
            ClearedStages ??= new List<StageClearInfo>();

            for (int i = 0; i < ClearedStages.Count; i++)
            {
                if (ClearedStages[i].StageId == stageId)
                {
                    ClearedStages[i] = clearInfo;
                    return;
                }
            }

            // 새 기록 추가
            ClearedStages.Add(clearInfo);
        }

        /// <summary>
        /// 기본값으로 초기화된 진행 데이터 생성
        /// </summary>
        public static StageProgress CreateDefault()
        {
            return new StageProgress
            {
                CurrentChapter = 1,
                CurrentStageNumber = 1,
                ClearedStages = new List<StageClearInfo>()
            };
        }
    }
}
