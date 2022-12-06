using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SLGFramework
{
    public class EventManager : BaseService<IEventManager>, IEventManager
    {
        private Queue eventQueue = new Queue();
        private bool limitQueueProcesing = false;
        private float queueProcessTime = 0;

        private Dictionary<Type, EventDelegate> delegates = new Dictionary<Type, EventDelegate>();
        private Dictionary<Delegate, EventDelegate> delegateLookup = new Dictionary<Delegate, EventDelegate>();
        private Dictionary<Delegate, Delegate> onceLookups = new Dictionary<Delegate, Delegate>();

        public EventManager(IServiceProvider provider) : base(provider) { }

        public void AddListener<T>(EventDelegate<T> del) where T : IGameEvent
        {
            this.AddDelegate<T>(del);
        }

        public void AddListenerOnce<T>(EventDelegate<T> del) where T : IGameEvent
        {
            EventDelegate result = this.AddDelegate<T>(del);

            if (result != null) {
                this.onceLookups[result] = del;
            }
        }

        public void RemoveListener<T>(EventDelegate<T> del) where T : IGameEvent
        {
            EventDelegate internalDelegate;

            if (this.delegateLookup.TryGetValue(del, out internalDelegate)) {
                EventDelegate tempDel;

                if (this.delegates.TryGetValue(typeof(T), out tempDel)) {
                    tempDel -= internalDelegate;

                    if (tempDel == null) {
                        this.delegates.Remove(typeof(T));
                    }
                    else {
                        this.delegates[typeof(T)] = tempDel;
                    }
                }

                this.delegateLookup.Remove(del);
            }
        }

        public bool HasListener<T>(EventDelegate<T> del) where T : IGameEvent
        {
            return this.delegateLookup.ContainsKey(del);
        }

        public void Trigger(IGameEvent e)
        {
            EventDelegate del;

            if (this.delegates.TryGetValue(e.GetType(), out del)) {
                del.Invoke(e);

                foreach (EventDelegate k in this.delegates[e.GetType()].GetInvocationList()) {
                    if (this.onceLookups.ContainsKey(k)) {
                        this.delegates[e.GetType()] -= k;

                        if (this.delegates[e.GetType()] == null) {
                            this.delegates.Remove(e.GetType());
                        }

                        this.delegateLookup.Remove(onceLookups[k]);
                        this.onceLookups.Remove(k);
                    }
                }
            }
            else {
                Debug.Log($"Event '{e.GetType().ToString()}' has no listeners...");
            }
        }

        private EventDelegate AddDelegate<T>(EventDelegate<T> del) where T : IGameEvent
        {
            if (this.delegateLookup.ContainsKey(del)) {
                return null;
            }

            EventDelegate tempDel;
            EventDelegate internalDelegate = (e) => del((T)e);

            this.delegateLookup[del] = internalDelegate;

            if (this.delegates.TryGetValue(typeof(T), out tempDel)) {
                this.delegates[typeof(T)] = tempDel += internalDelegate;
            }
            else {
                this.delegates[typeof(T)] = internalDelegate;
            }

            return internalDelegate;
        }
    }
}
