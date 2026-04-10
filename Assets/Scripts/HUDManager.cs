using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    public Slider healthBar;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void UpdateHealth(float health)
    {
        if (healthBar != null)
        {
            healthBar.value = health;
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void UpdateTime(float timeInSeconds)
    {
        if (timeText != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(timeInSeconds);
            timeText.text = string.Format("Time: {0:D2}:{1:D2}", time.Minutes, time.Seconds);
        }
    }
}
