using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DisplayPaper : MonoBehaviour, ITrigger
{
    public UIDocument uiDocument;  // Reference to UIDocument
    public string playerTag = "Player";  // Tag to identify player objects

    private bool isDisplayed = false;  // Tracks if the note is open
    private bool isNearPaper = false;  // Tracks if the player is near the paper
    private VisualElement backgroundOverlay;
    private VisualElement paperDisplay;
    private VisualElement textElement;

    private void Start()
    {
        // Get UI elements from the UIDocument
        var root = uiDocument.rootVisualElement;
        backgroundOverlay = root.Q<VisualElement>("BackgroundOverlay");
        paperDisplay = root.Q<VisualElement>("PaperDisplay");
        textElement = root.Q<VisualElement>("TextElement");  // Your Text UI element

        // Hide UI elements initially
        backgroundOverlay.style.display = DisplayStyle.None;
        paperDisplay.style.display = DisplayStyle.None;
        textElement.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        // Check for input to toggle the note display (e.g., "E" key)
        if (isNearPaper && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleNote();
        }

        // Allow closing the note with "Escape"
        if (isDisplayed && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleNote();
        }
    }

    private void ToggleNote()
    {
        isDisplayed = !isDisplayed;

        // Show or hide the note UI
        backgroundOverlay.style.display = isDisplayed ? DisplayStyle.Flex : DisplayStyle.None;
        paperDisplay.style.display = isDisplayed ? DisplayStyle.Flex : DisplayStyle.None;

        // Disable or enable player input (optional for multiplayer)
        // Add logic here to stop specific player actions if needed.
    }

    // Implementation of ITrigger
    public void OnTriggerActivated()
    {
        isNearPaper = true;
        textElement.style.display = DisplayStyle.Flex;  // Show "Press E to Read" text
    }

    public void OnTriggerDesactivated()
    {
        isNearPaper = false;
        textElement.style.display = DisplayStyle.None;  // Hide "Press E to Read" text
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has the player tag
        if (other.CompareTag(playerTag))
        {
            OnTriggerActivated();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object exiting the trigger has the player tag
        if (other.CompareTag(playerTag))
        {
            OnTriggerDesactivated();
        }
    }
}
