using UnityEngine;
using TMPro;

public class PlatformUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private GameObject _onScreenSticks;

    void Start()
    {
        var platform = PlatformManager.Current;

        // Debug / info label
        if (_infoText != null)
        {
            _infoText.enabled = true;
            _infoText.text = platform.ToString();
        }

        // Show on-screen sticks on touch-like devices,
        if (_onScreenSticks != null)
        {
            bool useTouchControls = PlatformManager.IsTouchScreen();
            _onScreenSticks.SetActive(useTouchControls);
        }

        Debug.Log($"[PlatformUIController] Running on: {platform}");
    }
}
