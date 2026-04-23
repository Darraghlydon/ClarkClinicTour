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
    [SerializeField] private GUIManager guiManager;

    [Header("Buttons")]
    [SerializeField] private Button joystickButton;
    [SerializeField] private Button buttonModeButton;

    [Header("Button Background Images")]
    [SerializeField] private Image joystickButtonImage;
    [SerializeField] private Image buttonModeButtonImage;

    [Header("Optional Selected Indicators")]
    [SerializeField] private GameObject joystickSelectedIndicator;
    [SerializeField] private GameObject buttonSelectedIndicator;

    [Header("Colours")]
    [SerializeField] private Color selectedColor = new Color(0.10f, 0.85f, 0.95f, 1f);
    [SerializeField] private Color deselectedColor = new Color(0.25f, 0.25f, 0.60f, 1f);

    [Header("Fallback Default")]
    [SerializeField] private MobileControlStyle defaultStyle = MobileControlStyle.Joysticks;

    private MobileControlStyle _currentStyle;

    public MobileControlStyle CurrentStyle => _currentStyle;

    private void Awake()
    {
        if (joystickButton != null)
            joystickButton.onClick.AddListener(SelectJoysticks);

        if (buttonModeButton != null)
            buttonModeButton.onClick.AddListener(SelectButtons);
    }

    private void Start()
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

        if (guiManager != null)
            guiManager.SetControlStyle(_currentStyle);
    }

    private void UpdateVisuals()
    {
        bool joystickSelected = _currentStyle == MobileControlStyle.Joysticks;
        bool buttonsSelected = _currentStyle == MobileControlStyle.Buttons;

        if (joystickButtonImage != null)
            joystickButtonImage.color = joystickSelected ? selectedColor : deselectedColor;

        if (buttonModeButtonImage != null)
            buttonModeButtonImage.color = buttonsSelected ? selectedColor : deselectedColor;

        if (joystickSelectedIndicator != null)
            joystickSelectedIndicator.SetActive(joystickSelected);

        if (buttonSelectedIndicator != null)
            buttonSelectedIndicator.SetActive(buttonsSelected);
    }

    private void SaveControlStyle(MobileControlStyle style)
    {
        PlayerPrefs.SetInt(ControlStylePrefKey, (int)style);
        PlayerPrefs.Save();
    }

    private MobileControlStyle LoadSavedControlStyle()
    {
        int defaultValue = (int)defaultStyle;
        int savedValue = PlayerPrefs.GetInt(ControlStylePrefKey, defaultValue);

        if (System.Enum.IsDefined(typeof(MobileControlStyle), savedValue))
            return (MobileControlStyle)savedValue;

        return defaultStyle;
    }
}