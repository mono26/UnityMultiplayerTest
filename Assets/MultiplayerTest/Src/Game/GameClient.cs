using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace MultiplayerTest
{
    public class GameClient : PUNSingleton<GameClient>
    {
        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        // TODO remove this.
        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;

        private bool isConnecting = false;

        private GameApp appReference;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            PhotonNetwork.AutomaticallySyncScene = true;
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            Debug.Log("OnBeginPlay");

            // this.ConnectToServer();

            this.progressLabel.SetActive(false);
            this.controlPanel.SetActive(true);

            GameRunner.Instance.Initialize();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

            if (this.isConnecting) {
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
            }
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);

            this.isConnecting = false;

            this.progressLabel.SetActive(false);
            this.controlPanel.SetActive(true);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

            this.isConnecting = false;

            GameRunner.Instance.BeginPlay();
        }

        public void ConnectToServer()
        {
            this.progressLabel.SetActive(true);
            this.controlPanel.SetActive(false);

            if (PhotonNetwork.IsConnected) {
                PhotonNetwork.JoinRandomRoom();
            }
            else {
                this.isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.appReference.AppVersion;
            }
        }

        public void SetAppReference(GameApp app)
        {
            this.appReference = app;
        }
    }
}
