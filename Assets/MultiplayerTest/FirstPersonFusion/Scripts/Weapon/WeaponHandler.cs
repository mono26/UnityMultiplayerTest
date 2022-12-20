using System.Collections;
using UnityEngine;
using Fusion;
using System;

/// <summary>
/// Handles the weapon logic.
/// </summary>
public class WeaponHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnFireChanged))]
    public bool isFiring { get; set; }

    public ParticleSystem fireParticleSystem;
    public Transform aimPoint;
    public LayerMask collisionLayers;

    float lastTimeFired = 0f;

    HPHandler hpHandler;
    NetworkPlayer networkPlayer;

    private void Awake()
    {
        hpHandler = GetComponent<HPHandler>();
        networkPlayer = GetComponent<NetworkPlayer>();
    }

    public override void FixedUpdateNetwork()
    {
        if (hpHandler.isDead)
            return;

        if (GetInput(out NetworkInputData inputData))
        {
            if (inputData.fireRequest)
            {
                Fire(inputData.aimForwardVector);
            }
        }
    }

    /// <summary>
    /// Makes the player fire.
    /// </summary>
    /// <param name="aimForwardVector">Direction of the bullet.</param>
    void Fire(Vector3 aimForwardVector)
    {
        if (Time.time - lastTimeFired < 0.15f)
            return;

        Runner.LagCompensation.Raycast(aimPoint.position, aimForwardVector, 100, Object.InputAuthority, out var hitInfo, collisionLayers, HitOptions.IncludePhysX);

        StartCoroutine(FireEffect(aimForwardVector));

        float hitDistance = 100;

        if (hitInfo.Distance > 0)
            hitDistance = hitInfo.Distance;

        if (hitInfo.Hitbox != null) // hit a player
        {
            if (Object.HasStateAuthority)
                hitInfo.Hitbox.transform.root.GetComponent<HPHandler>().OnTakeDamage(networkPlayer.nickName.Value);
        }
        else if (hitInfo.Collider != null) // hit something else
        {
            Debug.Log($"Hit {hitInfo.Collider.gameObject.name}");
        }

        lastTimeFired = Time.time;
    }

    IEnumerator FireEffect(Vector3 aimForwardVector)
    {
        isFiring = true;
        fireParticleSystem.transform.rotation = Quaternion.LookRotation(aimForwardVector);
        fireParticleSystem.Play();

        yield return new WaitForSeconds(0.09f);
        isFiring = false;
    }

    /// <summary>
    /// Called when the player fires.
    /// </summary>
    /// <param name="changed">The changed data.</param>
    static void OnFireChanged(Changed<WeaponHandler> changed)
    {
        bool isFiringCurrent = changed.Behaviour.isFiring;

        changed.LoadOld();

        bool isFiringOld = changed.Behaviour.isFiring;

        if (isFiringCurrent && !isFiringOld)
            changed.Behaviour.OnFireRemote();
    }

    /// <summary>
    /// Called when another player fires.
    /// </summary>
    void OnFireRemote()
    {
        if (!Object.HasInputAuthority)
        {
            fireParticleSystem.Play();
        }
    }
}
