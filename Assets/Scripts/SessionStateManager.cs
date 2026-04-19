using UnityEngine;

public class SessionStateManager : MonoBehaviour
{
    public static SessionStateManager Instance { get; private set; }

    public bool HasViewedControlsThisSession { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        HasViewedControlsThisSession = false;
    }

    public void MarkControlsAsViewed()
    {
        HasViewedControlsThisSession = true;
    }

    public void ResetControlsViewed()
    {
        HasViewedControlsThisSession = false;
    }
}
