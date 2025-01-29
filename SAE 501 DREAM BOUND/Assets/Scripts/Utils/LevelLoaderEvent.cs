using UnityEngine;
using System;

public class LevelLoaderEvent : MonoBehaviour
{
    public static event Action OnLevelLoad;

    public static void TriggerLevelLoad()
    {
        OnLevelLoad?.Invoke();
    }
}