using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

namespace TomAg
{
    public class PlayerMotor : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField]
        private float walkForce = 510;
        [SerializeField]
        private float strafeForce = 450;

        [Header("Rotation")]
        [SerializeField] 
        private float rotationSensitivity = 10;
        [SerializeField] 
        private Transform cameraTransform; 

        [Header("Jump")]
        [SerializeField]
        public float jumpForce = 100f;
        [SerializeField]
        private float movementAttenuationOnJump = 0.5f;
        // [SerializeField]
        // private float coyoteTime = 0.2f;

        [Header("Ground")]
        [SerializeField]
        private LayerMask groundMask;
        [SerializeField]
        private float groundMaxDistance = 0.3f;
        [SerializeField]
        private float groundMaxDistanceAfterJump = 0.05f;
        [SerializeField]
        private float dragOnGround = 8f;
        [SerializeField]
        private float movementAttenuationInAir = 0.1f;

        [Header("Spring")]
        [SerializeField]
        private float rideSpringStrength = 1f;
        [SerializeField]
        private float rideSpringDampener = 1f;
        [SerializeField]
        private float rideSpringMaxDistance = 0.2f;
        [SerializeField]
        private float rideSpringHeight = 1f;
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
        private float _lastGroundedTime;  

        public bool isGrounded => _isGrounded;

        public delegate void GroundChangeHandler(bool isGrounded);
        public event GroundChangeHandler onGroundChanged; 

        public delegate void JumpHandler();
        public event JumpHandler onJump;  

        private void Awake()
        {
            if (!TryGetComponent(out _controller))
                Debug.LogError("Missing PlayerController", this);

            if (!TryGetComponent(out _rb))
                Debug.LogError("Missing Rigidbody", this);

            if (cameraTransform == null)
                cameraTransform = Camera.main.transform;

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
            UpdateDrag();
            //UpdateRotation();
            UpdateMove();

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

            bool wasGrounded = _isGrounded; // Sauvegarde l'état précédent

            if (Physics.Raycast(origin, direction, distance, groundMask, QueryTriggerInteraction.Ignore))
            {
                _isGrounded = true;

                if (!wasGrounded)
                {
                    onGroundChanged?.Invoke(true);
                    _isJumping = false; // Si au sol, alors il ne saute plus
                    _rb.drag = 8f; // Assurez-vous de définir le drag immédiatement lors de l'atterrissage
                }

                _lastGroundedTime = Time.timeSinceLevelLoad;
                Debug.DrawLine(origin, origin + direction * distance, Color.green);
            }
            else
            {
                _isGrounded = false;

                if (wasGrounded)
                {
                    onGroundChanged?.Invoke(false);
                    _rb.drag = 1f; // Définir le drag à 1 quand il quitte le sol
                }
            }
        }

        /* Ancien UpdateGrounded
         * 
         * private void UpdateGrounded()
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
                if (!_isGrounded)
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
        }*/

        private bool PlayerHasStartedJumping() 
        {
            return Time.timeSinceLevelLoad < _lastJumpTime + rideSpringJumpTime;
        }

        private void UpdateDrag()
        {

            if (_isGrounded)
            {
                _rb.drag = dragOnGround;
            }
            else
            {
                _rb.drag = 0;
                ApplyAirAttenuation();
            }
        }

        private void ApplyAirAttenuation()
        {
            // Apply attenuation when in the air to simulate reduced control
            Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            Vector3 attenuatedVelocity = horizontalVelocity * movementAttenuationInAir;
            _rb.velocity = new Vector3(attenuatedVelocity.x, _rb.velocity.y, attenuatedVelocity.z);
        }

        private void OnMove(Vector2 axis)
        {
            _moveInput = new Vector2(axis.x, axis.y);
        }

        private void OnAim(Vector2 axis)
        {
            _localOffsetAngleY = axis.x * rotationSensitivity;
        }

        private void OnJumpStart()
        {
            if (_isJumping) 
                return;

            if (!PlayerIsGrounded()) 
                return;

            _rb.velocity *= movementAttenuationOnJump;
            _rb.AddRelativeForce(0, jumpForce, 0, ForceMode.Impulse);
            _lastJumpTime = Time.timeSinceLevelLoad;
            _isJumping = true;
            _rb.drag = 1f;

            onJump?.Invoke(); 
        }

        private void OnJumpStop()
        {
            _isJumping = false;
        }

        private void OnCrouchStart() { }

        private void OnCrouchStop() { }

        private void UpdateMove()
        {
            if (_moveInput == Vector3.zero)
                return;

            // Obtient la rotation de la cam�ra (en ignorant la rotation X/inclinaison)
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            // Projette les vecteurs sur le plan horizontal
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calcule la direction du mouvement bas�e sur la rotation de la cam�ra
            Vector3 moveDirection = (cameraForward * _moveInput.y * walkForce) +
                                  (cameraRight * _moveInput.x * strafeForce);

            // Applique le mouvement
            _rb.AddForce(moveDirection * Time.fixedDeltaTime, ForceMode.VelocityChange);

            // Fait pivoter le personnage vers la direction du mouvement
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSensitivity));
            }
        }

        private bool PlayerIsGrounded()
        {
            var origin = transform.position + transform.up;
            var direction = -transform.up;
            float distance = 1 + groundMaxDistance;

            if (PlayerHasStartedJumping())
                distance = 1 + groundMaxDistanceAfterJump;

            return Physics.Raycast(origin, direction, distance, groundMask, QueryTriggerInteraction.Ignore);
        }


        private void UpdateSpring()
        {
            if (_isJumping)
                return;

            var origin = transform.position + transform.up;
            var direction = -transform.up;
            float distance = 1 + rideSpringMaxDistance;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, groundMask, QueryTriggerInteraction.Ignore))
            {
                // Calculate velocities on direction
                float playerVelocityOnDirection = Vector3.Dot(direction, _rb.velocity);
                float hitBodyVelocityOnDirection = hit.rigidbody != null ? Vector3.Dot(direction, hit.rigidbody.velocity) : 0;

                // Resulting velocity
                float relativeVelocity = playerVelocityOnDirection - hitBodyVelocityOnDirection;

                // Distance inside hit object
                float x = hit.distance - rideSpringHeight;

                // Calculate the Force to apply
                float springForce = (x * rideSpringStrength) - (relativeVelocity * rideSpringDampener);

                // Apply force to Player
                _rb.AddForce(direction * springForce);

                // Apply force to hit body
                if (hit.rigidbody != null)
                    hit.rigidbody.AddForceAtPosition(-direction * springForce, hit.point);
            }
        }

        private void UpdateRotation()
        {
            // Mettre à jour l'angle de rotation en fonction de l'axe de visée
            _rotationAngleY += _localOffsetAngleY * Time.fixedDeltaTime;
            Quaternion newRotation = Quaternion.Euler(0, _rotationAngleY, 0);
            _rb.MoveRotation(newRotation);
        }
    }
}
