using UnityEngine;
using Unity.Netcode;
using TomAg;

public class ButtonScript : NetworkBehaviour, IInteractable
{
    [SerializeField] private GameObject door; // Référence à la porte
    private NetworkVariable<bool> isPressed = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public void Interact(PlayerInfo playerInfo)
    {
        if (IsServer)
        {
            SetButtonStateServerRpc(true);
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
