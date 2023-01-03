using UnityEngine;
using Fusion;

/// <summary>
/// Object used as a projectile.
/// </summary>
public class Sample_Ball : NetworkBehaviour
{
    /// <summary> Timer used to destroy the object after a certain amount of time. </summary>
    [Networked] private TickTimer life { get; set; }

    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5f);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
            Runner.Despawn(Object);
        else
            transform.position += 5 * transform.forward * Runner.DeltaTime;
    }
}
