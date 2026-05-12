using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour
{
    [SerializeField]
    float timeout = 3f;

    readonly HashSet<Fruit> _fruitsInZone = new();
    float _timer;

    void OnTriggerEnter2D(Collider2D col)
    {
        Fruit f = col.GetComponent<Fruit>();
        if (f != null)
            _fruitsInZone.Add(f);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Fruit f = col.GetComponent<Fruit>();
        if (f != null)
            _fruitsInZone.Remove(f);
    }

    void Update()
    {
        if (
            GameManager.Instance == null
            || GameManager.Instance.State != GameManager.GameState.Playing
        )
        {
            _timer = 0f;
            return;
        }

        // Remove destroyed or currently-merging fruits
        _fruitsInZone.RemoveWhere(f => f == null || f.IsMerging);

        if (_fruitsInZone.Count > 0)
        {
            _timer += Time.deltaTime;
            if (_timer >= timeout)
                GameManager.Instance.TriggerGameOver();
        }
        else
        {
            _timer = 0f;
        }
    }

    // Read-only progress for UI (0~1)
    public float DangerProgress => _fruitsInZone.Count > 0 ? _timer / timeout : 0f;
}
