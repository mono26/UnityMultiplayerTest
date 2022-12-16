using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
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

    // Update is called once per frame
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
