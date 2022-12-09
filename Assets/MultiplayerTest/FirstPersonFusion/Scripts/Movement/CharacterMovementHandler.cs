using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    Vector2 viewInput;

    float cameraRotationX = 0f;

    NetworkCharacterControllerPrototypeCustom characterController;
    Camera localCamera;
    
    void Awake()
    {
        characterController = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        localCamera = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        // This just reflects locally, it doesn't send anything to the server
        // 
        cameraRotationX += viewInput.y * Time.deltaTime * characterController.viewUpDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);

        localCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            //Rotate
            characterController.Rotate(networkInputData.rotationInput);

            //Move
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();

            characterController.Move(moveDirection);

            //Jump
            if (networkInputData.jumpRequest)
                characterController.Jump();
        }
    }

    public void SetViewInputVector(Vector2 viewInputVector)
    {
        viewInput = viewInputVector;
    }
}
