using UnityEngine;
using UnityEngine.UI;

public enum MobileControlStyle
{
    Joysticks = 0,
    Buttons = 1
}

public class ControlStyleSelector : MonoBehaviour
{
    private const string ControlStylePrefKey = "MobileControlStyle";

    [Header("References")]
    [SerializeField] private GUIManager _guiManager;

    [Header("Buttons")]
    [SerializeField] private Button _joystickButton;
    [SerializeField] private Button _buttonModeButton;

    [Header("Button Background Images")]
    [SerializeField] private Image _joystickButtonImage;
    [SerializeField] private Image _buttonModeButtonImage;

    [Header("Instructions")]
    [SerializeField] private GameObject _joystickInstructionsImageObject;
    [SerializeField] private GameObject _joystickInsructionsObject;
    [SerializeField] private GameObject _buttonInstructionsImageObject;
    [SerializeField] private GameObject _buttonInsructionsObject;

    [Header("Colours")]
    [SerializeField] private Color _selectedColor = new Color(0.10f, 0.85f, 0.95f, 1f);
    [SerializeField] private Color _deselectedColor = new Color(0.25f, 0.25f, 0.60f, 1f);

    [Header("Fallback Default")]
    [SerializeField] private MobileControlStyle _defaultStyle = MobileControlStyle.Joysticks;

    private MobileControlStyle _currentStyle;

    public MobileControlStyle CurrentStyle => _currentStyle;

    private void Awake()
    {
        if (_joystickButton != null)
            _joystickButton.onClick.AddListener(SelectJoysticks);

        if (_buttonModeButton != null)
            _buttonModeButton.onClick.AddListener(SelectButtons);
    }

    private void Start()
    {
        MobileControlStyle savedStyle = LoadSavedControlStyle();
        ApplySelection(savedStyle, true);
    }

    private void OnEnable()
    {
        MobileControlStyle savedStyle = LoadSavedControlStyle();
        ApplySelection(savedStyle, true);
    }

    public void SelectJoysticks()
    {
        ApplySelection(MobileControlStyle.Joysticks);
    }

    public void SelectButtons()
    {
        ApplySelection(MobileControlStyle.Buttons);
    }

    public void ApplySelection(MobileControlStyle style, bool forceNotify = false)
    {
        if (!forceNotify && _currentStyle == style)
            return;

        _currentStyle = style;

        SaveControlStyle(_currentStyle);
        UpdateVisuals();

        if (_guiManager != null)
            _guiManager.SetControlStyle(_currentStyle);
    }

    private void UpdateVisuals()
    {
        bool joystickSelected = _currentStyle == MobileControlStyle.Joysticks;
        bool buttonsSelected = _currentStyle == MobileControlStyle.Buttons;

        if (_joystickButtonImage != null)
            _joystickButtonImage.color = joystickSelected ? _selectedColor : _deselectedColor;


        if (_buttonModeButtonImage != null)
            _buttonModeButtonImage.color = buttonsSelected ? _selectedColor : _deselectedColor;

        _joystickInstructionsImageObject.SetActive(joystickSelected);
        _joystickInsructionsObject.SetActive(joystickSelected);
        _buttonInstructionsImageObject.SetActive(buttonsSelected);
        _buttonInsructionsObject.SetActive(buttonsSelected);

    }

    private void SaveControlStyle(MobileControlStyle style)
    {
        PlayerPrefs.SetInt(ControlStylePrefKey, (int)style);
        PlayerPrefs.Save();
    }

    private MobileControlStyle LoadSavedControlStyle()
    {
        int defaultValue = (int)_defaultStyle;
        int savedValue = PlayerPrefs.GetInt(ControlStylePrefKey, defaultValue);

        if (System.Enum.IsDefined(typeof(MobileControlStyle), savedValue))
            return (MobileControlStyle)savedValue;

        return _defaultStyle;
    }
}