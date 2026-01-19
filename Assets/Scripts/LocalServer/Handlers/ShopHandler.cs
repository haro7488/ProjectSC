using System.Collections.Generic;
using Sc.Data;

namespace Sc.LocalServer
{
    /// <summary>
    /// 상점 구매 요청 핸들러 (서버측)
    /// 상품 구매, 재화 차감, 보상 지급 처리
    /// </summary>
    public class ShopHandler : IRequestHandler<ShopPurchaseRequest, ShopPurchaseResponse>
    {
        private readonly ServerValidator _validator;
        private readonly RewardService _rewardService;
        private readonly ServerTimeService _timeService;

        public ShopHandler(
            ServerValidator validator,
            RewardService rewardService,
            ServerTimeService timeService)
        {
            _validator = validator;
            _rewardService = rewardService;
            _timeService = timeService;
        }

        public ShopPurchaseResponse Handle(ShopPurchaseRequest request, ref UserSaveData userData)
        {
            // 상품 정보 조회 (실제로는 마스터 데이터에서)
            // TODO: ShopProductDatabase 연동
            if (request.ProductId == "gold_pack_small")
            {
                return HandleGoldPackPurchase(request, ref userData);
            }

            return ShopPurchaseResponse.Fail(1002, "존재하지 않는 상품입니다.");
        }

        private ShopPurchaseResponse HandleGoldPackPurchase(
            ShopPurchaseRequest request,
            ref UserSaveData userData)
        {
            const int gemCost = 100;
            const int goldReward = 10000;

            // 재화 확인
            if (!_validator.HasEnoughGem(userData.Currency, gemCost))
            {
                return ShopPurchaseResponse.Fail(1001, "보석이 부족합니다.");
            }

            // 재화 차감
            _rewardService.DeductGem(ref userData.Currency, gemCost);

            // 골드 지급
            userData.Currency.Gold += goldReward;

            // 보상 목록 생성
            var rewards = new List<PurchaseRewardItem>
            {
                new PurchaseRewardItem
                {
                    RewardType = "Currency",
                    RewardId = "Gold",
                    Amount = goldReward
                }
            };

            // Delta 생성
            var delta = _rewardService.CreateCurrencyDelta(userData.Currency);

            return ShopPurchaseResponse.Success(request.ProductId, rewards, delta);
        }
    }
}
