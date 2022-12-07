using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMaster : MonoBehaviour
{
    StarterInputActions inputActions;

    public static NetworkInputData data = new NetworkInputData();

    private Vector2 _movementVector;

    private void Awake()
    {
        inputActions = new StarterInputActions();
        inputActions.Player.Move.performed += Move;
        inputActions.Player.Move.canceled += _ctx => data.direction = Vector3.zero;
    }

    public void Jump()
    {
        Debug.Log("Jumping");
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        _movementVector = ctx.ReadValue<Vector2>();

        data.direction.x = _movementVector.x;
        data.direction.z = _movementVector.y;
    }

    public void OnEnable() => inputActions.Enable();
    public void OnDisable() => inputActions.Disable();
}
