using UnityEngine;
using System;

public class LevelLoaderEvent : MonoBehaviour
{
    public static event Action OnLevelLoad;

    // Triggers the level load event
    public static void TriggerLevelLoad()
    {
        OnLevelLoad?.Invoke();
    }
}
