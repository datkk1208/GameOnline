using Fusion;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    // Biến mạng đồng bộ điểm
    [Networked, OnChangedRender(nameof(OnScoreChanged))]
    public int Score { get; set; }

    void Update()
    {
        // Phím tắt để test điểm nhanh (Chỉ người sở hữu nhân vật mới bấm được)
        if (HasStateAuthority && Input.GetKeyDown(KeyCode.F))
        {
            AddScore(10); // Giả lập giết địch được 10 điểm
            SubmitScoreToPlayFab(); // Gửi thẳng lên PlayFab để test
        }
    }

    public void AddScore(int points)
    {
        if (HasStateAuthority)
        {
            Score += points;
            Debug.Log($"Điểm hiện tại của {PlayFabManager.Instance.DisplayName} là: {Score}");
        }
    }

    // Tách riêng hàm gửi điểm để sau này gọi khi Game Over
    public void SubmitScoreToPlayFab()
    {
        if (HasStateAuthority && PlayFabManager.Instance != null)
        {
            PlayFabManager.Instance.SendLeaderboard(Score);
        }
    }

    // Hàm cập nhật UI cá nhân
    void OnScoreChanged()
    {
        if (HasStateAuthority && HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateScore(Score);
        }
    }
}