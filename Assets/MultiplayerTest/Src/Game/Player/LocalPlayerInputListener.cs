﻿using Fusion;
using SLGFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MultiplayerTest
{
    public class LocalPlayerInputListener : SLGBehaviour
    {
        public Vector2 MoveInputValue { get; private set; }
        public Vector2 LookInputValue { get; private set; }
        public bool SprintInputValue { get; private set; }
        public bool JumpInputValue { get; private set; }

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        public void OnMove(InputValue value)
        {
            Log.Info("OnMove");

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
            Log.Info("OnLook");

            this.LookInput(value.Get<Vector2>());
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
            this.MoveInputValue = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            this.LookInputValue = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            this.JumpInputValue = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            this.SprintInputValue = newSprintState;
        }
    }
}