using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;

public class Sample_Matchmaking_Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    /// <summary> Represents a Server or Client Simulation. </summary>
    private NetworkRunner _runner;
    /// <summary> Prefab to spawn for each player. </summary>
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    /// <summary> List of spawned players. </summary>
    private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

    Sample_InputHandler _inputHandler;

    /// <summary> Finds any available session and joins it. </summary>
    public async Task JoinRandomSession()
    {
        _runner = gameObject.GetComponent<NetworkRunner>() ?? gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SceneManager = gameObject.GetComponent<NetworkSceneManagerDefault>() ?? gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (result.Ok)
        {
            string gameMap =  _runner.SessionInfo.Properties.ContainsKey("Map")
                              ? Enum.GetName(typeof(Sample_Utils.GameMap), _runner.SessionInfo.Properties["Map"].PropertyValue)
                              : "Unknown";
            string gameType = _runner.SessionInfo.Properties.ContainsKey("GameType")
                              ? Enum.GetName(typeof(Sample_Utils.GameType), _runner.SessionInfo.Properties["GameType"].PropertyValue)
                              : "Unknown";
            Debug.Log($"Game started \n \t Map: {gameMap} \t GameType: {gameType}");
        }
        else
            Debug.Log($"Game start failed: {result.ShutdownReason}");
    }

    /// <summary> Creates a new session with custom properties. </summary>
    /// <param name="map">Map to play on.</param>
    /// <param name="gameType">Game type to play.</param>
    public async Task CreateSessionWithCustomProperties(Sample_Utils.GameMap map, Sample_Utils.GameType gameType)
    {
        Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty>();

        customProperties["Map"] = (int)map;
        customProperties["GameType"] = (int)gameType;

        _runner = gameObject.GetComponent<NetworkRunner>() ?? gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionProperties = customProperties,
            SceneManager = gameObject.GetComponent<NetworkSceneManagerDefault>() ?? gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (result.Ok)
            Debug.Log($"Custom Game started:\n \t Map: {map} \t GameType: {gameType}");
        else
            Debug.Log($"Game start failed: {result.ShutdownReason}");
    }

    /// <summary> Leaves the current session. </summary>
    public async Task LeaveSession()
    {
        await _runner.Shutdown();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        _inputHandler = runner.gameObject.GetComponent<Sample_InputHandler>() ?? runner.gameObject.AddComponent<Sample_InputHandler>();

        if (_inputHandler != null)
            input.Set(_inputHandler.GetNetworkInputData());
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
