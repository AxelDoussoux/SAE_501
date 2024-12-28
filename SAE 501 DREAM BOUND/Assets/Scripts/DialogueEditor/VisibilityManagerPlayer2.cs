using UnityEngine;
using Unity.Netcode;

public class VisibilityManagerForPlayer2 : NetworkBehaviour
{
    private NetworkObject networkObject;

    private void Awake()
    {
        networkObject = GetComponent<NetworkObject>();

        // V�rifier si le NetworkObject existe
        if (networkObject == null)
        {
            Debug.LogError("[VisibilityManagerForPlayer2] NetworkObject manquant !");
            return;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Si le serveur g�re la connexion des clients
        if (IsServer)
        {
            // Abonnement � l'�v�nement de connexion des clients
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            // G�rer les clients d�j� connect�s
            foreach (ulong clientId in Unity.Netcode.NetworkManager.Singleton.ConnectedClientsIds)
            {
                HandleClientVisibility(clientId);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        // D�sabonnement de l'�v�nement de connexion des clients
        if (IsServer)
        {
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // G�rer la visibilit� de l'objet pour chaque client connect�
        if (IsServer)
        {
            Debug.Log($"[VisibilityManagerForPlayer2] Nouveau client connect�: {clientId}");
            HandleClientVisibility(clientId);
        }
    }

    private void HandleClientVisibility(ulong clientId)
    {
        Debug.Log($"[VisibilityManagerForPlayer2] Gestion de la visibilit� pour le client: {clientId}");

        // Si le client est le joueur 1 (clientId == 0), on cache l'objet
        if (clientId == 0)
        {
            HideObjectClientRpc();
            Debug.Log("[VisibilityManagerForPlayer2] Objet cach� pour le joueur 1.");
        }
        else // Si le client est le joueur 2, on le montre
        {
            ShowObjectClientRpc();
            Debug.Log("[VisibilityManagerForPlayer2] Objet montr� pour le joueur 2.");
        }
    }

    [ClientRpc]
    private void HideObjectClientRpc()
    {
        // Cache l'objet pour ce client
        networkObject.gameObject.SetActive(false);
    }

    [ClientRpc]
    private void ShowObjectClientRpc()
    {
        // Montre l'objet pour ce client
        networkObject.gameObject.SetActive(true);
    }
}
