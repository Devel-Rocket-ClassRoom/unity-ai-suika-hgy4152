using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip dropClip;
    [SerializeField] private AudioClip mergeClip;
    [SerializeField] private AudioClip clickClip;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }

    public void PlayDrop() => PlayClip(dropClip);
    public void PlayMerge() => PlayClip(mergeClip);
    public void PlayClick() => PlayClip(clickClip);

    private void PlayClip(AudioClip clip)
    {
        if (clip != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}
