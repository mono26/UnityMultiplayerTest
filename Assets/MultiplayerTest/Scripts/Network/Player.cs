using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _cc;

    [SerializeField]
    private float speed = 5f;
    public Transform cam;

    float targetAngle;
    float angle;
    Vector3 moveDir;
    float turnSmothVelocity;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            /* data.direction.Normalize();

            if (data.direction != Vector3.zero)
            {

            targetAngle = Mathf.Atan2(data.direction.x, data.direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmothVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            }
            else
                moveDir = Vector3.zero; */

            _cc.Move(speed * moveDir * Runner.DeltaTime);

            //_cc.Move(speed * data.direction * Runner.DeltaTime);

            if (data.jumpRequest)
                _cc.Jump();
        }
    }
}
