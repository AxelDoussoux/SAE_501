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
            if (_playerController != null)
            {
                _playerController.onInteract += HandleInteract;
            }
        }

        private void OnDisable()
        {
            if (_playerController != null)
            {
                _playerController.onInteract -= HandleInteract;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
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
            Debug.Log($"Player {OwnerClientId}: HandleInteract called");

            if (_currentInteractable != null)
            {
                Debug.Log($"Player {OwnerClientId}: Attempting to interact");
                _currentInteractable.Interact(_playerInfo);
            }
            else
            {
                Debug.Log($"Player {OwnerClientId}: No interactable object in range.");
            }
        }
    }
}
