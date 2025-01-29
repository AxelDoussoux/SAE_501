using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovementSettings
{
    // Direction of movement (default is upward with a magnitude of 4)
    public Vector3 moveDirection = Vector3.up * 4f;

    // Movement speed (default is 2)
    public float moveSpeed = 2f;
}
