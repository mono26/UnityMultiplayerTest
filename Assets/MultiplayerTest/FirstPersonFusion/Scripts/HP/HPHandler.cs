using System.Collections;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

/// <summary>
/// This script is attached to the player object in the scene. It handles the player's health and death.
/// </summary>
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

    // death particle effect
    public GameObject deathGameObjectPrefab;

    public bool skipSettingStartValues = false;

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
        if (!skipSettingStartValues)
        {
            HP = startingHP;
            isDead = false;
        }

        defaultMeshColor = bodyMeshRenderer.material.color;

        isInitialized = true;
    }

    /// <summary> Visual effect to know when player is hit by other players </summary>
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

    /// <summary> Revives the player. </summary>
    IEnumerator ServerRevive()
    {
        yield return new WaitForSeconds(2f);

        characterMovementHandler.RequestSpawn();
    }

    /// <summary> This method is called when the player is hit by an enemy. </summary>
    /// <param name="damageCausedBy">The name of the player who hit the player.</param>
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

    /// <summary> This method is called when the player hits an enemy. </summary>
    /// <param name="changed"> The Changed object that contains the old and new values of the property. </param>
    static void OnHPChanged(Changed<HPHandler> changed)
    {
        byte newHP = changed.Behaviour.HP;

        changed.LoadOld();

        byte oldHP = changed.Behaviour.HP;

        if (newHP < oldHP)
            changed.Behaviour.OnHPReduce();
    }

    /// <summary> This method is called when the player's HP is reduced. </summary>
    private void OnHPReduce()
    {
        if (!isInitialized)
            return;

        StartCoroutine(OnHit());
    }

    /// <summary> This method is called when the player's state is changed. </summary>
    /// <param name="changed"> The Changed object that contains the old and new values of the property. </param>
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

    /// <summary> This method is called when the player dies. </summary>
    private void OnDeath()
    {
        playerModel.gameObject.SetActive(false);
        hitboxRoot.HitboxRootActive = false;
        characterMovementHandler.SetCharacterControllerEnabled(false);

        Instantiate(deathGameObjectPrefab, transform.position, Quaternion.identity);
    }

    /// <summary> This method is called when the player is revived. </summary>
    private void onRevive()
    {
        if (Object.HasInputAuthority)
            uiOnHitImage.color = Color.clear;

        playerModel.gameObject.SetActive(true);
        hitboxRoot.HitboxRootActive = true;
        characterMovementHandler.SetCharacterControllerEnabled(true);
    }

    /// <summary> Resets player's HP when respawned </summary>
    public void OnRespawned()
    {
        HP = startingHP;
        isDead = false;
    }
}
