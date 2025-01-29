using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class NetworkEndingManager : NetworkBehaviour
{
    [SerializeField] private string endingSceneName = "EndingScene";
    [SerializeField] private float transitionDelay = 1f;

    private void OnTriggerEnter(Collider other)
    {
        // Check if this is running on the server
        if (!IsServer) return;

        // Check if the colliding object is a player
        if (other.TryGetComponent<NetworkObject>(out NetworkObject playerNetObj))
        {
            // Trigger the ending sequence for all clients
            TriggerEndingSequenceServerRpc();
        }
    }

    [ServerRpc]
    private void TriggerEndingSequenceServerRpc()
    {
        // Call the client RPC to handle the transition on all clients
        TriggerEndingSequenceClientRpc();
    }

    [ClientRpc]
    private void TriggerEndingSequenceClientRpc()
    {
        // Start the transition coroutine
        StartCoroutine(TransitionToEndingScene());
    }

    private System.Collections.IEnumerator TransitionToEndingScene()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(transitionDelay);

        // Shutdown the network manager
        Unity.Netcode.NetworkManager.Singleton.Shutdown();

        // Load the ending scene
        SceneManager.LoadScene(endingSceneName);
    }
}