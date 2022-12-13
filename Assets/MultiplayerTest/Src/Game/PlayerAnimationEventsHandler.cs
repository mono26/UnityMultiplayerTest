#if UNITY_SERVER
#define GAME_SERVER
#undef GAME_CLIENT
#endif

using SLGFramework;
using StarterAssets;
using UnityEngine;

namespace MultiplayerTest
{
    public class PlayerAnimationEventsHandler : SLGBehaviour
    {
        [SerializeField]
        private GameCharacterController characterController = null;

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