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
            // TODO use event.
#if GAME_CLIENT
            AppWrapper.Instance.AppReference.GameClient.ConnectToServer();
#endif
        }
    }
}
