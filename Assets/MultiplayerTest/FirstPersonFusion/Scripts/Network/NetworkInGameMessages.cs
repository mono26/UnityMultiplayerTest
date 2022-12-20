using UnityEngine;
using Fusion;

/// <summary>
/// This class is used to send messages to all players in the game.
/// </summary>
public class NetworkInGameMessages : NetworkBehaviour
{
    InGameMessagesUI inGameMessagesUI;

    /// <summary>
    /// Builds the message to send
    /// </summary>
    /// <param name="userNickName">The nickname of the user who sent the message.</param>
    /// <param name="message">The message to send.</param>
    public void SendInGameRPCMessage(string userNickName, string message)
    {
        RPC_InGameMessage($"<b>{userNickName}</b> {message}");
    }

    /// <summary>
    /// Sends the message to all players in the game.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="info">The RPC info.</param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_InGameMessage(string message, RpcInfo info = default)
    {
        if (inGameMessagesUI == null)
            inGameMessagesUI = NetworkPlayer.Local.localCameraHandler.GetComponentInChildren<InGameMessagesUI>();
        if (inGameMessagesUI != null)
            inGameMessagesUI.OnGameMessageReceived(message);
    }
}
