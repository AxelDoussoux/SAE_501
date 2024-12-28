using UnityEngine;
using Unity.Netcode;

public class VisibilityManagerForPlayer2 : NetworkBehaviour
{
    private NetworkObject networkObject;

    private void Awake()
    {
        networkObject = GetComponent<NetworkObject>();

        // Vérifier si le NetworkObject existe
        if (networkObject == null)
        {
            Debug.LogError("[VisibilityManagerForPlayer2] NetworkObject manquant !");
            return;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Si le serveur gère la connexion des clients
        if (IsServer)
        {
            // Abonnement à l'événement de connexion des clients
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            // Gérer les clients déjà connectés
            foreach (ulong clientId in Unity.Netcode.NetworkManager.Singleton.ConnectedClientsIds)
            {
                HandleClientVisibility(clientId);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        // Désabonnement de l'événement de connexion des clients
        if (IsServer)
        {
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // Gérer la visibilité de l'objet pour chaque client connecté
        if (IsServer)
        {
            Debug.Log($"[VisibilityManagerForPlayer2] Nouveau client connecté: {clientId}");
            HandleClientVisibility(clientId);
        }
    }

    private void HandleClientVisibility(ulong clientId)
    {
        Debug.Log($"[VisibilityManagerForPlayer2] Gestion de la visibilité pour le client: {clientId}");

        // Si le client est le joueur 1 (clientId == 0), on cache l'objet
        if (clientId == 0)
        {
            HideObjectClientRpc();
            Debug.Log("[VisibilityManagerForPlayer2] Objet caché pour le joueur 1.");
        }
        else // Si le client est le joueur 2, on le montre
        {
            ShowObjectClientRpc();
            Debug.Log("[VisibilityManagerForPlayer2] Objet montré pour le joueur 2.");
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
