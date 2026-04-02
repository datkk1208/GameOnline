using Fusion;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerSpawner : MonoBehaviour
{
    public NetworkObject playerPrefab;

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

        // Sử dụng hàm SpawnAsync thay vì Spawn
        await runner.SpawnAsync(playerPrefab, new Vector3(0, 5, 0), Quaternion.identity, runner.LocalPlayer);
    }
}