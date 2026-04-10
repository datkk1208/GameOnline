using Fusion;
using UnityEngine;

/// <summary>
/// Đính kèm vào Player Prefab.
/// Dùng Fusion RPC để gửi/nhận chat messages giữa các player.
/// </summary>
public class ChatNetworkBehaviour : NetworkBehaviour
{
    public static ChatNetworkBehaviour Local;

    public override void Spawned()
    {
        // Trong Shared Mode: HasInputAuthority == true với object của chính mình
        if (HasInputAuthority)
        {
            Local = this;
            string playerName = "Player_" + Runner.LocalPlayer.PlayerId;
            ChatManager.Instance?.SetPlayerName(playerName);
            Debug.Log($"[Chat] Local chat ready, tên: {playerName}");
        }
    }

    /// <summary>Gửi tin nhắn public tới TẤT CẢ player.</summary>
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SendPublicMessage(string senderName, string message)
    {
        ChatUI.Instance?.OnMessageReceived(senderName, message, false);
    }

    /// <summary>Gửi tin nhắn private — gửi tới all nhưng chỉ sender và target hiển thị.</summary>
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SendPrivateMessage(string senderName, string targetName, string message)
    {
        string localName = ChatManager.Instance != null ? ChatManager.Instance.LocalPlayerName : "";

        // Chỉ hiển thị nếu mình là người gửi hoặc người nhận
        if (localName == senderName || localName == targetName)
        {
            string display = (localName == senderName)
                ? $">> {targetName}: {message}"
                : $"<< {senderName}: {message}";
            ChatUI.Instance?.OnMessageReceived(senderName, display, true);
        }
    }
}
