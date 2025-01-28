using UnityEngine;
using Unity.Netcode;
using DG.Tweening;
using TomAg;

public class ButtonScript : NetworkBehaviour, IInteractable
{
    private NetworkVariable<bool> isPressed = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private Vector3 initialPosition;
    public float pressDepth = 0.2f;
    public float pressDuration = 0.1f;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip pressSound;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    public void Interact(PlayerInfo playerInfo)
    {
        if (isPressed.Value) return; // Si déjà pressé, ne rien faire

        Debug.Log($"Button Interact called by Player {(playerInfo != null ? playerInfo.OwnerClientId.ToString() : "null")}");

        if (playerInfo == null)
        {
            Debug.LogError("PlayerInfo is null. Cannot interact with the button.");
            return;
        }

        if (!playerInfo.canInteractWithButtons)
        {
            Debug.Log($"Player {playerInfo.OwnerClientId} cannot interact with this button.");
            return;
        }

        Debug.Log($"Player {playerInfo.OwnerClientId} attempting to press button. IsServer: {IsServer}");
        SetButtonStateServerRpc();
        PlayLocalSound();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetButtonStateServerRpc()
    {
        if (!isPressed.Value)
        {
            isPressed.Value = true;
            AnimateButtonPressClientRpc();
            DoorManager.Instance.CheckButtonsState();
        }
    }

    [ClientRpc]
    private void AnimateButtonPressClientRpc()
    {
        AnimateButtonPress();
    }

    public bool IsPressed()
    {
        return isPressed.Value;
    }

    private void AnimateButtonPress()
    {
        // Animation simple sans retour à la position initiale
        transform.DOLocalMove(initialPosition + Vector3.back * pressDepth, pressDuration);
    }

    private void PlayLocalSound()
    {
        if (audioSource != null && pressSound != null)
        {
            audioSource.PlayOneShot(pressSound);
            Debug.Log("Sound played locally for the interacting player.");
        }
    }
}