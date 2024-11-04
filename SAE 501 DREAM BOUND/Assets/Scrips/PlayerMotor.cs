using System;
using UnityEngine;

namespace TomAg
{
    public class PlayerMotor : MonoBehaviour
    {

        // PARAMS

        [Header("Movement")]
        [SerializeField] private float walkForce = 100;
        [SerializeField] private float strafeForce = 80;

        [Header("Rotation")]
        [SerializeField] private float rotationSensivity = 10;
        [SerializeField] private Transform cameraTransform; // Référence à la caméra

        [Header("Jump")]
        [SerializeField] 
        private float jumpForce = 10;
        [SerializeField]
        private float movementAttenuationOnJump = 0.5f;
        [SerializeField]
        private float coyoteTime = 0.2f;

        [Header("Ground")]
        [SerializeField]
        private LayerMask groundMask;
        [SerializeField]
        private float groundMaxDistance = 0.05f;
        [SerializeField]
        private float groundMaxDistanceAfterJump = 0.05f;
        [SerializeField]
        private float dragOnGround = 8;
        [SerializeField]
        private float movementAttenuationInAir = 0.1f;

        [Header("Spring")]
        [SerializeField]
        private float rideSpringStrength = 1;
        [SerializeField]
        private float rideSpringDamper = 1;
        [SerializeField]
        private float rideSpringMaxDistance = 0.2f;
        [SerializeField]
        private float rideSpringHeight = 1;
        [SerializeField]
        private float rideSpringJumpTime = 0.3f;


        private Rigidbody _rb;
        private PlayerController _controller;
        private Vector3 _moveInput;
        private float _localOffsetAngleY;
        private float _rotationAngleY;
        private bool _isGrounded;
        private bool _isJumping;
        private float _lastJumpTime;

        public bool isGrounded => _isGrounded;

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
            UpdateGrounded();
            UpdateSpring();
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
            // Do nothing if Player is jumping
            if (_isJumping)
                return;

            // Do nothing if not considered grounded in coyote style
            if (!PlayerIsGrounded())
            {
                return;
            }

            // Reduce velocity before jump
            _rb.velocity *= movementAttenuationOnJump;
            // Apply jump force
            _rb.AddRelativeForce(x: 0, jumpForce, z: 0, ForceMode.Impulse);
            // Jump time
            _lastJumpTime = Time.timeSinceLevelLoad;
            _isJumping = true;
            // Event
            onJump?.Invoke();
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
        private void UpdateGrounded()
        {

            // Skip calculation 50% of the time (1 frame every 2 frames)
            if (Time.frameCount % 2 == 0)
                return;

            var origin = transform.position + transform.up;
            var direction = -transform.up;
            float distance = 1 + groundMaxDistance;
            if (PlayerHasStartedJumping())
                distance = 1 + groundMaxDistanceAfterJump;

            if (Physics.Raycast(origin, direction, distance, groundMask, QueryTriggerInteraction.Ignore))
            {
                // Event
                if (! _isGrounded)
                    onGroundChanged?.Invoke(true);
                // State
                _isGrounded = true;
                // Has landed after jump
                if (!PlayerHasStartedJumping())
                    _isJumping = false;
                // Grounded time
                _lastGroundedTime = Time.timeSinceLevelLoad;
                // Debug
                Debug.DrawLine(origin, origin + direction * distance, Color.green);
            }
            else
            {
                _isGrounded = false;
                // Debug
                Debug.DrawLine(origin, origin + direction * distance, Color.red);

            }

            // Update Drag
            if (_isGrounded)
                _rb.drag = dragOnGround;
            else
                _rb.drag = 0;
        }

        private void UpdateSpring()
        {
            if (_isJumping)
                return;

            var origin = transform.position + transform.up;
            var direction = -transform.up;
            float distance = 1 + rideSpringMaxDistance;

            //Debug. DrawLine(origin, origin + direction * distance, Color.cyan);

            if (!Physics.Raycast(origin, direction, out var hit, distance, groundMask, QueryTriggerInteraction.Ignore))
                return;

            // Calculate velocities on direction
            float playerVelocityOnDirection = Vector3.Dot(direction, _rb.velocity);
            float hitBodyVelocityOnDirection = Vector3.Dot(direction, hit.rigidbody ? hit.rigidbody.velocity : direction);
    
            // Resulting velocity
            float relativeVelocity = playerVelocityOnDirection - hitBodyVelocityOnDirection;

            // Distance inside hit object
            float x = hit.distance - rideSpringHeight;

            // Calculate the Force to apply
            float springForce = (x * rideSpringStrength) - (relativeVelocity * rideSpringDamper);
            // Make the spring force independant from mass
            springForce *= _rb.mass;

            // Apply force to Player
            _rb.AddForce(direction * springForce);

            // Apply force to hit body
            if (hit.rigidbody != null)
                hit.rigidbody.AddForceAtPosition(direction * -springForce, hit.point);
        }

        private bool PlayerHasStartedJumping()
        {
            return Time.timeSinceLevelLoad < _lastJumpTime + rideSpringJumpTime;
        }
    }
}