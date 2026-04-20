using Fusion;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerSpawner : MonoBehaviour
{
    public NetworkObject playerPrefab;
    [Header("Spawn Settings")]
    public Transform[] spawnPoints;

    private async void Start()
    {
        NetworkRunner runner = Object.FindFirstObjectByType<NetworkRunner>();

        // Chờ đến khi Runner kết nối xong
        while (runner == null || !runner.IsRunning)
        {
            await Task.Yield();
            runner = Object.FindFirstObjectByType<NetworkRunner>();
        }

        // Đợi thêm 0.5s để ổn định Scene
        await Task.Delay(500);

        // Chọn vị trí xuất hiện mặc định nếu mảng trống
        Vector3 spawnPosition = new Vector3(0, 5, 0);

        // Lấy vị trí ngẫu nhiên nếu có cấu hình Spawn Points
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            // Kiểm tra tránh Transform bị null
            if (spawnPoints[randomIndex] != null) 
            {
                spawnPosition = spawnPoints[randomIndex].position;
            }
        }

        // Sử dụng hàm SpawnAsync thay vì Spawn
        await runner.SpawnAsync(playerPrefab, spawnPosition, Quaternion.identity, runner.LocalPlayer);
    }
}