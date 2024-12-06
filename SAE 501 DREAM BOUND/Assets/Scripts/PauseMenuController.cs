using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

namespace TomAg
{
    public class PauseMenuController : NetworkBehaviour
    {
        [SerializeField] private UIDocument pauseMenuDocument;
        private PlayerController localPlayerController;
        private VisualElement rootElement;
        private bool isMenuOpen = false;

        private void Start()
        {
            Debug.Log("PauseMenuController - Start called");

            if (pauseMenuDocument == null)
            {
                Debug.LogError("PauseMenuController - Pause Menu UI Document is not assigned!");
                return;
            }

            rootElement = pauseMenuDocument.rootVisualElement;
            pauseMenuDocument.enabled = false;

            if (Unity.Netcode.NetworkManager.Singleton != null)
            {
                Unity.Netcode.NetworkManager.Singleton.OnClientStarted += OnClientStarted;
                Debug.Log("PauseMenuController - Subscribed to OnClientStarted");
            }
            else
            {
                Debug.LogError("PauseMenuController - NetworkManager.Singleton is null!");
            }
        }

        private void OnClientStarted()
        {
            Debug.Log("PauseMenuController - OnClientStarted called");
            FindLocalPlayerController();
        }

        private void FindLocalPlayerController()
        {
            PlayerController[] allControllers = FindObjectsOfType<PlayerController>();
            Debug.Log($"PauseMenuController - Found {allControllers.Length} PlayerControllers");

            foreach (PlayerController controller in allControllers)
            {
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

        private void TogglePauseMenu()
        {
            isMenuOpen = !isMenuOpen;
            Debug.Log($"PauseMenuController - Toggling menu - IsOpen: {isMenuOpen}");

            pauseMenuDocument.enabled = isMenuOpen;
            Debug.Log($"PauseMenuController - UIDocument enabled: {pauseMenuDocument.enabled}");

            if (isMenuOpen)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                Debug.Log("PauseMenuController - Cursor unlocked and visible");
            }
            else
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
                Debug.Log("PauseMenuController - Cursor locked and hidden");
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
            }

            base.OnDestroy(); // Appel de la méthode de base
        }
    }
}