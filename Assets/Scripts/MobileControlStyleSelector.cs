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

    [Header("Fallback Default")]
    [SerializeField] private MobileControlStyle _defaultStyle = MobileControlStyle.Joysticks;

    private Color _joystickBaseColor;
    private Color _buttonModeBaseColor;

    private MobileControlStyle _currentStyle;

    public MobileControlStyle CurrentStyle => _currentStyle;

    private void Awake()
    {
        if (_joystickButton != null)
            _joystickButton.onClick.AddListener(SelectJoysticks);

        if (_buttonModeButton != null)
            _buttonModeButton.onClick.AddListener(SelectButtons);

        if (_joystickButtonImage != null)
            _joystickBaseColor = _joystickButtonImage.color;

        if (_buttonModeButtonImage != null)
            _buttonModeBaseColor = _buttonModeButtonImage.color;
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


    private void ApplyButtonVisual(Button button, Image buttonImage, Color baseColor, bool isSelected)
    {
        if (button == null || buttonImage == null)
            return;

        ColorBlock colors = button.colors;
        Color tintColor = isSelected ? colors.selectedColor : colors.normalColor;

        buttonImage.color = MultiplyColors(baseColor, tintColor);
    }

    private Color MultiplyColors(Color a, Color b)
    {
        return new Color(
            a.r * b.r,
            a.g * b.g,
            a.b * b.b,
            a.a * b.a
        );
    }

    private void UpdateVisuals()
    {
        bool joystickSelected = _currentStyle == MobileControlStyle.Joysticks;
        bool buttonsSelected = _currentStyle == MobileControlStyle.Buttons;

        ApplyButtonVisual(_joystickButton, _joystickButtonImage, _joystickBaseColor, joystickSelected);
        ApplyButtonVisual(_buttonModeButton, _buttonModeButtonImage, _buttonModeBaseColor, buttonsSelected);

        if (_joystickInstructionsImageObject != null)
            _joystickInstructionsImageObject.SetActive(joystickSelected);

        if (_joystickInsructionsObject != null)
            _joystickInsructionsObject.SetActive(joystickSelected);

        if (_buttonInstructionsImageObject != null)
            _buttonInstructionsImageObject.SetActive(buttonsSelected);

        if (_buttonInsructionsObject != null)
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