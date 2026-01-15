using System;
using Cysharp.Threading.Tasks;
using Sc.Data;

namespace Sc.Packet
{
    /// <summary>
    /// API 서비스 인터페이스
    /// 서버 통신 (또는 로컬 시뮬레이션) 담당
    /// </summary>
    /// <remarks>
    /// [DEPRECATED] NetworkManager + IApiClient 아키텍처로 대체됨
    /// 새로운 코드에서는 NetworkManager.Instance.Send() 사용
    /// </remarks>
    [Obsolete("Use NetworkManager + IApiClient instead. This interface is deprecated.")]
    public interface IApiService
    {
        /// <summary>
        /// 서비스 초기화
        /// </summary>
        UniTask<bool> InitializeAsync();

        /// <summary>
        /// 로그인
        /// </summary>
        UniTask<LoginResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// 유저 데이터 전체 조회
        /// </summary>
        UniTask<UserSaveData> FetchUserDataAsync();

        /// <summary>
        /// 가챠 실행
        /// </summary>
        UniTask<GachaResponse> GachaAsync(GachaRequest request);

        /// <summary>
        /// 상점 구매
        /// </summary>
        UniTask<ShopPurchaseResponse> PurchaseAsync(ShopPurchaseRequest request);

        /// <summary>
        /// 범용 요청 (확장용)
        /// </summary>
        UniTask<TResponse> SendAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IRequest
            where TResponse : IResponse;

        /// <summary>
        /// 서비스 상태
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// 현재 로그인된 유저 ID
        /// </summary>
        string CurrentUserId { get; }
    }
}
