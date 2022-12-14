using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class HPHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHPChanged))]
    byte HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    bool isInitialized = false;

    const byte startingHP = 5;

    public Color uiOnHitColor;
    public Image uiOnHitImage;

    public SkinnedMeshRenderer bodyMeshRenderer;
    Color defaultMeshColor;

    public GameObject playerModel;
    public GameObject deathGameObjectPrefab;

    HitboxRoot hitboxRoot;
    CharacterMovementHandler characterMovementHandler;
    NetworkInGameMessages networkInGameMessages;
    NetworkPlayer networkPlayer;

    private void Awake()
    {
        hitboxRoot = GetComponent<HitboxRoot>();
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        networkPlayer = GetComponent<NetworkPlayer>();
    }

    void Start()
    {
        HP = startingHP;
        isDead = false;

        defaultMeshColor = bodyMeshRenderer.material.color;


        isInitialized = true;
    }

    IEnumerator OnHit()
    {
        bodyMeshRenderer.material.color = Color.white;

        if (Object.HasInputAuthority)
            uiOnHitImage.color = uiOnHitColor;
        
        yield return new WaitForSeconds(0.1f);

        bodyMeshRenderer.material.color = defaultMeshColor;
        
        if (Object.HasInputAuthority && !isDead)
            uiOnHitImage.color = Color.clear;
    }

    IEnumerator ServerRevive()
    {
        yield return new WaitForSeconds(2f);

        characterMovementHandler.RequestSpawn();
    }

    public void OnTakeDamage(string damageCausedBy)
    {
        if (isDead)
            return;
        
        HP--;

        if (HP <= 0)
        {
            networkInGameMessages.SendInGameRPCMessage(damageCausedBy, $"killed <b>{networkPlayer.nickName.Value}</b>");

            StartCoroutine(ServerRevive());
            isDead = true;
        }
    }

    static void OnHPChanged(Changed<HPHandler> changed)
    {
        byte newHP = changed.Behaviour.HP;

        changed.LoadOld();

        byte oldHP = changed.Behaviour.HP;

        if (newHP < oldHP)
            changed.Behaviour.OnHPReduce();
    }

    private void OnHPReduce()
    {
        if (!isInitialized)
            return;
        
        StartCoroutine(OnHit());
    }

    static void OnStateChanged(Changed<HPHandler> changed)
    {
        bool isDeadCurrent = changed.Behaviour.isDead;

        changed.LoadOld();

        bool isDeadOld = changed.Behaviour.isDead;

        if (isDeadCurrent)
            changed.Behaviour.OnDeath();
        else if (!isDeadCurrent && isDeadOld)
            changed.Behaviour.onRevive();
    }

    private void OnDeath()
    {
        playerModel.gameObject.SetActive(false);
        hitboxRoot.HitboxRootActive = false;
        characterMovementHandler.SetCharacterControllerEnabled(false);

        Instantiate(deathGameObjectPrefab, transform.position, Quaternion.identity);
    }

    private void onRevive()
    {
        if (Object.HasInputAuthority)
            uiOnHitImage.color = Color.clear;
        
        playerModel.gameObject.SetActive(true);
        hitboxRoot.HitboxRootActive = true;
        characterMovementHandler.SetCharacterControllerEnabled(true);
    }

    public void OnRespawned()
    {
        HP = startingHP;
        isDead = false;
    }
}
