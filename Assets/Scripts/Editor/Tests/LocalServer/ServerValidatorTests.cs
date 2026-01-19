using NUnit.Framework;
using Sc.Data;
using Sc.LocalServer;

namespace Sc.Editor.Tests.LocalServer
{
    /// <summary>
    /// ServerValidator 단위 테스트.
    /// 재화 및 캐릭터 보유 검증 로직.
    /// </summary>
    [TestFixture]
    public class ServerValidatorTests
    {
        private ServerValidator _validator;
        private UserSaveData _testUserData;

        [SetUp]
        public void SetUp()
        {
            var timeService = new ServerTimeService();
            _validator = new ServerValidator(timeService);
            _testUserData = UserSaveData.CreateNew("test_uid", "TestUser");
        }

        #region HasEnoughGem Tests

        [Test]
        public void HasEnoughGem_ReturnsTrue_WhenTotalGemSufficient()
        {
            var currency = new UserCurrency { Gem = 100, FreeGem = 200 }; // Total: 300

            var result = _validator.HasEnoughGem(currency, 250);

            Assert.That(result, Is.True);
        }

        [Test]
        public void HasEnoughGem_ReturnsFalse_WhenTotalGemInsufficient()
        {
            var currency = new UserCurrency { Gem = 100, FreeGem = 100 }; // Total: 200

            var result = _validator.HasEnoughGem(currency, 300);

            Assert.That(result, Is.False);
        }

        [Test]
        public void HasEnoughGem_ReturnsTrue_WhenExactAmount()
        {
            var currency = new UserCurrency { Gem = 150, FreeGem = 150 }; // Total: 300

            var result = _validator.HasEnoughGem(currency, 300);

            Assert.That(result, Is.True);
        }

        #endregion

        #region HasEnoughGold Tests

        [Test]
        public void HasEnoughGold_ReturnsTrue_WhenGoldSufficient()
        {
            var currency = new UserCurrency { Gold = 10000 };

            var result = _validator.HasEnoughGold(currency, 5000);

            Assert.That(result, Is.True);
        }

        [Test]
        public void HasEnoughGold_ReturnsFalse_WhenGoldInsufficient()
        {
            var currency = new UserCurrency { Gold = 1000 };

            var result = _validator.HasEnoughGold(currency, 5000);

            Assert.That(result, Is.False);
        }

        [Test]
        public void HasEnoughGold_ReturnsTrue_WhenExactAmount()
        {
            var currency = new UserCurrency { Gold = 1000 };

            var result = _validator.HasEnoughGold(currency, 1000);

            Assert.That(result, Is.True);
        }

        #endregion

        #region HasEnoughStamina Tests

        [Test]
        public void HasEnoughStamina_ReturnsTrue_WhenStaminaSufficient()
        {
            var currency = new UserCurrency { Stamina = 100 };

            var result = _validator.HasEnoughStamina(currency, 20);

            Assert.That(result, Is.True);
        }

        [Test]
        public void HasEnoughStamina_ReturnsFalse_WhenStaminaInsufficient()
        {
            var currency = new UserCurrency { Stamina = 10 };

            var result = _validator.HasEnoughStamina(currency, 20);

            Assert.That(result, Is.False);
        }

        #endregion

        #region HasCharacter Tests

        [Test]
        public void HasCharacter_ReturnsTrue_WhenCharacterOwned()
        {
            _testUserData.Characters.Add(OwnedCharacter.Create("char_001"));

            var result = _validator.HasCharacter(_testUserData, "char_001");

            Assert.That(result, Is.True);
        }

        [Test]
        public void HasCharacter_ReturnsFalse_WhenCharacterNotOwned()
        {
            _testUserData.Characters.Add(OwnedCharacter.Create("char_001"));

            var result = _validator.HasCharacter(_testUserData, "char_999");

            Assert.That(result, Is.False);
        }

        [Test]
        public void HasCharacter_ReturnsFalse_WhenCharactersEmpty()
        {
            _testUserData.Characters.Clear();

            var result = _validator.HasCharacter(_testUserData, "char_001");

            Assert.That(result, Is.False);
        }

        #endregion
    }
}
