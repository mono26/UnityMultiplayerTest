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

    void Start()
    {
        
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
            localcamera.enabled = false;

            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            Debug.Log("Spawned remote player");
        }

        transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
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
    }
}
