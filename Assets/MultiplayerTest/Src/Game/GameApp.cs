using SLGFramework;
using UnityEngine;

namespace MultiplayerTest
{
    public class GameApp : SLGScript
    {
        private ServiceProvider serviceProvider = null;

        public string AppVersion { get; private set; } = "1";

        public GameApp()
        {
            this.AppVersion = "1";

            this.serviceProvider = new ServiceProvider();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.serviceProvider.InitService<IEventManager>();
        }

        private void BeginApp()
        {
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
