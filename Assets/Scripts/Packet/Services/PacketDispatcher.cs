using System;
using System.Collections.Generic;
using Sc.Core;
using UnityEngine;

namespace Sc.Packet
{
    /// <summary>
    /// 패킷 응답 디스패처
    /// 응답을 이벤트로 변환하여 EventManager로 발행
    /// </summary>
    public class PacketDispatcher
    {
        private readonly Dictionary<Type, IPacketHandler> _handlers = new();

        /// <summary>
        /// 응답 핸들러 등록
        /// </summary>
        /// <typeparam name="TResponse">응답 타입</typeparam>
        /// <param name="handler">핸들러</param>
        public void RegisterHandler<TResponse>(IPacketHandler<TResponse> handler)
            where TResponse : IResponse
        {
            var type = typeof(TResponse);
            if (_handlers.ContainsKey(type))
            {
                Debug.LogWarning($"[PacketDispatcher] Handler already registered for {type.Name}");
                return;
            }

            _handlers[type] = handler;
            Debug.Log($"[PacketDispatcher] Registered handler for {type.Name}");
        }

        /// <summary>
        /// 응답 핸들러 해제
        /// </summary>
        /// <typeparam name="TResponse">응답 타입</typeparam>
        public void UnregisterHandler<TResponse>()
            where TResponse : IResponse
        {
            _handlers.Remove(typeof(TResponse));
        }

        /// <summary>
        /// 응답 처리 (이벤트 발행)
        /// </summary>
        /// <param name="request">원본 요청</param>
        /// <param name="response">응답</param>
        public void Dispatch(QueuedRequest request, IResponse response)
        {
            if (response == null)
            {
                Debug.LogError($"[PacketDispatcher] Null response for {request.Metadata.RequestType}");
                DispatchError(request, new Exception("Null response"));
                return;
            }

            var responseType = response.GetType();

            // 등록된 핸들러로 처리
            if (_handlers.TryGetValue(responseType, out var handler))
            {
                try
                {
                    handler.HandleResponse(response);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[PacketDispatcher] Handler error for {responseType.Name}: {ex.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"[PacketDispatcher] No handler for {responseType.Name}");
            }

            // 공통 완료 이벤트 발행
            EventManager.Instance.Publish(new RequestCompletedEvent
            {
                RequestId = request.RequestId,
                RequestType = request.Metadata.RequestType,
                IsSuccess = response.IsSuccess,
                ErrorCode = response.ErrorCode,
                ErrorMessage = response.ErrorMessage
            });
        }

        /// <summary>
        /// 에러 처리 (에러 이벤트 발행)
        /// </summary>
        /// <param name="request">원본 요청</param>
        /// <param name="exception">예외</param>
        public void DispatchError(QueuedRequest request, Exception exception)
        {
            Debug.LogError($"[PacketDispatcher] Error for {request.Metadata.RequestType}: {exception.Message}");

            // 네트워크 에러 이벤트 발행
            EventManager.Instance.Publish(new NetworkErrorEvent
            {
                RequestId = request.RequestId,
                RequestType = request.Metadata.RequestType,
                ErrorCode = -1,
                ErrorMessage = exception.Message,
                IsRecoverable = exception is TimeoutException || exception is System.Net.WebException
            });
        }

        /// <summary>
        /// 모든 핸들러 제거
        /// </summary>
        public void ClearAll()
        {
            _handlers.Clear();
        }
    }

    /// <summary>
    /// 패킷 핸들러 기본 인터페이스
    /// </summary>
    public interface IPacketHandler
    {
        /// <summary>
        /// 응답 처리 (비제네릭)
        /// </summary>
        void HandleResponse(IResponse response);
    }

    /// <summary>
    /// 타입별 패킷 핸들러 인터페이스
    /// </summary>
    /// <typeparam name="TResponse">응답 타입</typeparam>
    public interface IPacketHandler<TResponse> : IPacketHandler
        where TResponse : IResponse
    {
        /// <summary>
        /// 응답 처리 (타입 안전)
        /// </summary>
        void Handle(TResponse response);
    }

    /// <summary>
    /// 패킷 핸들러 기본 구현
    /// </summary>
    /// <typeparam name="TResponse">응답 타입</typeparam>
    public abstract class PacketHandlerBase<TResponse> : IPacketHandler<TResponse>
        where TResponse : IResponse
    {
        public void HandleResponse(IResponse response)
        {
            if (response is TResponse typedResponse)
            {
                Handle(typedResponse);
            }
        }

        public abstract void Handle(TResponse response);
    }
}
