using SLGFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerTest
{
    /// <summary>
    /// Class in charge of running the game logic.
    /// </summary>
    public class GameRunner : SLGBehaviour
    {
        private GameObject localPlayer = null;

        public void LeaveRoom()
        {
            //PhotonNetwork.LeaveRoom();
        }
    }
}
