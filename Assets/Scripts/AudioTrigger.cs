using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] float _resetTriggerTimeSeconds =80f;
    private AudioSource audioSource;
    private Renderer rend;
    public bool waitingForCooldown = false;
    public KeyboardAndMouseController playerController;
    private bool attachedToCharacter = false;
    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            attachedToCharacter = true;
        }

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
      // playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<KeyboardAndMouseController>();
        
     }

    // Trigger detection
    private void OnTriggerEnter(Collider other) // For 3D Colliders
    {
      
        if (other.CompareTag("Player")&&!waitingForCooldown) // Ensure the player has the "Player" tag
        {
            
            PlayAudio();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // For 2D Colliders
    {
        if (other.CompareTag("Player")) // Ensure the player has the "Player" tag
        {
            PlayAudio();
        }
    }


    private void ResetTrigger()
    {
        waitingForCooldown = false;
        if (!attachedToCharacter)
            rend.enabled = true;
    }
    private void PlayAudio()
    {
        if (audioSource && !audioSource.isPlaying)
        {
            audioSource.Play();
            if(!attachedToCharacter)
                rend.enabled = false;
            
            waitingForCooldown = true;
            playerController.PauseMovementForAudio(audioSource.clip.length);
            Invoke("ResetTrigger", _resetTriggerTimeSeconds);
        }
    }
}