using UnityEngine;
using Fusion;

public class Sample_Player : NetworkBehaviour
{
    [SerializeField] private Sample_Ball _ballPrefab;

    private Vector3 _forward;
    private NetworkCharacterControllerPrototype _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
        _forward = transform.forward;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out Sample_NetworkInputData data))
        {
            data.direction.Normalize();

            if (data.direction != Vector3.zero)
                _forward = data.direction;
            if (data.leftMouseButton)
                ShootBall(data);

            _cc.Move(data.direction * Runner.DeltaTime * 5);
        }
    }

    public void ShootBall(Sample_NetworkInputData data)
    {
        Runner.Spawn(_ballPrefab,
                     transform.position + _forward,
                     Quaternion.LookRotation(_forward),
                     Object.InputAuthority,
                     (runner, o) => // Initialize the Ball before synchronizing it
                     {
                        o.GetComponent<Sample_Ball>().Init();
                     });
    }
}
