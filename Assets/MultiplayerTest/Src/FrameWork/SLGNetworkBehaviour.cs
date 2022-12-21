using Fusion;

namespace MultiplayerTest
{
    public abstract class SLGNetworkBehaviour : NetworkBehaviour
    {
        protected bool IsInitialized { get; private set; } = false;
        protected bool IsPlaying { get; private set; } = false;

        /// <summary>
        /// Should be called in the Awake(). Better used for catching references, and stuff needed before start playing.
        /// </summary>
        public void Initialize()
        {
            if (this.IsInitialized) {
                return;
            }

            this.OnInitialize();

            this.IsInitialized = true;
        }

        /// <summary>
        /// Should be called in the Start(). Better used for initializing everything to start playing.
        /// </summary>
        public void BeginPlay()
        {
            if (this.IsPlaying) {
                return;
            }

            this.OnBeginPlay();

            this.IsPlaying = true;
        }

        protected virtual void OnBeginPlay() { }
        protected virtual void OnInitialize() { }
    }
}

