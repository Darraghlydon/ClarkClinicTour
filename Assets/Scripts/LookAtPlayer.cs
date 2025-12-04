using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform _player;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _player = player.transform;
    }

    void Update()
    {
        if (_player == null) return;

        // Direction to the player but ignoring vertical difference
        Vector3 direction = _player.position - transform.position;
        direction.y = 0f; // Stop looking up/down

        if (direction.sqrMagnitude > 0.001f) // Avoid zero-length direction
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
    }
}