using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

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
                Fire(inputData.aimForwardVector);
        }
    }

    void Fire(Vector3 aimForwardVector)
    {
        if (Time.time - lastTimeFired < 0.15f)
            return;
        
        StartCoroutine(FireEffect());

        Runner.LagCompensation.Raycast(aimPoint.position, aimForwardVector, 100, Object.InputAuthority, out var hitInfo, collisionLayers, HitOptions.IncludePhysX);

        float hitDistance = 100;
        bool isHitOtherPlayer = false;

        if (hitInfo.Distance > 0)
            hitDistance = hitInfo.Distance;
        
        if (hitInfo.Hitbox != null)
        {
            if (Object.HasStateAuthority)
                hitInfo.Hitbox.transform.root.GetComponent<HPHandler>().OnTakeDamage(networkPlayer.nickName.Value);

            isHitOtherPlayer = true;
        }
        else if (hitInfo.Collider != null)
        {
            Debug.Log($"Hit {hitInfo.Collider.gameObject.name}");
        }

        if (isHitOtherPlayer)
            Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.red, 1f);
        else
            Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.green, 1f);
        
        lastTimeFired = Time.time;
    }

    IEnumerator FireEffect()
    {
        isFiring = true;
        fireParticleSystem.Play();

        yield return new WaitForSeconds(0.09f);
        isFiring = false;
    }

    static void OnFireChanged(Changed<WeaponHandler> changed)
    {
        bool isFiringCurrent = changed.Behaviour.isFiring;

        changed.LoadOld();

        bool isFiringOld = changed.Behaviour.isFiring;

        if (isFiringCurrent && !isFiringOld)
            changed.Behaviour.OnFireRemote();
    }

    void OnFireRemote()
    {
        if (!Object.HasInputAuthority)
            fireParticleSystem.Play();
    }
}
