using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    const string BestKey = "BestScore";

    public int CurrentScore { get; private set; }
    public int BestScore { get; private set; }

    public static event System.Action<int, int> OnScoreChanged; // (current, best)

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        BestScore = PlayerPrefs.GetInt(BestKey, 0);
        GameManager.OnGameRestart += ResetScore;
    }

    void OnDestroy()
    {
        GameManager.OnGameRestart -= ResetScore;
        if (Instance == this)
            Instance = null;
    }

    void ResetScore()
    {
        CurrentScore = 0;
        OnScoreChanged?.Invoke(CurrentScore, BestScore);
    }

    // level × (level+1) / 2 base, multiplied by chain bonus
    public void AddScore(int level, int chainCount)
    {
        int baseScore = level * (level + 1) / 2;
        float multiplier =
            chainCount >= 3 ? 2f
            : chainCount == 2 ? 1.5f
            : 1f;
        Apply(Mathf.RoundToInt(baseScore * multiplier));
    }

    public void AddBonus(int bonus) => Apply(bonus);

    void Apply(int amount)
    {
        CurrentScore += amount;
        if (CurrentScore > BestScore)
        {
            BestScore = CurrentScore;
            PlayerPrefs.SetInt(BestKey, BestScore);
            PlayerPrefs.Save();
        }
        OnScoreChanged?.Invoke(CurrentScore, BestScore);
    }
}
