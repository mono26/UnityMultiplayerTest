#if UNITY_SERVER
#define GAME_SERVER
#undef GAME_CLIENT
#endif

using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using SLGFramework;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace MultiplayerTest
{
    public class GameServer : SLGBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField]
        private NetworkPrefabRef playerPrefab;

        private bool isConnecting = false;

        private GameApp appReference = null;

        private ServerConfig serverConfig = null;

        private GameRunner gameRunner = null;
        private PFBFactory<GameRunner> gameRunnerFactory = null;

        private NetworkRunner networkRunner = null;

        private Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

        private void Awake()
        {
            this.Initialize();
        }

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
            Log.Info("Initialize GameServer");

            base.OnInitialize();

            if (!this.TryGetComponent<NetworkRunner>(out this.networkRunner)) {
                this.networkRunner = this.gameObject.AddComponent<NetworkRunner>();
            }
            this.networkRunner.ProvideInput = true;
            this.networkRunner.AddCallbacks(this);

            this.gameRunnerFactory = new PFBFactory<GameRunner>();
        }

        protected override void OnBeginPlay()
        {
            if (this.isConnecting) {
                return;
            }

            Log.Info("BeginPlay GameServer");

            base.OnBeginPlay();

            this.ConnectToNetwork();
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (this.networkRunner != runner || !this.networkRunner.IsServer) {
                return;
            }

            Log.Info("OnPlayerJoined");

            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % this.networkRunner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
            NetworkObject networkPlayerObject = this.ServerSpawn(this.playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars so we can remove it when they disconnect.
            spawnedCharacters.Add(player, networkPlayerObject);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (this.networkRunner != runner || !this.networkRunner.IsServer) {
                return;
            }

            // Find and remove the players avatar
            if (this.spawnedCharacters.TryGetValue(player, out NetworkObject networkObject)) {
                this.networkRunner.Despawn(networkObject);
                this.spawnedCharacters.Remove(player);
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
            Log.Info("OnConnectedToServer");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.LogError("OnDisconnectedFromServer");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Log.Info("OnConnectRequest");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Log.Error("OnConnectFailed");
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

        public async void ConnectToNetwork()
        {
            Log.Info($"ConnectToNetwork");

            this.serverConfig = ServerConfig.Resolve();

            // Build Custom Photon Config
            AppSettings photonSettings = PhotonAppSettings.Instance.AppSettings.GetCopy();

            if (string.IsNullOrEmpty(this.serverConfig.Region) == false) {
                photonSettings.FixedRegion = this.serverConfig.Region.ToLower();
            }

            // Build Custom External Addr
            NetAddress? externalAddr = null;

            if (string.IsNullOrEmpty(this.serverConfig.PublicIP) == false && this.serverConfig.PublicPort > 0) {
                if (IPAddress.TryParse(this.serverConfig.PublicIP, out var _)) {
                    externalAddr = NetAddress.CreateFromIpPort(this.serverConfig.PublicIP, this.serverConfig.PublicPort);
                }
                else {
                    Log.Warn("Unable to parse 'Custom Public IP'");
                }
            }

            this.isConnecting = true;

            NetAddress address;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            address = NetAddress.LocalhostIPv4();
#elif UNITY_SERVER || GAME_SERVER
            address = NetAddress.Any(this.serverConfig.Port);
#endif

            Log.Info($"Attempting to create server at {address} and with config: {this.serverConfig}.");

            // Start Runner
            StartGameResult result = await this.networkRunner.StartGame(new StartGameArgs() {
                SessionName = this.serverConfig.SessionName,
                GameMode = GameMode.Server,
                SceneManager = this.networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
                Scene = 2,
                SessionProperties = this.serverConfig.SessionProperties,
                Address = address,
                // CustomPublicAddress = externalAddr,
                // CustomLobbyName = this.serverConfig.Lobby,
                CustomPhotonAppSettings = photonSettings
            });

            if (result.Ok) {
                Log.Info($"Server Start DONE");

                this.isConnecting = false;

                // Initialize the instance in charge of handling game logic.
                this.gameRunner = this.gameRunnerFactory.CreateInstance(this.transform, Vector3.zero, Quaternion.identity);
            }
            else {
                // Quit the application if startup fails
                Log.Info($"Error while starting Server: {result.ShutdownReason}");

                // it can be used any error code that can be read by an external application
                // using 0 means all went fine
                Application.Quit(1);
            }
        }

        public void SetAppReference(GameApp app)
        {
            this.appReference = app;
        }

        public NetworkObject ServerSpawn(NetworkPrefabRef objectToSpawn, Vector3 position, Quaternion rotation, PlayerRef? owner)
        {
            if (objectToSpawn == null) {
                return null;
            }

            return this.networkRunner.Spawn(objectToSpawn, position, rotation, owner);
        }
    }
}