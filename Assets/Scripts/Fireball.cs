using Fusion;
using UnityEngine;

public class Fireball : NetworkBehaviour
{
    public float speed = 15f;
    public GameObject explosionVFX;
    public float lifeTime = 3f;

    [Networked] private TickTimer lifeTimer { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            lifeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if (lifeTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
            return;
        }

        // Tính toán khoảng cách viên đạn sẽ bay trong khung hình này
        float moveDistance = speed * Runner.DeltaTime;

        // Bắn tia dò đường TRƯỚC KHI di chuyển thực sự
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, moveDistance))
        {
            // Kiểm tra xem có trúng bản thân người bắn không
            NetworkObject hitNetObj = hit.collider.GetComponent<NetworkObject>();
            if (hit.collider.CompareTag("Player") && hitNetObj != null && hitNetObj.StateAuthority == Object.StateAuthority)
            {
                // Trúng chính mình -> Bỏ qua không nổ
            }
            else
            {
                // Trúng tường, trúng đất hoặc người chơi khác -> NỔ
                RPC_ShowExplosion(hit.point);
                Runner.Despawn(Object); // Hủy viên đạn
                return; // Dừng code lại ngay lập tức
            }
        }

        // Nếu tia dò đường không đụng gì -> cho phép viên đạn bay lên phía trước
        transform.position += transform.forward * moveDistance;
    }

    // Hàm gọi nổ trên mọi máy
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ShowExplosion(Vector3 pos)
    {
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, pos, Quaternion.identity);
        }
    }
}