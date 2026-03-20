using UnityEngine;

public class MobileFrameRateLimiter : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 0;

        if (PlatformManager.IsTouchScreen())
        {
            Application.targetFrameRate = 30;
            Debug.Log("Mobile detected - target frame rate set to 30");
        }
        else
        {
            Debug.Log("Non-mobile detected - frame rate unchanged");
        }
    }
}