using UnityEngine;
using Unity.Netcode;

namespace TomAg
{
    public class PlayerInteractor : NetworkBehaviour
    {
        [Header("Interaction Settings")]
        public LayerMask interactableLayer;

        private PlayerController _playerController;
        private PlayerInfo _playerInfo;
        private IInteractable _currentInteractable;

        private void Awake()
        {
            // Initialize components and check for missing components
            _playerController = GetComponent<PlayerController>();
            _playerInfo = GetComponent<PlayerInfo>();

            if (_playerController == null)
            {
                Debug.LogError("PlayerController component is missing!");
            }

            if (_playerInfo == null)
            {
                Debug.LogError("PlayerInfo component is missing!");
            }
        }

        private void OnEnable()
        {
            // Subscribe to the interact event when enabled
            if (_playerController != null)
            {
                _playerController.onInteract -= HandleInteract;
                _playerController.onInteract += HandleInteract;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from the interact event when disabled
            if (_playerController != null)
            {
                _playerController.onInteract -= HandleInteract;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Handle entering an interaction trigger area
            Debug.Log($"Player {OwnerClientId}: Trigger Enter with {other.gameObject.name} on layer {other.gameObject.layer}");

            if (other.gameObject.layer == LayerMask.NameToLayer("Interactable") &&
                other.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                _currentInteractable = interactable;
                Debug.Log($"Player {OwnerClientId}: Found interactable object");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Handle exiting an interaction trigger area
            Debug.Log($"Player {OwnerClientId}: Trigger Exit with {other.gameObject.name}");

            if (_currentInteractable != null && other.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                if (_currentInteractable == interactable)
                {
                    _currentInteractable = null;
                    Debug.Log($"Player {OwnerClientId}: Cleared interactable object");
                }
            }
        }

        private void HandleInteract()
        {
            // Handle the interaction logic when triggered
            Debug.Log($"Player {OwnerClientId}: HandleInteract called");

            if (_currentInteractable != null)
            {
                if (_currentInteractable is MonoBehaviour monoBehaviour && monoBehaviour != null)
                {
                    Debug.Log($"Player {OwnerClientId}: Attempting to interact with a valid interactable");
                    _currentInteractable.Interact(_playerInfo);
                }
                else
                {
                    Debug.Log($"Player {OwnerClientId}: Interactable object has been destroyed or is no longer valid.");
                    _currentInteractable = null; // Clear the reference to the destroyed interactable
                }
            }
            else
            {
                Debug.Log($"Player {OwnerClientId}: No interactable object in range.");
            }
        }
    }
}
