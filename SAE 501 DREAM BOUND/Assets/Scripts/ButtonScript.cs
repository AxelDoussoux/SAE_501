using UnityEngine;
using Unity.Netcode;

public class ButtonScript : NetworkBehaviour
{
    [SerializeField] private GameObject door; // Référence à la porte
    private NetworkVariable<bool> isPressed = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && IsOwner) // Vérifie si c'est un joueur local
        {
            SetButtonStateServerRpc(true); // Informe le serveur que le bouton est appuyé
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && IsOwner)
        {
            SetButtonStateServerRpc(false); // Informe le serveur que le bouton est relâché
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetButtonStateServerRpc(bool pressed)
    {
        isPressed.Value = pressed; // Met à jour l'état du bouton sur le serveur
        DoorManager.Instance.CheckButtonsState(); // Vérifie l'état des boutons
    }

    public bool IsPressed()
    {
        return isPressed.Value;
    }
}
