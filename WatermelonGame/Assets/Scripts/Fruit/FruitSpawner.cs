using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FruitSpawner : MonoBehaviour
{
    public static FruitSpawner Instance { get; private set; }

    [Header("References")]
    [SerializeField]
    FruitConfig config;

    [SerializeField]
    GameObject fruitBasePrefab;

    [SerializeField]
    LineRenderer dropGuide;

    [Header("Box Bounds")]
    [SerializeField]
    float spawnY = 4.5f;

    [SerializeField]
    float minX = -2.5f;

    [SerializeField]
    float maxX = 2.5f;

    [Header("Timing")]
    [SerializeField]
    float spawnDelay = 0.55f;

    Fruit _current;
    int _nextLevel;
    bool _canDrop = true;

    const int MaxDropLevel = 5;

    public static event System.Action<int> OnNextFruitChanged;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 싱글톤 인스턴스만 이벤트 구독
        GameManager.OnGameStart += HandleGameStart;
        GameManager.OnGameRestart += HandleRestart;
    }

    void OnDestroy()
    {
        GameManager.OnGameStart -= HandleGameStart;
        GameManager.OnGameRestart -= HandleRestart;
        if (Instance == this)
            Instance = null;
    }

    void Start()
    {
        _nextLevel = RandomLevel();
        SetGuideVisible(false);
    }

    void HandleGameStart()
    {
        Fruit.ResetStatics();
        SpawnCurrentFruit();
    }

    void HandleRestart()
    {
        // RestartGame() 이 과일을 이미 Destroy 한 뒤 호출됨
        StopAllCoroutines();
        _current = null;
        _canDrop = true;
        _nextLevel = RandomLevel();
        SetGuideVisible(false);
        // HandleGameStart 는 곧이어 발행되는 OnGameStart 가 처리
    }

    void Update()
    {
        if (!IsPlaying() || _current == null)
            return;
        MoveWithMouse();
        if (_canDrop)
            HandleDrop();
        UpdateGuide();
    }

    // ── Input ──────────────────────────────────────────────────────────────
    void MoveWithMouse()
    {
        Vector2 screen = Mouse.current.position.ReadValue();
        Vector3 world = Camera.main.ScreenToWorldPoint(
            new Vector3(screen.x, screen.y, Mathf.Abs(Camera.main.transform.position.z))
        );
        float x = Mathf.Clamp(world.x, minX, maxX);
        _current.transform.position = new Vector3(x, spawnY, 0f);
    }

    void HandleDrop()
    {
        bool drop =
            Mouse.current.leftButton.wasPressedThisFrame
            || Keyboard.current.spaceKey.wasPressedThisFrame;
        if (!drop)
            return;

        Fruit.ResetChain();
        _current.SetKinematic(false);
        _current = null;
        _canDrop = false;
        SetGuideVisible(false);

        StartCoroutine(DelayedSpawn());
    }

    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(spawnDelay);
        if (IsPlaying())
            SpawnCurrentFruit();
    }

    // ── Spawning ───────────────────────────────────────────────────────────
    void SpawnCurrentFruit()
    {
        int lvl = _nextLevel;
        _nextLevel = RandomLevel();
        OnNextFruitChanged?.Invoke(_nextLevel);

        _current = CreateFruit(lvl, new Vector3(0f, spawnY, 0f));
        _current.SetKinematic(true);
        _canDrop = true;
        SetGuideVisible(true);
    }

    public void SpawnMergedFruit(int level, Vector3 position)
    {
        if (config.IsValidLevel(level))
            CreateFruit(level, position);
    }

    Fruit CreateFruit(int level, Vector3 pos)
    {
        var go = Instantiate(fruitBasePrefab, pos, Quaternion.identity);
        var fruit = go.GetComponent<Fruit>();
        fruit.Init(level, config);
        return fruit;
    }

    // ── Drop Guide ─────────────────────────────────────────────────────────
    void UpdateGuide()
    {
        if (dropGuide == null || _current == null)
            return;
        float x = _current.transform.position.x;
        dropGuide.SetPosition(0, new Vector3(x, spawnY - 0.1f, 0f));
        dropGuide.SetPosition(1, new Vector3(x, -5.5f, 0f));
    }

    void SetGuideVisible(bool visible)
    {
        if (dropGuide != null)
            dropGuide.enabled = visible;
    }

    // ── Helpers ────────────────────────────────────────────────────────────
    bool IsPlaying() =>
        GameManager.Instance != null && GameManager.Instance.State == GameManager.GameState.Playing;

    int RandomLevel() => Random.Range(1, MaxDropLevel + 1);
}
