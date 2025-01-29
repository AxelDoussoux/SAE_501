using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : NetworkBehaviour
{
    [SerializeField] private string endingSceneName = "Ending";

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NetworkObject>(out NetworkObject playerNetObj))
        {
            NotifyHostCollisionServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyHostCollisionServerRpc()
    {
        if (IsHost)
        {
            // Notify all clients to prepare for disconnection
            PrepareForEndingClientRpc();
        }
    }

    [ClientRpc]
    private void PrepareForEndingClientRpc()
    {
        EndingSequence();
    }

    private void EndingSequence()
    {
        // First, notify all clients that we're about to end
        if (IsHost)
        {
            Unity.Netcode.NetworkManager.Singleton.DisconnectClient(Unity.Netcode.NetworkManager.Singleton.LocalClientId);
        }

        // Now safely shutdown the network
        Unity.Netcode.NetworkManager.Singleton.Shutdown();

        // Finally load the ending scene
        SceneManager.LoadScene(endingSceneName);
    }
}