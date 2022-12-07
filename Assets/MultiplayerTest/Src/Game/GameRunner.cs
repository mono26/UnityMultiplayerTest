using SLGFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerTest
{
    /// <summary>
    /// Class in charge of running the game logic.
    /// </summary>
    public class GameRunner : Singleton<GameRunner>
    {
        private GameObject localPlayer = null;

#if GAME_SERVER
        private void LoadLevel()
        {
            // TODO server should be the only one to load levels.
        }
#endif

        private void Start()
        {
            this.BeginPlay();
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();
        }

        public void LeaveRoom()
        {
            //PhotonNetwork.LeaveRoom();
        }
    }
}
