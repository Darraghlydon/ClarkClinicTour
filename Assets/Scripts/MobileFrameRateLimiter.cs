using UnityEngine;

public class MobileFrameRateLimiter : MonoBehaviour
{
    [SerializeField] private int _targetFrameRate = 30;
    void Awake()
    {
        QualitySettings.vSyncCount = 0;

        if (PlatformManager.IsTouchScreen())
        {
            Application.targetFrameRate = _targetFrameRate;
            Debug.Log("Mobile detected - target frame rate set to 30");
        }
        else
        {
            Debug.Log("Non-mobile detected");
        }
    }
}