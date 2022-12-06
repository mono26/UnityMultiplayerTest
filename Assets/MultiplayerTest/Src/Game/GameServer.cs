//using Photon.Pun;
//using Photon.Realtime;
using Fusion;
using Fusion.Sockets;
using SLGFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerTest
{
    public class GameServer : Singleton<GameServer>, INetworkRunnerCallbacks
    {
        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        private bool isConnecting = false;

        private GameApp appReference;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            NetworkRunner networkRunner = this.GetComponent<NetworkRunner>();
            networkRunner.AddCallbacks(this);
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            Debug.Log("OnBeginPlay");

            // this.ConnectToNetwork();

            GameRunner.Instance.Initialize();
        }

        private void CreateRoom()
        {
            // PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
        }

        public void ConnectToNetwork()
        {
            //if (PhotonNetwork.IsConnected) {
            //    this.CreateRoom();
            //}
            //else {
            //    this.isConnecting = PhotonNetwork.ConnectUsingSettings();
            //    PhotonNetwork.GameVersion = this.appReference.AppVersion;
            //}
        }

        public void SetAppReference(GameApp app)
        {
            this.appReference = app;
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
            
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            
        }
    }
}
