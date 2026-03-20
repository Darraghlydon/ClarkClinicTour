using UnityEngine;

public class RotateScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject _rotateScreen;
    [SerializeField] private GameObject _mainGUIScreen;

    private bool _wasPortrait;
    private float _storedTimeScale = 1f;
    private bool _storedAudioListenerState;
    private bool _rotateScreenActive;

    private void Start()
    {
        if (!PlatformManager.IsTouchScreen())
        {
            gameObject.SetActive(false);
            return;
        }

        _storedAudioListenerState = AudioListener.pause;
        _wasPortrait = IsPortrait();
        ApplyState(_wasPortrait, true);
    }

    private void Update()
    {
        bool isPortrait = IsPortrait();

        if (isPortrait != _wasPortrait)
        {
            _wasPortrait = isPortrait;
            ApplyState(isPortrait, false);
        }
    }

    private bool IsPortrait()
    {
        return Screen.height > Screen.width;
    }

    private void ApplyState(bool isPortrait, bool isInitialSetup)
    {
        if (isPortrait)
        {
            if (!_rotateScreenActive)
            {
                _storedTimeScale = Time.timeScale;
                _storedAudioListenerState = AudioListener.pause;
                AudioListener.pause = true;
                _rotateScreenActive = true;
            }

            _rotateScreen.SetActive(true);
            _mainGUIScreen.SetActive(false);
            Time.timeScale = 0f;
        }
        else
        {
            _rotateScreen.SetActive(false);
            _mainGUIScreen.SetActive(true);

            if (_rotateScreenActive && !isInitialSetup)
            {
                Time.timeScale = _storedTimeScale;
                AudioListener.pause = _storedAudioListenerState; ;
            }

            _rotateScreenActive = false;
        }
    }
}