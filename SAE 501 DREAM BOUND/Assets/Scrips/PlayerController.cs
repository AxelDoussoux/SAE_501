using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

namespace TomAg
{
    public class PlayerController : MonoBehaviour, GameInputs.IPlayerActions
    {
      

        public int playerId => _playerId;
        public PlayerInput playerInput => _playerInput;

        public event Action<Vector2> onAim;
        public event Action<Vector2> onMove;
        public event Action onJumpStart;
        public event Action onJumpStop;
        public event Action onCrouchStart;
        public event Action onCrouchStop;
        public event Action onInteract;

        private int _playerId;
        private PlayerInput _playerInput;


        private void Start()
        {


            TryGetComponent(out PlayerInput playerInput);
            _playerId = playerInput.playerIndex;
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            var axis = ctx.ReadValue<Vector2>();
            Debug.Log("Move called with: " + axis);
            onMove?.Invoke(axis);
        }

        public void OnAim(InputAction.CallbackContext ctx)
        {
            var axis = ctx.ReadValue<Vector2>();
            onAim?.Invoke(axis);
        }

        public void OnJump(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                onJumpStart?.Invoke();
            else if (ctx.canceled)
                onJumpStop?.Invoke();
        }

        public void OnCrouch(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                onCrouchStart?.Invoke();
            else if (ctx.canceled)
                onCrouchStop?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            onInteract?.Invoke();
        }
    }
}
