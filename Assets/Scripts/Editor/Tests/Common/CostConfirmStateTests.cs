using NUnit.Framework;
using Sc.Common.UI;
using Sc.Data;

namespace Sc.Editor.Tests.Common
{
    /// <summary>
    /// CostConfirmState 단위 테스트
    /// </summary>
    [TestFixture]
    public class CostConfirmStateTests
    {
        #region Default Values Tests

        [Test]
        public void DefaultValues_AreSetCorrectly()
        {
            var state = new CostConfirmState();

            Assert.That(state.Title, Is.EqualTo("확인"));
            Assert.That(state.Message, Is.EqualTo(""));
            Assert.That(state.CostType, Is.EqualTo(CostType.Gold));
            Assert.That(state.CostAmount, Is.EqualTo(0));
            Assert.That(state.CurrentAmount, Is.Null);
            Assert.That(state.ConfirmText, Is.EqualTo("확인"));
            Assert.That(state.CancelText, Is.EqualTo("취소"));
            Assert.That(state.OnConfirm, Is.Null);
            Assert.That(state.OnCancel, Is.Null);
        }

        #endregion

        #region Property Setting Tests

        [Test]
        public void Title_CanBeSet()
        {
            var state = new CostConfirmState { Title = "구매 확인" };

            Assert.That(state.Title, Is.EqualTo("구매 확인"));
        }

        [Test]
        public void CostType_CanBeSet()
        {
            var state = new CostConfirmState { CostType = CostType.Gem };

            Assert.That(state.CostType, Is.EqualTo(CostType.Gem));
        }

        [Test]
        public void CostType_Stamina_CanBeSet()
        {
            var state = new CostConfirmState { CostType = CostType.Stamina };

            Assert.That(state.CostType, Is.EqualTo(CostType.Stamina));
        }

        [Test]
        public void CostAmount_CanBeSet()
        {
            var state = new CostConfirmState { CostAmount = 100 };

            Assert.That(state.CostAmount, Is.EqualTo(100));
        }

        [Test]
        public void CurrentAmount_CanBeSet()
        {
            var state = new CostConfirmState { CurrentAmount = 500 };

            Assert.That(state.CurrentAmount, Is.EqualTo(500));
        }

        [Test]
        public void CurrentAmount_CanBeNull()
        {
            var state = new CostConfirmState { CurrentAmount = null };

            Assert.That(state.CurrentAmount, Is.Null);
            Assert.That(state.CurrentAmount.HasValue, Is.False);
        }

        #endregion

        #region IsInsufficient Tests

        [Test]
        public void IsInsufficient_WhenCurrentAmountIsNull_ReturnsFalse()
        {
            var state = new CostConfirmState
            {
                CostAmount = 100,
                CurrentAmount = null
            };

            Assert.That(state.IsInsufficient, Is.False);
        }

        [Test]
        public void IsInsufficient_WhenCurrentAmountLessThanCostAmount_ReturnsTrue()
        {
            var state = new CostConfirmState
            {
                CostAmount = 100,
                CurrentAmount = 50
            };

            Assert.That(state.IsInsufficient, Is.True);
        }

        [Test]
        public void IsInsufficient_WhenCurrentAmountEqualsToCostAmount_ReturnsFalse()
        {
            var state = new CostConfirmState
            {
                CostAmount = 100,
                CurrentAmount = 100
            };

            Assert.That(state.IsInsufficient, Is.False);
        }

        [Test]
        public void IsInsufficient_WhenCurrentAmountGreaterThanCostAmount_ReturnsFalse()
        {
            var state = new CostConfirmState
            {
                CostAmount = 100,
                CurrentAmount = 500
            };

            Assert.That(state.IsInsufficient, Is.False);
        }

        [Test]
        public void IsInsufficient_WhenCurrentAmountIsZero_ReturnsTrue()
        {
            var state = new CostConfirmState
            {
                CostAmount = 100,
                CurrentAmount = 0
            };

            Assert.That(state.IsInsufficient, Is.True);
        }

        #endregion

        #region Validate Tests

        [Test]
        public void Validate_WithValidCostAmount_ReturnsTrue()
        {
            var state = new CostConfirmState { CostAmount = 100 };

            var result = state.Validate();

            Assert.That(result, Is.True);
        }

        [Test]
        public void Validate_WithZeroCostAmount_ReturnsFalse()
        {
            var state = new CostConfirmState { CostAmount = 0 };

            var result = state.Validate();

            Assert.That(result, Is.False);
        }

        [Test]
        public void Validate_WithNegativeCostAmount_ReturnsFalse()
        {
            var state = new CostConfirmState { CostAmount = -100 };

            var result = state.Validate();

            Assert.That(result, Is.False);
        }

        [Test]
        public void Validate_WithMinimumValidCostAmount_ReturnsTrue()
        {
            var state = new CostConfirmState { CostAmount = 1 };

            var result = state.Validate();

            Assert.That(result, Is.True);
        }

        #endregion

        #region Callback Tests

        [Test]
        public void OnConfirm_CanBeSet()
        {
            bool called = false;
            var state = new CostConfirmState { OnConfirm = () => called = true };

            state.OnConfirm?.Invoke();

            Assert.That(called, Is.True);
        }

        [Test]
        public void OnCancel_CanBeSet()
        {
            bool called = false;
            var state = new CostConfirmState { OnCancel = () => called = true };

            state.OnCancel?.Invoke();

            Assert.That(called, Is.True);
        }

        #endregion

        #region Real Usage Scenario Tests

        [Test]
        public void PurchaseScenario_WithSufficientGems()
        {
            var state = new CostConfirmState
            {
                Title = "구매 확인",
                Message = "이 아이템을 구매하시겠습니까?",
                CostType = CostType.Gem,
                CostAmount = 100,
                CurrentAmount = 500,
                ConfirmText = "구매"
            };

            Assert.That(state.Validate(), Is.True);
            Assert.That(state.IsInsufficient, Is.False);
        }

        [Test]
        public void PurchaseScenario_WithInsufficientGems()
        {
            var state = new CostConfirmState
            {
                Title = "구매 확인",
                Message = "이 아이템을 구매하시겠습니까?",
                CostType = CostType.Gem,
                CostAmount = 100,
                CurrentAmount = 50,
                ConfirmText = "구매"
            };

            Assert.That(state.Validate(), Is.True);
            Assert.That(state.IsInsufficient, Is.True);
        }

        [Test]
        public void StaminaScenario_WithSufficientStamina()
        {
            var state = new CostConfirmState
            {
                Title = "출전 확인",
                Message = "스테이지에 출전합니다.",
                CostType = CostType.Stamina,
                CostAmount = 10,
                CurrentAmount = 100,
                ConfirmText = "출전"
            };

            Assert.That(state.Validate(), Is.True);
            Assert.That(state.IsInsufficient, Is.False);
        }

        [Test]
        public void EnhanceScenario_WithGold()
        {
            var state = new CostConfirmState
            {
                Title = "강화 확인",
                Message = "장비를 강화하시겠습니까?",
                CostType = CostType.Gold,
                CostAmount = 5000,
                CurrentAmount = 10000,
                ConfirmText = "강화"
            };

            Assert.That(state.Validate(), Is.True);
            Assert.That(state.IsInsufficient, Is.False);
        }

        #endregion
    }
}
