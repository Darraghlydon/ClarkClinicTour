using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLPlatformChecker : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern int IsMobile();

    public static bool IsWebGLMobile()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            return IsMobile() == 1;
        #else
        return false; // Default to 'false' in the Editor or other platforms
        #endif
    }

    void Start()
    {
        if (IsWebGLMobile())
        {
            Debug.Log("Running on WebGL - Mobile");
        }
        else
        {
            Debug.Log("Running on WebGL - Desktop");
        }
    }
}
