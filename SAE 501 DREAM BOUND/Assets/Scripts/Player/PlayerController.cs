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
            base.OnNetworkSpawn();
            if (IsOwner)
            {
                Debug.Log($"PlayerController - Network object spawned: {gameObject.name}");
                PauseMenuController.Instance.RegisterLocalPlayer(this);
            }
        }

        private void OnEnable()
        {
            if (_gameInputs != null && IsOwner)
            {
                _gameInputs.Player.Enable();
                _gameInputs.App.Enable();
            }
        }

        private void OnDisable()
        {
            if (_gameInputs != null)
            {
                _gameInputs.Player.Disable();
                _gameInputs.App.Disable();
            }
        }

        public void ResumeMovement()
        {
            _isPaused = false;
            Debug.Log("PlayerController - Movement resumed.");
        }

        public void SetMovementEnabled(bool enabled)
        {
            if (IsOwner)
            {
                _canMove = enabled;
                Debug.Log($"Movement set to: {enabled}");
            }
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
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
            if (_isPaused)
            {
                return;
            }

            Vector2 axis = ctx.ReadValue<Vector2>();
            onAim?.Invoke(axis);
        }

        public void OnJump(InputAction.CallbackContext ctx)
        {
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
            if (_isPaused)
            {
                return;
            }

            if (ctx.started)
                onInteract?.Invoke();
        }

        public void OnSprint(InputAction.CallbackContext ctx)
        {
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
            if (ctx.started)
            {
                _isPaused = !_isPaused;
                Debug.Log($"Pause state toggled - IsPaused: {_isPaused}");
                onPauseToggle?.Invoke();
            }
        }

        public void OnAgileInteract(InputAction.CallbackContext ctx)
        {
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
            _isPaused = state;
            Debug.Log($"_isPaused = {state}");
        }

        public void OnBack(InputAction.CallbackContext context)
        {
            // Empty
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            // Empty
        }

        public void OnNaviguate(InputAction.CallbackContext context)
        {
            // Empty
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            // Empty
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            // Empty
        }
    }
}
