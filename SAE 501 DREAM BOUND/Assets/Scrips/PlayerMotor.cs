using UnityEngine;

namespace TomAg
{
    public class PlayerMotor : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkForce = 100;
        [SerializeField] private float strafeForce = 80;

        [Header("Rotation")]
        [SerializeField] private float rotationSensivity = 10;
        [SerializeField] private Transform cameraTransform; // Référence à la caméra

        [Header("Jump")]
        [SerializeField] private float jumpForce = 10;

        private Rigidbody _rb;
        private PlayerController _controller;
        private Vector3 _moveInput;
        private float _localOffsetAngleY;
        private float _rotationAngleY;

        private void Awake()
        {
            if (!TryGetComponent(out _rb))
                Debug.LogError("Missing Rigidbody", this);
            if (!TryGetComponent(out _controller))
                Debug.LogError("Missing PlayerController", this);

            // Vérifie que la caméra est assignée
            if (cameraTransform == null)
                cameraTransform = Camera.main.transform;

            // Add event hooks
            _controller.onMove += OnMove;
            _controller.onAim += OnAim;
            _controller.onJumpStart += OnJumpStart;
            _controller.onJumpStop += OnJumpStop;
            _controller.onCrouchStart += OnCrouchStart;
            _controller.onCrouchStop += OnCrouchStop;
        }

        private void FixedUpdate()
        {
            UpdateMove();
        }

        private void OnMove(Vector2 axis)
        {
            _moveInput = new Vector2(axis.x, axis.y);
        }

        private void OnAim(Vector2 axis)
        {
            _localOffsetAngleY = axis.x * rotationSensivity;
        }

        private void OnJumpStart()
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private void OnJumpStop() { }
        private void OnCrouchStart() { }
        private void OnCrouchStop() { }

        private void UpdateMove()
        {
            if (_moveInput == Vector3.zero)
                return;

            // Obtient la rotation de la caméra (en ignorant la rotation X/inclinaison)
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            // Projette les vecteurs sur le plan horizontal
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calcule la direction du mouvement basée sur la rotation de la caméra
            Vector3 moveDirection = (cameraForward * _moveInput.y * walkForce) +
                                  (cameraRight * _moveInput.x * strafeForce);

            // Applique le mouvement
            _rb.AddForce(moveDirection * Time.fixedDeltaTime, ForceMode.VelocityChange);

            // Fait pivoter le personnage vers la direction du mouvement
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSensivity));
            }
        }
    }
}