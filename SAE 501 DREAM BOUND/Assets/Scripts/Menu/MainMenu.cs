using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private UIDocument _document; // Reference to the UIDocument for the UI

    private Button createhost; // Reference to the button for creating a host

    // Called when the object is created
    private void Awake()
    {
        _document = GetComponent<UIDocument>(); // Get the UIDocument component attached to this GameObject
    }
}
