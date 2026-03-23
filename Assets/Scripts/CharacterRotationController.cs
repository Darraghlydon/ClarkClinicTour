using UnityEngine;

public class CharacterRotationController : MonoBehaviour
{
    public float rotationSpeed = 180f;

    private Quaternion _originalRotation;
    private bool _shouldRotateToTarget = false;
    private Transform _target;
    private InfoPointController _infoPointController;

    void Start()
    {
        _originalRotation = transform.rotation;
        _infoPointController = GetComponentInChildren<InfoPointController>();
    }

    void Update()
    {
        if (_shouldRotateToTarget && _target != null && _infoPointController.IsAudioPlaying())
        {
            // Get direction to target but remove vertical component (Y)
            Vector3 direction = _target.position - transform.position;
            direction.y = 0f;  

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
        else
        {
            // Rotate back to original rotation (Y only)
            // Extract only Y from original rotation
            Vector3 originalEuler = _originalRotation.eulerAngles;
            Vector3 currentEuler = transform.eulerAngles;

            Quaternion targetRot = Quaternion.Euler(
                0f,
                originalEuler.y,
                0f
            );

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _target = other.transform;
        _shouldRotateToTarget = true;
    }

}
