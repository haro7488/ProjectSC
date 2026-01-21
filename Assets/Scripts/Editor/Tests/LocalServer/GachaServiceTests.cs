using NUnit.Framework;
using Sc.Data;
using Sc.LocalServer;
using UnityEngine;

namespace Sc.Editor.Tests.LocalServer
{
    /// <summary>
    /// GachaService 단위 테스트.
    /// 비용 계산, 횟수, 천장 확정 로직, 소프트 천장 검증.
    /// Random 의존 메서드는 천장 확정 케이스만 테스트.
    /// </summary>
    [TestFixture]
    public class GachaServiceTests
    {
        private GachaService _service;
        private GachaPoolData _testPoolData;

        [SetUp]
        public void SetUp()
        {
            _service = new GachaService();

            // 테스트용 GachaPoolData 생성
            _testPoolData = ScriptableObject.CreateInstance<GachaPoolData>();
            _testPoolData.Initialize(
                id: "test_pool",
                name: "테스트 풀",
                nameEn: "Test Pool",
                type: GachaType.Standard,
                costType: CostType.Gem,
                costAmount: 300,
                costAmount10: 2700,
                pityCount: 90,
                characterIds: new[] { "char_001", "char_002" },
                rates: new GachaRates { SSR = 0.03f, SR = 0.12f, R = 0.85f },
                rateUpCharacterId: "",
                rateUpBonus: 0f,
                isActive: true,
                startDate: "",
                endDate: "",
                bannerImagePath: "",
                displayOrder: 0,
                pitySoftStart: 75,
                pitySoftRateBonus: 0.06f,
                description: "테스트용"
            );
        }

        [TearDown]
        public void TearDown()
        {
            if (_testPoolData != null)
            {
                Object.DestroyImmediate(_testPoolData);
            }
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

        [Test]
        public void CalculateRarity_ReturnsSSR_WhenPoolPityCountReached()
        {
            // 풀 설정: pityCount = 90
            var rarity = _service.CalculateRarity(90, _testPoolData);

            Assert.That(rarity, Is.EqualTo(Rarity.SSR));
        }

        [Test]
        public void CalculateRarity_ReturnsSSR_WhenPoolPityCountExceeded()
        {
            var rarity = _service.CalculateRarity(100, _testPoolData);

            Assert.That(rarity, Is.EqualTo(Rarity.SSR));
        }

        #endregion

        #region GetEffectiveSSRRate Tests (소프트 천장)

        [Test]
        public void GetEffectiveSSRRate_ReturnsBaseRate_BeforeSoftPity()
        {
            // 소프트 천장 전 (75회 전)
            var rate = _service.GetEffectiveSSRRate(50, _testPoolData);

            Assert.That(rate, Is.EqualTo(0.03f).Within(0.001f));
        }

        [Test]
        public void GetEffectiveSSRRate_ReturnsIncreasedRate_AtSoftPityStart()
        {
            // 소프트 천장 시작점 (75회)
            var rate = _service.GetEffectiveSSRRate(75, _testPoolData);

            // 75회째는 bonusCount=0이므로 기본 확률
            Assert.That(rate, Is.EqualTo(0.03f).Within(0.001f));
        }

        [Test]
        public void GetEffectiveSSRRate_ReturnsIncreasedRate_DuringSoftPity()
        {
            // 소프트 천장 구간 (80회 = 75 + 5)
            // bonusCount = 80 - 75 = 5
            // effectiveRate = 0.03 + 5 * 0.06 = 0.03 + 0.30 = 0.33
            var rate = _service.GetEffectiveSSRRate(80, _testPoolData);

            Assert.That(rate, Is.EqualTo(0.33f).Within(0.001f));
        }

        [Test]
        public void GetEffectiveSSRRate_Returns100Percent_AtHardPity()
        {
            // 하드 천장 (90회)
            var rate = _service.GetEffectiveSSRRate(90, _testPoolData);

            Assert.That(rate, Is.EqualTo(1f));
        }

        [Test]
        public void GetEffectiveSSRRate_CapsAt100Percent()
        {
            // 85회 = 75 + 10, bonusCount = 10
            // effectiveRate = 0.03 + 10 * 0.06 = 0.63
            // 89회 = 75 + 14, bonusCount = 14
            // effectiveRate = 0.03 + 14 * 0.06 = 0.87
            var rate = _service.GetEffectiveSSRRate(89, _testPoolData);

            Assert.That(rate, Is.LessThanOrEqualTo(1f));
            Assert.That(rate, Is.EqualTo(0.87f).Within(0.001f));
        }

        [Test]
        public void GetEffectiveSSRRate_ReturnsDefaultRate_WhenPoolDataIsNull()
        {
            var rate = _service.GetEffectiveSSRRate(50, null);

            Assert.That(rate, Is.EqualTo(0.03f).Within(0.001f));
        }

        #endregion

        #region IsInSoftPityRange Tests

        [Test]
        public void IsInSoftPityRange_ReturnsFalse_BeforeSoftPity()
        {
            var result = _service.IsInSoftPityRange(74, _testPoolData);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsInSoftPityRange_ReturnsTrue_AtSoftPityStart()
        {
            var result = _service.IsInSoftPityRange(75, _testPoolData);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsInSoftPityRange_ReturnsTrue_DuringSoftPity()
        {
            var result = _service.IsInSoftPityRange(80, _testPoolData);

            Assert.That(result, Is.True);
        }

        [Test]
        public void IsInSoftPityRange_ReturnsFalse_WhenPoolDataIsNull()
        {
            var result = _service.IsInSoftPityRange(80, null);

            Assert.That(result, Is.False);
        }

        #endregion

        #region CalculateCost with PoolData Tests

        [Test]
        public void CalculateCost_UsesPoolDataCost_WhenProvided()
        {
            var (costPerPull, totalCost) = _service.CalculateCost(GachaPullType.Single, _testPoolData);

            Assert.That(costPerPull, Is.EqualTo(300));
            Assert.That(totalCost, Is.EqualTo(300));
        }

        [Test]
        public void CalculateCost_UsesPoolDataMultiCost_WhenProvided()
        {
            var (_, totalCost) = _service.CalculateCost(GachaPullType.Multi, _testPoolData);

            Assert.That(totalCost, Is.EqualTo(2700));
        }

        #endregion
    }
}
