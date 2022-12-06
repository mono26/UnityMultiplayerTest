using System;
using UnityEngine;

namespace SLGFramework
{
    public class BaseServiceFactory : IServiceFactory
    {
        private IServiceProvider provider = null;

        public BaseServiceFactory(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public IService CreateDefaultService<T>() where T : IService
        {
            IService service = default(T);

            Type serviceType = typeof(T);

            if (serviceType == typeof(IEventManager)) {
                service = new EventManager(provider);
            }

            return service;
        }

        public IService CreateService<T>() where T : IService
        {
            Type serviceType = typeof(T);
            if (serviceType.IsInterface) {
                return this.CreateDefaultService<T>();
            }
            else {
                return (IService)Activator.CreateInstance(serviceType, args: new object[1] { this.provider });
            }
        }
    }
}
