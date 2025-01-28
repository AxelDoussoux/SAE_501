using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerSFX : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepClips;

    public event Action<string> OnAnimationEvent;

    private void Start()
    {
        OnAnimationEvent += HandleAnimationEvent;
    }

    private void OnDestroy()
    {
        OnAnimationEvent -= HandleAnimationEvent;
    }

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

    private void HandleAnimationEvent(string arg)
    {
        if (arg == "Footstep")
        {
            PlayFootstepSound();
        }
        else
        {
            Debug.Log($"Aucun comportement d�fini pour l'argument : {arg}");
        }
    }

    private void PlayFootstepSound()
    {
        if (footstepClips.Length == 0 || audioSource == null)
        {
            Debug.LogWarning("Aucun clip ou AudioSource configur� pour FootstepSound.");
            return;
        }

        AudioClip clip = footstepClips[UnityEngine.Random.Range(0, footstepClips.Length)];
        audioSource.PlayOneShot(clip);

        Debug.Log("Bruit de pas jou�.");
    }
}
