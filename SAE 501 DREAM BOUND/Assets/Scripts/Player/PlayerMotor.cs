using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

namespace TomAg
{
    public class PlayerMotor : NetworkBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkForce = 400f;
        [SerializeField] private float strafeForce = 400f;
        [SerializeField] private float maxVelocity = 30f;
        [SerializeField] private float maxVerticalVelocity = 50f;

        [Header("Sprint")]
        [SerializeField] private float sprintForceMultiplier = 2f;
        [SerializeField] private float sprintVelocityMultiplier = 2f;
        private float defaultWalkForce;
        private float defaultStrafeForce;
        private float defaultMaxVelocity;

        [Header("Jumping")]
        [SerializeField] private float jumpForce = 20f;
        [SerializeField] private float jumpHorizontalDrag = 2f; // Horizontal drag during jump
        [SerializeField] private float jumpVerticalDrag = 0f; // Vertical drag during jump

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundCheckDistance;
        [SerializeField] private float groundDrag;

        [Header("Slope Handling")]
        [SerializeField] private float maxSlopeAngle = 45f;
        [SerializeField] private float slopeForce = 10f;
        private RaycastHit slopeHit;
        private bool exitingSlope;

        [Header("Air Control")]
        [SerializeField] private float airControlFactor = 1f;
        [SerializeField] private float gravityMultiplier = 4f;

        [Header("Camera Settings")]
        [SerializeField] private Transform cameraTransform;

        private Rigidbody _rb;
        private PlayerController _controller;
        private Vector3 _moveInput;
        private PlayerInfo _playerInfo;

        public bool _isGrounded { get; private set; }
        public bool _isJumping { get; private set; }

        public delegate void GroundChangeHandler(bool isGrounded);
        public event GroundChangeHandler onGroundChanged;

        public delegate void JumpHandler();
        public event JumpHandler onJump;

        private void Awake()
        {
            // Initialize necessary components and settings
            if (!TryGetComponent(out _controller))
                Debug.LogError("Missing PlayerController", this);

            if (!TryGetComponent(out _rb))
                Debug.LogError("Missing Rigidbody", this);

            if (!cameraTransform)
                Debug.LogError("Camera Transform not assigned", this);

            InitializeRigidbody();

            _controller.onMove += OnMove;
            _controller.onJumpStart += OnJumpStart;
            _controller.onJumpStop += OnJumpStop;
            _controller.onSprintStart += OnSprintStart;
            _controller.onSprintStop += OnSprintStop;

            _playerInfo = GetComponent<PlayerInfo>();

            defaultWalkForce = walkForce;
            defaultStrafeForce = strafeForce;
            defaultMaxVelocity = maxVelocity;
        }

        private void InitializeRigidbody()
        {
            // Set up Rigidbody settings (drag, gravity, constraints)
            _rb.drag = 8f;
            _rb.angularDrag = 0.05f;
            _rb.useGravity = true;

            // Constrain physical rotations on all axes
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        }

        private void FixedUpdate()
        {
            // Update various movement-related actions
            UpdateGrounded();
            UpdateMove();
            ApplyGravity();
            HandleSlopeMovement();
            LimitVelocity();
        }

        private void UpdateMove()
        {
            // Handle movement based on camera direction and player input
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = (cameraForward * _moveInput.z * walkForce) +
                                    (cameraRight * _moveInput.x * strafeForce);

            if (!_isGrounded)
            {
                moveDirection *= airControlFactor;
            }

            // Apply force for movement
            _rb.AddForce(moveDirection * Time.fixedDeltaTime, ForceMode.VelocityChange);

            // Manual rotation control (based only on user input)
            if (_moveInput != Vector3.zero)
            {
                Vector3 direction = moveDirection.normalized;
                if (direction.magnitude >= 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    // Apply rotation only on the Y axis
                    targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
                    _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * 10f));
                }
            }
        }

        private void HandleSlopeMovement()
        {
            // Handle movement on slopes
            if (_isGrounded && !exitingSlope)
            {
                if (OnSlope())
                {
                    Vector3 slopeMoveDirection = GetSlopeMoveDirection();
                    _rb.AddForce(slopeMoveDirection * walkForce, ForceMode.Force);

                    if (_rb.velocity.y > 0)
                    {
                        _rb.AddForce(Vector3.down * slopeForce, ForceMode.Force);
                    }
                }
            }
        }

        private bool OnSlope()
        {
            // Check if the player is on a slope
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, groundCheckDistance, groundMask))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }

            return false;
        }

        private Vector3 GetSlopeMoveDirection()
        {
            // Get the direction to move on a slope
            return Vector3.ProjectOnPlane(new Vector3(_moveInput.x, 0, _moveInput.z), slopeHit.normal).normalized;
        }

        private void UpdateGrounded()
        {
            // Check if the player is grounded
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            bool wasGrounded = _isGrounded;

            _isGrounded = Physics.Raycast(origin, Vector3.down, out _, groundCheckDistance, groundMask);

            if (_isGrounded != wasGrounded)
            {
                _rb.drag = _isGrounded ? 8f : 0f;
                onGroundChanged?.Invoke(_isGrounded);
            }
        }

        private void ApplyGravity()
        {
            // Apply gravity and drag when in the air
            if (!_isGrounded)
            {
                // Apply horizontal drag
                Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                Vector3 horizontalDrag = -horizontalVelocity.normalized * horizontalVelocity.magnitude * jumpHorizontalDrag * Time.fixedDeltaTime;
                _rb.AddForce(horizontalDrag, ForceMode.VelocityChange);

                // Apply vertical drag
                float verticalVelocity = _rb.velocity.y;
                float verticalDrag = -Mathf.Sign(verticalVelocity) * Mathf.Abs(verticalVelocity) * jumpVerticalDrag * Time.fixedDeltaTime;
                _rb.AddForce(Vector3.up * verticalDrag, ForceMode.VelocityChange);

                // Apply additional gravity
                _rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
            }
        }

        private void LimitVelocity()
        {
            // Limit the player's velocity to prevent exceeding max speed
            Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            if (horizontalVelocity.magnitude > maxVelocity)
            {
                Vector3 limitedVelocity = horizontalVelocity.normalized * maxVelocity;
                _rb.velocity = new Vector3(limitedVelocity.x, _rb.velocity.y, limitedVelocity.z);
            }

            if (Mathf.Abs(_rb.velocity.y) > maxVerticalVelocity)
            {
                _rb.velocity = new Vector3(_rb.velocity.x, Mathf.Sign(_rb.velocity.y) * maxVerticalVelocity, _rb.velocity.z);
            }
        }

        private void OnMove(Vector2 axis)
        {
            // Handle player movement input
            _moveInput = new Vector3(axis.x, 0, axis.y);
        }

        private void OnSprintStart()
        {
            // Increase speed when sprinting
            if (_playerInfo.HaveSpeedShoes == true)
            {
                walkForce *= sprintForceMultiplier;
                strafeForce *= sprintForceMultiplier;
                maxVelocity *= sprintVelocityMultiplier;
            }
        }

        private void OnSprintStop()
        {
            // Reset speed when sprinting stops
            if (_playerInfo.HaveSpeedShoes == true)
            {
                walkForce = defaultWalkForce;
                strafeForce = defaultStrafeForce;
                maxVelocity = defaultMaxVelocity;
            }
        }

        private void OnJumpStart()
        {
            // Start jump
            if (!_isGrounded) return;

            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            _isJumping = true;
            StartCoroutine(WaitJumpStop());
            onJump?.Invoke();
        }

        private void OnJumpStop()
        {
            // Stop jump
            _isJumping = false;
        }

        private IEnumerator WaitJumpStop()
        {
            // Wait for a short time before stopping the jump
            yield return new WaitForSeconds(0.05f);
            _isJumping = false;
        }
    }
}

