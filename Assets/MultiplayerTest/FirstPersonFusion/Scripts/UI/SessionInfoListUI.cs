using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System;

/// <summary>
/// Displays the sessions information; session name, player count and join button.
/// </summary>
public class SessionInfoListUI : MonoBehaviour
{
    public TextMeshProUGUI sessionNameText;
    public TextMeshProUGUI playerCountText;
    public Button joinButton;

    SessionInfo sessionInfo;

    public event Action<SessionInfo> OnJoinSession;

    /// <summary>
    /// Sets the session information.
    /// </summary>
    /// <param name="sessionInfo">The session information.</param>
    public void SetInformation(SessionInfo sessionInfo)
    {
        this.sessionInfo = sessionInfo;

        sessionNameText.text = sessionInfo.Name;
        playerCountText.text = $"{sessionInfo.PlayerCount}/{sessionInfo.MaxPlayers}";

        bool isJoinButtonActive = true;

        if (sessionInfo.PlayerCount >= sessionInfo.MaxPlayers)
            isJoinButtonActive = false;
        
        joinButton.gameObject.SetActive(isJoinButtonActive);
    }


    public void OnClick()
    {
        OnJoinSession?.Invoke(sessionInfo);
    }
}
