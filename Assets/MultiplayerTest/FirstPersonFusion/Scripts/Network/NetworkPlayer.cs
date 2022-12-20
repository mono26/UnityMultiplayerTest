using UnityEngine;
using Fusion;
using TMPro;

/// <summary>
/// This class represents the player in the game.
/// </summary>
public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

    public Transform playerModel;

    public TextMeshProUGUI playerNameText;

    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName { get; set; }

    [Networked] public int token { get; set; }

    bool isPublicJoinMessageSent = false;

    NetworkInGameMessages networkInGameMessages;

    public LocalCameraHandler localCameraHandler;
    public GameObject localUI;

    void Awake()
    {
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
    }

    /// <summary>
    /// Called when a player is spawned.
    /// </summary>
    public override void Spawned()
    {
        if (Object.HasInputAuthority) // This is the local player
        {
            Local = this;

            if (Camera.main != null)
                Camera.main.gameObject.SetActive(false);

            AudioListener audioListener = GetComponentInChildren<AudioListener>(true);
            audioListener.enabled = true;

            localCameraHandler.localCamera.enabled = true;
            localCameraHandler.transform.parent = null;

            localUI.SetActive(true);

            RPC_SetNickName(GameManager.instance.playerNickName);
        }
        else // This is a remote player
        {
            localCameraHandler.localCamera.enabled = false;
            localUI.SetActive(false);

            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;
        }

        Runner.SetPlayerObject(Object.InputAuthority, Object);

        transform.name = $"P_{Object.Id} - {nickName.Value}";
    }

    /// <summary>
    /// Called when a player leaves the game.
    /// </summary>
    /// <param name="player">The player who left the game.</param>
    public void PlayerLeft(PlayerRef player)
    {
        if (Object.HasInputAuthority)
        {
            if (Runner.TryGetPlayerObject(player, out NetworkObject playerLeftNetworkObject))
            {
                if (playerLeftNetworkObject == Object)
                    Local.GetComponent<NetworkInGameMessages>().SendInGameRPCMessage(playerLeftNetworkObject.GetComponent<NetworkPlayer>().nickName.Value, "left the game");
            }
        }

        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }

    static void OnNickNameChanged(Changed<NetworkPlayer> changed)
    {
        changed.Behaviour.OnNickNameChanged();
    }

    private void OnNickNameChanged()
    {
        playerNameText.text = nickName.Value;
    }

    /// <summary>
    /// Sets the nickname of the player. And sends a message to all players saying the player joined.
    /// </summary>
    /// <param name="nickName">The nickname to set.</param>
    /// <param name="info">The RPC info.</param>
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickName(string nickName, RpcInfo info = default)
    {
        this.nickName = nickName;

        if (!isPublicJoinMessageSent)
        {
            networkInGameMessages.SendInGameRPCMessage(nickName, "joined the game");

            isPublicJoinMessageSent = true;
        }
    }

    void OnDestroy()
    {
        if (localCameraHandler != null)
            Destroy(localCameraHandler.gameObject);
    }
}
