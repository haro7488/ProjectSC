using System;
using System.Collections.Generic;
using Sc.Data;

namespace Sc.LocalServer
{
    /// <summary>
    /// 가챠 요청 핸들러 (서버측)
    /// 가챠 실행, 재화 차감, 캐릭터 지급 처리
    /// </summary>
    public class GachaHandler : IRequestHandler<GachaRequest, GachaResponse>
    {
        private readonly ServerValidator _validator;
        private readonly GachaService _gachaService;
        private readonly RewardService _rewardService;
        private readonly ServerTimeService _timeService;

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

        public GachaResponse Handle(GachaRequest request, ref UserSaveData userData)
        {
            var pullCount = _gachaService.GetPullCount(request.PullType);
            var (_, totalCost) = _gachaService.CalculateCost(request.PullType);

            // 재화 확인
            if (!_validator.HasEnoughGem(userData.Currency, totalCost))
            {
                return GachaResponse.Fail(1001, "보석이 부족합니다.");
            }

            // 가챠 실행
            var results = new List<GachaResultItem>();
            var addedCharacters = new List<OwnedCharacter>();

            // 천장 정보
            var pityInfo = userData.GachaPity.GetOrCreatePityInfo(request.GachaPoolId);
            var currentPity = pityInfo.PityCount;

            for (int i = 0; i < pullCount; i++)
            {
                currentPity++;

                var rarity = _gachaService.CalculateRarity(currentPity);
                var characterId = _gachaService.GetRandomCharacterByRarity(rarity, request.GachaPoolId);

                var isNew = !userData.HasCharacter(characterId);
                var isPity = currentPity >= 90 && rarity == Rarity.SSR;

                results.Add(new GachaResultItem
                {
                    CharacterId = characterId,
                    Rarity = rarity,
                    IsNew = isNew,
                    IsPity = isPity
                });

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
            _rewardService.DeductGem(ref userData.Currency, totalCost);

            // 천장 정보 업데이트
            UpdatePityInfo(ref userData, request.GachaPoolId, currentPity, pityInfo.TotalPullCount + pullCount);

            // Delta 생성
            var delta = _rewardService.CreateCharacterDelta(
                userData.Currency,
                addedCharacters,
                userData.GachaPity);

            return GachaResponse.Success(results, delta, currentPity);
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
    }
}
