using UnityEngine;
using Unity.Netcode;

public class VisibilityManager : NetworkBehaviour
{
    private NetworkObject networkObject;

    private void Awake()
    {
        networkObject = GetComponent<NetworkObject>();

        // S'assurer que le NetworkObject existe
        if (networkObject == null)
        {
            Debug.LogError("[VisibilityManager] NetworkObject manquant!");
            return;
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Si nous sommes le serveur, g�rer la visibilit� pour chaque client
        if (IsServer)
        {
            // S'abonner � l'�v�nement de connexion des clients
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

        // Si nous sommes le serveur, se d�sabonner de l'�v�nement de connexion des clients
        if (IsServer)
        {
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // G�rer la visibilit� du client qui vient de se connecter
        if (IsServer)
        {
            Debug.Log($"[VisibilityManager] Nouveau client connect�: {clientId}");
            HandleClientVisibility(clientId);
        }
    }

    private void HandleClientVisibility(ulong clientId)
    {
        Debug.Log($"[VisibilityManager] Gestion de la visibilit� pour le client: {clientId}");

        // Ici on d�cide de cacher ou montrer l'objet en fonction de l'ID du client
        if (clientId == 1) // Par exemple, si c'est le joueur 2
        {
            // Cache l'objet pour le client 1 (joueur 2)
            HideObjectClientRpc();
            Debug.Log($"[VisibilityManager] Objet cach� pour le client {clientId}");
        }
        else
        {
            // Sinon, montre l'objet pour tous les autres clients (par exemple le joueur 1)
            ShowObjectClientRpc();
            Debug.Log($"[VisibilityManager] Objet montr� pour le client {clientId}");
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
