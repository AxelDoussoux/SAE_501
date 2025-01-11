using UnityEngine;
using Unity.Netcode;

public class PlayerAudioListener : NetworkBehaviour
{
    private AudioListener audioListener;

    private void Start()
    {
        // Recherche le composant AudioListener dans les enfants du joueur
        audioListener = GetComponentInChildren<AudioListener>();

        if (audioListener == null)
        {
            Debug.LogWarning("No AudioListener found on this player!");
            return;
        }

        // Désactiver l'AudioListener si ce n'est pas le joueur local
        if (!IsOwner)
        {
            audioListener.enabled = false;
        }
        else
        {
            Debug.Log("AudioListener enabled for local player.");
        }
    }
}
