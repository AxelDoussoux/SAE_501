using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TomAg
{
    public class PlayerMotor : MonoBehaviour
    {
        // PARAMS
        [Header(header: "Movement")]

        [SerializeField]
        private float walkForce = 100;
        [SerializeField]
        private float strafeForce = 80;

        [Header(header: "Rotation")]

        [SerializeField]
        private float rotationSensivity = 10;

        [Header(header: "Jump")]

        [SerializeField]
        private float jumpForce = 10;

        // PRIVATE

        private Rigidbody _rb;
        private PlayerController _controller;

        private Vector3 _localMoveAxis;
        private float _local0ffsetAngleY;
        private float _rotationAngleY;


        // UNITY

        private void Awake()
        {
            if (!TryGetComponent(out _rb))
                Debug.LogError(message: "Missing Rigidbody", this);
            if (!TryGetComponent(out _controller))
                Debug.LogError(message: "Missing PlayerController", this);

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
            UpdateRotation();
            UpdateMove();
        }


        // HOOKS


        private void OnMove(Vector2 axis)
        {

            _localMoveAxis = new Vector3(axis.x * strafeForce, y: 0, axis.y * walkForce);
        }
        private void OnAim(Vector2 axis)
        {
            _local0ffsetAngleY = axis.x * rotationSensivity;
        }
        private void OnJumpStart()
        {
            _rb.AddRelativeForce(x: 0, jumpForce, z: 0, ForceMode.Impulse);
        }
                
        private void OnJumpStop() { }
        

        private void OnCrouchStart() { }

               
        private void OnCrouchStop() { }

        // INTERNAL


        private void UpdateMove()
        {
            if (_localMoveAxis == Vector3.zero)
                return;

            _rb.AddRelativeForce(_localMoveAxis * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        private void UpdateRotation()
        {
            _rotationAngleY += _local0ffsetAngleY * Time.fixedDeltaTime;
            var newRotation = Quaternion.Euler(x: 0, _rotationAngleY, z: 0);

            _rb.MoveRotation(newRotation);
        }



    }


}
