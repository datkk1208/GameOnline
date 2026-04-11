using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;
    
    public string PlayFabId { get; private set; }
    public string DisplayName { get; private set; }

    // Sự kiện callback khi có dữ liệu
    public Action OnLoginSuccess;
    public Action<List<PlayerLeaderboardEntry>> OnLeaderboardUpdate;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Login();
    }

  public void Login()
    {
        string myId = SystemInfo.deviceUniqueIdentifier;

#if UNITY_EDITOR
        // Phân biệt ID nếu đang chạy cửa sổ Clone
        if (ParrelSync.ClonesManager.IsClone())
        {
            myId += "_clone"; 
        }
#endif

        var request = new LoginWithCustomIDRequest
        {
            CustomId = myId, // Dùng myId thay vì SystemInfo... trực tiếp
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLogin, OnError);
    }
    private void OnLogin(LoginResult result)
    {
        Debug.Log("Đăng nhập PlayFab thành công! PlayFabId: " + result.PlayFabId);
        PlayFabId = result.PlayFabId;
        
        if (result.InfoResultPayload != null && 
            result.InfoResultPayload.PlayerProfile != null && 
            !string.IsNullOrEmpty(result.InfoResultPayload.PlayerProfile.DisplayName))
        {
            DisplayName = result.InfoResultPayload.PlayerProfile.DisplayName;
            Debug.Log("Chào mừng trở lại: " + DisplayName);
        }
        else
        {
            // Tự động gán tên ban đầu nếu chưa có
            SubmitName("Player_" + UnityEngine.Random.Range(1000, 9999));
        }

        OnLoginSuccess?.Invoke();
    }

    public void SubmitName(string name)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    private void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Đã cập nhật tên hiển thị: " + result.DisplayName);
        DisplayName = result.DisplayName;
    }

    // Gửi điểm lên bảng xếp hạng có tên là "Score"
    public void SendLeaderboard(int score)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Chưa đăng nhập PlayFab, không thể gửi điểm.");
            return;
        }

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Score",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdateSuccess, OnError);
    }

    private void OnLeaderboardUpdateSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Đã gửi điểm lên Leaderboard thành công!");
    }

    // Yêu cầu lấy dữ liệu Top 10 bảng xếp hạng "Score"
    public void GetLeaderboard()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Chưa đăng nhập PlayFab, không thể lấy Leaderboard.");
            return;
        }

        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    private void OnLeaderboardGet(GetLeaderboardResult result)
    {
        Debug.Log("Lấy danh sách Leaderboard thành công! Có " + result.Leaderboard.Count + " người.");
        OnLeaderboardUpdate?.Invoke(result.Leaderboard);
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Lỗi PlayFab: " + error.GenerateErrorReport());
    }
}
