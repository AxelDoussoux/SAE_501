using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TomAg
{
    public class PlayerMotor : MonoBehaviour
    {
        [Header("Movement")]

        [SerializeField]
        private float walkSpeed = 5f;  // Vitesse de marche ajust�e
        [SerializeField]
        private float strafeSpeed = 5f;  // Vitesse de d�placement lat�ral

        [Header("Rotation")]

        [SerializeField]
        private float rotationSensitivity = 10f;

        [Header("Jump")]

        [SerializeField]
        private float jumpHeight = 2f;  // Hauteur du saut

        private PlayerController _controller;

        private Vector3 _localMoveAxis;
        private float _localOffsetAngleY;
        private float _rotationAngleY;
        private bool _isGrounded;
        private Vector3 _velocity;


        private void Awake()
        {
            if (!TryGetComponent(out _controller))
                Debug.LogError("Missing PlayerController", this);

            _controller.onMove += OnMove;
            _controller.onAim += OnAim;
            _controller.onJumpStart += OnJumpStart;
            _controller.onJumpStop += OnJumpStop;
            _controller.onCrouchStart += OnCrouchStart;
            _controller.onCrouchStop += OnCrouchStop;
        }

        private void Update()
        {
            UpdateRotation();
            UpdateMove();
        }

        private void OnMove(Vector2 axis)
        {
            _localMoveAxis = new Vector3(axis.x * strafeSpeed, 0, axis.y * walkSpeed);
        }

        private void OnAim(Vector2 axis)
        {
            _localOffsetAngleY = axis.x * rotationSensitivity;
        }

        private void OnJumpStart()
        {
            if (_isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);  // Applique une impulsion verticale pour le saut
            }
        }

        private void OnJumpStop() { }

        private void OnCrouchStart() { }

        private void OnCrouchStop() { }

        private void UpdateMove()
        {
            if (_localMoveAxis != Vector3.zero)
            {
                // Convertir les coordonn�es locales en coordonn�es mondiales
                Vector3 moveDirection = transform.TransformDirection(_localMoveAxis);

                // D�placer le joueur en modifiant directement son transform
                transform.position += moveDirection * Time.deltaTime;
            }

            // Appliquer la gravit�
            _velocity.y += Physics.gravity.y * Time.deltaTime;
            transform.position += _velocity * Time.deltaTime;

            // G�rer la d�tection au sol
            if (transform.position.y <= 0.5f)  // Exemple de d�tection si le joueur touche le sol (� ajuster selon votre sc�ne)
            {
                _isGrounded = true;
                _velocity.y = 0;
            }
            else
            {
                _isGrounded = false;
            }
        }

        private void UpdateRotation()
        {
            _rotationAngleY += _localOffsetAngleY * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, _rotationAngleY, 0);  // Applique directement la rotation au transform
        }
    }
}
