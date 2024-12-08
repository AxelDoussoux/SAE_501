using UnityEngine;
using UnityEngine.UIElements;

public class PaperDisplayController : MonoBehaviour
{
    public UIDocument uiDocument; // Reference to the UIDocument
    private VisualElement backgroundOverlay;
    private VisualElement paperDisplay;

    private bool isDisplayed = false; // Tracks if the paper is visible

    void Start()
    {
        // Get the root element from the UI Document
        var root = uiDocument.rootVisualElement;

        // Find elements by their names
        backgroundOverlay = root.Q<VisualElement>("BackgroundOverlay");
        paperDisplay = root.Q<VisualElement>("PaperDisplay");
    }

    void Update()
    {
        // Toggle UI on key press (e.g., "E")
        if (Input.GetKeyDown(KeyCode.E))
        {
            isDisplayed = !isDisplayed;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (isDisplayed)
        {
            // Show UI
            backgroundOverlay.style.display = DisplayStyle.Flex;
            paperDisplay.style.display = DisplayStyle.Flex;
        }
        else
        {
            // Hide UI
            backgroundOverlay.style.display = DisplayStyle.None;
            paperDisplay.style.display = DisplayStyle.None;
        }
    }
}
