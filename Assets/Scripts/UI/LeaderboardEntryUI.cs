using UnityEngine;
using TMPro;

public class LeaderboardEntryUI : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    public void Setup(int rank, string playerName, int score)
    {
        if (rankText) rankText.text = rank.ToString();
        if (nameText) nameText.text = playerName;
        if (scoreText) scoreText.text = score.ToString();
    }
}
