using UnityEngine;

public static class TourSaveData
{
    private const string ControlsSeenKey = "ControlsSeen";

    public static bool HasSeenControls()
    {
        return PlayerPrefs.GetInt(ControlsSeenKey, 0) == 1;
    }

    public static void SetControlsSeen(bool value)
    {
        PlayerPrefs.SetInt(ControlsSeenKey, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}
