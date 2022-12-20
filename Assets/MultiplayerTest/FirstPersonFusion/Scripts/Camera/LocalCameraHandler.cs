using UnityEngine;

/// <summary>
/// This script is attached to the camera object in the scene. It handles the camera movement and rotation.
/// </summary>
public class LocalCameraHandler : MonoBehaviour
{
    /// <summary>
    /// The anchor point for the camera to follow.
    /// </summary>
    public Transform cameraAnchorPoint;

    Vector2 viewInput = Vector2.zero;
    float cameraRotationX = 0f;
    float cameraRotationY = 0f;

    public Camera localCamera;
    NetworkCharacterControllerPrototypeCustom characterController;

    void Awake()
    {
        localCamera = GetComponent<Camera>();
        characterController = GetComponentInParent<NetworkCharacterControllerPrototypeCustom>();
    }

    void Start()
    {
        cameraRotationX = GameManager.instance.cameraViewRotation.x;
        cameraRotationY = GameManager.instance.cameraViewRotation.y;
    }

    void LateUpdate()
    {
        if (cameraAnchorPoint == null || !localCamera.enabled)
            return;

        localCamera.transform.position = cameraAnchorPoint.position;

        cameraRotationX += viewInput.y * Time.deltaTime * characterController.viewUpDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);

        cameraRotationY += viewInput.x * Time.deltaTime * characterController.rotationSpeed;

        localCamera.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0f);
    }

    /// <summary>
    /// Sets the view input vector. This is called from <see cref="CharacterInputHandler"/> class.
    /// </summary>
    /// <param name="viewInput">The view input vector.</param>
    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }

    private void OnDestroy()
    {
        if (cameraRotationX != 0f || cameraRotationY != 0f)
            GameManager.instance.cameraViewRotation = new Vector2(cameraRotationX, cameraRotationY);
    }
}
