using SLGFramework;
using StarterAssets;
using UnityEngine;

namespace MultiplayerTest
{
    public class PlayerAnimationEventsHandler : SLGBehaviour
    {
        [SerializeField]
        private ThirdPersonController characterController = null;

        private void OnFootstep(AnimationEvent animationEvent)
        {
            this.characterController?.OnFootstep(animationEvent);
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            this.characterController?.OnLand(animationEvent);
        }
    }
}