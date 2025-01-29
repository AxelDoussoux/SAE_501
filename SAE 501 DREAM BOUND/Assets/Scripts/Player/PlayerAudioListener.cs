using UnityEngine;
using Unity.Netcode;

public class PlayerAudioListener : NetworkBehaviour
{
    private AudioListener audioListener;
    private Camera playerCamera;

    private void Awake()
    {
        // Get the player's camera
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            audioListener = playerCamera.GetComponent<AudioListener>();

            // If there's no AudioListener on the camera, add one
            if (audioListener == null)
            {
                audioListener = playerCamera.gameObject.AddComponent<AudioListener>();
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        // Called when the network object is spawned
        base.OnNetworkSpawn();

        if (audioListener != null)
        {
            // Enable the AudioListener only for the local player
            audioListener.enabled = IsOwner;

            if (IsOwner)
            {
                Debug.Log($"AudioListener enabled for Player {OwnerClientId}");
            }
            else
            {
                Debug.Log($"AudioListener disabled for non-local Player {OwnerClientId}");
            }
        }
        else
        {
            Debug.LogWarning("No AudioListener found!");
        }
    }
}
