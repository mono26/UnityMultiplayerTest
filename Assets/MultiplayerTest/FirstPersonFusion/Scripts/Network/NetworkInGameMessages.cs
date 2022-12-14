using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkInGameMessages : NetworkBehaviour
{
    InGameMessagesUI inGameMessagesUI;

    void Start()
    {
        
    }

    public void SendInGameRPCMessage(string userNickName, string message)
    {
        RPC_InGameMessage($"<b>{userNickName}</b> {message}");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_InGameMessage(string message, RpcInfo info = default)
    {
        if (inGameMessagesUI == null)
            inGameMessagesUI = NetworkPlayer.Local.localCameraHandler.GetComponentInChildren<InGameMessagesUI>();
        if (inGameMessagesUI != null)
            inGameMessagesUI.OnGameMessageReceived(message);
    }
}
