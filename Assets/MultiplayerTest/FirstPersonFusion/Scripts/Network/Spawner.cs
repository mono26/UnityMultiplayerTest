using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

/// <summary>
/// This class is used to spawn players in the world.
/// </summary>
public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer playerPrefab;

    Dictionary<int, NetworkPlayer> mapTokenIDWithNetworkPlayer;

    CharacterInputHandler characterInputHandler;
    SessionListUIHandler sessionListUIHandler;

    void Awake()
    {
        sessionListUIHandler = FindObjectOfType<SessionListUIHandler>(true);
        mapTokenIDWithNetworkPlayer = new Dictionary<int, NetworkPlayer>();
    }

    /// <summary>
    /// Gets the player token.
    /// </summary>
    /// <param name="runner">Server or Client simulation.</param>
    /// <param name="player">The Fusion player.</param>
    /// <returns> <see langword="int" /> with The player token.</returns>
    int GetPlayerToken(NetworkRunner runner, PlayerRef player)
    {
        if (runner.LocalPlayer == player)
            return ConnectionTokenUtils.HashToToken(GameManager.instance.GetConnectionToken());
        else
        {
            var token = runner.GetPlayerConnectionToken(player);

            if (token != null)
                return ConnectionTokenUtils.HashToToken(token);
            
            return 0;
        }
    }

    public void SetConnectionTokenMapping(int token, NetworkPlayer networkPlayer)
    {
        mapTokenIDWithNetworkPlayer.Add(token, networkPlayer);
    }

    /// <summary>
    /// Spawns the player.
    /// </summary>
    /// <param name="runner">Server or Client simulation.</param>
    /// <param name="player">The Fusion player.</param>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            int playerToken = GetPlayerToken(runner, player);

            if (mapTokenIDWithNetworkPlayer.TryGetValue(playerToken, out NetworkPlayer networkPlayer))
            {
                networkPlayer.GetComponent<NetworkObject>().AssignInputAuthority(player);
                networkPlayer.Spawned();
            }
            else
            {
                NetworkPlayer spawnedNetworkPlayer = runner.Spawn(playerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, player);

                spawnedNetworkPlayer.token = playerToken;

                mapTokenIDWithNetworkPlayer[playerToken] = spawnedNetworkPlayer;
            }
        }
    }

    /// <summary>
    /// Sends the player's input to the server.
    /// </summary>
    /// <param name="runner">Server or Client simulation.</param>
    /// <param name="input">The input.</param>
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (characterInputHandler == null && NetworkPlayer.Local != null)
            characterInputHandler = NetworkPlayer.Local.GetComponent<CharacterInputHandler>();

        if (characterInputHandler != null)
            input.Set(characterInputHandler.GetNetworkInput());
    }

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("OnHostMigration");

        await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

        FindObjectOfType<NetworkRunnerHandler>().StartHostMigration(hostMigrationToken);
    }

    /// <summary>
    /// Updates the session list.
    /// </summary>
    /// <param name="runner">Server or Client simulation.</param>
    /// <param name="sessionList">The session list to update.</param>
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if (sessionListUIHandler == null)
            return;

        if (sessionList.Count == 0)
        {
            Debug.Log("OnSessionListUpdated: No sessions found");
            sessionListUIHandler.OnNoSessionFound();
        }
        else
        {
            sessionListUIHandler.ClearList();

            foreach (SessionInfo sessionInfo in sessionList)
            {
                sessionListUIHandler.AddToList(sessionInfo);
                Debug.Log("OnSessionListUpdated: Found session: " + sessionInfo.Name);
            }
        }            
    }

    /// <summary>
    /// Removes the player from the dictionary and despawns it.
    /// </summary>
    public void OnHostMigrationCleanUp()
    {
        foreach(KeyValuePair<int, NetworkPlayer> entry in mapTokenIDWithNetworkPlayer)
        {
            NetworkObject networkObjectInDictionary = entry.Value.GetComponent<NetworkObject>();

            if (networkObjectInDictionary.InputAuthority.IsNone)
            {
                mapTokenIDWithNetworkPlayer.Remove(entry.Key);
                networkObjectInDictionary.Runner.Despawn(networkObjectInDictionary);
            }
        }
    }

#region Unused callbacks
    public void OnConnectedToServer(NetworkRunner runner) {}

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}

    public void OnDisconnectedFromServer(NetworkRunner runner) {}


    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) {}

    public void OnSceneLoadDone(NetworkRunner runner) {}

    public void OnSceneLoadStart(NetworkRunner runner) {}


    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}    
#endregion
}
