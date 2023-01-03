using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// This class is used to set the filters for matchmaking.
/// </summary>
public class CustomProperties : MonoBehaviour
{
    /// <summary> Maps available for matchmaking </summary>
    public TMP_Dropdown mapDropdown;
    /// <summary> Game types available for matchmaking </summary>
    public TMP_Dropdown gameTypeDropdown;

    private void Start()
    {
        SetMapOptions();
        SetGameTypeOptions();
    }

    private void SetMapOptions()
    {
        mapDropdown.ClearOptions();
        mapDropdown.AddOptions(new List<string>(System.Enum.GetNames(typeof(Sample_Utils.GameMap))));
    }

    private void SetGameTypeOptions()
    {
        gameTypeDropdown.ClearOptions();
        gameTypeDropdown.AddOptions(new List<string>(System.Enum.GetNames(typeof(Sample_Utils.GameType))));
    }
}
