using System;
using System.Collections.Generic;

namespace Sc.Foundation
{
    /// <summary>
    /// 간단한 ServiceLocator 패턴 구현.
    /// Inspector SO 할당의 폴백으로 사용하거나 테스트 시 Mock 주입에 활용.
    /// </summary>
    public static class Services
    {
        private static readonly Dictionary<Type, object> _services = new();

        /// <summary>
        /// 서비스 등록
        /// </summary>
        public static void Register<T>(T service) where T : class
        {
            _services[typeof(T)] = service;
        }

        /// <summary>
        /// 서비스 조회 (없으면 null)
        /// </summary>
        public static T Get<T>() where T : class
        {
            return _services.TryGetValue(typeof(T), out var service)
                ? (T)service
                : null;
        }

        /// <summary>
        /// 서비스 조회 시도
        /// </summary>
        public static bool TryGet<T>(out T service) where T : class
        {
            service = Get<T>();
            return service != null;
        }

        /// <summary>
        /// 서비스 등록 해제
        /// </summary>
        public static void Unregister<T>() where T : class
        {
            _services.Remove(typeof(T));
        }

        /// <summary>
        /// 모든 서비스 제거 (테스트 정리용)
        /// </summary>
        public static void Clear()
        {
            _services.Clear();
        }

        /// <summary>
        /// 등록된 서비스 수
        /// </summary>
        public static int Count => _services.Count;
    }
}
