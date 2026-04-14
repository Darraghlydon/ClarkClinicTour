using UnityEngine;

public class DisableOnStartup : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
}
