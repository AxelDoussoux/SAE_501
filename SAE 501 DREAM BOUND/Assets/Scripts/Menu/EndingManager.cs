using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : NetworkBehaviour
{
    [SerializeField] private string endingSceneName = "Ending"; // Scene to load after the game ends

    // Called when another object enters the collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NetworkObject>(out NetworkObject playerNetObj)) // Check if the object is a player
        {
            NotifyHostCollisionServerRpc(); // Notify the host about the collision
        }
    }

    // ServerRpc to notify the host that a player has collided with the trigger
    [ServerRpc(RequireOwnership = false)]
    private void NotifyHostCollisionServerRpc()
    {
        if (IsHost) // Ensure this only runs on the host
        {
            // Notify all clients to prepare for the ending sequence
            PrepareForEndingClientRpc();
        }
    }

    // ClientRpc to prepare clients for the ending sequence
    [ClientRpc]
    private void PrepareForEndingClientRpc()
    {
        EndingSequence(); // Run the ending sequence on the client
    }

    // Handles the ending sequence
    private void EndingSequence()
    {
        // First, notify all clients that the game is ending
        if (IsHost) // Only the host can disconnect the local client
        {
            Unity.Netcode.NetworkManager.Singleton.DisconnectClient(Unity.Netcode.NetworkManager.Singleton.LocalClientId);
        }

        // Shutdown the network connection
        Unity.Netcode.NetworkManager.Singleton.Shutdown();

        // Finally, load the ending scene
        SceneManager.LoadScene(endingSceneName);
    }
}
