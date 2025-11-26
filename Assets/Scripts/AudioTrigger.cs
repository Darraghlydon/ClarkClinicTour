using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] private float _resetTriggerTimeSeconds = 80f;
    [SerializeField] private GameObject _infoSign;

    private AudioSource _audioSource;
    private Renderer _rend;
    private Collider _collider;

    public bool WaitingForCooldown = false;
    private bool _attachedToCharacter = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _rend = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
        if (_rend == null)
        {
            _attachedToCharacter = true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !WaitingForCooldown)
        {
            PlayAudio();
        }
    }

    public bool IsAudioPlaying()
    {
        return _audioSource != null && _audioSource.isPlaying;
    }

    private void PlayAudio()
    {
        if (_audioSource && !_audioSource.isPlaying)
        {
            _audioSource.Play();
            Events.AudioStart.Publish(this);

            if (!_attachedToCharacter)
                _rend.enabled = false;

            if (_infoSign != null)
                _infoSign.SetActive(false);

            _collider.enabled = false;
            WaitingForCooldown = true;

            Invoke("NotifyAudioFinished", _audioSource.clip.length);
            Invoke("ResetTrigger", _resetTriggerTimeSeconds);
        }
    }

    private void NotifyAudioFinished()
    {
        // Called after clip length — treat as finished regardless of isPlaying flag
        Events.AudioStop.Publish(this);
    }

    private void ResetTrigger()
    {
        WaitingForCooldown = false;
        _collider.enabled = true;

        if (!_attachedToCharacter)
            _rend.enabled = true;
        if(_infoSign!=null)
            _infoSign.SetActive(false);
    }

    // Called by GUIManager when skip is pressed
    public void StopAudioExternally()
    {
        if (_audioSource && _audioSource.isPlaying)
        {
            _audioSource.Stop();
            Events.AudioStop.Publish(this);
        }
        CancelInvoke(nameof(NotifyAudioFinished));
    }
}
