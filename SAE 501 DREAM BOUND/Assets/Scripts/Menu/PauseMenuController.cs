using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;

namespace TomAg
{
    public class PauseMenuController : NetworkBehaviour
    {
        public static PauseMenuController Instance { get; private set; }

        [SerializeField] private UIDocument _pauseMenuDocument;
        [SerializeField] private UIDocument _mainMenuDocument;
        [SerializeField] private JoinChannel echoChannel;
        [SerializeField] private OptionsMenuController _optionsMenuController;

        private MenuUI _mainMenu;
        private VisualElement _pauseMenuRoot;
        private VisualElement _mainMenuRoot;
        private Button _continueButton;
        private Button _optionsButton;
        private Button _quitButton;
        private PlayerController _localPlayer;
        private bool _isMenuVisible = false;
        private bool _isInitialized = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void OnEnable()
        {
            if (!_isInitialized)
            {
                InitializeUI();
            }
        }

        private void InitializeUI()
        {
            if (_pauseMenuDocument == null)
            {
                Debug.LogError("Pause Menu UIDocument reference is missing!");
                return;
            }

            _pauseMenuDocument.rootVisualElement.schedule.Execute(() => {
                _pauseMenuRoot = _pauseMenuDocument.rootVisualElement;
                if (_pauseMenuRoot == null)
                {
                    Debug.LogError("Pause menu root is null after initialization!");
                    return;
                }

                _continueButton = _pauseMenuRoot.Q<Button>("continue");
                _quitButton = _pauseMenuRoot.Q<Button>("quit");

                if (_continueButton == null || _quitButton == null)
                {
                    Debug.LogError($"Buttons not found! Continue: {_continueButton != null}, Quit: {_quitButton != null}");
                    return;
                }

                _optionsButton = _pauseMenuRoot.Q<Button>("options");
                if (_optionsButton != null)
                {
                    _optionsButton.clicked += OnOptionsClicked;
                    _optionsMenuController.Initialize();
                }

                _continueButton.clicked += OnContinueClicked;
                _quitButton.clicked += OnQuitClicked;

                if (_mainMenuDocument != null)
                {
                    _mainMenuRoot = _mainMenuDocument.rootVisualElement;
                    _mainMenuRoot.style.display = DisplayStyle.Flex;
                    _mainMenu = _mainMenuDocument.GetComponentInParent<MenuUI>();
                }

                _pauseMenuRoot.style.display = DisplayStyle.None;
                _isMenuVisible = false;

                UnityEngine.Cursor.visible = false; // Ensure the cursor is hidden when starting
                _isInitialized = true;
            });
        }

        public void RegisterLocalPlayer(PlayerController player)
        {
            if (player == null)
            {
                Debug.LogError("Attempting to register null player!");
                return;
            }

            _localPlayer = player;
            _localPlayer.onPauseToggle += TogglePauseMenu;
        }

        private void TogglePauseMenu()
        {
            // Check if the settings menu is visible
            if (_optionsMenuController != null && _optionsMenuController.IsVisible())
            {
                _optionsMenuController.Hide();
                ShowPauseMenu(); // Show the pause menu immediately
                return;
            }

            if (_pauseMenuRoot == null)
            {
                Debug.LogError("Menu not initialized properly. Attempting to reinitialize...");
                InitializeUI();
                return;
            }

            // Toggle the pause menu visibility
            _isMenuVisible = !_isMenuVisible;
            _pauseMenuRoot.style.display = _isMenuVisible ? DisplayStyle.Flex : DisplayStyle.None;

            UnityEngine.Cursor.visible = _isMenuVisible;
            UnityEngine.Cursor.lockState = _isMenuVisible ? CursorLockMode.None : CursorLockMode.Locked;

            if (_localPlayer != null)
            {
                _localPlayer.SetPauseState(_isMenuVisible);
            }
        }

        public void ShowPauseMenu()
        {
            _isMenuVisible = true;
            _pauseMenuRoot.style.display = DisplayStyle.Flex;

            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;

            if (_localPlayer != null)
            {
                _localPlayer.SetPauseState(true); // Ensure the pause state remains active
            }
        }

        private void OnContinueClicked()
        {
            if (_localPlayer != null)
            {
                _isMenuVisible = false;
                _pauseMenuRoot.style.display = DisplayStyle.None;
                _localPlayer.SetPauseState(false);
                _localPlayer.ResumeMovement();

                // Hide the cursor when resuming the game
                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void OnQuitClicked()
        {
            if (_localPlayer != null)
            {
                TogglePauseMenu();
                if (_mainMenuRoot != null)
                {
                    UnityEngine.Cursor.visible = true;
                    _mainMenu.UIVisibility();
                }

                if (Unity.Netcode.NetworkManager.Singleton.IsHost)
                {
                    if (echoChannel != null)
                    {
                        echoChannel.LeaveChannel();
                    }
                    Unity.Netcode.NetworkManager.Singleton.Shutdown();
                    Debug.Log("Host: Shutting down server");
                }
                else if (Unity.Netcode.NetworkManager.Singleton.IsClient)
                {
                    if (echoChannel != null)
                    {
                        echoChannel.LeaveChannel();
                    }
                    Unity.Netcode.NetworkManager.Singleton.Shutdown();
                    Debug.Log("Client: Disconnecting from server");
                }
            }
        }

        private void OnOptionsClicked()
        {
            _pauseMenuRoot.style.display = DisplayStyle.None;
            _optionsMenuController.Show();
        }

        private new void OnDestroy()
        {
            if (_localPlayer != null)
            {
                _localPlayer.onPauseToggle -= TogglePauseMenu;
            }

            if (_continueButton != null)
            {
                _continueButton.clicked -= OnContinueClicked;
            }

            if (_quitButton != null)
            {
                _quitButton.clicked -= OnQuitClicked;
            }
            if (_optionsButton != null)
            {
                _optionsButton.clicked -= OnOptionsClicked;
            }
        }
    }
}
