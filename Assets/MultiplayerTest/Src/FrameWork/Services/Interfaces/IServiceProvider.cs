using System;

namespace SLGFramework
{
    public interface IServiceProvider
    {
        T GetService<T>() where T : IService;

        void AddService<T>(T service) where T : IService;
        void RemoveService<T>(T service) where T : IService;

        void InitService<T>() where T : IService;
    }
}
