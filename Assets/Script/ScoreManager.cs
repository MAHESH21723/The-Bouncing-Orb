using UnityEngine;
using TMPro; // Use if you're using TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public int Score { get; private set; } = 0;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes if needed
        UpdateScoreText();
    }
    public void AddBouncePoint()
    {
        Score += 1;
        UpdateScoreText();
    }
    public void ResetScore()
    {
        Score = 0;
        UpdateScoreText();
    }
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {Score}";
        }
    }
}
