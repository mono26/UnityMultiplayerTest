using System;
using System.Collections.Generic;
using UnityEngine;

namespace SLGFramework
{
    public class ServiceProvider : SLGScript, IServiceProvider
    {
        private IServiceFactory serviceFactory = null;

        private List<IService> servicesList = new List<IService>();
        private Dictionary<Type, IService> services = new Dictionary<Type, IService>();

        public ServiceProvider() : base()
        {
            this.services = new Dictionary<Type, IService>();

            this.serviceFactory = new BaseServiceFactory(this);
        }

        public T GetService<T>() where T : IService
        {
            Type serviceType = typeof(T);
            if (this.services.ContainsKey(serviceType)) {
                return (T)this.services[serviceType];
            }

            IService service = this.serviceFactory.CreateService<T>();
            if (service != null && (service is T)) {
                service.Initialize();
                return (T)service;
            }
            else if (service != null && !(service is T)) {
                throw new InvalidOperationException($"Constructed object {service.GetType().Name} is not of type {serviceType.Name}.");
            }

#if UNITY_EDITOR
            Debug.LogError($"Can't get {serviceType.Name} service because it wasn't added.");
#endif

            return default(T);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (this.services == null) {
                this.services = new Dictionary<Type, IService>();
            }
        }

        public void AddService<T>(T service) where T : IService
        {
            if (this.services == null) {
                this.services = new Dictionary<Type, IService>();
            }

            Type serviceType = typeof(T);
            if (this.services.ContainsKey(serviceType)) {
#if UNITY_EDITOR
                Debug.LogWarning($"{serviceType.Name} service already added.");
#endif
            }
            else {
                this.services.Add(serviceType, service);
                this.servicesList.Add(service);
            }
        }

        public void RemoveService<T>(T service) where T : IService
        {
            Type serviceType = typeof(T);
            if (this.services.ContainsKey(serviceType)) {
                this.servicesList.Remove(this.services[serviceType]);
                this.services.Remove(serviceType);

            }
            else {
#if UNITY_EDITOR
                Debug.LogWarning($"{serviceType.Name} service can't be removed because is not added.");
#endif
            }
        }

        public void InitService<T>() where T : IService
        {
            Type serviceType = typeof(T);
            if (this.services.ContainsKey(serviceType)) {
                this.services[serviceType].Initialize();
            }
            else {
                IService service = this.serviceFactory.CreateService<T>();
                // Check if service is of type T or if service implements T (In the case of an interface.).
                if (service != null && (service.GetType() == serviceType || service.GetType().IsAssignableFrom(serviceType))) {
                    service.Initialize();
                }
                else if (service != null && !(service is T)) {
                    throw new InvalidOperationException($"Constructed object {service.GetType().Name} is not of type {serviceType.Name}.");
                }
            }
        }
    }
}

