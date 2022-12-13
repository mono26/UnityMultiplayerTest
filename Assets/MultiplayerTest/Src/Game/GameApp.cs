#if UNITY_SERVER
#define GAME_SERVER
#undef GAME_CLIENT
#endif

using Fusion;
using SLGFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
#if GAME_SERVER
using UnityEngine;
#endif

namespace MultiplayerTest
{
    public class GameApp : SLGBehaviour
    {
        private ServiceProvider serviceProvider = null;

#if GAME_SERVER
        private PFBFactory<GameServer> gameServerFactory = null;
        public GameServer GameServer { get; private set; } = null;
#elif GAME_CLIENT
        private PFBFactory<GameClient> gameClientFactory = null;
        public GameClient GameClient { get; private set; } = null;
#endif

        public string AppVersion { get; private set; } = "1";

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.AppVersion = "1";

            this.serviceProvider = new ServiceProvider();

            this.serviceProvider.InitService<IEventManager>();

#if GAME_SERVER
            // Limit frame rate in server.
            Application.targetFrameRate = 30;
            this.gameServerFactory = new PFBFactory<GameServer>();
#elif GAME_CLIENT
            this.gameClientFactory = new PFBFactory<GameClient>();
#endif
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

#if GAME_SERVER
            this.GameServer = this.gameServerFactory.CreateInstance(null, Vector3.zero, Quaternion.identity);
            this.GameServer.SetAppReference(this);
            this.GameServer.Initialize();
#elif GAME_CLIENT
            this.GameClient = this.gameClientFactory.CreateInstance(null, Vector3.zero, Quaternion.identity);
            this.GameClient.SetAppReference(this);
            this.GameClient.Initialize();
#endif

            // TODO use transition service.
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
