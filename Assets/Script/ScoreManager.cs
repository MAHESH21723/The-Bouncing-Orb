using System;
using UnityEngine;

// Keeps the bounce score for the current level.
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; } = 0;

    // The interface listens to this to redraw the score.
    public event Action<int> ScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void AddBouncePoint()
    {
        Score += 1;
        ScoreChanged?.Invoke(Score);
    }

    public void ResetScore()
    {
        Score = 0;
        ScoreChanged?.Invoke(Score);
    }
}
