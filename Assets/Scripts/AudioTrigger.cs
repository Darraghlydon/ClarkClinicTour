using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] float _resetTriggerTimeSeconds =80f;
    private AudioSource _audioSource;
    private Renderer _rend;
    public bool WaitingForCooldown = false;
    private bool _attachedToCharacter = false;
    private Collider _collider;

    public void Awake()
    {
  
        //_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<KeyboardAndMouseController>();
    }
    void Start()
    {
        _rend = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
        if (_rend == null)
        {
            _attachedToCharacter = true;
        }

        // Get the AudioSource component
        _audioSource = GetComponent<AudioSource>();
     }

    void OnEnable()
    {
        Events.AudioSkip.Subscribe(StopAudio);
        Events.AudioStop.Subscribe(StopAudio);
    }

    void OnDisable()
    {
        Events.AudioSkip.Unsubscribe(StopAudio);
        Events.AudioStop.Subscribe(StopAudio);
    }

    // Trigger detection
    private void OnTriggerEnter(Collider other) // For 3D Colliders
    {
      
        if (other.CompareTag("Player")&&!WaitingForCooldown) // Ensure the player has the "Player" tag
        {
            
            PlayAudio();
        }
    }

    public bool IsAudioPlaying()
    { 
        return _audioSource.isPlaying; 
    }
    private void ResetTrigger()
    {
        WaitingForCooldown = false;
        _collider.enabled = true;
        if (!_attachedToCharacter)
            _rend.enabled = true;
    }
    private void PlayAudio()
    {
        if (_audioSource && !_audioSource.isPlaying)
        {
            Events.AudioStop.Publish();
            _audioSource.Play();
            Events.AudioStart.Publish();
            if (!_attachedToCharacter)
                _rend.enabled = false;
            _collider.enabled = false;
            WaitingForCooldown = true;
            Invoke("WaitForAudio",_audioSource.clip.length);
            Invoke("ResetTrigger",_resetTriggerTimeSeconds);
        }
    }

    private void WaitForAudio()
    {
        if (_audioSource.isPlaying)
        {
            Events.AudioStop.Publish();
        }
    }

    private void StopAudio(InputAction.CallbackContext context)
    {
        _audioSource.Stop();
    }

    private void StopAudio()
    {
        _audioSource.Stop();
    }
}