using SLGFramework;

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
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            this.appReference.StartApp();
        }
    }
}
