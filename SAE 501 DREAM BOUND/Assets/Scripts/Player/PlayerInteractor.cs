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
            // Vérifie si l'objet qui entre dans la zone est interactable
            if (other.gameObject.layer == LayerMask.NameToLayer("Interactable") &&
                other.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                _currentInteractable = interactable;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Si l'objet interactable sort de la zone, on réinitialise _currentInteractable
            if (_currentInteractable != null && other.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                if (_currentInteractable == interactable)
                {
                    _currentInteractable = null;
                }
            }
        }

        private void HandleInteract()
        {
            // Si un objet interactable est présent dans la zone, on appelle sa méthode Interact
            if (_currentInteractable != null)
            {
                _currentInteractable.Interact(_playerInfo);
            }
            else
            {
                Debug.Log("No interactable object in range.");
            }
        }
    }
}
