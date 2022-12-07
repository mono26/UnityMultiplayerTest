#if GAME_CLIENT
using Fusion;
using Fusion.Sockets;
using SLGFramework;
using StarterAssets;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
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

        private NetworkInputData inputData = new NetworkInputData();

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
            Debug.Log("OnPlayerJoined");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("OnPlayerLeft");
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (this.networkRunner != runner) {
                return;
            }

            input.Set(this.inputData);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {

        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {

        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("OnConnectedToServer");
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

        public void ConnectToRoom()
        {
            this.networkRunner.StartGame(new StartGameArgs() {
                GameMode = GameMode.Client,
                SessionName = "localhost",
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }

        public void SetAppReference(GameApp app)
        {
            this.appReference = app;
        }

        // TODO move this.
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        public void OnMove(InputValue value)
        {
            this.MoveInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            this.JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            this.SprintInput(value.isPressed);
        }
#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            this.inputData.MoveDirection = newMoveDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            this.inputData.Jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            this.inputData.Sprint = newSprintState;
        }
    }
}
#endif