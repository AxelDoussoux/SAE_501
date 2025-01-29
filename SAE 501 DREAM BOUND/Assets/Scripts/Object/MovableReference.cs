using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovableReference
{
    [SerializeField] private CubeMovement movableObject; // Reference to the movable object
    public IMovable Movable => movableObject; // Property to get the IMovable interface from the object
}
