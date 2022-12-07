using Fusion;

namespace MultiplayerTest
{
    public abstract class SLGNetworkBehaviour : NetworkBehaviour
    {
        private bool isInitialized = false;
        private bool isPlaying = false;

        /// <summary>
        /// Should be called in the Awake(). Better used for catching references, and stuff needed before start playing.
        /// </summary>
        public void Initialize()
        {
            if (this.isInitialized) {
                return;
            }

            this.OnInitialize();

            this.isInitialized = true;
        }

        /// <summary>
        /// Should be called in the Start(). Better used for initializing everything to start playing.
        /// </summary>
        public void BeginPlay()
        {
            if (this.isPlaying) {
                return;
            }

            this.OnBeginPlay();

            this.isPlaying = true;
        }

        protected virtual void OnBeginPlay() { }
        protected virtual void OnInitialize() { }
    }
}

