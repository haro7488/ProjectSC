using Sc.Core;

namespace Sc.Contents.Event
{
    /// <summary>
    /// 이벤트 배지 제공자 - 수령 가능한 미션 보상 카운트
    /// </summary>
    public class EventBadgeProvider : IBadgeProvider
    {
        public BadgeType Type => BadgeType.Event;

        public int CalculateBadgeCount()
        {
            var dataManager = DataManager.Instance;
            if (dataManager == null || !dataManager.IsInitialized)
                return 0;

            int count = 0;
            var userData = dataManager.GetUserDataCopy();

            if (userData.EventProgresses != null)
            {
                foreach (var progress in userData.EventProgresses.Values)
                {
                    count += progress.GetClaimableMissionCount();
                }
            }

            return count;
        }
    }
}
