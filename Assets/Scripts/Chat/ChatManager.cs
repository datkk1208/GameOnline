using UnityEngine;

/// <summary>
/// Singleton quản lý chat. Không dùng Photon Chat SDK — dùng Fusion RPC qua ChatNetworkBehaviour.
/// </summary>
public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;

    /// <summary>Tên hiển thị của local player trong chat.</summary>
    public string LocalPlayerName { get; private set; } = "Player";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>Được gọi bởi ChatNetworkBehaviour.Spawned() khi player spawn.</summary>
    public void SetPlayerName(string name)
    {
        LocalPlayerName = name;
        Debug.Log($"[ChatManager] Player name đặt là: {name}");
    }

    /// <summary>Gửi tin nhắn public tới tất cả.</summary>
    public void SendPublicMessage(string message)
    {
        if (ChatNetworkBehaviour.Local == null)
        {
            Debug.LogWarning("[ChatManager] Chưa kết nối — player chưa spawn.");
            return;
        }
        ChatNetworkBehaviour.Local.RPC_SendPublicMessage(LocalPlayerName, message);
    }

    /// <summary>Gửi tin nhắn private tới một người.</summary>
    public void SendPrivateMessage(string targetPlayer, string message)
    {
        if (ChatNetworkBehaviour.Local == null)
        {
            Debug.LogWarning("[ChatManager] Chưa kết nối — player chưa spawn.");
            return;
        }
        ChatNetworkBehaviour.Local.RPC_SendPrivateMessage(LocalPlayerName, targetPlayer, message);
    }
}