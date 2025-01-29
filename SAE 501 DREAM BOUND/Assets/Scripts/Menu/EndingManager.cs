using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : NetworkBehaviour
{
    [SerializeField] private string endingSceneName = "Ending";

    private void OnTriggerEnter(Collider other)
    {
        // V�rifie si l'objet qui entre est bien un joueur
        if (other.TryGetComponent<NetworkObject>(out NetworkObject playerNetObj))
        {
            // Demande au host de d�clencher la fin du jeu
            NotifyHostCollisionServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)] // Permet � n'importe quel client d'appeler ce ServerRpc
    private void NotifyHostCollisionServerRpc()
    {
        // V�rifie si on est le host avant de d�clencher la fin
        if (IsHost)
        {
            // D�clenche la s�quence de fin pour tous les clients
            TriggerEndingSequenceClientRpc();
        }
    }

    [ClientRpc]
    private void TriggerEndingSequenceClientRpc()
    {
        // Arr�te le NetworkManager
        Unity.Netcode.NetworkManager.Singleton.Shutdown();

        // Charge directement la sc�ne de fin
        SceneManager.LoadScene(endingSceneName);
    }
}
