using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovableReference
{
    [SerializeField] private CubeMovement movableObject;
    public IMovable Movable => movableObject;
}
