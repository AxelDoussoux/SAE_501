
using UnityEngine;
using System;

public class AnimatorEvent : MonoBehaviour
{
    public event Action<string> onAnimationEvent;
    public void Event(string arg)
    { 
        Debug.Log($"Animation event :{arg}");
        onAnimationEvent?.Invoke(arg);
    }
}
