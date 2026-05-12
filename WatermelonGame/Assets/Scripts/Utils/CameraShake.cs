using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    static CameraShake _instance;

    void Awake() => _instance = this;

    public static void Shake(float duration, float magnitude)
    {
        if (_instance != null)
            _instance.StartCoroutine(_instance.ShakeRoutine(duration, magnitude));
    }

    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Vector3 origin = transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = 1f - elapsed / duration;
            transform.localPosition = origin + (Vector3)Random.insideUnitCircle * magnitude * t;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = origin;
    }
}
