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
using UnityEngine.SceneManagement;

namespace MultiplayerTest
{
    public class GameClient : SLGBehaviour, INetworkRunnerCallbacks
    {
        private bool isConnecting = false;

        private ServerConfig serverConfig = null;

        private GameApp appReference = null;

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

        private void OnDestroy()
        {
            if (!AppWrapper.HasInstance()) {
                return;
            }

            AppWrapper.Instance.AppReference.ServiceProvider.GetService<IEventManager>().RemoveListener<GamePlayerSpawnedEvent>(this.OnGamePlayerSpawned);
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
            Log.Info("Initialize GameClient");

            base.OnInitialize();

            if (!this.TryGetComponent<NetworkRunner>(out this.networkRunner)) {
                this.networkRunner = this.gameObject.AddComponent<NetworkRunner>();
            }
            this.networkRunner.ProvideInput = true;
            this.networkRunner.AddCallbacks(this);

            this.gameRunnerFactory = new PFBFactory<GameRunner>();

            AppWrapper.Instance.AppReference.ServiceProvider.GetService<IEventManager>().AddListener<GamePlayerSpawnedEvent>(this.OnGamePlayerSpawned);
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            // Initialize the instance in charge of handling game logic.
            // this.ConnectToServer();
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("OnPlayerJoined");

            if (this.networkRunner != runner) {
                return;
            }

            //Log.Info("Try store player obj.");
            //if (this.networkRunner.TryGetPlayerObject(player, out NetworkObject playerObject)) {
            //    Log.Info("Store player obj.");

            //    this.spawnedCharacters.Add(player, playerObject);
            //}
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("OnPlayerLeft");

            if (this.networkRunner != runner) {
                return;
            }

            if (this.spawnedCharacters.ContainsKey(player)) {
                this.spawnedCharacters.Remove(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (this.networkRunner != runner) {
                return;
            }

            if (!this.spawnedCharacters.TryGetValue(this.networkRunner.LocalPlayer, out NetworkObject localPlayerObject)) {
                return;
            }

            LocalPlayerInputHandler localPlayerInput = localPlayerObject.GetComponent<LocalPlayerInputHandler>();

            input.Set(localPlayerInput.InputData);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {

        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            // Reload scene after shutdown
            if (Application.isPlaying) {
                SceneManager.LoadScene(1);
            }
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("OnConnectedToServer");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.LogError("OnDisconnectedFromServer");

            runner.Shutdown();
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log("OnConnectRequest");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.LogError("OnConnectFailed");

            runner.Shutdown();
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {

        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Log.Debug($"Received: {sessionList.Count}");
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

        public async void ConnectToServer()
        {
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

            NetAddress address;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            address = NetAddress.LocalhostIPv4();
            // TODO delete this. This is just for being able to connect to the hosted server in the editor.
            // address = NetAddress.CreateFromIpPort(this.serverConfig.PublicIP, this.serverConfig.PublicPort);
#elif GAME_CLIENT
            address = NetAddress.CreateFromIpPort(this.serverConfig.PublicIP, this.serverConfig.PublicPort);
#endif

            this.isConnecting = true;

            Log.Info($"Attempting to join server at {address} and with config: {this.serverConfig}.");

            StartGameResult result = await this.networkRunner.StartGame(new StartGameArgs() {
                GameMode = GameMode.Client,
                SessionName = this.serverConfig.SessionName,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                Address = address,
                //CustomPublicAddress = externalAddr,
                CustomPhotonAppSettings = photonSettings,
                DisableClientSessionCreation = true
            });

            if (result.Ok) {
                Log.Info($"Client Start DONE");

                this.isConnecting = false;

                // Initialize the instance in charge of handling game logic.
                this.gameRunner = this.gameRunnerFactory.CreateInstance(this.transform, Vector3.zero, Quaternion.identity);
            }
            else {
                // Quit the application if startup fails
                Log.Info($"Error while joining Server: {result.ShutdownReason}");
            }
        }

        public void OnGamePlayerSpawned(GamePlayerSpawnedEvent e)
        {
            Log.Info("OnGamePlayerSpawned");

            if (e.GamePlayerRef == null) {
                return;
            }

            PlayerRef playerRef = e.GamePlayerRef.Object.InputAuthority;
            if (this.spawnedCharacters.ContainsKey(playerRef)) {
                this.spawnedCharacters[playerRef] = e.GamePlayerRef.Object;
            }
            else {
                this.spawnedCharacters.Add(playerRef, e.GamePlayerRef.Object);
            }
        }

        public void SetAppReference(GameApp app)
        {
            this.appReference = app;
        }
    }
}