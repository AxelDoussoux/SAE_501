using UnityEngine;
using System;
using Unity.Netcode;

public class AnimatorEvent : NetworkBehaviour
{
    // Event triggered when an animation event occurs
    public event Action<string> OnAnimationEvent;

    // Method to invoke the animation event with an argument
    public void Event(string arg)
    {
        // Check if the argument is null or empty
        if (string.IsNullOrEmpty(arg))
        {
            Debug.LogWarning("The argument for TriggerEvent is null or empty.");
            return;
        }

        // Log the event triggered with the argument
        Debug.Log($"Animation event triggered: {arg}");

        // Invoke the event, passing the argument
        OnAnimationEvent?.Invoke(arg);
    }
}
