using Fusion;
using SLGFramework;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTest
{
    [RequireComponent(typeof(Button))]
    public class PlayGameButton : SLGBehaviour
    {
        [SerializeField]
        private GameMode gameMode = GameMode.Host;

        private void Awake()
        {
            this.Initialize();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Button buttonComponent = this.GetComponent<Button>();
            buttonComponent.onClick.AddListener(this.OnButtonTriggered);
        }

        private void OnButtonTriggered()
        {
            GameClient.Instance.ConnectToRoom(this.gameMode);
        }
    }
}
