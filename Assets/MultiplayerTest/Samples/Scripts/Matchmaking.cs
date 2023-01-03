using UnityEngine;
using Fusion;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// This class handles the matchmaking logic.
/// </summary>
public class Matchmaking : MonoBehaviour
{
    private NetworkRunner _runner;
    [SerializeField] private Sample_Matchmaking_Spawner _spawner;
    [SerializeField] private CustomProperties _customProperties;
    

    /// <summary> Finds any available session and joins it. </summary>
    public async void JoinRandomSession()
    {
        await _spawner.JoinRandomSession();
    }

    /// <summary> Creates a new session with custom properties. </summary>
    public async void CreateSessionWithCustomProperties()
    {
        Sample_Utils.GameMap map = (Sample_Utils.GameMap)_customProperties.mapDropdown.value;
        Sample_Utils.GameType gameType = (Sample_Utils.GameType)_customProperties.gameTypeDropdown.value;
        SceneRef scene = SceneManager.GetActiveScene().buildIndex + (int)gameType + 1;

        Debug.Log($"Map: {map} \t GameType: {gameType} \t Scene: {scene}");

        await _spawner.CreateSessionWithCustomProperties(map, gameType, scene);
    }

    /// <summary> Leaves the current session. </summary>
    public async void LeaveSession()
    {
        await _spawner.LeaveSession();
    }
}
