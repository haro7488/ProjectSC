using System;
using Sc.Data;
using UnityEngine;

namespace Sc.LocalServer
{
    /// <summary>
    /// 로컬 게임 서버 (서버 시뮬레이션 진입점)
    /// 요청을 적절한 Handler로 라우팅하고 응답을 반환
    /// </summary>
    public class LocalGameServer
    {
        private readonly LoginHandler _loginHandler;
        private readonly GachaHandler _gachaHandler;
        private readonly ShopHandler _shopHandler;
        private readonly ServerTimeService _timeService;

        public LocalGameServer()
        {
            _timeService = new ServerTimeService();
            var validator = new ServerValidator(_timeService);
            var rewardService = new RewardService();
            var gachaService = new GachaService();

            _loginHandler = new LoginHandler();
            _gachaHandler = new GachaHandler(validator, gachaService, rewardService, _timeService);
            _shopHandler = new ShopHandler(validator, rewardService, _timeService);
        }

        /// <summary>
        /// 요청 처리 (타입별 라우팅)
        /// </summary>
        public IResponse HandleRequest(IRequest request, ref UserSaveData userData)
        {
            try
            {
                return request switch
                {
                    LoginRequest loginRequest => _loginHandler.Handle(loginRequest, ref userData),
                    GachaRequest gachaRequest => _gachaHandler.Handle(gachaRequest, ref userData),
                    ShopPurchaseRequest shopRequest => _shopHandler.Handle(shopRequest, ref userData),
                    _ => throw new NotImplementedException($"Handler not found for {request.GetType().Name}")
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LocalGameServer] Request handling failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 서버 시간 조회
        /// </summary>
        public long GetServerTime() => _timeService.ServerTimeUtc;
    }
}
