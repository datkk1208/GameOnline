using UnityEngine;
using System.Collections.Generic;
using PlayFab.ClientModels;

public class LeaderboardUI : MonoBehaviour
{
    public GameObject leaderboardPanel;
    public GameObject entryPrefab;
    public Transform contentContainer;

    private void OnEnable()
    {
        if (PlayFabManager.Instance != null)
        {
            PlayFabManager.Instance.OnLeaderboardUpdate += UpdateUI;
        }
    }

    private void OnDisable()
    {
        if (PlayFabManager.Instance != null)
        {
            PlayFabManager.Instance.OnLeaderboardUpdate -= UpdateUI;
        }
    }

    private void Start()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false); // Hide by default
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleLeaderboard();
        }
    }

    public void ToggleLeaderboard()
    {
        if (leaderboardPanel == null) return;
        
        bool isActive = !leaderboardPanel.activeSelf;
        leaderboardPanel.SetActive(isActive);

        if (isActive)
        {
            FetchLeaderboard();
        }
    }

    public void FetchLeaderboard()
    {
        if (PlayFabManager.Instance != null)
        {
            PlayFabManager.Instance.GetLeaderboard();
        }
    }

    private void UpdateUI(List<PlayerLeaderboardEntry> leaderboard)
    {
        if (contentContainer == null || entryPrefab == null) return;

        // Xoá các element cũ (Không xoá entryPrefab gốc)
        foreach (Transform child in contentContainer)
        {
            if (child.gameObject != entryPrefab)
            {
                Destroy(child.gameObject);
            }
        }

        // Tạo element mới
        foreach (var entry in leaderboard)
        {
            GameObject newObj = Instantiate(entryPrefab, contentContainer);
            newObj.SetActive(true); // Kích hoạt UI hiển thị
            LeaderboardEntryUI entryUI = newObj.GetComponent<LeaderboardEntryUI>();
            if (entryUI != null)
            {
                entryUI.Setup(entry.Position + 1, !string.IsNullOrEmpty(entry.DisplayName) ? entry.DisplayName : entry.PlayFabId, entry.StatValue);
            }
        }
    }
}
