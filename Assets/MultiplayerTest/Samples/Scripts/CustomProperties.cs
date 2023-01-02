using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CustomProperties : MonoBehaviour
{
    public TMP_Dropdown mapDropdown;
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
