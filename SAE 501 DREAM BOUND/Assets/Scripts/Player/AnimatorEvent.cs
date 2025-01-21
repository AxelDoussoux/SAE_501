using UnityEngine;
using System;
using Unity.Netcode;

public class AnimatorEvent : NetworkBehaviour
{
    public event Action<string> OnAnimationEvent;

    public void Event(string arg)
    {
        if (string.IsNullOrEmpty(arg))
        {
            Debug.LogWarning("L'argument de TriggerEvent est null ou vide.");
            return;
        }

        Debug.Log($"Animation event déclenché : {arg}");
        OnAnimationEvent?.Invoke(arg);
    }
}
