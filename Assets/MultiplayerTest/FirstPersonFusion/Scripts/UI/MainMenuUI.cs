using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    public TMP_InputField nickNameInputField;

    void Start()
    {
        if (PlayerPrefs.HasKey("NickName"))
            nickNameInputField.text = PlayerPrefs.GetString("NickName");
    }

    public void OnJoinGameClicked()
    {
        PlayerPrefs.SetString("NickName", nickNameInputField.text);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
