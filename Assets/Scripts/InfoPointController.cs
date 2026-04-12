using TMPro;
using UnityEngine;

public class InfoPointController : MonoBehaviour
{
    [SerializeField] private float _resetTriggerTimeSeconds = 80f;
    [SerializeField] private GameObject _infoSign;
    [SerializeField] private bool _movePlayerToInfoPoint = true;
    [SerializeField] private Transform _playerOrientation;
    [SerializeField] private string _infoPointHeadingText;
    [SerializeField] private string _subtitleText;

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

    void OnEnable()
    {
        Events.InfoPointSelected.Subscribe(OnInfoPointSelected);
    }

    void OnDisable()
    {
        Events.InfoPointSelected.Unsubscribe(OnInfoPointSelected);
    }

    void OnInfoPointSelected()
    {
        StopAudioExternally();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || WaitingForCooldown)
            return;

        PlayerNavigationController navigation = other.GetComponentInParent<PlayerNavigationController>();

        if (navigation != null)
        {
            if (navigation.IsNavigationActive())
            {
                if (!navigation.IsNavigatingToInfoPoint(this))
                    return;

                navigation.StopAtInfoPoint(this);
            }
        }

        PlayAudio();
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
            Events.InfoPointStart.Publish(this);
            Events.DisplaySubtitles.Publish(_subtitleText, _audioSource.clip.length);
            Events.DisplayInfoPointHeading.Publish(_infoPointHeadingText);

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
        Events.InfoPointStop.Publish(this);
    }

    private void ResetTrigger()
    {
        WaitingForCooldown = false;
        _collider.enabled = true;

        if (!_attachedToCharacter)
            _rend.enabled = true;
        if (_infoSign != null)
            _infoSign.SetActive(true);
    }

    // Called by GUIManager when skip is pressed
    public void StopAudioExternally()
    {
        if (_audioSource && _audioSource.isPlaying)
        {
            _audioSource.Stop();
            Events.InfoPointStop.Publish(this);
        }
        Events.SubtitlesSkip.Publish();
        Events.ClearInfoPointHeading.Publish();
        CancelInvoke(nameof(NotifyAudioFinished));
    }

    public Transform GetInfoPointPlayerOrientation()
    {
        return _playerOrientation;
    }
    public bool ShouldMovePlayerToInfoPoint()
    {
        return _movePlayerToInfoPoint;
    }
}
