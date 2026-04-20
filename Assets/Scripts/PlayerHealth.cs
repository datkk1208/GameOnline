using Fusion;
using System.Collections;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [Networked] public float Health { get; set; }
    [Networked] public int Score { get; set; }
    [Networked] public bool IsDead { get; set; }

    public WorldSpaceHealthBar worldSpaceHealthBar; 
    private ChangeDetector _changeDetector;

    private CharacterController _controller;
    private Animator _animator;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

   public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        
        if (HasStateAuthority)
        {
            Health = 100f;
            Score = 0;
            IsDead = false;
        }

        // Cập nhật UI 2D cá nhân (chỉ mình thấy)
        if (HasInputAuthority && HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateHealth(Health);
            HUDManager.Instance.UpdateScore(Score);
        }

        // THÊM ĐOẠN NÀY: Cập nhật thanh máu 3D ngay khi spawn (ai cũng thấy)
        if (worldSpaceHealthBar != null)
        {
            worldSpaceHealthBar.UpdateHealthBar(Health, 100f);
        }
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(Health):
                    // 1. Cập nhật UI màn hình của riêng người đó
                    if (HasInputAuthority && HUDManager.Instance != null)
                        HUDManager.Instance.UpdateHealth(Health);

                    // 2. THÊM DÒNG NÀY: Cập nhật thanh máu 3D trên đầu cho TẤT CẢ mọi người cùng thấy
                    if (worldSpaceHealthBar != null)
                        worldSpaceHealthBar.UpdateHealthBar(Health, 100f);
                    break;

                case nameof(Score):
                    if (HasInputAuthority && HUDManager.Instance != null)
                        HUDManager.Instance.UpdateScore(Score);
                    break;

                case nameof(IsDead):
                    if (IsDead && HasInputAuthority && PlayFabManager.Instance != null)
                    {
                        PlayFabManager.Instance.SendLeaderboard(Score);
                    }
                    break;
            }
        }
    }

    // Hàm gọi bởi Fireball.cs. Sẽ gửi lệnh chạy trên StateAuthority của người chơi này (để được phép sửa biến Health)
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage(float damage, PlayerRef shooterID)
    {
        if (IsDead) return;

        Health -= damage;
        Debug.Log("Took damage: " + damage + ". Current Health: " + Health);

        if (Health <= 0)
        {
            Health = 0;
            IsDead = true;
            
            // Xử lý báo điểm cho người bắn
            if (Runner.TryGetPlayerObject(shooterID, out NetworkObject shooterObject))
            {
                var shooterHealth = shooterObject.GetComponent<PlayerHealth>();
                if (shooterHealth != null)
                {
                    shooterHealth.RPC_AddScore(1);
                }
            }

            // Tiến hành hồi sinh
            StartCoroutine(RespawnRoutine());
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddScore(int points)
    {
        Score += points;
    }

    private IEnumerator RespawnRoutine()
    {
        // Phản hồi UI/Animator (nếu cần)
        if (_animator != null) _animator.SetTrigger("Die"); // Nếu model bạn có animation chết

        // Rớt xuống hoặc vô hình
        var meshParts = GetComponentsInChildren<Renderer>();
        foreach (var mesh in meshParts) mesh.enabled = false;
        
        if (_controller != null) _controller.enabled = false;

        yield return new WaitForSeconds(3f);

        // Hồi sinh xong, reset thông số
        Health = 100f;
        IsDead = false;
        
        // Random dịch chuyển nhẹ tới điểm khác
        Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f)); // Default fallback
        PlayerSpawner spawner = UnityEngine.Object.FindFirstObjectByType<PlayerSpawner>();
        if (spawner != null && spawner.spawnPoints != null && spawner.spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawner.spawnPoints.Length);
            if (spawner.spawnPoints[randomIndex] != null)
            {
                spawnPosition = spawner.spawnPoints[randomIndex].position;
            }
        }
        transform.position = spawnPosition;

        // Bật lại
        foreach (var mesh in meshParts) mesh.enabled = true;
        if (_controller != null) _controller.enabled = true;
        
        if (_animator != null) _animator.Play("Idle"); // Quay về đứng im
    }
}
