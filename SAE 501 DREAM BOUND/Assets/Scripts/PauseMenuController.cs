using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using System.Collections;
using UnityEngine.SceneManagement;

namespace TomAg
{
    public class PauseMenuController : NetworkBehaviour
    {
        [SerializeField] private UIDocument pauseMenuDocument;
        private PlayerController localPlayerController;
        private VisualElement rootElement;
        private bool isMenuOpen = false;

        private Button continueButton;
        private Button quitButton;

        private bool isMenuInitialized = false;

        public static PauseMenuController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogError("PauseMenuController - Multiple instances detected!");
                Destroy(this);
            }
        }

        private void Start()
        {
            StartCoroutine(WaitForLocalPlayerController());

            if (pauseMenuDocument == null)
            {
                Debug.LogError("PauseMenuController - Pause Menu UI Document is not assigned!");
                return;
            }

            pauseMenuDocument.enabled = false;
        }


        // Wait for the client and the network to be setup


        public void RegisterLocalPlayer(PlayerController playerController)
        {
            if (playerController == null)
            {
                Debug.LogError("PauseMenuController - Tried to register a null PlayerController");
                return;
            }

            if (playerController.IsOwner)
            {
                localPlayerController = playerController;
                localPlayerController.onPauseToggle += TogglePauseMenu;
                Debug.Log("PauseMenuController - Local PlayerController registered");
            }
        }

        private void OnClientStarted()
        {
            Debug.Log("PauseMenuController - OnClientStarted called");
        }

        private IEnumerator WaitForLocalPlayerController()
        {
            while (localPlayerController == null)
            {
                PlayerController[] allControllers = FindObjectsOfType<PlayerController>();
                foreach (var controller in allControllers)
                {
                    if (controller.IsOwner)
                    {
                        localPlayerController = controller;
                        localPlayerController.onPauseToggle += TogglePauseMenu;
                        Debug.Log("PauseMenuController - Local PlayerController found");
                        yield break;
                    }
                }

                Debug.Log("PauseMenuController - Retrying to find PlayerController...");
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void OnNetworkObjectSpawned(NetworkObject networkObject)
        {
            PlayerController controller = networkObject.GetComponent<PlayerController>();

            if (controller != null && controller.IsOwner)
            {
                Debug.Log($"PauseMenuController - Found spawned PlayerController: {controller.gameObject.name}");
                localPlayerController = controller;
                localPlayerController.onPauseToggle += TogglePauseMenu;
            }
        }

        private void OnSceneLoadComplete(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
        {
            Debug.Log($"PauseMenuController - Scene {sceneName} loaded. Searching for PlayerControllers...");
            FindLocalPlayerController();
        }

        private void FindLocalPlayerController()
        {
            PlayerController[] allControllers = FindObjectsOfType<PlayerController>();
            Debug.Log($"PauseMenuController - Found {allControllers.Length} PlayerControllers");

            foreach (PlayerController controller in allControllers)
            {
                Debug.Log($"PauseMenuController - Checking PlayerController {controller.gameObject.name}, IsOwner: {controller.IsOwner}");

                if (controller.IsOwner)
                {
                    localPlayerController = controller;
                    localPlayerController.onPauseToggle += TogglePauseMenu;
                    Debug.Log("PauseMenuController - Local PlayerController found and connected");
                    return;
                }
            }

            Debug.LogError("PauseMenuController - No local PlayerController found!");
        }


        //  Gestion du menu

        private void OnEnable()
        {
            // Initialiser le menu mais différer la recherche des boutons
            StartCoroutine(InitializeMenuWithDelay());
        }

        private IEnumerator InitializeMenuWithDelay()
        {
            // Attendre une frame pour être sûr que tout est initialisé
            yield return null;

            InitializeMenu();
        }



        private void InitializeMenu()
        {
            if (pauseMenuDocument == null)
            {
                Debug.LogError("PauseMenuController - UIDocument is not assigned!");
                return;
            }

            // Activer le document pour s'assurer que tout est prêt
            if (!pauseMenuDocument.enabled)
            {
                pauseMenuDocument.enabled = true;
            }

            // Récupérer le rootVisualElement
            rootElement = pauseMenuDocument.rootVisualElement;

            if (rootElement == null)
            {
                Debug.LogError("PauseMenuController - RootVisualElement is null!");
                return;
            }

            // Recherche des boutons
            continueButton = rootElement.Q<Button>("Continue");
            quitButton = rootElement.Q<Button>("Quit");

            if (continueButton == null)
            {
                Debug.LogError("PauseMenuController - 'Continue' button not found in UIDocument.");
            }
            else
            {
                continueButton.clicked += () =>
                {
                    Debug.Log("Continue button clicked!");
                    OnContinueClicked();
                };
            }

            if (quitButton == null)
            {
                Debug.LogError("PauseMenuController - 'Quit' button not found in UIDocument.");
            }
            else
            {
                quitButton.clicked += () =>
                {
                    Debug.Log("Quit button clicked!");
                    OnQuitClicked();
                };
            }

            Debug.Log("PauseMenuController - Menu initialized successfully.");
        }




        private void OnContinueClicked()
        {
            if (!isMenuInitialized) return;

            pauseMenuDocument.enabled = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;

            Debug.Log("PauseMenuController - Resuming game.");
        }

        private void OnQuitClicked()
        {
            if (!isMenuInitialized) return;

            if (IsHost())
            {
                Unity.Netcode.NetworkManager.Singleton.Shutdown();
            }

            // Charger le menu principal
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");

            Debug.Log("PauseMenuController - Quitting to main menu.");
        }

        private void StopGame()
        {
            if (Unity.Netcode.NetworkManager.Singleton != null)
            {
                Unity.Netcode.NetworkManager.Singleton.Shutdown(); // Arrête le serveur
                ReturnToMainMenu();
            }
        }

        private void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu"); // Charge la scène du menu principal
        }

        private bool IsHost()
        {
            return Unity.Netcode.NetworkManager.Singleton != null && Unity.Netcode.NetworkManager.Singleton.IsHost;
        }

        private void TogglePauseMenu()
        {
            isMenuOpen = !isMenuOpen;
            pauseMenuDocument.enabled = isMenuOpen;

            if (isMenuOpen)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                Debug.Log("Cursor unlocked and visible.");
            }
            else
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
                Debug.Log("Cursor locked and hidden.");
            }
        }


        public override void OnDestroy()
        {
            if (localPlayerController != null)
            {
                localPlayerController.onPauseToggle -= TogglePauseMenu;
            }

            if (Unity.Netcode.NetworkManager.Singleton != null)
            {
                Unity.Netcode.NetworkManager.Singleton.OnClientStarted -= OnClientStarted;
                Unity.Netcode.NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoadComplete;
            }

            if (continueButton != null)
                continueButton.clicked -= OnContinueClicked;

            if (quitButton != null)
                quitButton.clicked -= OnQuitClicked;

            base.OnDestroy();
        }

    }
}