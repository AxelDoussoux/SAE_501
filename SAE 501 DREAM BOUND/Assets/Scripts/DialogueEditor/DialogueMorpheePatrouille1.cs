using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMorpheePatrouille1 : MonoBehaviour
{
    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible
    }
}
