using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMaster : MonoBehaviour
{
    StarterInputActions inputActions;

    public static NetworkInputData data = new NetworkInputData();

    private Vector3 moveVectorInput;
    private bool jumpRequest;

    public Transform cam;

    private void Awake()
    {
        inputActions = new StarterInputActions();

        inputActions.Player.Move.performed += Move;
        inputActions.Player.Move.canceled += ctx =>
        {
            //data.direction = Vector3.zero;
            //moveVectorInput = Vector3.zero;
        };

        inputActions.Player.Jump.performed += Jump;
        inputActions.Player.Jump.canceled += ctx =>
        {
            //jumpRequest = false;
            data.jumpRequest = false;
        };

    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        data.jumpRequest = true;
        //jumpRequest = true;
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        //data.direction.x = ctx.ReadValue<Vector2>().x;
        //data.direction.z = ctx.ReadValue<Vector2>().y;
        /* moveVectorInput.x = ctx.ReadValue<Vector2>().x;
        moveVectorInput.z = ctx.ReadValue<Vector2>().y; */
    }

    private void Update()
    {
    }

    public void OnEnable() => inputActions.Enable();
    public void OnDisable() => inputActions.Disable();

/*     public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.direction = moveVectorInput;
        networkInputData.jumpRequest = jumpRequest;
        return networkInputData;

    } */
}
