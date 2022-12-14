using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    bool jumpRequest = false;
    bool fireRequest = false;

    LocalCameraHandler localCameraHandler;
    CharacterMovementHandler characterMovementHandler;
    StarterInputActions inputActions;

    void Awake()
    {
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        inputActions = new StarterInputActions();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!characterMovementHandler.Object.HasInputAuthority) // Makes sure to run this only on the client been controlled
            return;                                             // by the local player
        // Move Input  
        moveInputVector = inputActions.Player.Move.ReadValue<Vector2>();

        // View Input
        viewInputVector = inputActions.Player.Look.ReadValue<Vector2>();

        // Jump Input
        if (inputActions.Player.Jump.triggered)
            jumpRequest = true;
        
        // Fire Input
        if (inputActions.Player.Fire.triggered)
            fireRequest = true;
        
        localCameraHandler.SetViewInputVector(viewInputVector);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.movementInput = moveInputVector;
        networkInputData.aimForwardVector = localCameraHandler.transform.forward; // Just sending the x axis for now
        networkInputData.jumpRequest = jumpRequest;
        networkInputData.fireRequest = fireRequest;

        jumpRequest = false;
        fireRequest = false;

        return networkInputData;
    }

    public void OnEnable() => inputActions.Enable();
    public void OnDisable() => inputActions.Disable();
}
