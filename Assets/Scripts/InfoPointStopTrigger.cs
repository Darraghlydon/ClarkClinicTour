using UnityEngine;

public class InfoPointStopTrigger : MonoBehaviour
{
    [SerializeField] private InfoPointController infoPointController;

    private void OnTriggerEnter(Collider other)
    {
        PlayerNavigationController navigation = other.GetComponent<PlayerNavigationController>();

        if (navigation != null)
        {
            navigation.StopAtInfoPoint(infoPointController);
        }
    }
}