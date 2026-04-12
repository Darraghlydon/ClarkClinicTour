using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class MouseRaycastLayerCheck : MonoBehaviour
{
    [SerializeField] private Camera _targetCamera;
    [SerializeField] private float _maxDistance = 100f;
    [SerializeField] private LayerMask _raycastLayers;
    [SerializeField] private PlayerNavigationController _playerNavigationController;
    [SerializeField] private float _navMeshSampleDistance = 2f;

    private void Awake()
    {
        if (_targetCamera == null)
            _targetCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            CheckMouseRaycast();
        }
    }

    private void CheckMouseRaycast()
    {
        if (_targetCamera == null || _playerNavigationController == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = _targetCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _raycastLayers))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            InfoPointController infoPoint = hit.collider.GetComponentInParent<InfoPointController>();

            if (infoPoint != null)
            {
                Vector3 destination = hit.point;

                Transform orientation = infoPoint.GetInfoPointPlayerOrientation();
                if (orientation != null)
                {
                    destination = orientation.position;
                }

                if (NavMesh.SamplePosition(destination, out NavMeshHit navHit, _navMeshSampleDistance, NavMesh.AllAreas))
                {
                    destination = navHit.position;
                    _playerNavigationController.MoveToInfoPoint(destination, infoPoint);
                    Debug.Log("InfoPoint destination sampled to NavMesh: " + destination);
                }
                else
                {
                    Debug.LogWarning("Could not find nearby NavMesh position for info point: " + infoPoint.name);
                }
            }
            else
            {
                Vector3 destination = hit.point;

                if (NavMesh.SamplePosition(destination, out NavMeshHit navHit, _navMeshSampleDistance, NavMesh.AllAreas))
                {
                    destination = navHit.position;
                }

                _playerNavigationController.MoveToPoint(destination);
                Debug.Log("Other destination: " + destination);
            }
        }
    }
}