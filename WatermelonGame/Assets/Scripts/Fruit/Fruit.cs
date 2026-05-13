using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(SpriteRenderer))]
public class Fruit : MonoBehaviour
{
    public int Level { get; private set; }
    public bool IsMerging { get; private set; }

    Rigidbody2D _rb;
    SpriteRenderer _sr;

    // ── Chain tracking (static, resets per drop) ──────────────────────────
    static int _chainCount = 0;
    static float _lastMergeTime = -999f;
    const float ChainWindow = 0.8f;

    public static void ResetChain()
    {
        _chainCount = 0;
        _lastMergeTime = -999f;
    }

    // ── Watermelon first-time bonus ────────────────────────────────────────
    static bool _watermelonBonusUsed;

    public static void ResetStatics()
    {
        ResetChain();
        _watermelonBonusUsed = false;
    }

    // ──────────────────────────────────────────────────────────────────────
    FruitConfig _config;

    public void Init(int level, FruitConfig config)
    {
        Level = level;
        _config = config;
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();

        var entry = config.Get(level);
        _sr.sprite = entry.sprite != null ? entry.sprite : SpriteFactory.GetCircle();

        // Base prefab: CircleCollider2D.radius = 0.5, scale = radius * 2
        // → effective physics radius matches entry.radius
        float s = entry.radius * 2f;
        transform.localScale = new Vector3(s, s, 1f);
    }

    public void SetKinematic(bool value) =>
        _rb.bodyType = value ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;

    void OnCollisionEnter2D(Collision2D col2d)
    {
        if (IsMerging)
            return;
        if (
            GameManager.Instance == null
            || GameManager.Instance.State != GameManager.GameState.Playing
        )
            return;

        Fruit other = col2d.gameObject.GetComponent<Fruit>();
        if (other == null || other.IsMerging || other.Level != Level)
            return;

        // Lower instanceID wins to avoid both sides triggering
        if (gameObject.GetInstanceID() > other.gameObject.GetInstanceID())
            return;

        StartCoroutine(MergeRoutine(other));
    }

    IEnumerator MergeRoutine(Fruit other)
    {
        IsMerging = true;
        other.IsMerging = true;

        AudioManager.Instance?.PlayMerge();
        MergeParticlePlayer.Instance?.Play((transform.position + other.transform.position) * 0.5f, _config.Get(Level).color);

        Vector3 mid = (transform.position + other.transform.position) * 0.5f;
        Color particleColor = _config != null ? _config.Get(Level).color : Color.white;
        MergeParticlePlayer.Instance?.Play(mid, particleColor);

        yield return StartCoroutine(PopAnim());

        // Chain bonus
        float now = Time.time;
        if (now - _lastMergeTime > ChainWindow)
            _chainCount = 0;
        _chainCount++;
        _lastMergeTime = now;

        ScoreManager.Instance?.AddScore(Level, _chainCount);

        // Watermelon + Watermelon → vanish
        if (Level == 11)
        {
            CameraShake.Shake(0.35f, 0.25f);
            Destroy(other.gameObject);
            Destroy(gameObject);
            yield break;
        }

        // First watermelon creation bonus (Lv.10 merge → Lv.11)
        if (Level == 10 && !_watermelonBonusUsed)
        {
            _watermelonBonusUsed = true;
            ScoreManager.Instance?.AddBonus(200);
        }

        if (_chainCount >= 2)
            CameraShake.Shake(0.15f, 0.12f);

        FruitSpawner.Instance?.SpawnMergedFruit(Level + 1, mid);

        Destroy(other.gameObject);
        Destroy(gameObject);
    }

    IEnumerator PopAnim()
    {
        Vector3 original = transform.localScale;
        float t = 0f,
            dur = 0.07f;
        while (t < dur)
        {
            transform.localScale = Vector3.Lerp(original, original * 1.35f, t / dur);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = original;
    }
}
