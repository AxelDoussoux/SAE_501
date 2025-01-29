using System;
using System.Collections.Generic;
using Sphax;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TomAg
{
    public class InputManager : Singleton<InputManager>
    {
        // Structure to hold player device information
        private struct PlayerDevice
        {
            public int playerId;  // Player's ID
            public InputDevice primaryDevice;  // Primary input device (e.g., Keyboard or Controller)
            public InputDevice secondaryDevice;  // Secondary input device (if any)
        }

        [SerializeField]
        private GameObject playerPrefab;  // Prefab for the player to instantiate

        // Event to notify when a player joins
        public event Action<PlayerController, int> OnPlayerJoin;

        private GameInputs _gameInputs;  // Game inputs handler
        private PlayerInput _playerInput;  // Player input component

        private List<PlayerDevice> _playerDeviceList;  // List of devices assigned to players

        // Initialize the InputManager
        protected override void OnInit()
        {
            _gameInputs = new GameInputs();  // Initialize the game inputs

            // Try to get the PlayerInput component
            if (!TryGetComponent(out _playerInput))
                Debug.LogError("Missing playerInput component", this);

            // Register for control changes
            _playerInput.onControlsChanged += OnControlsChanged;

            // Enable game input actions
            _gameInputs.Player.Enable();
            _gameInputs.App.Enable();

            // Optionally, configure the cursor (commented out for now)
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }

        // Called when the player's input controls change
        private void OnControlsChanged(PlayerInput playerInput)
        {
            Debug.Log($"Controls changed to {playerInput.currentControlScheme}");
        }

        // Called when the player joins (performed action)
        private void OnPlayerJoinPerformed(InputAction.CallbackContext ctx)
        {
            if (DeviceIsAssociated(ctx.control.device))  // Check if device is already associated
                return;

            BindDeviceToPlayer(ctx.control.device);  // Bind the device to a player
        }

        // Check if a device is already associated with a player
        private bool DeviceIsAssociated(InputDevice device)
        {
            if (_playerDeviceList == null)
                _playerDeviceList = new List<PlayerDevice>();  // Initialize player device list if needed

            // Check if the device is already in use by any player
            foreach (var playerDevice in _playerDeviceList)
                if (playerDevice.primaryDevice == device || playerDevice.secondaryDevice == device)
                    return true;

            return false;
        }

        // Bind a device (primary or secondary) to a player
        private void BindDeviceToPlayer(InputDevice joinDevice)
        {
            InputDevice primaryDevice = joinDevice;  // Set the joining device as the primary device
            InputDevice secondaryDevice = null;

            // If the primary device is a keyboard, set mouse as secondary and vice versa
            if (joinDevice is Keyboard)
                secondaryDevice = InputSystem.GetDevice<Mouse>();
            else if (joinDevice is Mouse)
                secondaryDevice = InputSystem.GetDevice<Keyboard>();

            int playerId = _playerDeviceList.Count;  // Assign a unique player ID

            var playerDevice = new PlayerDevice() { playerId = playerId, primaryDevice = primaryDevice, secondaryDevice = secondaryDevice };
            _playerDeviceList.Add(playerDevice);  // Add the new player device to the list

            PlayerInput player;
            if (secondaryDevice == null)
                player = PlayerInput.Instantiate(playerPrefab, playerId, null, playerId, primaryDevice);  // Instantiate the player with primary device only
            else
                player = PlayerInput.Instantiate(playerPrefab, playerId, null, playerId, primaryDevice, secondaryDevice);  // Instantiate with both devices

            // Check if PlayerController component is present on the instantiated player
            if (!player.TryGetComponent(out PlayerController playerController))
                Debug.LogError($"Missing PlayerController on Player <{player.name}>", this);
            else
                OnPlayerJoin?.Invoke(playerController, playerId);  // Invoke event when a player joins

            Debug.Log($"Binded <{primaryDevice}> + <{secondaryDevice}> to Player#{playerId}", this);
        }

        // Clean up resources if necessary
        protected override void OnDispose()
        {
            // Add cleanup logic here if needed
        }
    }
}
