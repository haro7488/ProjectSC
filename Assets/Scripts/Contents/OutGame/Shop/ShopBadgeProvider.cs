using Sc.Core;
using Sc.Data;

namespace Sc.Contents.Shop
{
    /// <summary>
    /// 상점 배지 제공자 - 무료 상품 존재 여부
    /// </summary>
    public class ShopBadgeProvider : IBadgeProvider
    {
        public BadgeType Type => BadgeType.Shop;

        public int CalculateBadgeCount()
        {
            var dataManager = DataManager.Instance;
            if (dataManager == null || !dataManager.IsInitialized)
                return 0;

            var shopDatabase = dataManager.ShopProducts;
            if (shopDatabase == null)
                return 0;

            int count = 0;
            var userData = dataManager.GetUserDataCopy();

            foreach (var product in shopDatabase.Products)
            {
                // 무료 상품 (Price == 0)
                if (product.Price > 0 || !product.IsEnabled)
                    continue;

                // 구매 가능한지 체크 (제한 횟수)
                if (product.HasLimit)
                {
                    var purchaseRecord = userData.FindShopPurchaseRecord(product.Id);
                    int purchaseCount = purchaseRecord?.PurchaseCount ?? 0;

                    if (purchaseCount >= product.LimitCount)
                        continue;
                }

                count++;
            }

            return count;
        }
    }
}
