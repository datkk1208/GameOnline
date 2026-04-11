#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class LeaderboardSetupEditor : EditorWindow
{
    [MenuItem("PlayFab/Setup Leaderboard UI")]
    public static void ShowWindow()
    {
        // Ngăn tạo trùng lắp PlayFabManager
        if (GameObject.Find("PlayFabManager") == null)
        {
            GameObject pfObj = new GameObject("PlayFabManager");
            pfObj.AddComponent<PlayFabManager>();
            Debug.Log("Created PlayFabManager.");
        }

        // Tìm Canvas
       Canvas canvas = null;
        // Quét toàn bộ Canvas, chỉ lấy Canvas 2D (UI màn hình), bỏ qua Canvas 3D (thanh máu)
        foreach (var c in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (c.renderMode == RenderMode.ScreenSpaceOverlay || c.renderMode == RenderMode.ScreenSpaceCamera)
            {
                canvas = c;
                break;
            }
        }
        if (canvas == null)
        {
            Debug.LogError("Error: Cannot find Canvas in scene");
            return;
        }

        if (canvas.transform.Find("LeaderboardPanel") == null)
        {
            // 1. LeaderboardPanel
            GameObject panelObj = new GameObject("LeaderboardPanel", typeof(RectTransform));
            panelObj.transform.SetParent(canvas.transform, false);
            RectTransform panelRT = panelObj.GetComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.2f, 0.2f);
            panelRT.anchorMax = new Vector2(0.8f, 0.8f);
            panelRT.offsetMin = Vector2.zero;
            panelRT.offsetMax = Vector2.zero;
            
            Image panelBg = panelObj.AddComponent<Image>();
            panelBg.color = new Color(0, 0, 0, 0.8f);
            
            // Tắt mặc định
            panelObj.SetActive(false);

            // Title
            GameObject titleObj = new GameObject("Title", typeof(RectTransform));
            titleObj.transform.SetParent(panelRT, false);
            RectTransform titleRT = titleObj.GetComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0, 0.85f);
            titleRT.anchorMax = new Vector2(1, 1);
            titleRT.offsetMin = Vector2.zero;
            titleRT.offsetMax = Vector2.zero;
            
            TextMeshProUGUI titleTxt = titleObj.AddComponent<TextMeshProUGUI>();
            titleTxt.text = "LEADERBOARD";
            titleTxt.fontSize = 40;
            titleTxt.alignment = TextAlignmentOptions.Center;
            titleTxt.color = Color.white;

            // Header Row
            GameObject headerObj = new GameObject("Header", typeof(RectTransform));
            headerObj.transform.SetParent(panelRT, false);
            RectTransform headerRT = headerObj.GetComponent<RectTransform>();
            headerRT.anchorMin = new Vector2(0.05f, 0.75f);
            headerRT.anchorMax = new Vector2(0.95f, 0.85f);
            headerRT.offsetMin = Vector2.zero;
            headerRT.offsetMax = Vector2.zero;
            
            HorizontalLayoutGroup headerGroup = headerObj.AddComponent<HorizontalLayoutGroup>();
            headerGroup.childForceExpandWidth = false;
            
            string[] headers = { "RANK", "NAME", "SCORE" };
            foreach(var h in headers)
            {
                GameObject colObj = new GameObject(h, typeof(RectTransform));
                colObj.transform.SetParent(headerRT, false);
                TextMeshProUGUI txt = colObj.AddComponent<TextMeshProUGUI>();
                txt.text = h;
                txt.fontSize = 24;
                txt.color = Color.yellow;
                txt.alignment = TextAlignmentOptions.Center;
                LayoutElement le = colObj.AddComponent<LayoutElement>();
                le.minWidth = (h == "NAME") ? 300 : 100;
            }

            // Content Container
            GameObject contentObj = new GameObject("Content", typeof(RectTransform));
            contentObj.transform.SetParent(panelRT, false);
            RectTransform contentRT = contentObj.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0.05f, 0.05f);
            contentRT.anchorMax = new Vector2(0.95f, 0.7f);
            contentRT.offsetMin = Vector2.zero;
            contentRT.offsetMax = Vector2.zero;
            
            VerticalLayoutGroup vlg = contentObj.AddComponent<VerticalLayoutGroup>();
            vlg.childForceExpandHeight = false;
            vlg.spacing = 10;

            // Leaderboard Entry Prefab
            GameObject entryPrefab = new GameObject("LeaderboardEntry", typeof(RectTransform));
            entryPrefab.transform.SetParent(panelRT, false); // Tạm để trong panel
            RectTransform entryRT = entryPrefab.GetComponent<RectTransform>();
            entryRT.sizeDelta = new Vector2(0, 40);
            
            HorizontalLayoutGroup hlg = entryPrefab.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandWidth = false;

            LeaderboardEntryUI entryScript = entryPrefab.AddComponent<LeaderboardEntryUI>();
            
            // Rank Text
            GameObject rankObj = new GameObject("Rank", typeof(RectTransform));
            rankObj.transform.SetParent(entryRT, false);
            TextMeshProUGUI rTxt = rankObj.AddComponent<TextMeshProUGUI>();
            rTxt.fontSize = 20;
            rTxt.alignment = TextAlignmentOptions.Center;
            LayoutElement leR = rankObj.AddComponent<LayoutElement>(); leR.minWidth = 100;
            entryScript.rankText = rTxt;

            // Name Text
            GameObject nameObj = new GameObject("Name", typeof(RectTransform));
            nameObj.transform.SetParent(entryRT, false);
            TextMeshProUGUI nTxt = nameObj.AddComponent<TextMeshProUGUI>();
            nTxt.fontSize = 20;
            nTxt.alignment = TextAlignmentOptions.Center;
            LayoutElement leN = nameObj.AddComponent<LayoutElement>(); leN.minWidth = 300;
            entryScript.nameText = nTxt;

            // Score Text
            GameObject scoreObj = new GameObject("Score", typeof(RectTransform));
            scoreObj.transform.SetParent(entryRT, false);
            TextMeshProUGUI sTxt = scoreObj.AddComponent<TextMeshProUGUI>();
            sTxt.fontSize = 20;
            sTxt.alignment = TextAlignmentOptions.Center;
            LayoutElement leS = scoreObj.AddComponent<LayoutElement>(); leS.minWidth = 100;
            entryScript.scoreText = sTxt;
            
            entryPrefab.SetActive(false); // Make it a prefab visually

            // Gắn script LeaderboardUI
            LeaderboardUI uiScript = canvas.gameObject.AddComponent<LeaderboardUI>();
            uiScript.leaderboardPanel = panelObj;
            uiScript.entryPrefab = entryPrefab;
            uiScript.contentContainer = contentRT;
            
            Debug.Log("Created Leaderboard UI components.");
        }
    }
}
#endif
