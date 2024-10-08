using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TomAg
{
    public class PlayerMotor : MonoBehaviour
    {
        [Header("Movement")]

        [SerializeField]
        private float walkForce = 100;
        [SerializeField]
        private float strafeForce = 80;

        [Header("Rotation")]

        [SerializeField]
        private float rotationSensivity = 10;

        [Header("Jump")]

        [SerializeField]
        private float jumpForce = 10;

        private Rigidbody _rb;
        private PlayerController _controller;

        private Vector3 _localMoveAxis;
        private float _localOffsetAngleY;
        private float _rotationAngleY;

        private void Awake()
        {
            if (!TryGetComponent(out _rb))
                Debug.LogError("Missing Rigidbody", this);
            if (!TryGetComponent(out _controller))
                Debug.LogError("Missing PlayerController", this);

            _controller.onMove += OnMove;
            _controller.onAim += OnAim;
            _controller.onJumpStart += OnJumpStart;
            _controller.onJumpStop += OnJumpStop;
            _controller.onCrouchStart += OnCrouchStart;
            _controller.onCrouchStop += OnCrouchStop;
        }

        private void FixedUpdate()
        {
            UpdateRotation();
            UpdateMove();
        }

        private void OnMove(Vector2 axis)
        {
            _localMoveAxis = new Vector3(axis.x * strafeForce, 0, axis.y * walkForce);
        }

        private void OnAim (Vector2 axis)
        {
            _localOffsetAngleY = axis.x * rotationSensivity;
        }

        private void OnJumpStart()
        {
            _rb.AddRelativeForce(0, jumpForce, 0, ForceMode.Impulse);
        }

        private void OnJumpStop() { }

        private void OnCrouchStart() { }

        private void OnCrouchStop() { }

        private void UpdateMove()
        {
            if (_localMoveAxis == Vector3.zero)
                return;

            _rb.AddRelativeForce(_localMoveAxis * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        private void UpdateRotation()
        {
            _rotationAngleY += _localOffsetAngleY * Time.fixedDeltaTime;
            var newRotation = Quaternion.Euler(0, _rotationAngleY, 0);

            _rb.MoveRotation(newRotation);
        }
    }
}
