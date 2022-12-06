using Fusion;
using Fusion.Sockets;
using SLGFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerTest
{
    public class GameClient : Singleton<GameClient>, INetworkRunnerCallbacks
    {
        [SerializeField] 
        private NetworkPrefabRef playerPrefab;

        private Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

        private GameApp appReference = null;

        private NetworkRunner networkRunner = null;

        private void Start()
        {
            this.BeginPlay();   
        }

#if UNITY_EDITOR
        //private void OnGUI()
        //{
        //    if (GUI.Button(new Rect(0, 0, 200, 40), "Host")) {
        //        this.ConnectToRoom(GameMode.Host);
        //    }
        //    if (GUI.Button(new Rect(0, 40, 200, 40), "Join")) {
        //        this.ConnectToRoom(GameMode.Client);
        //    }
        //}
#endif

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if (!this.TryGetComponent<NetworkRunner>(out this.networkRunner)) {
                this.networkRunner = this.gameObject.AddComponent<NetworkRunner>();
            }
            this.networkRunner.ProvideInput = true;
            this.networkRunner.AddCallbacks(this);
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            // Initialize the instance in charge of handling game logic.
            GameRunner.Instance.Initialize();
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (this.networkRunner == runner && this.networkRunner.IsServer) {
                this.CreatePlayer(player);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            // Find and remove the players avatar
            if (spawnedCharacters.TryGetValue(player, out NetworkObject networkObject)) {
                runner.Despawn(networkObject);
                spawnedCharacters.Remove(player);
            }
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

        private void CreatePlayer(PlayerRef player)
        {
            if (this.playerPrefab == null) {
                Debug.Log("Need a player prefab for instantiation.");
                return;
            }

            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % this.networkRunner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
            NetworkObject networkPlayerObject = this.networkRunner.Spawn(this.playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars so we can remove it when they disconnect
            spawnedCharacters.Add(player, networkPlayerObject);
        }

        public void ConnectToRoom(GameMode mode)
        {
            this.networkRunner.StartGame(new StartGameArgs() {
                GameMode = mode,
                SessionName = "TestRoom",
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }

        public void SetAppReference(GameApp app)
        {
            this.appReference = app;
        }
    }
}
