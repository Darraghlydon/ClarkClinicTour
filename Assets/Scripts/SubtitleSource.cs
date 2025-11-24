using UnityEngine;

public class SubtitleSource : MonoBehaviour
{
    [SerializeField] private string _subtitleText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter(Collider other)
    {
        Events.DisplaySubtitles.Publish(_subtitleText);
    }
}
