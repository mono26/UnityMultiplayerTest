using SLGFramework;
using UnityEngine.SceneManagement;

namespace MultiplayerTest
{
    public class AppWrapper : Singleton<AppWrapper>
    {
        private GameApp appReference;

        private void Start()
        {
            this.BeginPlay();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.appReference = new GameApp();
            this.appReference.Initialize();

            // TODO use transition service.
            SceneManager.LoadScene("LobbyScene");
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            this.appReference.StartApp();
        }
    }
}
