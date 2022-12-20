using Fusion;
using UnityEngine;

/// <summary>
/// Represents the input data sent to the server.
/// </summary>
public struct NetworkInputData : INetworkInput
{
    /// <summary> The movement input. </summary>
    public Vector2 movementInput;

    /// <summary> The aim input. </summary>
    public Vector3 aimForwardVector;

    /// <summary> The jump input. </summary>
    public NetworkBool jumpRequest;

    /// <summary> The fire input. </summary>
    public NetworkBool fireRequest;
}
