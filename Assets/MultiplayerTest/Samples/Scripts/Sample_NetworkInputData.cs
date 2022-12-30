using UnityEngine;
using Fusion;

public struct Sample_NetworkInputData : INetworkInput
{
    public Vector3 direction;
    public NetworkBool leftMouseButton;
}
