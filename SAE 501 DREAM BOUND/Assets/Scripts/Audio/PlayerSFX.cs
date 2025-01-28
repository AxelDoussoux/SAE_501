using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerSFX : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource; // Source audio pour jouer les sons
    [SerializeField] private AudioClip[] footstepClips; // Liste des bruits de pas

    public event Action<string> OnAnimationEvent;

    private void Start()
    {
        // Abonne l'événement interne pour déclencher le son
        OnAnimationEvent += HandleAnimationEvent;
    }

    private void OnDestroy()
    {
        // Désabonne pour éviter les erreurs si le GameObject est détruit
        OnAnimationEvent -= HandleAnimationEvent;
    }

    /// <summary>
    /// Cette méthode est appelée par l'événement d'animation via Event(string arg).
    /// </summary>
    public void Event(string arg)
    {
        if (string.IsNullOrEmpty(arg))
        {
            Debug.LogWarning("L'argument de l'événement d'animation est null ou vide.");
            return;
        }

        Debug.Log($"Événement d'animation reçu avec l'argument : {arg}");
        OnAnimationEvent?.Invoke(arg);
    }

    /// <summary>
    /// Gestionnaire des événements d'animation avec des arguments.
    /// </summary>
    /// <param name="arg">L'argument transmis par l'événement d'animation.</param>
    private void HandleAnimationEvent(string arg)
    {
        if (arg == "Footstep") // Vérifie si l'argument correspond à un bruit de pas
        {
            PlayFootstepSound();
        }
        else
        {
            Debug.Log($"Aucun comportement défini pour l'argument : {arg}");
        }
    }

    /// <summary>
    /// Joue un son de pas aléatoire.
    /// </summary>
    private void PlayFootstepSound()
    {
        if (footstepClips.Length == 0 || audioSource == null)
        {
            Debug.LogWarning("Aucun clip ou AudioSource configuré pour FootstepSound.");
            return;
        }

        // Choisit un clip audio aléatoire et le joue
        AudioClip clip = footstepClips[UnityEngine.Random.Range(0, footstepClips.Length)];
        audioSource.PlayOneShot(clip);

        Debug.Log("Bruit de pas joué.");
    }
}
