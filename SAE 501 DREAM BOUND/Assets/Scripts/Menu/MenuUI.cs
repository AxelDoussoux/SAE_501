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

    private void OnEnable()
    {
        // V�rifie si le composant UIDocument est pr�sent
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component is missing.");
            return;
        }

        root = uiDocument.rootVisualElement;

        // R�cup�ration des �l�ments de l'interface utilisateur
        createHostButton = root.Q<Button>("createhost");
        joinSessionButton = root.Q<Button>("joinsession");
        quitButton = root.Q<Button>("quit");
        codeLabel = root.Q<Label>("code");
        inputField = root.Q<TextField>("inputfield");
        menu = root.Q<VisualElement>("menu");
        wrapper = root.Q<VisualElement>("Wrapper");

        // V�rifie si les �l�ments de l'UI sont trouv�s
        if (createHostButton == null || joinSessionButton == null || quitButton == null ||
            codeLabel == null || inputField == null || menu == null || wrapper == null)
        {
            Debug.LogError("One or more UI elements are missing.");
            return;
        }

        // �v�nements des boutons
        createHostButton.clicked += OnCreateHostClicked;
        joinSessionButton.clicked += OnJoinSessionClicked;
        quitButton.clicked += OnQuitButtonClicked;
    }

    private void OnDisable()
    {
        // Supprime les abonnements pour �viter des r�f�rences persistantes
        createHostButton.clicked -= OnCreateHostClicked;
        joinSessionButton.clicked -= OnJoinSessionClicked;
        quitButton.clicked -= OnQuitButtonClicked;
    }

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

        // Masque les �l�ments de l'interface utilisateur
        UIVisibility();
    }

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

        // Masque les �l�ments de l'interface utilisateur
        UIVisibility();
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

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

