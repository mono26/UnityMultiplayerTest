using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;
using System;

/// <summary>
/// This class handles the UI session list.
/// </summary>
public class SessionListUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;

    public GameObject sessionInfoListPrefab;
    public VerticalLayoutGroup verticalLayoutGroup;

    private void Awake() => ClearList();

    /// <summary>
    /// Clears the session list.
    /// </summary>
    public void ClearList()
    {
        foreach (Transform child in verticalLayoutGroup.transform)
            Destroy(child.gameObject);

        statusText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Adds a new session to the list.
    /// </summary>
    /// <param name="sessionInfo">The session information.</param>
    public void AddToList(SessionInfo sessionInfo)
    {
        SessionInfoListUI sessionInfoListUI = Instantiate(sessionInfoListPrefab, verticalLayoutGroup.transform).GetComponent<SessionInfoListUI>();

        sessionInfoListUI.SetInformation(sessionInfo);

        sessionInfoListUI.OnJoinSession += AddedSessionInfoListUI_OnJoinSession;
    }

    /// <summary>
    /// This method is called when the join button is clicked.
    /// </summary>
    private void AddedSessionInfoListUI_OnJoinSession(SessionInfo sessionInfo)
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.JoinGame(sessionInfo);

        MainMenuUI mainMenuUI = FindObjectOfType<MainMenuUI>();
        mainMenuUI.OnJoiningServer();
    }

    /// <summary>
    /// This method is called when the session list is empty.
    /// </summary>
    public void OnNoSessionFound()
    {
        ClearList();

        statusText.text = "No Session Found";
        statusText.gameObject.SetActive(true);
    }

    /// <summary>
    /// This method is called when the session list is being searched.
    /// </summary>
    public void OnLookingForSessions()
    {
        ClearList();

        statusText.text = "Looking for Sessions";
        statusText.gameObject.SetActive(true);
    }
}
