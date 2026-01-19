using System.Collections.Generic;
using NUnit.Framework;
using Sc.Data;
using Sc.LocalServer;

namespace Sc.Editor.Tests.LocalServer
{
    /// <summary>
    /// RewardService 단위 테스트.
    /// Delta 생성 및 재화 차감 로직 검증.
    /// </summary>
    [TestFixture]
    public class RewardServiceTests
    {
        private RewardService _service;
        private UserSaveData _testUserData;

        [SetUp]
        public void SetUp()
        {
            _service = new RewardService();
            _testUserData = UserSaveData.CreateNew("test_uid", "TestUser");
        }

        #region CreateCurrencyDelta Tests

        [Test]
        public void CreateCurrencyDelta_ReturnsDelta_WithCurrency()
        {
            var currency = new UserCurrency { Gold = 1000, Gem = 50 };

            var delta = _service.CreateCurrencyDelta(currency);

            Assert.That(delta.Currency.HasValue, Is.True);
            Assert.That(delta.Currency.Value.Gold, Is.EqualTo(1000));
            Assert.That(delta.Currency.Value.Gem, Is.EqualTo(50));
        }

        #endregion

        #region CreateCharacterDelta Tests

        [Test]
        public void CreateCharacterDelta_ReturnsDelta_WithCurrencyAndCharacters()
        {
            var currency = new UserCurrency { Gold = 500 };
            var characters = new List<OwnedCharacter>
            {
                OwnedCharacter.Create("char_001"),
                OwnedCharacter.Create("char_002")
            };

            var delta = _service.CreateCharacterDelta(currency, characters);

            Assert.That(delta.Currency.HasValue, Is.True);
            Assert.That(delta.Currency.Value.Gold, Is.EqualTo(500));
            Assert.That(delta.AddedCharacters, Is.Not.Null);
            Assert.That(delta.AddedCharacters.Count, Is.EqualTo(2));
        }

        [Test]
        public void CreateCharacterDelta_ReturnsDelta_WithPityData()
        {
            var currency = new UserCurrency();
            var characters = new List<OwnedCharacter>();
            var pityData = new GachaPityData
            {
                PityInfos = new List<GachaPityInfo>
                {
                    new GachaPityInfo { GachaPoolId = "pool_001", PityCount = 50 }
                }
            };

            var delta = _service.CreateCharacterDelta(currency, characters, pityData);

            Assert.That(delta.GachaPity.HasValue, Is.True);
            Assert.That(delta.GachaPity.Value.PityInfos, Is.Not.Null);
            Assert.That(delta.GachaPity.Value.PityInfos.Count, Is.EqualTo(1));
            Assert.That(delta.GachaPity.Value.PityInfos[0].PityCount, Is.EqualTo(50));
        }

        #endregion

        #region DeductGem Tests

        [Test]
        public void DeductGem_ReturnsFalse_WhenInsufficientGem()
        {
            var currency = new UserCurrency { Gem = 100, FreeGem = 100 }; // Total: 200

            var result = _service.DeductGem(ref currency, 300);

            Assert.That(result, Is.False);
            Assert.That(currency.Gem, Is.EqualTo(100)); // 변경 없음
            Assert.That(currency.FreeGem, Is.EqualTo(100));
        }

        [Test]
        public void DeductGem_DeductsFreeGemFirst_WhenEnough()
        {
            var currency = new UserCurrency { Gem = 100, FreeGem = 200 };

            var result = _service.DeductGem(ref currency, 150);

            Assert.That(result, Is.True);
            Assert.That(currency.FreeGem, Is.EqualTo(50)); // 200 - 150
            Assert.That(currency.Gem, Is.EqualTo(100)); // 유료 젬 그대로
        }

        [Test]
        public void DeductGem_DeductsPaidGem_WhenFreeGemInsufficient()
        {
            var currency = new UserCurrency { Gem = 200, FreeGem = 50 };

            var result = _service.DeductGem(ref currency, 150);

            Assert.That(result, Is.True);
            Assert.That(currency.FreeGem, Is.EqualTo(0)); // 무료 젬 전부 소모
            Assert.That(currency.Gem, Is.EqualTo(100)); // 200 - (150 - 50)
        }

        [Test]
        public void DeductGem_DeductsBothGems_WhenMixed()
        {
            var currency = new UserCurrency { Gem = 100, FreeGem = 100 };

            var result = _service.DeductGem(ref currency, 150);

            Assert.That(result, Is.True);
            Assert.That(currency.FreeGem, Is.EqualTo(0));
            Assert.That(currency.Gem, Is.EqualTo(50)); // 100 - 50
        }

        [Test]
        public void DeductGem_ReturnsTrue_WhenExactAmount()
        {
            var currency = new UserCurrency { Gem = 100, FreeGem = 100 };

            var result = _service.DeductGem(ref currency, 200);

            Assert.That(result, Is.True);
            Assert.That(currency.FreeGem, Is.EqualTo(0));
            Assert.That(currency.Gem, Is.EqualTo(0));
        }

        [Test]
        public void DeductGem_SetsZero_WhenDeductingAll()
        {
            var currency = new UserCurrency { Gem = 50, FreeGem = 50 };

            _service.DeductGem(ref currency, 100);

            Assert.That(currency.TotalGem, Is.EqualTo(0));
        }

        #endregion

        #region DeductGold Tests

        [Test]
        public void DeductGold_ReturnsFalse_WhenInsufficientGold()
        {
            var currency = new UserCurrency { Gold = 500 };

            var result = _service.DeductGold(ref currency, 1000);

            Assert.That(result, Is.False);
            Assert.That(currency.Gold, Is.EqualTo(500)); // 변경 없음
        }

        [Test]
        public void DeductGold_ReturnsTrue_WhenEnoughGold()
        {
            var currency = new UserCurrency { Gold = 1000 };

            var result = _service.DeductGold(ref currency, 300);

            Assert.That(result, Is.True);
            Assert.That(currency.Gold, Is.EqualTo(700));
        }

        [Test]
        public void DeductGold_DeductsExactAmount()
        {
            var currency = new UserCurrency { Gold = 500 };

            var result = _service.DeductGold(ref currency, 500);

            Assert.That(result, Is.True);
            Assert.That(currency.Gold, Is.EqualTo(0));
        }

        #endregion

        #region CreateRewardDelta Tests

        [Test]
        public void CreateRewardDelta_AppliesCurrencyReward()
        {
            var initialGold = _testUserData.Currency.Gold;
            var rewards = new[] { RewardInfo.Currency(CostType.Gold, 1000) };

            var delta = _service.CreateRewardDelta(rewards, ref _testUserData);

            Assert.That(delta.Currency.HasValue, Is.True);
            Assert.That(_testUserData.Currency.Gold, Is.EqualTo(initialGold + 1000));
        }

        [Test]
        public void CreateRewardDelta_AppliesCharacterReward()
        {
            var initialCount = _testUserData.Characters.Count;
            var rewards = new[] { RewardInfo.Character("char_001") };

            var delta = _service.CreateRewardDelta(rewards, ref _testUserData);

            Assert.That(delta.AddedCharacters, Is.Not.Null);
            Assert.That(delta.AddedCharacters.Count, Is.EqualTo(1));
            Assert.That(_testUserData.Characters.Count, Is.EqualTo(initialCount + 1));
        }

        [Test]
        public void CreateRewardDelta_AppliesPlayerExpReward()
        {
            var initialExp = _testUserData.Profile.Exp;
            var rewards = new[] { RewardInfo.PlayerExp(500) };

            _service.CreateRewardDelta(rewards, ref _testUserData);

            Assert.That(_testUserData.Profile.Exp, Is.EqualTo(initialExp + 500));
        }

        [Test]
        public void CreateRewardDelta_HandlesMultipleRewards()
        {
            var rewards = new[]
            {
                RewardInfo.Currency(CostType.Gold, 100),
                RewardInfo.Character("char_001"),
                RewardInfo.PlayerExp(50)
            };

            var delta = _service.CreateRewardDelta(rewards, ref _testUserData);

            Assert.That(delta.Currency.HasValue, Is.True);
            Assert.That(delta.AddedCharacters.Count, Is.EqualTo(1));
        }

        [Test]
        public void CreateRewardDelta_IgnoresItemReward_WhenTodoNotImplemented()
        {
            // Item 보상은 TODO 상태이므로 무시됨
            var rewards = new[] { RewardInfo.Item("item_001", 5) };

            var delta = _service.CreateRewardDelta(rewards, ref _testUserData);

            // Item은 처리되지 않으므로 AddedCharacters도 null
            Assert.That(delta.AddedCharacters, Is.Null.Or.Empty);
        }

        #endregion
    }
}
