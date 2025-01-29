using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TomAg
{
    public class PlayerController : NetworkBehaviour, GameInputs.IPlayerActions, GameInputs.IAppActions
    {
        public int PlayerId => _playerId;
        public bool IsPaused => _isPaused;

        public event Action<Vector2> onAim;
        public event Action<Vector2> onMove;
        public event Action onJumpStart;
        public event Action onJumpStop;
        public event Action onCrouchStart;
        public event Action onCrouchStop;
        public event Action onInteract;
        public event Action onPauseToggle;
        public event Action onSprintStart;
        public event Action onSprintStop;
        public event Action onAgileInteractStart;
        public event Action onAgileInteractStop;

        private int _playerId;
        private GameInputs _gameInputs;
        private bool _isPaused = false;
        private bool _canMove = true;

        private void Start()
        {
            // Initialize player input and game controls
            Debug.Log($"PlayerController - Start called for {gameObject.name}, IsOwner: {IsOwner}");

            if (IsOwner)
            {
                _gameInputs = new GameInputs();
                _gameInputs.Player.SetCallbacks(this);
                _gameInputs.App.SetCallbacks(this);
                _gameInputs.Player.Enable();
                _gameInputs.App.Enable();

                UnityEngine.Cursor.visible = false;

                if (TryGetComponent(out PlayerInput playerInput))
                {
                    _playerId = playerInput.playerIndex;
                    Debug.Log($"Player ID assigned: {_playerId}");
                }
                else
                {
                    Debug.LogError("PlayerInput component missing on this GameObject.");
                }

                if (IsHost)
                {
                    Debug.Log("This player is the host!");
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            // When the network object is spawned
            base.OnNetworkSpawn();
            if (IsOwner)
            {
                Debug.Log($"PlayerController - Network object spawned: {gameObject.name}");
                PauseMenuController.Instance.RegisterLocalPlayer(this);
            }
        }

        private void OnEnable()
        {
            // Enable input actions when the script is enabled
            if (_gameInputs != null && IsOwner)
            {
                _gameInputs.Player.Enable();
                _gameInputs.App.Enable();
            }
        }

        private void OnDisable()
        {
            // Disable input actions when the script is disabled
            if (_gameInputs != null)
            {
                _gameInputs.Player.Disable();
                _gameInputs.App.Disable();
            }
        }

        public void ResumeMovement()
        {
            // Resume movement after being paused
            _isPaused = false;
            Debug.Log("PlayerController - Movement resumed.");
        }

        public void SetMovementEnabled(bool enabled)
        {
            // Enable or disable movement for the player
            if (IsOwner)
            {
                _canMove = enabled;
                Debug.Log($"Movement set to: {enabled}");
            }
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            // Handle player movement input
            if (!_canMove || _isPaused)
            {
                onMove?.Invoke(Vector2.zero);
                return;
            }

            Vector2 axis = ctx.ReadValue<Vector2>();
            onMove?.Invoke(axis);
        }

        public void OnAim(InputAction.CallbackContext ctx)
        {
            // Handle player aiming input
            if (_isPaused)
            {
                return;
            }

            Vector2 axis = ctx.ReadValue<Vector2>();
            onAim?.Invoke(axis);
        }

        public void OnJump(InputAction.CallbackContext ctx)
        {
            // Handle player jump input
            if (_isPaused)
            {
                return;
            }

            if (ctx.started)
                onJumpStart?.Invoke();
            else if (ctx.canceled)
                onJumpStop?.Invoke();
        }

        public void OnCrouch(InputAction.CallbackContext ctx)
        {
            // Handle player crouch input
            if (_isPaused)
            {
                return;
            }

            if (ctx.started)
                onCrouchStart?.Invoke();
            else if (ctx.canceled)
                onCrouchStop?.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            // Handle player interact input
            if (_isPaused)
            {
                return;
            }

            if (ctx.started)
                onInteract?.Invoke();
        }

        public void OnSprint(InputAction.CallbackContext ctx)
        {
            // Handle player sprint input
            if (_isPaused)
            {
                return;
            }

            if (ctx.started)
                onSprintStart?.Invoke();
            else if (ctx.canceled)
                onSprintStop?.Invoke();
        }

        public void OnPause(InputAction.CallbackContext ctx)
        {
            // Handle pause toggle input
            if (ctx.started)
            {
                _isPaused = !_isPaused;
                Debug.Log($"Pause state toggled - IsPaused: {_isPaused}");
                onPauseToggle?.Invoke();
            }
        }

        public void OnAgileInteract(InputAction.CallbackContext ctx)
        {
            // Handle agile interact input
            if (_isPaused)
            {
                return;
            }

            if (ctx.started)
                onAgileInteractStart?.Invoke();
            else if (ctx.canceled)
                onAgileInteractStop?.Invoke();
        }

        public void SetPauseState(bool state)
        {
            // Set the pause state directly
            _isPaused = state;
            Debug.Log($"_isPaused = {state}");
        }

        public void OnBack(InputAction.CallbackContext context)
        {
            // Empty method (for unimplemented back action)
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            // Empty method (for unimplemented click action)
        }

        public void OnNaviguate(InputAction.CallbackContext context)
        {
            // Empty method (for unimplemented navigate action)
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            // Empty method (for unimplemented submit action)
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            // Empty method (for unimplemented point action)
        }
    }
}
