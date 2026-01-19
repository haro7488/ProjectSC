using NUnit.Framework;
using Sc.Data;
using Sc.LocalServer;

namespace Sc.Editor.Tests.LocalServer
{
    /// <summary>
    /// LoginHandler 단위 테스트.
    /// 신규/기존 유저 로그인 처리 검증.
    /// </summary>
    [TestFixture]
    public class LoginHandlerTests
    {
        private LoginHandler _handler;
        private UserSaveData _emptyUserData;
        private UserSaveData _existingUserData;

        [SetUp]
        public void SetUp()
        {
            _handler = new LoginHandler();
            // 빈 Uid = 신규 유저로 판단됨 (LoginHandler.Handle에서 string.IsNullOrEmpty 체크)
            _emptyUserData = UserSaveData.CreateNew("", "");
            _existingUserData = UserSaveData.CreateNew("existing_uid", "ExistingUser");
        }

        #region NewUser Tests

        [Test]
        public void Handle_CreatesNewUser_WhenUidEmpty()
        {
            var request = LoginRequest.CreateGuest("device_001", "1.0.0", "Editor");

            var response = _handler.Handle(request, ref _emptyUserData);

            Assert.That(response.IsSuccess, Is.True);
            Assert.That(_emptyUserData.Profile.Uid, Is.Not.Null.And.Not.Empty);
            Assert.That(_emptyUserData.Profile.Nickname, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void Handle_AssignsInitialCharacter_ForNewUser()
        {
            var request = LoginRequest.CreateGuest("device_001", "1.0.0", "Editor");

            _handler.Handle(request, ref _emptyUserData);

            Assert.That(_emptyUserData.Characters.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(_emptyUserData.Characters[0].CharacterId, Is.EqualTo("char_005"));
        }

        [Test]
        public void Handle_ReturnsIsNewUserTrue_ForNewUser()
        {
            var request = LoginRequest.CreateGuest("device_001", "1.0.0", "Editor");

            var response = _handler.Handle(request, ref _emptyUserData);

            Assert.That(response.IsNewUser, Is.True);
        }

        #endregion

        #region ExistingUser Tests

        [Test]
        public void Handle_UpdatesLastLoginAt_ForExistingUser()
        {
            var request = LoginRequest.CreateRelogin("existing_uid", "1.0.0", "Editor");
            var previousLoginAt = _existingUserData.Profile.LastLoginAt;

            _handler.Handle(request, ref _existingUserData);

            Assert.That(_existingUserData.Profile.LastLoginAt, Is.GreaterThanOrEqualTo(previousLoginAt));
        }

        [Test]
        public void Handle_ReturnsIsNewUserFalse_ForExistingUser()
        {
            var request = LoginRequest.CreateRelogin("existing_uid", "1.0.0", "Editor");

            var response = _handler.Handle(request, ref _existingUserData);

            Assert.That(response.IsNewUser, Is.False);
        }

        [Test]
        public void Handle_ReturnsSessionToken_Always()
        {
            var request = LoginRequest.CreateGuest("device_001", "1.0.0", "Editor");

            var response = _handler.Handle(request, ref _emptyUserData);

            Assert.That(response.SessionToken, Is.Not.Null.And.Not.Empty);
        }

        #endregion
    }
}
