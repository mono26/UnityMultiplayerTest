using SLGFramework;
using UnityEngine;

namespace MultiplayerTest
{
    public class AppWrapper : Singleton<AppWrapper>
    {
        private PFBFactory<GameApp> gameAppFactory = null;

        public GameApp AppReference { get; private set; } = null;

        private void Start()
        {
            this.BeginPlay();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.gameAppFactory = new PFBFactory<GameApp>();
        }

        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            this.AppReference = this.gameAppFactory.CreateInstance(null, Vector3.zero, Quaternion.identity);
        }
    }
}
