using System;
using System.Collections.Generic;
using Sc.Data;

namespace Sc.LocalServer
{
    /// <summary>
    /// 가챠 요청 핸들러 (서버측)
    /// 가챠 실행, 재화 차감, 캐릭터 지급, 히스토리 저장 처리
    /// </summary>
    public class GachaHandler : IRequestHandler<GachaRequest, GachaResponse>
    {
        private readonly ServerValidator _validator;
        private readonly GachaService _gachaService;
        private readonly RewardService _rewardService;
        private readonly ServerTimeService _timeService;
        private GachaPoolDatabase _poolDatabase;

        public GachaHandler(
            ServerValidator validator,
            GachaService gachaService,
            RewardService rewardService,
            ServerTimeService timeService)
        {
            _validator = validator;
            _gachaService = gachaService;
            _rewardService = rewardService;
            _timeService = timeService;
        }

        /// <summary>
        /// GachaPoolDatabase 설정 (외부에서 주입)
        /// </summary>
        public void SetPoolDatabase(GachaPoolDatabase database)
        {
            _poolDatabase = database;
        }

        public GachaResponse Handle(GachaRequest request, ref UserSaveData userData)
        {
            // 풀 데이터 조회
            var poolData = _poolDatabase?.GetById(request.GachaPoolId);
            if (poolData == null)
            {
                return GachaResponse.Fail(1002, "존재하지 않는 가챠 풀입니다.");
            }

            // 풀 활성화 상태 확인
            var serverTime = _timeService.ServerDateTime;
            if (!poolData.IsActiveAt(serverTime))
            {
                return GachaResponse.Fail(1003, "현재 이용할 수 없는 가챠 풀입니다.");
            }

            var pullCount = _gachaService.GetPullCount(request.PullType);
            var (_, totalCost) = _gachaService.CalculateCost(request.PullType, poolData);

            // 재화 확인 (무료 소환이 아닌 경우)
            if (totalCost > 0 && !_validator.HasEnoughGem(userData.Currency, totalCost))
            {
                return GachaResponse.Fail(1001, "보석이 부족합니다.");
            }

            // 가챠 실행
            var results = new List<GachaResultItem>();
            var historyResults = new List<GachaHistoryResultItem>();
            var addedCharacters = new List<OwnedCharacter>();
            var hitPity = false;

            // 천장 정보
            var pityInfo = userData.GachaPity.GetOrCreatePityInfo(request.GachaPoolId);
            var currentPity = pityInfo.PityCount;
            var pityThreshold = poolData.PityCount;

            for (int i = 0; i < pullCount; i++)
            {
                currentPity++;

                // 소프트 천장 적용된 희귀도 계산
                var rarity = _gachaService.CalculateRarity(currentPity, poolData);
                var characterId = _gachaService.GetRandomCharacterByRarity(rarity, request.GachaPoolId, poolData);

                var isNew = !userData.HasCharacter(characterId);
                var isPity = pityThreshold > 0 && currentPity >= pityThreshold && rarity == Rarity.SSR;

                if (isPity)
                {
                    hitPity = true;
                }

                // GachaResultItem (응답용)
                results.Add(new GachaResultItem
                {
                    CharacterId = characterId,
                    Rarity = rarity,
                    IsNew = isNew,
                    IsPity = isPity
                });

                // GachaHistoryResultItem (히스토리용)
                historyResults.Add(new GachaHistoryResultItem
                {
                    CharacterId = characterId,
                    Rarity = rarity,
                    IsNew = isNew,
                    IsPity = isPity
                });

                // 캐릭터 추가
                var newCharacter = OwnedCharacter.Create(characterId);
                addedCharacters.Add(newCharacter);
                userData.Characters.Add(newCharacter);

                // SSR 획득 시 천장 초기화
                if (rarity == Rarity.SSR)
                {
                    currentPity = 0;
                }
            }

            // 재화 차감
            if (totalCost > 0)
            {
                _rewardService.DeductGem(ref userData.Currency, totalCost);
            }

            // 천장 정보 업데이트
            UpdatePityInfo(ref userData, request.GachaPoolId, currentPity, pityInfo.TotalPullCount + pullCount);

            // 히스토리 저장
            SaveGachaHistory(ref userData, request, poolData, historyResults);

            // Delta 생성
            var delta = _rewardService.CreateCharacterDelta(
                userData.Currency,
                addedCharacters,
                userData.GachaPity);

            return GachaResponse.Success(
                results,
                delta,
                currentPity,
                pityThreshold,
                poolData.PitySoftStart,
                hitPity);
        }

        private void UpdatePityInfo(ref UserSaveData userData, string gachaPoolId, int pityCount, int totalPullCount)
        {
            userData.GachaPity.PityInfos ??= new List<GachaPityInfo>();

            for (int i = 0; i < userData.GachaPity.PityInfos.Count; i++)
            {
                if (userData.GachaPity.PityInfos[i].GachaPoolId == gachaPoolId)
                {
                    var info = userData.GachaPity.PityInfos[i];
                    info.PityCount = pityCount;
                    info.TotalPullCount = totalPullCount;
                    info.LastPullAt = _timeService.ServerTimeUtc;
                    userData.GachaPity.PityInfos[i] = info;
                    return;
                }
            }

            // 새로운 풀 정보 추가
            userData.GachaPity.PityInfos.Add(new GachaPityInfo
            {
                GachaPoolId = gachaPoolId,
                PityCount = pityCount,
                TotalPullCount = totalPullCount,
                LastPullAt = _timeService.ServerTimeUtc
            });
        }

        private void SaveGachaHistory(
            ref UserSaveData userData,
            GachaRequest request,
            GachaPoolData poolData,
            List<GachaHistoryResultItem> results)
        {
            // 히스토리 초기화 확인
            userData.GachaHistory ??= new List<GachaHistoryRecord>();

            // 풀 타입을 GachaPullType으로 변환
            var pullType = request.PullType == GachaPullType.Multi
                ? Data.GachaPullType.Multi
                : Data.GachaPullType.Single;

            // 히스토리 레코드 생성
            var historyRecord = GachaHistoryRecord.Create(
                request.GachaPoolId,
                poolData.Name,
                _timeService.ServerTimeUtc,
                pullType,
                results);

            // 히스토리 추가 (최신 기록을 앞에)
            userData.AddGachaHistory(historyRecord);

            // 오래된 히스토리 정리 (최대 500개 유지)
            userData.CleanupOldGachaHistory(500);
        }
    }
}
