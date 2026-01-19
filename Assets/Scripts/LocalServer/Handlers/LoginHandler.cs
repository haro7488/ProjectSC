using System;
using Sc.Data;
using UnityEngine;

namespace Sc.LocalServer
{
    /// <summary>
    /// 로그인 요청 핸들러 (서버측)
    /// 신규 유저 생성 또는 기존 유저 로그인 처리
    /// </summary>
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        public LoginResponse Handle(LoginRequest request, ref UserSaveData userData)
        {
            bool isNewUser = false;

            // 기존 유저 데이터가 없으면 신규 생성
            if (string.IsNullOrEmpty(userData.Profile.Uid))
            {
                var uid = request.UserId ?? Guid.NewGuid().ToString();
                var nickname = $"Player_{uid.Substring(0, 6)}";
                userData = UserSaveData.CreateNew(uid, nickname);
                isNewUser = true;

                // 초기 캐릭터 지급 (튜토리얼 캐릭터)
                userData.Characters.Add(OwnedCharacter.Create("char_005"));

                Debug.Log($"[LoginHandler] 신규 유저 생성: {nickname}");
            }
            else
            {
                // 기존 유저 로그인
                userData.Profile.LastLoginAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                Debug.Log($"[LoginHandler] 기존 유저 로그인: {userData.Profile.Nickname}");
            }

            var sessionToken = Guid.NewGuid().ToString();

            return LoginResponse.Success(userData, isNewUser, sessionToken);
        }
    }
}
