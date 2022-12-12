#if UNITY_SERVER
#define GAME_SERVER
#undef GAME_CLIENT
#endif

using UnityEngine;

namespace SLGFramework
{
    public class DestroyInServer : SLGBehaviour
    {
#if GAME_SERVER
        private void Awake()
        {
            this.Initialize();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            GameObject.Destroy(this.gameObject);
        }
#endif
    }
}