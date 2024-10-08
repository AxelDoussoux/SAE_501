using System;
using System.Collections.Generic;
using Sphax;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TomAg
{
    public class InputManager : Singleton<InputManager>
    {
        private struct PlayerDevice
        {
            public int playerId;
            public InputDevice primaryDevice;
            public InputDevice secondaryDevice;
        }

        [SerializeField]
        private GameObject playerPrefab;

        public event Action<PlayerController, int> OnPlayerJoin; 

        private GameInputs _gameInputs;
        private PlayerInput _playerInput;

        private List<PlayerDevice> _playerDeviceList;

        protected override void OnInit()
        {
            _gameInputs = new GameInputs();

            if (!TryGetComponent(out _playerInput))
                Debug.LogError("Missing playerInput component", this);

            _playerInput.onControlsChanged += OnControlsChanged;

            _gameInputs.PlayerJoin.Enable();
            _gameInputs.PlayerJoin.Join.performed += OnPlayerJoinPerformed;


            // Inputs
            _gameInputs.Player.Enable();
            _gameInputs.App.Enable();

            // Curseur
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnControlsChanged(PlayerInput playerInput)
        {
            Debug.Log($"Controls changed to {playerInput.currentControlScheme}");
        }

        private void OnPlayerJoinPerformed(InputAction.CallbackContext ctx) // Changement ici
        {
            if (DeviceIsAssociated(ctx.control.device))
                return;

            BindDeviceToPlayer(ctx.control.device);
        }

        private bool DeviceIsAssociated(InputDevice device)
        {
            if (_playerDeviceList == null)
                _playerDeviceList = new List<PlayerDevice>();

            foreach (var playerDevice in _playerDeviceList)
                if (playerDevice.primaryDevice == device || playerDevice.secondaryDevice == device)
                    return true;

            return false;
        }

        private void BindDeviceToPlayer(InputDevice joinDevice)
        {
            InputDevice primaryDevice = joinDevice;
            InputDevice secondaryDevice = null;

            if (joinDevice is Keyboard)
                secondaryDevice = InputSystem.GetDevice<Mouse>();
            else if (joinDevice is Mouse)
                secondaryDevice = InputSystem.GetDevice<Keyboard>();

            int playerId = _playerDeviceList.Count;

            var playerDevice = new PlayerDevice() { playerId = playerId, primaryDevice = primaryDevice, secondaryDevice = secondaryDevice };
            _playerDeviceList.Add(playerDevice);

            PlayerInput player;
            if (secondaryDevice == null)
                player = PlayerInput.Instantiate(playerPrefab, playerId, null, playerId, primaryDevice);
            else
                player = PlayerInput.Instantiate(playerPrefab, playerId, null, playerId, primaryDevice, secondaryDevice);

            if (!player.TryGetComponent(out PlayerController playerController))
                Debug.LogError($"Missing PlayerController on Player <{player.name}>", this);
            else
                OnPlayerJoin?.Invoke(playerController, playerId); // Utilisation de l'?v?nement ici

            Debug.Log($"Binded <{primaryDevice}> + <{secondaryDevice}> to Player#{playerId}", this);
        }

        protected override void OnDispose()
        {
            // Ajoute la logique de nettoyage ici si n?cessaire
        }
    }
}
