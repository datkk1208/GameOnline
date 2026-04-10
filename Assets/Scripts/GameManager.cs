using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    // Thời gian trôi qua từ đầu trận đấu
    [Networked] public float MatchTime { get; set; }

    public override void FixedUpdateNetwork()
    {
        // Chỉ State Authority (người tạo ra đối tượng GameManager này) mới được phép tăng biến thời gian
        if (HasStateAuthority)
        {
            MatchTime += Runner.DeltaTime;
        }
    }

    public override void Render()
    {
        // Liên tục cập nhật thời gian lên UI, cho dù chạy trên máy nào
        if (HUDManager.Instance != null && HUDManager.Instance.timeText != null)
        {
            HUDManager.Instance.UpdateTime(MatchTime);
        }
    }
}
