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
        // Abonne l'�v�nement interne pour d�clencher le son
        OnAnimationEvent += HandleAnimationEvent;
    }

    private void OnDestroy()
    {
        // D�sabonne pour �viter les erreurs si le GameObject est d�truit
        OnAnimationEvent -= HandleAnimationEvent;
    }

    /// <summary>
    /// Cette m�thode est appel�e par l'�v�nement d'animation via Event(string arg).
    /// </summary>
    public void Event(string arg)
    {
        if (string.IsNullOrEmpty(arg))
        {
            Debug.LogWarning("L'argument de l'�v�nement d'animation est null ou vide.");
            return;
        }

        Debug.Log($"�v�nement d'animation re�u avec l'argument : {arg}");
        OnAnimationEvent?.Invoke(arg);
    }

    /// <summary>
    /// Gestionnaire des �v�nements d'animation avec des arguments.
    /// </summary>
    /// <param name="arg">L'argument transmis par l'�v�nement d'animation.</param>
    private void HandleAnimationEvent(string arg)
    {
        if (arg == "Footstep") // V�rifie si l'argument correspond � un bruit de pas
        {
            PlayFootstepSound();
        }
        else
        {
            Debug.Log($"Aucun comportement d�fini pour l'argument : {arg}");
        }
    }

    /// <summary>
    /// Joue un son de pas al�atoire.
    /// </summary>
    private void PlayFootstepSound()
    {
        if (footstepClips.Length == 0 || audioSource == null)
        {
            Debug.LogWarning("Aucun clip ou AudioSource configur� pour FootstepSound.");
            return;
        }

        // Choisit un clip audio al�atoire et le joue
        AudioClip clip = footstepClips[UnityEngine.Random.Range(0, footstepClips.Length)];
        audioSource.PlayOneShot(clip);

        Debug.Log("Bruit de pas jou�.");
    }
}
