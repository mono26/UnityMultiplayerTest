//using Photon.Pun;
using SLGFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTest
{
    /// <summary>
    /// Player name input field. Let the user input his name, will appear above the player in the game.
    /// </summary>
    [RequireComponent(typeof(TMP_InputField))]
    public class PlayerNameInputField : SLGBehaviour
    {
        // Store the PlayerPref Key to avoid typos
        const string playerNamePrefKey = "PlayerName";

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            string defaultName = string.Empty;
            InputField inputField = this.GetComponent<InputField>();
            if (inputField != null) {
                if (PlayerPrefs.HasKey(playerNamePrefKey)) {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    inputField.text = defaultName;
                }
            }

            //PhotonNetwork.NickName = defaultName;
        }

        /// <summary>
        /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
        /// </summary>
        /// <param name="value">The name of the Player</param>
        public void SetPlayerName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value)) {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            //PhotonNetwork.NickName = value;


            PlayerPrefs.SetString(playerNamePrefKey, value);
        }
    }
}
