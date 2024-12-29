using UnityEngine;
using Unity.Netcode;

public class VisibilityManagerPlayer2 : NetworkBehaviour
{
    private NetworkObject networkObject;

    private void Awake()
    {
        networkObject = GetComponent<NetworkObject>();

        if (networkObject == null)
        {
            Debug.LogError("[VisibilityManagerPlayer2] NetworkObject manquant !");
            return;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            foreach (ulong clientId in Unity.Netcode.NetworkManager.Singleton.ConnectedClientsIds)
            {
                HandleClientVisibility(clientId);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsServer)
        {
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            Debug.Log($"[VisibilityManagerPlayer2] Nouveau client connecté: {clientId}");
            HandleClientVisibility(clientId);
        }
    }

    private void HandleClientVisibility(ulong clientId)
    {
        Debug.Log($"[VisibilityManagerPlayer2] Gestion de la visibilité pour le client: {clientId}");

        if (clientId == 0) // Joueur 1
        {
            HideObjectForClientRpc(clientId);
            Debug.Log("[VisibilityManagerPlayer2] Objet caché pour le joueur 1.");
        }
        else // Joueur 2
        {
            ShowObjectForClientRpc(clientId);
            Debug.Log("[VisibilityManagerPlayer2] Objet montré pour le joueur 2.");
        }
    }

    [ClientRpc]
    private void HideObjectForClientRpc(ulong targetClientId)
    {
        if (Unity.Netcode.NetworkManager.Singleton.LocalClientId == targetClientId)
        {
            networkObject.gameObject.SetActive(false);
        }
    }

    [ClientRpc]
    private void ShowObjectForClientRpc(ulong targetClientId)
    {
        if (Unity.Netcode.NetworkManager.Singleton.LocalClientId == targetClientId)
        {
            networkObject.gameObject.SetActive(true);
        }
    }
}
