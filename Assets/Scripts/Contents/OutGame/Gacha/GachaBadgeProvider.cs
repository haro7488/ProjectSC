using Sc.Core;

namespace Sc.Contents.Gacha
{
    /// <summary>
    /// 가챠 배지 제공자 - 무료 가챠 가능 여부
    /// TODO[FUTURE]: 무료 일일 가챠 기능 구현 시 활성화
    /// </summary>
    public class GachaBadgeProvider : IBadgeProvider
    {
        public BadgeType Type => BadgeType.Gacha;

        public int CalculateBadgeCount()
        {
            // 무료 가챠 기능 미구현 - 추후 확장
            // var dataManager = DataManager.Instance;
            // if (dataManager == null || !dataManager.IsInitialized)
            //     return 0;
            //
            // 무료 가챠 풀 체크 로직
            // - GachaPoolData에 HasFreeDaily 프로퍼티 추가 필요
            // - GachaPityData에 HasUsedFreeToday 메서드 추가 필요

            return 0;
        }
    }
}