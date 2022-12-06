using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerTest
{
    public class GameRunner : PUNSingleton<GameRunner>
    {
        [SerializeField]
        private GameObject playerPrefab = null;

        private GameObject localPlayer = null;

#if GAME_SERVER
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient) {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                this.LoadLevel();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient) {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                this.LoadLevel();
            }
        }

        private void LoadLevel()
        {
            // TODO server should be the only one to load levels.

            if (!PhotonNetwork.IsMasterClient) {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }

            Debug.LogFormat("PhotonNetwork : Players count : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.PlayerCount > 0 ? "Playground" : "LobbyScene");
        }
#endif

        private void Start()
        {
            this.BeginPlay();
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            this.CreateLocalPlayer();
        }

        private void CreateLocalPlayer()
        {
            if (this.playerPrefab == null) {
                Debug.Log("Need a player prefab for instantiation.");
                return;
            }

            if (this.localPlayer != null) {
                Debug.Log("The local player was already created.");
                return;
            }

            this.localPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
