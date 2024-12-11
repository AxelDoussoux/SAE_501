using Unity.Services.Vivox;
using UnityEngine;

public class VivoxMicrophoneManager : MonoBehaviour
{
    void Start()
    {
        if (VivoxService.Instance != null)
        {
            Unity.Services.Vivox.VivoxService.Instance.InputDevice = Microphone.devices[0]; // Utilise le premier microphone détecté
            Debug.Log($"Vivox is using microphone: {Microphone.devices[0]}");
        }
        else
        {
            Debug.LogError("Vivox service is not initialized.");
        }
    }
}
