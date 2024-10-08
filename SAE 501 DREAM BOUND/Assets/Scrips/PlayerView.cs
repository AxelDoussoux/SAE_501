using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TomAg
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private float aimSpeed = 10;
        [SerializeField]
        private float angleMin = -89;
        [SerializeField]
        private float angleMax = 73;

        private PlayerController _controller;

        private float _aimY;
        private float _angleX;

        private void Awake()
        {
            if (!TryGetComponent(out _controller))
                Debug.LogError("Missing PlayerController");

            _controller.onAim += OnAim;
        }

        private void LateUpdate()
        {
            UpdateView();
        }

        private void OnAim(Vector2 axis)
        {
            _aimY = axis.y;
        }

        private void UpdateView()
        {
            _angleX -= _aimY * aimSpeed * Time.deltaTime;
            
            if (_angleX < angleMin)
             _angleX = angleMin; 
            if (_angleX > angleMax)
                _angleX = angleMax;

            _camera.transform.localEulerAngles = new Vector3(_angleX, 0, 0);
        }
    }
}
