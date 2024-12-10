using UnityEngine;

namespace TomAg
{
    public class PlayerMotor : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkForce = 400f;
        [SerializeField] private float strafeForce = 400f;
        [SerializeField] private float maxVelocity = 30f;
        [SerializeField] private float maxVerticalVelocity = 50f;

        [Header("Jumping")]
        [SerializeField] private float jumpForce = 20f;
        [SerializeField] private float jumpHorizontalDrag = 2f; // Drag horizontal pendant le saut
        [SerializeField] private float jumpVerticalDrag = 0f; // Drag vertical pendant le saut

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundCheckDistance = 0.5f;

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
        private bool _isGrounded;
        private bool _isJumping;
        private PlayerInfo _playerInfo;

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

            if (!cameraTransform)
                Debug.LogError("Camera Transform not assigned", this);

            InitializeRigidbody();

            _controller.onMove += OnMove;
            _controller.onJumpStart += OnJumpStart;
            _controller.onJumpStop += OnJumpStop;

            _playerInfo = GetComponent<PlayerInfo>();

            if (_playerInfo.HaveSpeedShoes)
            {
                walkForce *= 2;
                strafeForce *= 2;
            }
        }

        private void InitializeRigidbody()
        {
            _rb.mass = 70f;
            _rb.drag = 8f;
            _rb.angularDrag = 0.05f;
            _rb.useGravity = true;

            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        private void FixedUpdate()
        {
            UpdateGrounded();
            UpdateMove();
            ApplyGravity();
            HandleSlopeMovement();
            LimitVelocity();
        }

        private void UpdateMove()
        {
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

            _rb.AddForce(moveDirection * Time.fixedDeltaTime, ForceMode.VelocityChange);

            if (_moveInput != Vector3.zero)
            {
                Vector3 direction = moveDirection.normalized;
                if (direction.magnitude >= 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * 10f));
                }
            }
        }

        private void HandleSlopeMovement()
        {
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
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, groundCheckDistance, groundMask))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }

            return false;
        }

        private Vector3 GetSlopeMoveDirection()
        {
            return Vector3.ProjectOnPlane(new Vector3(_moveInput.x, 0, _moveInput.z), slopeHit.normal).normalized;
        }

        private void UpdateGrounded()
        {
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
            if (!_isGrounded)
            {
                // Appliquer le drag horizontal
                Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                Vector3 horizontalDrag = -horizontalVelocity.normalized * horizontalVelocity.magnitude * jumpHorizontalDrag * Time.fixedDeltaTime;
                _rb.AddForce(horizontalDrag, ForceMode.VelocityChange);

                // Appliquer le drag vertical
                float verticalVelocity = _rb.velocity.y;
                float verticalDrag = -Mathf.Sign(verticalVelocity) * Mathf.Abs(verticalVelocity) * jumpVerticalDrag * Time.fixedDeltaTime;
                _rb.AddForce(Vector3.up * verticalDrag, ForceMode.VelocityChange);

                // Appliquer une gravité supplémentaire
                _rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
            }
        }

        private void LimitVelocity()
        {
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
            _moveInput = new Vector3(axis.x, 0, axis.y);
        }

        private void OnJumpStart()
        {
            if (!_isGrounded) return;

            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            _isJumping = true;
            onJump?.Invoke();
        }

        private void OnJumpStop()
        {
            _isJumping = false;
        }
    }
}
