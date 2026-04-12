using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycastLayerCheck : MonoBehaviour
{
    [SerializeField] private Camera _targetCamera;
    [SerializeField] private float _maxDistance = 100f;
    [SerializeField] private LayerMask _raycastLayers;
    [SerializeField] private PlayerNavigationController _playerNavigationController;

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

                _playerNavigationController.MoveToInfoPoint(destination, infoPoint);
                Debug.Log("Infopoint: " + hit.collider.gameObject.name);
            }
            else
            {
                _playerNavigationController.MoveToPoint(hit.point);
                Debug.Log("Other: " + hit.collider.gameObject.name);
            }
        }
    }
}