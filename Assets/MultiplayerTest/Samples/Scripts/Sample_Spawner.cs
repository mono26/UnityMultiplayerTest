using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;

public class Sample_Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    /// <summary> Represents a Server or Client Simulation. </summary>
    private NetworkRunner _runner;
    /// <summary> Prefab to spawn for each player. </summary>
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    /// <summary> List of spawned players. </summary>
    private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

    /// <summary> Starts a game with the specified mode: <br />
    /// <see langword ="GameMode.Server"/>, <see langword ="GameMode.Host" />, <see langword ="GameMode.Client" />.</summary>
    /// <param name="mode">Game mode to start (<b>Server</b>, <b>Host</b> or <b>Client</b>).</param>
    async void StartGame(GameMode mode)
    {
        _runner = gameObject.GetComponent<NetworkRunner>() ?? gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Spawner",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.GetComponent<NetworkSceneManagerDefault>() ?? gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });
        Debug.Log($"Game started; as {mode}");
    }

    /// <summary> Draws a simple GUI to start a game. </summary>
    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, 200, 40),"Start Server"))
                StartGame(GameMode.Server);

            if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 50, 200, 40), "Start Host"))
                StartGame(GameMode.Host);

            if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 100, 200, 40), "Start Client"))
                StartGame(GameMode.Client);
        }
        else
        {
            if (GUILayout.Button("Stop"))
            {
                _runner.Shutdown();
                Destroy(_runner);
                _runner = null;
            }
        }
    }


    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPos= new Vector3((player.RawEncoded%runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPos, Quaternion.identity, player);

            _spawnedPlayers.Add(player, networkPlayerObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedPlayers.TryGetValue(player, out NetworkObject networkPlayerObject))
        {
            runner.Despawn(networkPlayerObject);
            _spawnedPlayers.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new Sample_NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;
        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        input.Set(data);
    }

#region Unused callbacks
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
#endregion
}
