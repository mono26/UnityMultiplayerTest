namespace SLGFramework
{
    public delegate void EventDelegate<T>(T e) where T : IGameEvent;
    public delegate void EventDelegate(IGameEvent e);

    public interface IEventManager : IService
    {
        void AddListener<T>(EventDelegate<T> del) where T : IGameEvent;

        void AddListenerOnce<T>(EventDelegate<T> del) where T : IGameEvent;

        void RemoveListener<T>(EventDelegate<T> del) where T : IGameEvent;

        bool HasListener<T>(EventDelegate<T> del) where T : IGameEvent;

        void Trigger(IGameEvent e);
    }
}
