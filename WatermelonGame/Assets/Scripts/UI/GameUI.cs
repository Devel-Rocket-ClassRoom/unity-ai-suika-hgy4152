using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField]
    TMP_Text currentScoreText;

    [SerializeField]
    TMP_Text bestScoreText;

    [Header("Next Fruit Preview")]
    [SerializeField]
    Image nextFruitImage;

    [SerializeField]
    FruitConfig fruitConfig;

    [Header("Title Panel")]
    [SerializeField]
    GameObject titlePanel;

    [Header("Game Over Panel")]
    [SerializeField]
    GameObject gameOverPanel;

    [SerializeField]
    TMP_Text finalScoreText;

    [SerializeField]
    TMP_Text finalBestText;

    [Header("Pause Panel")]
    [SerializeField]
    GameObject pausePanel;

    [Header("Danger")]
    [SerializeField]
    Slider dangerFill;

    DangerZone _dangerZone;

    void OnEnable()
    {
        ScoreManager.OnScoreChanged += RefreshScore;
        FruitSpawner.OnNextFruitChanged += RefreshNext;
        GameManager.OnGameStart += OnGameStarted;
        GameManager.OnGameOver += OnGameOver;
        GameManager.OnPauseChanged += OnPause;
        GameManager.OnGoToTitle += OnGoToTitle;
    }

    void OnDisable()
    {
        ScoreManager.OnScoreChanged -= RefreshScore;
        FruitSpawner.OnNextFruitChanged -= RefreshNext;
        GameManager.OnGameStart -= OnGameStarted;
        GameManager.OnGameOver -= OnGameOver;
        GameManager.OnPauseChanged -= OnPause;
        GameManager.OnGoToTitle -= OnGoToTitle;
    }

    void Start()
    {
        bool waiting = GameManager.Instance?.State == GameManager.GameState.WaitingToStart;
        titlePanel?.SetActive(waiting);
        gameOverPanel?.SetActive(false);
        pausePanel?.SetActive(false);
        if (nextFruitImage)
        {
            var e = fruitConfig?.Get(1);
            nextFruitImage.sprite = e?.sprite != null ? e.sprite : SpriteFactory.GetCircle();
        }
        RefreshScore(0, PlayerPrefs.GetInt("BestScore", 0));
        _dangerZone = FindFirstObjectByType<DangerZone>();
    }

    void Update()
    {
        if (dangerFill != null && _dangerZone != null)
            dangerFill.value = _dangerZone.DangerProgress;

        if (Input.GetKeyDown(KeyCode.Escape))
            GameManager.Instance?.TriggerGameOver();
    }

    // ── Title / Restart ────────────────────────────────────────────────────
    void OnGameStarted()
    {
        titlePanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
    }

    // ── Score ──────────────────────────────────────────────────────────────
    void RefreshScore(int current, int best)
    {
        if (currentScoreText)
            currentScoreText.text = current.ToString("N0");
        if (bestScoreText)
            bestScoreText.text = $"BEST {best:N0}";
    }

    // ── Next Fruit ─────────────────────────────────────────────────────────
    void RefreshNext(int nextLevel)
    {
        if (nextFruitImage == null || fruitConfig == null)
            return;
        var entry = fruitConfig.Get(nextLevel);
        nextFruitImage.sprite = entry.sprite != null ? entry.sprite : SpriteFactory.GetCircle();
    }

    // ── Game Over ──────────────────────────────────────────────────────────
    void OnGameOver() => StartCoroutine(ShowGameOver());

    IEnumerator ShowGameOver()
    {
        yield return new WaitForSecondsRealtime(0.6f);
        gameOverPanel?.SetActive(true);
        int score = ScoreManager.Instance?.CurrentScore ?? 0;
        int best = ScoreManager.Instance?.BestScore ?? 0;
        if (finalScoreText)
            finalScoreText.text = score.ToString("N0");
        if (finalBestText)
            finalBestText.text = $"BEST {best:N0}";
    }

    // ── Pause ──────────────────────────────────────────────────────────────
    void OnPause(bool isPaused) => pausePanel?.SetActive(isPaused);

    void OnGoToTitle()
    {
        titlePanel?.SetActive(true);
        gameOverPanel?.SetActive(false);
        pausePanel?.SetActive(false);
    }

    // ── Button Callbacks ───────────────────────────────────────────────────
    public void OnPauseButton()
    {
        AudioManager.Instance?.PlayClick();
        GameManager.Instance?.TogglePause();
    }

    public void OnResumeButton()
    {
        AudioManager.Instance?.PlayClick();
        GameManager.Instance?.TogglePause();
    }

    public void OnRestartButton()
    {
        AudioManager.Instance?.PlayClick();
        GameManager.Instance?.RestartGame();
    }

    public void OnTitleButton()
    {
        AudioManager.Instance?.PlayClick();
        GameManager.Instance?.GoToTitle();
    }
    }
