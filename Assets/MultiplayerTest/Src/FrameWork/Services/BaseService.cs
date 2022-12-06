using UnityEngine;

namespace SLGFramework
{
    public abstract class BaseService<T> : SLGScript, IService where T : class, IService
    {
        protected IServiceProvider provider = null;

        public BaseService(IServiceProvider provider)
        {
            this.provider = provider;

            this.AddService();
        }

        public virtual void OnDispose()
        {
            this.RemoveService();
        }

        public void AddService()
        {
            if (this.provider != null) {
                this.provider.AddService(this as T);
            }
            else {
#if UNITY_EDITOR
                Debug.Log($"No {nameof(this.provider)} reference found.");
#endif
            }
        }

        public void RemoveService()
        {
            if (this.provider != null) {
                this.provider.RemoveService(this as T);
            }
            else {
#if UNITY_EDITOR
                Debug.Log($"No {nameof(this.provider)} reference found.");
#endif
            }
        }
    }
}
