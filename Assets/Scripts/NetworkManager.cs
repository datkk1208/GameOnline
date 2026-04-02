using Fusion;
using UnityEngine;
using TMPro; // Dùng cho TextMeshPro

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
           
            GameMode = GameMode.Shared, // Chế độ Shared
            SessionName = roomName,     // Tên phòng
            Scene = SceneRef.FromIndex(1), // Tải GameScene (Index số 1 trong Build Settings)
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
}