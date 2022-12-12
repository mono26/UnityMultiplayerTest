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
    //Color[] defaultMeshColor;

    public GameObject playerModel;
    public GameObject deathGameObjectPrefab;

    HitboxRoot hitboxRoot;
    CharacterMovementHandler characterMovementHandler;

    private void Awake()
    {
        hitboxRoot = GetComponent<HitboxRoot>();
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
    }

    void Start()
    {
        HP = startingHP;
        isDead = false;

        /* for (int i = 0; i < meshRenderer.materials.Length; i++)
        {
            defaultMeshColor[i] = meshRenderer.materials[i].color;
        } */
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

    public void OnTakeDamage()
    {
        if (isDead)
            return;
        
        HP--;

        if (HP <= 0)
        {
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
        playerModel.SetActive(false);
        hitboxRoot.HitboxRootActive = false;
        characterMovementHandler.SetCharacterControllerEnabled(false);

        Instantiate(deathGameObjectPrefab, transform.position, Quaternion.identity);
    }

    private void onRevive()
    {
        if (Object.HasInputAuthority)
            uiOnHitImage.color = Color.clear;
        
        playerModel.SetActive(true);
        hitboxRoot.HitboxRootActive = true;
        characterMovementHandler.SetCharacterControllerEnabled(true);
    }

    public void OnRespawned()
    {
        HP = startingHP;
        isDead = false;
    }
}
