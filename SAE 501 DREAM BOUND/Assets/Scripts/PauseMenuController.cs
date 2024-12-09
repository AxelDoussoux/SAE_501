using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace TomAg
{
    public class PauseMenuController : NetworkBehaviour
    {
        public static PauseMenuController Instance { get; private set; }

        [SerializeField] private UIDocument _pauseMenuDocument;
        [SerializeField] private UIDocument _mainMenuDocument;
        private VisualElement _pauseMenuRoot;
        private VisualElement _mainMenuRoot;
        private Button _continueButton;
        private Button _quitButton;
        private PlayerController _localPlayer;
        private bool _isMenuVisible = false;
        private bool _isInitialized = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            // S'assurer que l'UI est initialisée quand le document est activé
            if (!_isInitialized)
            {
                InitializeUI();
            }
        }

        private void Start()
        {
            Debug.Log("PauseMenuController Start called");
            if (!_isInitialized)
            {
                InitializeUI();
            }
        }

        private void InitializeUI()
        {
            Debug.Log("Initializing UI...");

            if (_pauseMenuDocument == null)
            {
                Debug.LogError("Pause Menu UIDocument reference is missing!");
                return;
            }

            // Attendre une frame pour s'assurer que le UIDocument est prêt
            // C'est important car le rootVisualElement peut ne pas être immédiatement disponible
            _pauseMenuDocument.rootVisualElement.schedule.Execute(() => {
                Debug.Log("Setting up pause menu elements...");

                // Récupérer la racine du menu pause
                _pauseMenuRoot = _pauseMenuDocument.rootVisualElement;
                if (_pauseMenuRoot == null)
                {
                    Debug.LogError("Pause menu root is null after initialization!");
                    return;
                }

                // Récupérer les boutons
                _continueButton = _pauseMenuRoot.Q<Button>("continue");
                _quitButton = _pauseMenuRoot.Q<Button>("quit");

                if (_continueButton == null || _quitButton == null)
                {
                    Debug.LogError($"Buttons not found! Continue: {_continueButton != null}, Quit: {_quitButton != null}");
                    return;
                }

                // Configurer les événements des boutons
                _continueButton.clicked += OnContinueClicked;
                _quitButton.clicked += OnQuitClicked;

                // Initialiser le menu principal si présent
                if (_mainMenuDocument != null)
                {
                    _mainMenuRoot = _mainMenuDocument.rootVisualElement;
                    _mainMenuRoot.style.display = DisplayStyle.Flex;
                }

                // Cacher le menu de pause au début
                _pauseMenuRoot.style.display = DisplayStyle.None;
                _isMenuVisible = false;

                _isInitialized = true;
                Debug.Log("UI Initialization completed successfully");
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
            Debug.Log($"Local player registered with ID: {player.PlayerId}");
        }

        private void TogglePauseMenu()
        {
            Debug.Log($"TogglePauseMenu called, current visibility: {_isMenuVisible}");

            if (_pauseMenuRoot == null)
            {
                Debug.LogError("Menu not initialized properly. Attempting to reinitialize...");
                InitializeUI();
                return;
            }

            _isMenuVisible = !_isMenuVisible;
            _pauseMenuRoot.style.display = _isMenuVisible ? DisplayStyle.Flex : DisplayStyle.None;

            if (_localPlayer != null)
            {
                _localPlayer.SetPauseState(_isMenuVisible);
                Debug.Log($"Player pause state set to: {_isMenuVisible}");
            }
        }

        private void OnContinueClicked()
        {
            Debug.Log("Continue button clicked");
            if (_localPlayer != null)
            {
                _isMenuVisible = false;
                _pauseMenuRoot.style.display = DisplayStyle.None;
                _localPlayer.SetPauseState(false);
                _localPlayer.ResumeMovement();
            }
        }

        private void OnQuitClicked()
        {
            Debug.Log("Quit button clicked");
            if (_localPlayer != null)
            {
                if (Unity.Netcode.NetworkManager.Singleton.IsHost)
                {
                    Unity.Netcode.NetworkManager.Singleton.Shutdown();
                    Debug.Log("Host: Shutting down server");
                }
                else if (Unity.Netcode.NetworkManager.Singleton.IsClient)
                {
                    Unity.Netcode.NetworkManager.Singleton.Shutdown();
                    Debug.Log("Client: Disconnecting from server");
                }

                _pauseMenuRoot.style.display = DisplayStyle.None;
                if (_mainMenuRoot != null)
                {
                    _mainMenuRoot.style.display = DisplayStyle.Flex;
                    Debug.Log("Main menu displayed");
                }
            }
        }

        /*private void OnDestroy()
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
        }
        */
    }
}