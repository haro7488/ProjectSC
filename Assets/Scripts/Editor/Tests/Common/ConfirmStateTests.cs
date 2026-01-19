using NUnit.Framework;
using Sc.Common.UI;

namespace Sc.Editor.Tests.Common
{
    /// <summary>
    /// ConfirmState 단위 테스트
    /// </summary>
    [TestFixture]
    public class ConfirmStateTests
    {
        #region Default Values Tests

        [Test]
        public void DefaultValues_AreSetCorrectly()
        {
            var state = new ConfirmState();

            Assert.That(state.Title, Is.EqualTo("확인"));
            Assert.That(state.Message, Is.EqualTo(""));
            Assert.That(state.ConfirmText, Is.EqualTo("확인"));
            Assert.That(state.CancelText, Is.EqualTo("취소"));
            Assert.That(state.ShowCancelButton, Is.True);
            Assert.That(state.OnConfirm, Is.Null);
            Assert.That(state.OnCancel, Is.Null);
        }

        #endregion

        #region Property Setting Tests

        [Test]
        public void Title_CanBeSet()
        {
            var state = new ConfirmState { Title = "테스트 제목" };

            Assert.That(state.Title, Is.EqualTo("테스트 제목"));
        }

        [Test]
        public void Message_CanBeSet()
        {
            var state = new ConfirmState { Message = "테스트 메시지" };

            Assert.That(state.Message, Is.EqualTo("테스트 메시지"));
        }

        [Test]
        public void ConfirmText_CanBeSet()
        {
            var state = new ConfirmState { ConfirmText = "예" };

            Assert.That(state.ConfirmText, Is.EqualTo("예"));
        }

        [Test]
        public void CancelText_CanBeSet()
        {
            var state = new ConfirmState { CancelText = "아니오" };

            Assert.That(state.CancelText, Is.EqualTo("아니오"));
        }

        [Test]
        public void ShowCancelButton_CanBeSetToFalse()
        {
            var state = new ConfirmState { ShowCancelButton = false };

            Assert.That(state.ShowCancelButton, Is.False);
        }

        #endregion

        #region Callback Tests

        [Test]
        public void OnConfirm_CanBeSet()
        {
            bool called = false;
            var state = new ConfirmState { OnConfirm = () => called = true };

            state.OnConfirm?.Invoke();

            Assert.That(called, Is.True);
        }

        [Test]
        public void OnCancel_CanBeSet()
        {
            bool called = false;
            var state = new ConfirmState { OnCancel = () => called = true };

            state.OnCancel?.Invoke();

            Assert.That(called, Is.True);
        }

        #endregion

        #region Validate Tests

        [Test]
        public void Validate_WithValidMessage_ReturnsTrue()
        {
            var state = new ConfirmState { Message = "유효한 메시지" };

            var result = state.Validate();

            Assert.That(result, Is.True);
        }

        [Test]
        public void Validate_WithEmptyMessage_ReturnsTrue()
        {
            // Note: Empty message logs warning but still returns true (display allowed)
            var state = new ConfirmState { Message = "" };

            var result = state.Validate();

            Assert.That(result, Is.True);
        }

        [Test]
        public void Validate_WithNullMessage_ReturnsTrue()
        {
            // Note: Null message logs warning but still returns true (display allowed)
            var state = new ConfirmState { Message = null };

            var result = state.Validate();

            Assert.That(result, Is.True);
        }

        #endregion

        #region Alert Mode Tests

        [Test]
        public void AlertMode_CanBeConfigured()
        {
            var state = new ConfirmState
            {
                Title = "알림",
                Message = "스태미나가 부족합니다.",
                ShowCancelButton = false
            };

            Assert.That(state.Title, Is.EqualTo("알림"));
            Assert.That(state.ShowCancelButton, Is.False);
        }

        #endregion
    }
}
