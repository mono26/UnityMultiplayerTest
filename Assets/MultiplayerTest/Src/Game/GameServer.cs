#if GAME_SERVER
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using SLGFramework;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        [SerializeField]
        private NetworkPrefabRef playerPrefab;

        private bool isConnecting = false;

        private NetworkRunner networkRunner = null;

        private Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

        private GameApp appReference = null;

        private void Start()
        {
            this.BeginPlay();
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Create Server")) {
                this.ConnectToNetwork();
            }
        }

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

            // this.ConnectToNetwork();
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (this.networkRunner != runner || !this.networkRunner.IsServer) {
                return;
            }

            Debug.Log("OnPlayerJoined");

            this.CreatePlayer(player);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (this.networkRunner != runner || !this.networkRunner.IsServer) {
                return;
            }

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
            Debug.LogError("OnConnectedToServer");

            // Initialize the instance in charge of handling game logic.
            GameRunner.Instance.Initialize();
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.LogError("OnDisconnectedFromServer");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log("OnConnectRequest");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.LogError("OnConnectFailed");
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

        public void ConnectToNetwork()
        {
            //this.networkRunner.StartGame(new StartGameArgs() {
            //    GameMode = GameMode.Host,
            //    SessionName = "TestRoom",
            //    Scene = SceneManager.GetActiveScene().buildIndex,
            //    SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            //});

            // Build Custom Photon Config
            var photonSettings = PhotonAppSettings.Instance.AppSettings.GetCopy();

            if (string.IsNullOrEmpty(customRegion) == false) {
                photonSettings.FixedRegion = customRegion.ToLower();
            }

            // Build Custom External Addr
            NetAddress? externalAddr = null;

            if (string.IsNullOrEmpty(customPublicIP) == false && customPublicPort > 0) {
                if (IPAddress.TryParse(customPublicIP, out var _)) {
                    externalAddr = NetAddress.CreateFromIpPort(customPublicIP, customPublicPort);
                }
                else {
                    Log.Warn("Unable to parse 'Custom Public IP'");
                }
            }

            // Start Runner
            return this.networkRunner.StartGame(new StartGameArgs() {
                SessionName = SessionName,
                GameMode = GameMode.Server,
                SceneManager = this.networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                Scene = 0,
                SessionProperties = customProps,
                Address = NetAddress.Any(port),
                CustomPublicAddress = externalAddr,
                CustomLobbyName = customLobby,
                CustomPhotonAppSettings = photonSettings,
            });
        }

        public void SetAppReference(GameApp app)
        {
            this.appReference = app;
        }
    }
}
#endif