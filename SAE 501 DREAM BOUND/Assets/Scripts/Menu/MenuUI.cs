using System.Collections;
using System.Collections.Generic;
using TomAg;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuUI : MonoBehaviour
{
    public NetworkManager multi;

    private UIDocument uiDocument;
    private VisualElement root;

    private PauseMenuController pauseMenuController;

    private Button createHostButton;
    private Button joinSessionButton;
    private Button quitButton;
    private Label codeLabel;
    private TextField inputField;
    private VisualElement menu;
    private VisualElement wrapper;

    private bool isHide = false;
    private bool stopUpdate = false;

    // Update is called once per frame
    private void Update()
    {
        // Check if update execution should continue
        if (stopUpdate) return;

        // Optionally ensure the cursor remains visible
        if (!UnityEngine.Cursor.visible)
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }

    // Called when the object is enabled
    private void OnEnable()
    {
        // Check if the UIDocument component is present
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component is missing.");
            return;
        }

        root = uiDocument.rootVisualElement;

        // Retrieve UI elements
        createHostButton = root.Q<Button>("createhost");
        joinSessionButton = root.Q<Button>("joinsession");
        quitButton = root.Q<Button>("quit");
        codeLabel = root.Q<Label>("code");
        inputField = root.Q<TextField>("inputfield");
        menu = root.Q<VisualElement>("menu");
        wrapper = root.Q<VisualElement>("Wrapper");

        // Check if any UI element is missing
        if (createHostButton == null || joinSessionButton == null || quitButton == null ||
            codeLabel == null || inputField == null || menu == null || wrapper == null)
        {
            Debug.LogError("One or more UI elements are missing.");
            return;
        }

        // Register button click events
        createHostButton.clicked += OnCreateHostClicked;
        joinSessionButton.clicked += OnJoinSessionClicked;
        quitButton.clicked += OnQuitButtonClicked;
    }

    // Called when the object is disabled
    private void OnDisable()
    {
        // Unsubscribe to prevent persistent references
        createHostButton.clicked -= OnCreateHostClicked;
        joinSessionButton.clicked -= OnJoinSessionClicked;
        quitButton.clicked -= OnQuitButtonClicked;
    }

    // Called when the "Create Host" button is clicked
    private void OnCreateHostClicked()
    {
        if (multi != null)
        {
            multi.CreateMultiplayerRelay(codeLabel);
        }
        else
        {
            Debug.LogError("NetworkManager is not assigned.");
        }

        stopUpdate = true;
        UIVisibility();
    }

    // Called when the "Join Session" button is clicked
    private void OnJoinSessionClicked()
    {
        if (multi != null && inputField != null)
        {
            multi.JoinSession(inputField.text);
        }
        else
        {
            Debug.LogError("NetworkManager or InputField is not assigned.");
        }

        stopUpdate = true;
        UIVisibility();
    }

    // Called when the "Quit" button is clicked
    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    // Toggles the visibility of the menu and wrapper elements
    public void UIVisibility()
    {
        if (!isHide)
        {
            menu.style.display = DisplayStyle.None;
            wrapper.style.display = DisplayStyle.None;
            isHide = true;
            return;
        }
        else
        {
            menu.style.display = DisplayStyle.Flex;
            wrapper.style.display = DisplayStyle.Flex;
            isHide = false;
            return;
        }
    }
}
