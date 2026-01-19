using Sc.Data;

namespace Sc.LocalServer
{
    /// <summary>
    /// 서버측 요청 핸들러 인터페이스
    /// 각 Handler는 특정 Request 타입을 처리하여 Response를 반환
    /// </summary>
    public interface IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest
        where TResponse : IResponse
    {
        /// <summary>
        /// 요청 처리
        /// </summary>
        /// <param name="request">클라이언트 요청</param>
        /// <param name="userData">현재 유저 데이터 (읽기/수정용)</param>
        /// <returns>처리 결과 응답</returns>
        TResponse Handle(TRequest request, ref UserSaveData userData);
    }
}
