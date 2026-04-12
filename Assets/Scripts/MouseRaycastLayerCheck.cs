using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MouseRaycastLayerCheck : MonoBehaviour
{
    [SerializeField] private Camera _targetCamera;
    [SerializeField] private float _maxDistance = 100f;
    [SerializeField] private LayerMask _raycastLayers;
    [SerializeField] private PlayerNavigationController _playerNavigationController;
    [SerializeField] private float _navMeshSampleDistance = 2f;
    [SerializeField] private KeyboardAndMouseController _keyboardAndMouseController;

    private void Awake()
    {
        if (!PlatformManager.IsTouchScreen())
        {
            this.enabled=false;
        }
        if (_targetCamera == null)
            _targetCamera = Camera.main;
    }

    private void Update()
    {
        if (TryGetPointerReleasePosition(out Vector2 pointerPosition))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Over GUI Element");
                return;
            }

            if (_playerNavigationController != null && _playerNavigationController.IsNavigationActive())
            {
                Debug.Log("Already navigating, raycast ignored.");
                return;
            }

            CheckRaycast(pointerPosition);
        }
    }

    private bool TryGetPointerReleasePosition(out Vector2 pointerPosition)
    {
        pointerPosition = default;

        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            pointerPosition = Mouse.current.position.ReadValue();
            return true;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
        {
            pointerPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }

        return false;
    }

    private void CheckRaycast(Vector2 screenPosition)
    {
        if (_targetCamera == null || _playerNavigationController == null)
            return;

        Ray ray = _targetCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _raycastLayers))
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            InfoPointController infoPoint = hit.collider.GetComponentInParent<InfoPointController>();

            if (infoPoint != null)
            {
                Events.InfoPointSelected.Publish();

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