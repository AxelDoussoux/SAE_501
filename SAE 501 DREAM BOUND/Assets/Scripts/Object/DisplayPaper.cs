using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System;
using TomAg;

public class DisplayPaper : MonoBehaviour //, ITrigger
{
    /*
    public UIDocument uiDocument;  // Reference to UIDocument
    public string playerTag = "Player";  // Tag to identify player objects
    public PlayerController playerController;  // Reference to the PlayerController

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
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        // Subscribe to the interact event
        if (playerController != null)
        {
            playerController.onInteract += HandleInteractInput;
        }
    }

    private void HandleInteractInput()
    {
        // Only toggle if player is near the paper
        if (isNearPaper)
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

        // Restrict or allow player movement
        if (playerController != null)
        {
            // Use a method in PlayerController to restrict movement
            playerController.SetMovementEnabled(!isDisplayed);
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
        // Check if entering object has player tag
        if (other.CompareTag(playerTag))
        {
            OnTriggerActivated();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if exiting object has player tag
        if (other.CompareTag(playerTag))
        {
            OnTriggerDesactivated();
        }
    }

    // Clean up event subscription
    private void OnDestroy()
    {
        if (playerController != null)
        {
            playerController.onInteract -= HandleInteractInput;
        }
    }
    */
}