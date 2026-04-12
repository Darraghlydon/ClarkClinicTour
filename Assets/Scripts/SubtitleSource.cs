using UnityEngine;

public class SubtitleSource : MonoBehaviour
{
    [SerializeField] private string _subtitleHeading;
    [SerializeField] private string _subtitleText;
    private  AudioSource _audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Events.DisplaySubtitles.Publish(_subtitleText, _audioSource.clip.length);
        }
        
    }
}
