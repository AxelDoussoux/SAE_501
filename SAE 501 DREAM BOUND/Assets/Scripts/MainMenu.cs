using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private UIDocument _document;

    private Button createhost ;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();


    }
}
