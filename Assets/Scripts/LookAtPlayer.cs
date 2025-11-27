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

        // Rotate this object to face the player
        transform.LookAt(_player);
    }
}