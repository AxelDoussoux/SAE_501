using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuUI : MonoBehaviour
{
    public NetworkManager multi;

    private void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        Button createhost = root.Q<Button>("createhost");
        Button joinsession = root.Q<Button>("joinsession");
        Button quitButton = root.Q<Button>("quit");
        Label codeLabel = root.Q<Label>("code");  
        TextField inputField = root.Q<TextField>("inputfield");  
        VisualElement menu = root.Q<VisualElement>("menu");  

        createhost.clicked += () =>
        {
            multi.CreateMultiplayerRelay(codeLabel); 
            createhost.style.display = DisplayStyle.None;
                menu.style.display = DisplayStyle.None;
            
        };

        joinsession.clicked += () =>
        {
            if (inputField != null)
            {
                multi.JoinSession(inputField.text); 
            }
            else
            {
                Debug.LogError("Input field for session code not found.");
            }


                menu.style.display = DisplayStyle.None;

        };

        quitButton.clicked += () => Application.Quit();

    }
}
