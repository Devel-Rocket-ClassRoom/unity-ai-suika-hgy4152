using UnityEngine;

public class MergeParticlePlayer : MonoBehaviour
{
    public static MergeParticlePlayer Instance { get; private set; }

    [SerializeField] ParticleSystem particlePrefab;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Play(Vector3 position, Color color)
    {
        if (particlePrefab != null)
            PlayFromPrefab(position, color);
        else
            PlayProgrammatic(position, color);
    }

    void PlayFromPrefab(Vector3 position, Color color)
    {
        var ps = Instantiate(particlePrefab, position, Quaternion.identity);
        var main = ps.main;
        main.startColor = color;
        ps.Play();
        Destroy(ps.gameObject, main.duration + main.startLifetime.constantMax + 0.1f);
    }

    void PlayProgrammatic(Vector3 position, Color color)
    {
        var go = new GameObject("MergeParticle");
        go.transform.position = position;

        var ps = go.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.loop = false;
        main.startLifetime = 0.55f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 4f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.22f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            Color.Lerp(color, Color.white, 0.4f),
            color
        );
        main.gravityModifier = 0.5f;
        main.maxParticles = 20;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 16) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.25f;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = new Material(Shader.Find("Sprites/Default"));

        ps.Play();
        Destroy(go, 1.5f);
    }
}
