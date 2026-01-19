using NUnit.Framework;
using Sc.Data;
using Sc.LocalServer;

namespace Sc.Editor.Tests.LocalServer
{
    /// <summary>
    /// GachaService 단위 테스트.
    /// 비용 계산, 횟수, 천장 확정 로직 검증.
    /// Random 의존 메서드는 천장 확정 케이스만 테스트.
    /// </summary>
    [TestFixture]
    public class GachaServiceTests
    {
        private GachaService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new GachaService();
        }

        #region CalculateCost Tests

        [Test]
        public void CalculateCost_Returns300_ForSinglePull()
        {
            var (costPerPull, totalCost) = _service.CalculateCost(GachaPullType.Single);

            Assert.That(costPerPull, Is.EqualTo(300));
            Assert.That(totalCost, Is.EqualTo(300));
        }

        [Test]
        public void CalculateCost_Returns2700_ForMultiPull()
        {
            var (_, totalCost) = _service.CalculateCost(GachaPullType.Multi);

            Assert.That(totalCost, Is.EqualTo(2700));
        }

        [Test]
        public void CalculateCost_AppliesDiscount_ForMultiPull()
        {
            var (costPerPull, totalCost) = _service.CalculateCost(GachaPullType.Multi);

            // 10연차: 300 * 10 = 3000 → 2700 (10% 할인)
            var fullPrice = costPerPull * 10;
            Assert.That(totalCost, Is.LessThan(fullPrice));
            Assert.That(fullPrice - totalCost, Is.EqualTo(300)); // 300 젬 할인
        }

        #endregion

        #region GetPullCount Tests

        [Test]
        public void GetPullCount_Returns1_ForSinglePull()
        {
            var count = _service.GetPullCount(GachaPullType.Single);

            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void GetPullCount_Returns10_ForMultiPull()
        {
            var count = _service.GetPullCount(GachaPullType.Multi);

            Assert.That(count, Is.EqualTo(10));
        }

        #endregion

        #region CalculateRarity Tests (천장 확정만)

        [Test]
        public void CalculateRarity_ReturnsSSR_WhenPityCountIs90()
        {
            var rarity = _service.CalculateRarity(90);

            Assert.That(rarity, Is.EqualTo(Rarity.SSR));
        }

        [Test]
        public void CalculateRarity_ReturnsSSR_WhenPityCountExceeds90()
        {
            var rarity = _service.CalculateRarity(100);

            Assert.That(rarity, Is.EqualTo(Rarity.SSR));
        }

        // 참고: pityCount < 90 케이스는 Random 의존성으로 인해 테스트하지 않음
        // Random 테스트가 필요하면 IRandom 인터페이스 추가 필요

        #endregion
    }
}
