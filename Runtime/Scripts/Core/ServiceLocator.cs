using System;
using System.Collections.Generic;

namespace Moonstone.Core
{
    /// <summary>
    /// 전역 서비스 등록 및 조회를 위한 Service Locator
    /// </summary>
    public static class ServiceLocator
    {
        static readonly Dictionary<Type, object> services = new();

        /// <summary>
        /// 서비스 등록
        /// </summary>
        public static void Register<T>(T service) where T : class
        {
            if (service == null) { throw new ArgumentNullException(nameof(service)); }

            var type = service.GetType();
            if (services.ContainsKey(type))
            {
                services[type] = service;
            }
            else
            {
                services.Add(type, service);
            }
        }

        /// <summary>
        /// 서비스 조회
        /// </summary>
        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var service))
            {
                return service as T;
            }
            throw new Exception($"ServiceLocator: {type} 서비스가 등록되어 있지 않습니다.");
        }

        /// <summary>
        /// 서비스 해제
        /// </summary>
        public static void Unregister<T>() where T : class
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                services.Remove(type);
            }
        }

        public static void Unregister<T>(T service) where T : class
        {
            var type = service.GetType();
            if (services.ContainsKey(type))
            {
                services.Remove(type);
            }
        }

        /// <summary>
        /// 모든 서비스 해제 (테스트/리셋용)
        /// </summary>
        public static void Clear()
        {
            services.Clear();
        }
    }
}
