using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour, ITrigger
{
    [SerializeField] private List<MovableReference> movableReferences;
    private bool isActivated;


    public void OnTriggerActivated()
    {
        if (!isActivated)
        {
            foreach (var reference in movableReferences)
            {
                reference.Movable?.StartMoving(true);
            }
            isActivated = true;
        }
    }

    public void OnTriggerDesactivated()
    {
        if (isActivated)
        {
            foreach (var reference in movableReferences)
            {
                reference.Movable?.StartMoving(false);
            }
            isActivated = false;
        }
    }

    // Unity trigger events
    private void OnTriggerEnter(Collider other)
    {
        OnTriggerActivated();
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerDesactivated();
    }
}