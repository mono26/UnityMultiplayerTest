#if UNITY_SERVER
#define GAME_SERVER
#undef GAME_CLIENT
#endif

using System.Collections;
using System.Collections.Generic;
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
            var deltaTime = Runner.DeltaTime;
            var previousPos = transform.position;
            var moveVelocity = Velocity;

            direction = direction.normalized;

            if (IsGrounded && moveVelocity.y < 0) {
                moveVelocity.y = 0f;
            }

            // moveVelocity.y += gravity * Runner.DeltaTime;

            var horizontalVel = default(Vector3);
            horizontalVel.x = moveVelocity.x;
            horizontalVel.z = moveVelocity.z;

            if (direction == default) {
                horizontalVel = Vector3.Lerp(horizontalVel, default, braking * deltaTime);
            }
            else {
                horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * acceleration * deltaTime, maxSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Runner.DeltaTime);
            }

            moveVelocity.x = horizontalVel.x;
            moveVelocity.z = horizontalVel.z;

            Controller.Move(moveVelocity * deltaTime);

            Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
            IsGrounded = this.characterController.Grounded;
#endif
        }
    }
}