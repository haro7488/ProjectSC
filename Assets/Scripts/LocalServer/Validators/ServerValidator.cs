using Sc.Data;

namespace Sc.LocalServer
{
    /// <summary>
    /// 서버측 검증 유틸리티
    /// 재화, 구매 제한, 기간 등 공통 검증 로직
    /// </summary>
    public class ServerValidator
    {
        private readonly ServerTimeService _timeService;

        public ServerValidator(ServerTimeService timeService)
        {
            _timeService = timeService;
        }

        /// <summary>
        /// 젬 재화 충분 여부 확인
        /// </summary>
        public bool HasEnoughGem(UserCurrency currency, int required)
        {
            return currency.TotalGem >= required;
        }

        /// <summary>
        /// 골드 재화 충분 여부 확인
        /// </summary>
        public bool HasEnoughGold(UserCurrency currency, int required)
        {
            return currency.Gold >= required;
        }

        /// <summary>
        /// 스태미나 충분 여부 확인
        /// </summary>
        public bool HasEnoughStamina(UserCurrency currency, int required)
        {
            return currency.Stamina >= required;
        }

        // TODO: 상점 구매 제한 기능 - ShopPurchaseRecord 타입 정의 후 구현
        // public bool CanPurchase(ShopPurchaseRecord record, LimitType limitType, int limitCount)

        /// <summary>
        /// 이벤트 기간 내 여부 확인
        /// </summary>
        public bool IsEventActive(long startTime, long endTime)
        {
            return _timeService.IsWithinPeriod(startTime, endTime);
        }

        /// <summary>
        /// 캐릭터 보유 여부 확인
        /// </summary>
        public bool HasCharacter(UserSaveData userData, string characterId)
        {
            return userData.HasCharacter(characterId);
        }

        /// <summary>
        /// 현재 서버 시간 조회
        /// </summary>
        public long GetServerTime() => _timeService.ServerTimeUtc;
    }
}
