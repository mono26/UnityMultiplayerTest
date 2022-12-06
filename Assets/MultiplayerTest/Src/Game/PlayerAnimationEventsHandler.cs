using SLGFramework;
using StarterAssets;
using UnityEngine;

namespace MultiplayerTest
{
    public class PlayerAnimationEventsHandler : SLGBehaviour
    {
        [SerializeField]
        private ThirdPersonController controller = null;

        private void OnFootstep(AnimationEvent animationEvent)
        {
            this.controller?.OnFootstep(animationEvent);
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            this.controller?.OnLand(animationEvent);
        }
    }
}