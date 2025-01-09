using UnityEngine;
using Unity.Netcode;
using TomAg;

public class ButtonScript : NetworkBehaviour, IInteractable
{
    private NetworkVariable<bool> isPressed = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

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
    }

    public bool IsPressed()
    {
        return isPressed.Value;
    }
}
