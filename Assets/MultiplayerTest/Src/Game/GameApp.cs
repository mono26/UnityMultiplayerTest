#if UNITY_SERVER
#define GAME_SERVER
#undef GAME_CLIENT
#endif

using Fusion;
using SLGFramework;
#if GAME_SERVER
using UnityEngine;
#endif

namespace MultiplayerTest
{
    public class GameApp : SLGScript
    {
#if GAME_SERVER
        private GameServer server = null;
#elif GAME_CLIENT
        private GameClient client = null;
#endif

        private ServiceProvider serviceProvider = null;

        public string AppVersion { get; private set; } = "1";

        public GameApp()
        {
            this.AppVersion = "1";

            this.serviceProvider = new ServiceProvider();

            Log.Info("Created gameapp instance.");
        }

        protected override void OnInitialize()
        {
            Log.Info("Initialize gameapp instance.");

            base.OnInitialize();

            this.serviceProvider.InitService<IEventManager>();

#if GAME_SERVER
            // Limit frame rate in server.
            Application.targetFrameRate = 30;
#endif
        }

        private void BeginApp()
        {
            Log.Info("Begin gameapp instance.");

#if GAME_SERVER
            GameServer.Instance.SetAppReference(this);
            GameServer.Instance.Initialize();
#elif GAME_CLIENT
            GameClient.Instance.SetAppReference(this);
            GameClient.Instance.Initialize();
#endif
        }

        public void StartApp()
        {
            this.BeginApp();
        }
    }
}
