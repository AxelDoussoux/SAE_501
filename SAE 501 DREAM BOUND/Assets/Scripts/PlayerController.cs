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

        private int _playerId;
        private GameInputs _gameInputs;
        private bool _isPaused;

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

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn(); // Appel de la m�thode de base
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
                _gameInputs.App.Enable(); // Activer App
            }
        }

        private void OnDisable()
        {
            if (_gameInputs != null)
            {
                _gameInputs.Player.Disable();
                _gameInputs.App.Disable(); // D�sactiver App
            }
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            if (_isPaused)
            {
                Debug.Log("Move ignored - game is paused");
                return;
            }

            Vector2 axis = ctx.ReadValue<Vector2>();
            Debug.Log($"Move called with: {axis}");
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


        public void OnPause(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                _isPaused = !_isPaused;
                Debug.Log($"Pause state toggled - IsPaused: {_isPaused}");
                onPauseToggle?.Invoke(); // Assurez-vous que l'�v�nement est bien invoqu�
            }
        }


        public void OnBack(InputAction.CallbackContext context)
        {
            // Laisser vide si vous ne voulez pas g�rer cette action pour le moment
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            // Laisser vide
        }

        public void OnNaviguate(InputAction.CallbackContext context)
        {
            // Laisser vide
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            // Laisser vide
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            // Laisser vide
        }

    }
}