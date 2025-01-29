using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : NetworkBehaviour
{
    [SerializeField] private string endingSceneName = "Ending";

    private void OnTriggerEnter(Collider other)
    {
        // Vérifie si l'objet qui entre est bien un joueur
        if (other.TryGetComponent<NetworkObject>(out NetworkObject playerNetObj))
        {
            // Demande au host de déclencher la fin du jeu
            NotifyHostCollisionServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)] // Permet à n'importe quel client d'appeler ce ServerRpc
    private void NotifyHostCollisionServerRpc()
    {
        // Vérifie si on est le host avant de déclencher la fin
        if (IsHost)
        {
            // Déclenche la séquence de fin pour tous les clients
            TriggerEndingSequenceClientRpc();
        }
    }

    [ClientRpc]
    private void TriggerEndingSequenceClientRpc()
    {
        // Arrête le NetworkManager
        Unity.Netcode.NetworkManager.Singleton.Shutdown();

        // Charge directement la scène de fin
        SceneManager.LoadScene(endingSceneName);
    }
}
