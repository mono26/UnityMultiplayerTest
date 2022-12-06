using UnityEngine;

namespace SLGFramework
{
    public class SLGScript
    {
        private bool isInitialized = false;

        /// <summary>
        /// Should be called after instantiation. Better used for initializing everything to start executing.
        /// </summary>
        public void Initialize()
        {
            if (this.isInitialized) {
                return;
            }

            this.OnInitialize();

            this.isInitialized = true;
        }

        protected virtual void OnInitialize() { }
    }
}