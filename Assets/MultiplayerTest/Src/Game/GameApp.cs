using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace MultiplayerTest
{
    public class GameApp
    {
        public string AppVersion { get; private set; } = "1";

        public GameApp()
        {
            this.InitializeApp();
        }

        private void InitializeApp()
        {
            this.AppVersion = "1";

#if GAME_SERVER
            GameServer.Instance.SetAppReference(this);
            GameServer.Instance.Initialize();
#elif GAME_CLIENT
            GameClient.Instance.SetAppReference(this);
            GameClient.Instance.Initialize();
#endif
        }

        private void BeginApp()
        {
#if GAME_SERVER
            GameServer.Instance.BeginPlay();
#elif GAME_CLIENT
            GameClient.Instance.BeginPlay();
#endif

        }

        public void StartApp()
        {
            this.BeginApp();
        }
    }
}
