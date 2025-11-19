using UnityEngine;
using TMPro;

public class PlatformUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private GameObject[] _mobileGUIObjects;

    void Start()
    {
        var platform = PlatformManager.Current;

        // Debug / info label
        if (_infoText != null)
        {
            _infoText.enabled = true;
            _infoText.text = "Platform: " + platform.ToString();
        }

        // Show mobile GUI on touch-like devices,
        if (_mobileGUIObjects != null)
        {
            bool useTouchControls = PlatformManager.IsTouchScreen();
            foreach (var obj in _mobileGUIObjects)
            {
                obj.SetActive(useTouchControls);
            }
        }

        Debug.Log($"[PlatformUIController] Running on: {platform}");
    }
}
