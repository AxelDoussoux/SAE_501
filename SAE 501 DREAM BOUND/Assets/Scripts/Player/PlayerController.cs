using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TomAg
{
    public class PlayerController : NetworkBehaviour, GameInputs.IPlayerActions
    {
        public int PlayerId => _playerId;

        public event Action<Vector2> onAim;
        public event Action<Vector2> onMove;
        public event Action onJumpStart;
        public event Action onJumpStop;
        public event Action onCrouchStart;
        public event Action onCrouchStop;
        public event Action onInteract;

        private int _playerId;
        private GameInputs _gameInputs; // Utilisation de GameInputs pour les actions

        private void Start()
        {
            if (IsOwner) // Vérifie si le joueur est le propriétaire en réseau
            {
                _gameInputs = new GameInputs(); // Instancie les actions de jeu
                _gameInputs.Player.SetCallbacks(this); // Associe ce script comme récepteur des actions
                _gameInputs.Player.Enable(); // Active les contrôles de l'action Map 'Player'

                if (TryGetComponent(out PlayerInput playerInput))
                {
                    _playerId = playerInput.playerIndex;
                    Debug.Log($"Player ID assigned: {_playerId}");
                }
                else
                {
                    Debug.LogError("PlayerInput component missing on this GameObject.");
                }
            }
        }

        private void OnEnable()
        {
            if (_gameInputs != null && IsOwner)
            {
                _gameInputs.Player.Enable();
            }
        }

        private void OnDisable()
        {
            if (_gameInputs != null)
            {
                _gameInputs.Player.Disable();
            }
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            Vector2 axis = ctx.ReadValue<Vector2>();
            Debug.Log("Move called with: " + axis);
            onMove?.Invoke(axis);
        }

        public void OnAim(InputAction.CallbackContext ctx)
        {
            Vector2 axis = ctx.ReadValue<Vector2>();
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
