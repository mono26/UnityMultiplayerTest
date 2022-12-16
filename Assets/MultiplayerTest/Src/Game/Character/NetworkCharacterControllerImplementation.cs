#if UNITY_SERVER
#define GAME_SERVER
#undef GAME_CLIENT
#endif

using Fusion;
using UnityEngine;

namespace MultiplayerTest
{
    public class NetworkCharacterControllerImplementation : NetworkCharacterControllerPrototype
    {
        private GameCharacterController characterController = null;

        protected override void Awake()
        {
            base.Awake();

            this.characterController = this.GetComponent<GameCharacterController>();
        }

        public override void Move(Vector3 direction)
        {
#if GAME_SERVER
            Vector3 previousPos = this.transform.position;

            Controller.Move(direction);

            Velocity = (this.transform.position - previousPos) * Runner.Simulation.Config.TickRate;
            IsGrounded = this.characterController.Grounded;
#endif
        }
    }
}