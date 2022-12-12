using Fusion;
using SLGFramework;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerTest
{
    [RequireComponent(typeof(Button))]
    public class PlayGameButton : SLGBehaviour
    {
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
#if GAME_CLIENT
            GameClient.Instance.ConnectToServer();
#endif
        }
    }
}
