using UnityEngine;
using System.Runtime.InteropServices;
using TMPro;

public class WebGLPlatformChecker : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern int IsMobile();
    [SerializeField] private TMP_Text infoText;

    public static bool IsWebGLMobile()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            return IsMobile() == 1;
        #else
        return false; 
        #endif
    }

    void Start()
    {
        if (IsWebGLMobile())
        {
            Debug.Log("Running on WebGL - Mobile");
            infoText.text = "Mobile";
            infoText.enabled = true;
        }
        else
        {
            Debug.Log("Running on WebGL - Desktop");
            infoText.text = "Not Mobile";
            infoText.enabled = true;
        }
    }

}
