#if UNITY_SERVER
#define GAME_SERVER
#undef GAME_CLIENT
#endif

using Fusion;
using SLGFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerTest
{
    public class GameApp : SLGBehaviour
    {
        public string AppVersion { get; private set; } = "1";

#if GAME_SERVER
        private PFBFactory<GameServer> gameServerFactory = null;
        public GameServer GameServer { get; private set; } = null;
#elif GAME_CLIENT
        private PFBFactory<GameClient> gameClientFactory = null;
        public GameClient GameClient { get; private set; } = null;
#endif

        public ServiceProvider ServiceProvider { get; private set; } = null;

        private void Awake()
        {
            this.Initialize();
        }

        private void Start()
        {
            this.BeginPlay();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.AppVersion = "1";

            this.ServiceProvider = new ServiceProvider();

            this.ServiceProvider.InitService<IEventManager>();

#if GAME_SERVER
            // Limit frame rate in server.
            Application.targetFrameRate = 60;
            this.gameServerFactory = new PFBFactory<GameServer>();
#elif GAME_CLIENT
            // Limit frame rate in client.
            Application.targetFrameRate = 60;
            this.gameClientFactory = new PFBFactory<GameClient>();
#endif
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

#if GAME_SERVER
            this.GameServer = this.gameServerFactory.CreateInstance(this.transform, Vector3.zero, Quaternion.identity);
            this.GameServer.SetAppReference(this);
            this.GameServer.Initialize();
#elif GAME_CLIENT
            this.GameClient = this.gameClientFactory.CreateInstance(this.transform, Vector3.zero, Quaternion.identity);
            this.GameClient.SetAppReference(this);
            this.GameClient.Initialize();
#endif

            // TODO use transition service.
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
