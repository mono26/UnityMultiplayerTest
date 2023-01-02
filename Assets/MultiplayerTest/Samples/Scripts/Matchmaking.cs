using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class Matchmaking : MonoBehaviour
{
    private NetworkRunner _runner;
    [SerializeField] private Sample_Matchmaking_Spawner _spawner;
    [SerializeField] private CustomProperties _customProperties;
    

    public async void JoinRandomSession()
    {
        await _spawner.JoinRandomSession();
    }

    public async void CreateSessionWithCustomProperties()
    {
        Sample_Utils.GameMap map = (Sample_Utils.GameMap)_customProperties.mapDropdown.value;
        Sample_Utils.GameType gameType = (Sample_Utils.GameType)_customProperties.gameTypeDropdown.value;

        await _spawner.CreateSessionWithCustomProperties(map, gameType);
    }

    public async void LeaveSession()
    {
        await _spawner.LeaveSession();
    }
}
