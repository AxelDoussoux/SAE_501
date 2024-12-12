using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System;

namespace TomAg
{

    public class PaperInteraction : MonoBehaviour, ITrigger, IInteractable
    {
        public UIDocument uiDocument;  // Reference to UIDocument
        public int playerLayer = 6;  // Layer to identify player objects (replace with your player layer number)
        private PlayerController _playerController;  // Reference to the PlayerController

        private bool isDisplayed = false;  // Tracks if the note is open
        private bool isNearPaper = false;  // Tracks if the player is near the paper
        private VisualElement backgroundOverlay;
        private VisualElement paperDisplay;

        private void Start()
        {
            // Get UI elements from UIDocument
            var root = uiDocument.rootVisualElement;
            backgroundOverlay = root.Q<VisualElement>("BackgroundOverlay");
            paperDisplay = root.Q<VisualElement>("PaperDisplay");

            // Hide UI elements at start
            backgroundOverlay.style.display = DisplayStyle.None;
            paperDisplay.style.display = DisplayStyle.None;

            // Find the PlayerController if not assigned in inspector
            if (_playerController == null)
            {
                _playerController = FindObjectOfType<PlayerController>();
            }

            // Subscribe to the interact event
            if (_playerController != null)
            {
                _playerController.onInteract += HandleInteractInput;
            }
        }

        private void HandleInteractInput()
        {
            // Only toggle if player is near the paper
            if (isNearPaper || isDisplayed)
            {
                ToggleNote();
            }
        }

        private void ToggleNote()
        {
            isDisplayed = !isDisplayed;
            // Show or hide paper UI
            backgroundOverlay.style.display = isDisplayed ? DisplayStyle.Flex : DisplayStyle.None;
            paperDisplay.style.display = isDisplayed ? DisplayStyle.Flex : DisplayStyle.None;

            // Update player's pause state
            if (_playerController != null)
            {
                if (isDisplayed)
                {
                    // If displaying the note, set pause state
                    _playerController.SetPauseState(true);
                }
                else
                {
                    // If closing the note, resume movement
                    _playerController.ResumeMovement();
                }
            }
        }


        // Implement ITrigger interface
        public void OnTriggerActivated()
        {
            isNearPaper = true;
        }

        public void OnTriggerDesactivated()
        {
            isNearPaper = false;

            // If paper is displayed when moving away, hide it and re-enable movement
            if (isDisplayed)
            {
                ToggleNote();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if entering object belongs to the player layer
            if (other.gameObject.layer == playerLayer)
            {
                OnTriggerActivated();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Check if exiting object belongs to the player layer
            if (other.gameObject.layer == playerLayer)
            {
                OnTriggerDesactivated();
            }
        }

        public void Interact(PlayerInfo playerInfo)
        {
            // Handle interaction logic when invoked by the player
            ToggleNote();
        }

        // Clean up event subscription
        private void OnDestroy()
        {
            if (_playerController != null)
            {
                _playerController.onInteract -= HandleInteractInput;
            }
        }
    }
}
