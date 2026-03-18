using UnityEngine;
using TMPro;

public class PlatformFPSLimiter : MonoBehaviour
{
    [SerializeField] private int _mobileFPS=30;

    void Start()
    {
        if (PlatformManager.IsTouchScreen())
        {
            Application.targetFrameRate = _mobileFPS;
        }

    }
}