using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This script handles the input for the player. It is attached to the player object in the scene.
/// </summary>
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

        if (inputActions.Player.Scape.triggered && Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (inputActions.Player.Scape.triggered && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

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

    /// <summary>
    /// Sets the input values to the network input data struct. This is called in <see cref="Spawner" /> class.
    /// </summary>
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
