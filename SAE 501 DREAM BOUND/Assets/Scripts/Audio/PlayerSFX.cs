using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerSFX : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepClips;
    private Animator localAnimator; // R�f�rence � l'Animator du joueur
    public event Action<string> OnAnimationEvent;

    private void Start()
    {
        OnAnimationEvent += HandleAnimationEvent;

        // Si on ne trouve pas l'Animator, on le cherche automatiquement
        if (localAnimator == null)
        {
            localAnimator = GetComponent<Animator>();
        }
    }

    private void OnDestroy()
    {
        OnAnimationEvent -= HandleAnimationEvent;
    }

    // Cette m�thode est appel�e par l'event d'animation
    public void Event(string arg, AnimationEvent animEvent)
    {
        if (string.IsNullOrEmpty(arg))
        {
            Debug.LogWarning("L'argument de l'�v�nement d'animation est null ou vide.");
            return;
        }

        // On v�rifie si l'animator qui a envoy� l'event est celui de ce joueur
        if (animEvent.animatorClipInfo.clip.name != localAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name)
        {
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