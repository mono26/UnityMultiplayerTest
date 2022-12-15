using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

    public Transform playerModel;

    public TextMeshProUGUI playerNameText;

    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName { get; set; }

    bool isPublicJoinMessageSent = false;

    NetworkInGameMessages networkInGameMessages;

    public LocalCameraHandler localCameraHandler;
    public GameObject localUI;

    void Awake()
    {
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;

            Camera.main.gameObject.SetActive(false);

            RPC_SetNickName(PlayerPrefs.GetString("NickName", $"P_{Object.Id}"));

            Debug.Log("Spawned local player");
        }
        else
        {
            Camera localcamera = GetComponentInChildren<Camera>();
            Destroy(localcamera.gameObject);

            Debug.Log("Spawned remote player");
        }

        Runner.SetPlayerObject(Object.InputAuthority, Object);

        transform.name = $"P_{Object.Id}";
    }

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
}
