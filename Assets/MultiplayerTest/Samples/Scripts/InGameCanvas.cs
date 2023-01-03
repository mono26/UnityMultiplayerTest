using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Canvas that handles the in-game UI.
/// </summary>
public class InGameCanvas : MonoBehaviour
{
    private Sample_Matchmaking_Spawner _spawner;

    [SerializeField] private TextMeshProUGUI _gameTypeText;
    [SerializeField] private TextMeshProUGUI _gameMapText;

    private void Start()
    {
        _spawner = GameObject.Find("Spawner").GetComponent<Sample_Matchmaking_Spawner>();

        _gameTypeText.text = "GameType: " + _spawner.GameType;
        _gameMapText.text = "GameMap: " + _spawner.GameMap;
    }

    public async void LeaveSession()
    {
        await _spawner.LeaveSession();
    }
}
