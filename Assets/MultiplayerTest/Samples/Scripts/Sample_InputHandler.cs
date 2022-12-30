using UnityEngine;
using UnityEngine.InputSystem;

public class Sample_InputHandler : MonoBehaviour
{
    Sample_InputActions inputActions;

    private Vector2 _moveInput;
    private bool _leftMouseButton = false;

    private void Awake() => inputActions = new Sample_InputActions();

    void Update()
    {
        _moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        if (inputActions.Player.MouseLeft.triggered)
            _leftMouseButton = true;
    }

    public Sample_NetworkInputData GetNetworkInputData()
    {
        Sample_NetworkInputData inputData = new Sample_NetworkInputData
        {
            direction = new Vector3(_moveInput.x, 0, _moveInput.y),
            leftMouseButton = _leftMouseButton
        };

        _leftMouseButton = false;

        return inputData;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
}
