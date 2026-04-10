using Fusion;
using UnityEngine;
using TMPro;

public class NetworkManager : MonoBehaviour
{
    private NetworkRunner _runner;
    public TMP_InputField roomInput; // Kéo Input Field vào đây

    public async void StartSharedSession()
    {
        // 1. Khởi tạo NetworkRunner
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // 2. Lấy tên phòng, nếu để trống thì mặc định là "DevRoom"
        string roomName = string.IsNullOrEmpty(roomInput.text) ? "DevRoom" : roomInput.text;

        // 3. Kết nối vào Photon Cloud và tải GameScene
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = roomName,
            Scene = SceneRef.FromIndex(1),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            ObjectProvider = gameObject.AddComponent<NetworkObjectPool>()
        });
        // Chat tự kết nối khi player spawn qua ChatNetworkBehaviour.Spawned()
    }
}