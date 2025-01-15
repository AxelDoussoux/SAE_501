using UnityEngine;
using Unity.Netcode;

public class PlayerAudioListener : NetworkBehaviour
{
    private AudioListener audioListener;
    private Camera playerCamera;

    private void Awake()
    {
        // Obtenir la caméra du joueur
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            audioListener = playerCamera.GetComponent<AudioListener>();

            // Si pas d'AudioListener sur la caméra, on l'ajoute
            if (audioListener == null)
            {
                audioListener = playerCamera.gameObject.AddComponent<AudioListener>();
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (audioListener != null)
        {
            // Activer l'AudioListener uniquement pour le joueur local
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