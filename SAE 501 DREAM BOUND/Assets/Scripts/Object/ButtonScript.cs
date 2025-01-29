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

    private Vector3 initialPosition; // Initial position of the button
    public float pressDepth = 0.2f; // Depth of the button press
    public float pressDuration = 0.1f; // Duration of the button press animation

    [Header("Audio Settings")]
    public AudioSource audioSource; // Audio source to play sound
    public AudioClip pressSound; // Sound to play when the button is pressed

    private void Start()
    {
        initialPosition = transform.localPosition; // Store the initial local position of the button
    }

    public void Interact(PlayerInfo playerInfo)
    {
        if (isPressed.Value) return; // Do nothing if the button is already pressed

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
        PlayLocalSound(); // Play sound locally when the button is pressed
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetButtonStateServerRpc()
    {
        if (!isPressed.Value)
        {
            isPressed.Value = true; // Set the button as pressed on the server
            AnimateButtonPressClientRpc(); // Trigger animation on all clients
            DoorManager.Instance.CheckButtonsState(); // Check if all buttons are pressed to open doors
        }
    }

    [ClientRpc]
    private void AnimateButtonPressClientRpc()
    {
        AnimateButtonPress(); // Trigger the button press animation
    }

    public bool IsPressed()
    {
        return isPressed.Value; // Return the current state of the button (pressed or not)
    }

    private void AnimateButtonPress()
    {
        // Animate the button press without returning to the initial position
        transform.DOLocalMove(initialPosition + Vector3.back * pressDepth, pressDuration);
    }

    private void PlayLocalSound()
    {
        if (audioSource != null && pressSound != null)
        {
            audioSource.PlayOneShot(pressSound); // Play the button press sound locally
            Debug.Log("Sound played locally for the interacting player.");
        }
    }
}
