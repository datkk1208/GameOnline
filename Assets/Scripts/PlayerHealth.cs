using Fusion;
using System.Collections;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [Networked] public float Health { get; set; }
    [Networked] public int Score { get; set; }

    [Networked] public bool IsDead { get; set; }

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

        if (HasInputAuthority && HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateHealth(Health);
            HUDManager.Instance.UpdateScore(Score);
        }
    }

    public override void Render()
    {
        // Khi giá trị của Health hoặc Score thay đổi, cập nhật ngay lên HUD
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(Health):
                    if (HasInputAuthority && HUDManager.Instance != null)
                        HUDManager.Instance.UpdateHealth(Health);
                    break;

                case nameof(Score):
                    if (HasInputAuthority && HUDManager.Instance != null)
                        HUDManager.Instance.UpdateScore(Score);
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
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));
        transform.position = randomPos;

        // Bật lại
        foreach (var mesh in meshParts) mesh.enabled = true;
        if (_controller != null) _controller.enabled = true;
        
        if (_animator != null) _animator.Play("Idle"); // Quay về đứng im
    }
}
