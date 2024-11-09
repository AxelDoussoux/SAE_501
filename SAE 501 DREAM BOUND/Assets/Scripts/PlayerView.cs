using UnityEngine;

namespace TomAg
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float aimSpeed = 10f;
        [SerializeField] private float angleMin = -30f;
        [SerializeField] private float angleMax = 60f;
        [SerializeField] private float cameraDistance = 5f;
        [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 2f, 0f);
        [SerializeField] private float smoothSpeed = 10f;
        [SerializeField] private float collisionOffset = 0.2f;

        private PlayerController _controller;
        private float _rotationX;
        private float _rotationY;
        private Vector3 _currentRotation;
        private Vector3 _smoothVelocity = Vector3.zero;
        private float _currentDistance;

        private void Awake()
        {
            if (!TryGetComponent(out _controller))
                Debug.LogError("Missing PlayerController");

            _controller.onAim += OnAim;
            _currentDistance = cameraDistance;

            // Initial position setup
            _camera.transform.position = transform.position + cameraOffset - transform.forward * cameraDistance;
            _camera.transform.LookAt(transform.position + cameraOffset);
        }

        private void LateUpdate()
        {
            UpdateCameraPosition();
        }

        private void OnAim(Vector2 axis)
        {
            _rotationX += axis.x * aimSpeed * Time.deltaTime;
            _rotationY -= axis.y * aimSpeed * Time.deltaTime;
            _rotationY = Mathf.Clamp(_rotationY, angleMin, angleMax);
        }

        private void UpdateCameraPosition()
        {
            // Calculate desired rotation
            _currentRotation = new Vector3(_rotationY, _rotationX, 0f);
            Quaternion rotation = Quaternion.Euler(_currentRotation);

            // Calculate desired position
            Vector3 targetPosition = transform.position + cameraOffset;
            Vector3 desiredPosition = targetPosition - rotation * Vector3.forward * cameraDistance;

            // Handle collision
            RaycastHit hit;
            if (Physics.Linecast(targetPosition, desiredPosition, out hit))
            {
                _currentDistance = Mathf.Clamp(hit.distance - collisionOffset, 0f, cameraDistance);
            }
            else
            {
                _currentDistance = cameraDistance;
            }

            // Apply position with smoothing
            Vector3 smoothedPosition = Vector3.SmoothDamp(
                _camera.transform.position,
                targetPosition - rotation * Vector3.forward * _currentDistance,
                ref _smoothVelocity,
                Time.deltaTime * smoothSpeed
            );

            // Update camera transform
            _camera.transform.position = smoothedPosition;
            _camera.transform.LookAt(targetPosition);
        }
    }
}