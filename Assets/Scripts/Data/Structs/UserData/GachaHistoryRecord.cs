using System;
using System.Collections.Generic;

namespace Sc.Data
{
    /// <summary>
    /// 가챠 히스토리 결과 아이템
    /// </summary>
    [Serializable]
    public struct GachaHistoryResultItem
    {
        /// <summary>
        /// 캐릭터 ID
        /// </summary>
        public string CharacterId;

        /// <summary>
        /// 희귀도
        /// </summary>
        public Rarity Rarity;

        /// <summary>
        /// 신규 획득 여부
        /// </summary>
        public bool IsNew;

        /// <summary>
        /// 천장으로 획득 여부
        /// </summary>
        public bool IsPity;
    }

    /// <summary>
    /// 가챠 히스토리 기록
    /// </summary>
    [Serializable]
    public struct GachaHistoryRecord
    {
        /// <summary>
        /// 기록 고유 ID
        /// </summary>
        public string Id;

        /// <summary>
        /// 가챠 풀 ID
        /// </summary>
        public string PoolId;

        /// <summary>
        /// 가챠 풀 이름 (기록 시점의 이름)
        /// </summary>
        public string PoolName;

        /// <summary>
        /// 뽑기 시간 (Unix Timestamp)
        /// </summary>
        public long Timestamp;

        /// <summary>
        /// 뽑기 유형 (1회/10연차)
        /// </summary>
        public GachaPullType PullType;

        /// <summary>
        /// 뽑기 결과 목록
        /// </summary>
        public List<GachaHistoryResultItem> Results;

        /// <summary>
        /// SSR 획득 수
        /// </summary>
        public int SSRCount;

        /// <summary>
        /// SR 획득 수
        /// </summary>
        public int SRCount;

        /// <summary>
        /// R 획득 수
        /// </summary>
        public int RCount;

        /// <summary>
        /// 새 히스토리 레코드 생성
        /// </summary>
        public static GachaHistoryRecord Create(
            string poolId,
            string poolName,
            long timestamp,
            GachaPullType pullType,
            List<GachaHistoryResultItem> results)
        {
            var record = new GachaHistoryRecord
            {
                Id = Guid.NewGuid().ToString(),
                PoolId = poolId,
                PoolName = poolName,
                Timestamp = timestamp,
                PullType = pullType,
                Results = results,
                SSRCount = 0,
                SRCount = 0,
                RCount = 0
            };

            // 희귀도별 카운트 계산
            if (results != null)
            {
                foreach (var item in results)
                {
                    switch (item.Rarity)
                    {
                        case Rarity.SSR:
                            record.SSRCount++;
                            break;
                        case Rarity.SR:
                            record.SRCount++;
                            break;
                        case Rarity.R:
                            record.RCount++;
                            break;
                    }
                }
            }

            return record;
        }
    }
}
