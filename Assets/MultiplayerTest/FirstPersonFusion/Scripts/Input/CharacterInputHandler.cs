using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputHandler : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    bool jumpRequest = false;

    CharacterMovementHandler characterMovementHandler;
    StarterInputActions inputActions;

    void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
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
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");  

        characterMovementHandler.SetViewInputVector(viewInputVector);

        // View Input
        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y") * -1;

        // Jump Input
        jumpRequest = Input.GetButtonDown("Jump");         
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.movementInput = moveInputVector;
        networkInputData.rotationInput = viewInputVector.x; // Just sending the x axis for now
        
        networkInputData.jumpRequest = jumpRequest;

        return networkInputData;
    }
}
