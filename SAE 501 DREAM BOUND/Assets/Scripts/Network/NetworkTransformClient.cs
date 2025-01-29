using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class NetworkTransformClient : NetworkTransform
{
    // Overrides the method to specify that this object is not authoritative on the server
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
