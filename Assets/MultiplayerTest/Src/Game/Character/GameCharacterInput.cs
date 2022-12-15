using UnityEngine;

namespace MultiplayerTest
{
    public class GameCharacterInput : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move = Vector2.zero;
        public Vector2 look = Vector2.zero;
        public bool jump = false;
        public bool sprint = false;

        [Header("Action Input Values")]
        public bool action = false;

        [Header("Movement Settings")]
        public bool analogMovement = false;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        public void MoveInput(Vector2 newMoveDirection)
        {
            this.move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            if (!this.cursorInputForLook) {
                return;
            }

            this.look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            this.jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            this.sprint = newSprintState;
        }

        public void ActionInput(bool newActionInput)
        {
            this.action = newActionInput;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            this.SetCursorState(this.cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}