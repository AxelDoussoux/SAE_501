using UnityEngine;
using Unity.Netcode;

public class VisibilityManager : NetworkBehaviour
{
    private NetworkObject networkObject;

    private void Awake()
    {
        networkObject = GetComponent<NetworkObject>();

        if (networkObject == null)
        {
            Debug.LogError("[VisibilityManager] NetworkObject manquant !");
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
            Debug.Log($"[VisibilityManager] Nouveau client connecté: {clientId}");
            HandleClientVisibility(clientId);
        }
    }

    private void HandleClientVisibility(ulong clientId)
    {
        Debug.Log($"[VisibilityManager] Gestion de la visibilité pour le client: {clientId}");

        if (clientId == 1) // Joueur 2
        {
            HideObjectForClientRpc(clientId);
            Debug.Log("[VisibilityManager] Objet caché pour le joueur 2.");
        }
        else // Joueur 1
        {
            ShowObjectForClientRpc(clientId);
            Debug.Log("[VisibilityManager] Objet montré pour le joueur 1.");
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
