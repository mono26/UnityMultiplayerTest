using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerTest
{
    public class LocalPlayerInputHandler : MonoBehaviour
    {
        private NetworkInputData inputData = new NetworkInputData();

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        public void OnMove(InputValue value)
        {
            Vector2 inputValue = value.Get<Vector2>();
            if (inputValue != Vector2.zero) {
                // TODO don't use camera main.
                float targetRotation = Mathf.Atan2(inputValue.x, inputValue.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                Vector3 inputRotated = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
                this.MoveInput(new Vector2(inputRotated.x, inputRotated.z));
            }
            else {
                this.MoveInput(inputValue);
            }
        }

        public void OnLook(InputValue value)
        {
            LookInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            this.JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            this.SprintInput(value.isPressed);
        }
#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            this.inputData.MoveDirection = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            this.inputData.LookInput = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            this.inputData.Jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            this.inputData.Sprint = newSprintState;
        }
    }
}