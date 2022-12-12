using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    bool jumpRequest = false;

    LocalCameraHandler localCameraHandler;
    StarterInputActions inputActions;

    void Awake()
    {
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        inputActions = new StarterInputActions();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Move Input  
        moveInputVector = inputActions.Player.Move.ReadValue<Vector2>();

        // View Input
        viewInputVector = inputActions.Player.Look.ReadValue<Vector2>();

        // Jump Input
        if (inputActions.Player.Jump.triggered)
            jumpRequest = true;
        
        localCameraHandler.SetViewInputVector(viewInputVector);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.movementInput = moveInputVector;
        networkInputData.aimForwardVector = localCameraHandler.transform.forward; // Just sending the x axis for now
        networkInputData.jumpRequest = jumpRequest;

        jumpRequest = false;

        return networkInputData;
    }

    public void OnEnable() => inputActions.Enable();
    public void OnDisable() => inputActions.Disable();
}
