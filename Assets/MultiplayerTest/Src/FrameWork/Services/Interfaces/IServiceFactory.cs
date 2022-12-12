namespace SLGFramework
{
    public interface IServiceFactory
    {
        IService CreateService<T>() where T : IService;
        IService CreateDefaultService<T>() where T : IService;
    }
}
