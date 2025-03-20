using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public enum UIState
{
    Default,
    Start,
    Pause,
    ControlMethodSelection,
}

public class GUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private GameObject _startScreen;
    [SerializeField] private GameObject _controlMethodSelectorScreen;
    [SerializeField] private KeyboardAndMouseController _keyBoardAndMouseController;
    [SerializeField] private GameObject _defaultButtonPause;
    [SerializeField] private GameObject _defaultButtonStart;
    [SerializeField] private GameObject _defaultButtonControls;
    [SerializeField] private string _mainMenuSceneName;
    private PlayerActionsAutoGenerated _playerActions;

    private UIState _currentState;
    private UIState _storedUIState = UIState.Default;

    //private bool tourTextDisabled=false;
    private bool _tourEnterDisabled = false;

    public void Awake()
    {
        _playerActions = new PlayerActionsAutoGenerated();
        //_playerActions.Player.Menu.performed += DisplayPauseScreen;

    }


    public void Start()
    {
        _currentState = UIState.Start;
        SwitchState(_currentState);
    }

    void OnEnable()
    {
        _playerActions.Player.Enable();
        _playerActions.Player.Menu.performed += DisplayPauseScreen;
    }

    void OnDisable()
    {
        _playerActions.Player.Disable();
        _playerActions.Player.Menu.performed -= DisplayPauseScreen;
    }

    private void EnableMouse()
    {
        if (!WebGLPlatformChecker.IsWebGLMobile())
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void DisableMouse()
    {
        if (!WebGLPlatformChecker.IsWebGLMobile())
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void SwitchState(UIState newState)
    {
        switch (newState)
        {

            case UIState.Pause:
                EnableMouse();
                Pause();
                _pauseScreen.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_defaultButtonPause);
                break;

            case UIState.ControlMethodSelection:
                _storedUIState = _currentState;
                EnableMouse();
                Pause();
                _controlMethodSelectorScreen.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_defaultButtonControls);
                break;
            case UIState.Start:
                EnableMouse();
                Pause();
                _startScreen.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_defaultButtonStart);
                break;


            default:
                Unpause();
                break;
        }
        _currentState = newState;
    }

    void Pause()
    {
        _keyBoardAndMouseController.enabled = false;
        Time.timeScale = 0;
        AudioListener.pause = true;

    }

    void Unpause()
    {
        Time.timeScale = 1;
        _keyBoardAndMouseController.enabled = true;
        AudioListener.pause = false;

    }

    public void DisplayPauseScreen()
    {
        DeactivateScreens();
        SwitchState(UIState.Pause);
    }

    public void DisplayPauseScreen(InputAction.CallbackContext context)
    {
        if (CheckForOpenScreens() == false)
        {
            DeactivateScreens();
            SwitchState(UIState.Pause);
        }
    }

    public void DisplayControlMethodScreen(UIState uIState)
    {
        if (CheckForOpenScreens() == false)
        {
            DeactivateScreens();
            _storedUIState = uIState;
            SwitchState(UIState.ControlMethodSelection);
        }
    }

    public void DisplayControlMethodScreen()
    {
        DeactivateScreens();
        SwitchState(UIState.ControlMethodSelection);
    }

    private bool CheckForOpenScreens()
    {
        if (_startScreen.activeSelf == true||_pauseScreen.activeSelf == true || _controlMethodSelectorScreen.activeSelf == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DeactivateScreens()
    {
        DisableMouse();
        Unpause();
        _pauseScreen.SetActive(false);
        _controlMethodSelectorScreen.SetActive(false);
        _startScreen.SetActive(false);
        if (_storedUIState != UIState.Default)
        {
            SwitchState(_storedUIState);
            _storedUIState = UIState.Default;
        }

    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(_mainMenuSceneName);
    }

}
