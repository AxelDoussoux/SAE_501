using UnityEngine;
using Unity.Netcode;

public class ButtonScript : NetworkBehaviour
{
    [SerializeField] private GameObject door; // R�f�rence � la porte
    private NetworkVariable<bool> isPressed = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && IsOwner) // V�rifie si c'est un joueur local
        {
            SetButtonStateServerRpc(true); // Informe le serveur que le bouton est appuy�
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && IsOwner)
        {
            SetButtonStateServerRpc(false); // Informe le serveur que le bouton est rel�ch�
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetButtonStateServerRpc(bool pressed)
    {
        isPressed.Value = pressed; // Met � jour l'�tat du bouton sur le serveur
        DoorManager.Instance.CheckButtonsState(); // V�rifie l'�tat des boutons
    }

    public bool IsPressed()
    {
        return isPressed.Value;
    }
}
