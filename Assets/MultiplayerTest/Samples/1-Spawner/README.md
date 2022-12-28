To start a game with Fusion the `Startgame` method needs to be called on the Fusion [NetworkRunner]

To do this, it is created a "Spawner" that implements the `INetworkRunnerCallback` Interface

---
#### `Startgame`
- Creates the `NetworkRunner` and lets it know that this client will be providing input
- Starts a new session with a name and specified `GameMode`

#### `OnGUI`
- Draws a UI for the player to choose which game mode starts in

#### `OnPlayerJoined` And `OnPlayerLeft`
- Sapwns/Despawns players in the scene

#### `_playerPrefab`
- [Network Object] that contains tha character for the player
- Has attached a CharacterController

#### `_spawnedCharacters`
- Saves a reference of the current players

---

<details>
<summary> Spawner Script
</summary>

```cs
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

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}
```


</details>

---

[NetworkRunner]:<https://doc-api.photonengine.com/en/fusion/current/class_fusion_1_1_network_runner.html#details>
[Network Object]: <https://doc-api.photonengine.com/en/fusion/current/class_fusion_1_1_network_object.html>