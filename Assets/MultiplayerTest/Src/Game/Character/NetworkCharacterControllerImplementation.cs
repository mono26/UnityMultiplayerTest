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

            this.Controller.Move(direction);

            this.Velocity = (this.transform.position - previousPos) * Runner.Simulation.Config.TickRate;
            // this.Velocity = this.Controller.velocity;
            this.IsGrounded = this.characterController.Grounded;
#endif
        }
    }
}