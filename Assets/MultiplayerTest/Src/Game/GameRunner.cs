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

            //if (!PhotonNetwork.IsMasterClient) {
            //    Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            //    return;
            //}

            Debug.LogFormat("PhotonNetwork : Players count : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            // PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.PlayerCount > 0 ? "Playground" : "LobbyScene");
        }
#endif

        private void Start()
        {
            this.BeginPlay();
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            // TODO create transition manager.
            // SceneManager.LoadScene("LobbyScene");
            // SceneManager.LoadScene("Playground");
        }

        public void LeaveRoom()
        {
            //PhotonNetwork.LeaveRoom();
        }
    }
}
