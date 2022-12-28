using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out Sample_NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(data.direction * Runner.DeltaTime * 5);
        }
    }
}
