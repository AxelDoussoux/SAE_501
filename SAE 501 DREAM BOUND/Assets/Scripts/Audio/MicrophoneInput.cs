using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    private AudioClip microphoneClip;
    private string microphoneDevice;
    public int sampleRate = 44100;

    void Start()
    {
        // Vérifiez si un microphone est disponible
        if (Microphone.devices.Length > 0)
        {
            microphoneDevice = Microphone.devices[0];
            Debug.Log($"Using microphone: {microphoneDevice}");

            // Démarrez l'enregistrement
            microphoneClip = Microphone.Start(microphoneDevice, true, 10, sampleRate);
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (microphoneClip == null) return;

        // Obtenez l'audio depuis le microphone
        int micPosition = Microphone.GetPosition(microphoneDevice) - data.Length;
        if (micPosition < 0) return;

        microphoneClip.GetData(data, micPosition);
    }

    void OnDestroy()
    {
        if (Microphone.IsRecording(microphoneDevice))
        {
            Microphone.End(microphoneDevice);
            Debug.Log("Microphone stopped.");
        }
    }
}
