using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// This class handles the main menu UI.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject playerDetailsPanel;
    public GameObject sessionBrowserPanel;
    public GameObject createSessionPanel;
    public GameObject statusPanel;

    [Header("Player Settings")]
    public TMP_InputField nickNameInputField;

    [Header("New Game Session")]
    public TMP_InputField sessionNmeInputField;

    void Start()
    {
        if (PlayerPrefs.HasKey("NickName"))
            nickNameInputField.text = PlayerPrefs.GetString("NickName");
    }

    /// <summary>
    /// hides all the panels.
    /// </summary>
    void HideAllPanels()
    {
        playerDetailsPanel.SetActive(false);
        sessionBrowserPanel.SetActive(false);
        createSessionPanel.SetActive(false);
        statusPanel.SetActive(false);
    }

    /// <summary>
    /// displays the <see cref="playerDetailsPanel"/>.
    /// Sets the player name and loads the available sessions.
    /// </summary>
    public void OnFindGameClicked()
    {
        PlayerPrefs.SetString("NickName", nickNameInputField.text);
        PlayerPrefs.Save();

        GameManager.instance.playerNickName = nickNameInputField.text;

        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.OnJoinLobby();

        HideAllPanels();

        sessionBrowserPanel.SetActive(true);
        FindObjectOfType<SessionListUIHandler>(true).OnLookingForSessions();
    }

    /// <summary>
    /// displays the <see cref="createSessionPanel"/> and let the user set a new game session name.
    /// </summary>
    public void OnCreateNewGameClicked()
    {
        HideAllPanels();
        createSessionPanel.SetActive(true);
    }

    /// <summary>
    /// creates and joins a new game session
    /// </summary>
    public void OnStartNewSessionClicked()
    {
        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.CreateGame(sessionNmeInputField.text, "fps-multiplayer");

        HideAllPanels();

        statusPanel.SetActive(true);
    }

    /// <summary>
    /// displays the <see cref="playerDetailsPanel"/> and let the user know that the game is connecting to the server.
    /// </summary>
    public void OnJoiningServer()
    {
        HideAllPanels();

        statusPanel.gameObject.SetActive(true);
    }
}
