using UnityEngine;
using System.Runtime.InteropServices;

public static class PlatformManager
{
    public enum Platform
    {
        Editor,
        WebGLDesktop,
        WebGLAndroid,
        WebGLiOS,
        WebGLUnknownMobile,
        Android,
        iOS,
        Windows,
        Mac,
        Linux,
        Unknown
    }

    private enum WebGLPlatform
    {
        Desktop = 0,
        Android = 1,
        iOS = 2
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern int GetWebGLPlatformNative();

    [DllImport("__Internal")]
    private static extern int IsMobile();
#endif

    /// <summary>
    /// Main entry point
    /// </summary>
    public static Platform Current
    {
        get
        {
#if UNITY_EDITOR
            return Platform.Editor;

#elif UNITY_WEBGL
            var webglPlatform = (WebGLPlatform)GetWebGLPlatformNativeSafe();

            switch (webglPlatform)
            {
                case WebGLPlatform.Android:
                    return Platform.WebGLAndroid;

                case WebGLPlatform.iOS:
                    return Platform.WebGLiOS;

                default:
                    return IsWebGLMobileSafe()
                        ? Platform.WebGLUnknownMobile
                        : Platform.WebGLDesktop;
            }

#elif UNITY_ANDROID
            return Platform.Android;

#elif UNITY_IOS
            return Platform.iOS;

#elif UNITY_STANDALONE_WIN
            return Platform.Windows;

#elif UNITY_STANDALONE_OSX
            return Platform.Mac;

#elif UNITY_STANDALONE_LINUX
            return Platform.Linux;

#else
            return Platform.Unknown;
#endif
        }
    }

    public static bool IsTouchScreen()
    {
        switch (Current)
        {
            case Platform.Android:
            case Platform.iOS:
            case Platform.WebGLAndroid:
            case Platform.WebGLiOS:
            case Platform.WebGLUnknownMobile:
                return true;

            default:
                return false;
        }
    }

    private static int GetWebGLPlatformNativeSafe()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            return GetWebGLPlatformNative();
        }
        catch
        {
            return 0;
        }
#else
        return 0;
#endif
    }

    private static bool IsWebGLMobileSafe()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            return IsMobile() == 1;
        }
        catch
        {
            return false;
        }
#else
        return false;
#endif
    }
}
