using UnityEngine;
using Unity.Netcode;
using DG.Tweening; // Import DOTween
using TomAg;

public class ButtonScript : NetworkBehaviour, IInteractable
{
    private NetworkVariable<bool> isPressed = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private Vector3 initialPosition; // Stocke la position initiale du bouton
    public float pressDepth = 0.2f; // Distance de l'enfoncement (en Z)
    public float pressDuration = 0.1f; // Durée de l'enfoncement et du retour

    private void Start()
    {
        initialPosition = transform.localPosition; // Enregistre la position initiale
    }

    public void Interact(PlayerInfo playerInfo)
    {
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

        if (IsServer)
        {
            SetButtonStateServerRpc(true);
        }
        else
        {
            SetButtonStateServerRpc(true);
            Debug.Log($"Player {playerInfo.OwnerClientId} sending ServerRpc");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetButtonStateServerRpc(bool pressed)
    {
        isPressed.Value = pressed;
        DoorManager.Instance.CheckButtonsState(); // Vérifie l'état des boutons
        AnimateButtonPressClientRpc(); // Lance l'animation sur tous les clients
    }

    [ClientRpc]
    private void AnimateButtonPressClientRpc()
    {
        AnimateButtonPress(); // Exécute l'animation localement sur chaque client
    }

    public bool IsPressed()
    {
        return isPressed.Value;
    }

    private void AnimateButtonPress()
    {
        // Enfonce le bouton vers le mur (Z négatif)
        transform.DOLocalMove(initialPosition + Vector3.back * pressDepth, pressDuration)
            .OnComplete(() =>
            {
                // Remonte le bouton à la position initiale
                transform.DOLocalMove(initialPosition, pressDuration);
            });
    }
}
