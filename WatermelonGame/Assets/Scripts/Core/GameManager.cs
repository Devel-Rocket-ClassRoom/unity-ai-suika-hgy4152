using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        WaitingToStart,
        Playing,
        Paused,
        GameOver,
    }

    public GameState State { get; private set; } = GameState.WaitingToStart;

    public static event System.Action OnGameStart;
    public static event System.Action OnGameRestart;
    public static event System.Action OnGameOver;
    public static event System.Action<bool> OnPauseChanged;
    public static event System.Action OnGoToTitle;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void StartGame()
    {
        if (State != GameState.WaitingToStart)
            return;
        State = GameState.Playing;
        OnGameStart?.Invoke();
    }

    public void TriggerGameOver()
    {
        if (State != GameState.Playing && State != GameState.Paused)
            return;
        State = GameState.GameOver;
        Time.timeScale = 0f; // 물리·입력 완전 정지
        PlayerPrefs.SetInt(
            "LastScore",
            ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0
        );
        PlayerPrefs.Save();
        OnGameOver?.Invoke();
    }

    public void TogglePause()
    {
        if (State != GameState.Playing && State != GameState.Paused)
            return;
        State = State == GameState.Playing ? GameState.Paused : GameState.Playing;
        Time.timeScale = State == GameState.Paused ? 0f : 1f;
        OnPauseChanged?.Invoke(State == GameState.Paused);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        // 필드에 남은 과일 전부 제거
        foreach (var fruit in FindObjectsByType<Fruit>(FindObjectsSortMode.None))
            Destroy(fruit.gameObject);

        State = GameState.WaitingToStart;
        OnGameRestart?.Invoke(); // ScoreManager·FruitSpawner 리셋
        StartGame(); // → Playing, OnGameStart 발행 → UI·Spawner 동작
    }

    public void GoToTitle()
    {
        Time.timeScale = 1f;
        foreach (var fruit in FindObjectsByType<Fruit>(FindObjectsSortMode.None))
            Destroy(fruit.gameObject);
        State = GameState.WaitingToStart;
        OnGameRestart?.Invoke(); // ScoreManager·FruitSpawner 리셋
        OnGoToTitle?.Invoke(); // UI에서 타이틀 패널 표시
    }
}
