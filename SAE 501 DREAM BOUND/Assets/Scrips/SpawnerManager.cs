using Unity.Netcode;
using UnityEngine;

public class SpawnerManager : NetworkBehaviour
{
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    [SerializeField] private Transform spawn1Position;
    [SerializeField] private Transform spawn2Position;

    // ClientRpc pour informer les clients de leur joueur assigné
    [ClientRpc]
    private void AssignPlayerClientRpc(ulong clientId, NetworkObjectReference playerRef)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            if (playerRef.TryGet(out NetworkObject playerObject))
            {
                Debug.Log($"Client {clientId} assigned to player {playerObject.gameObject.name}");
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        // Premier joueur connecté
        if (clientId == NetworkManager.Singleton.ServerClientId)
        {
            SpawnPlayerForClient(clientId, player1Prefab, spawn1Position);
        }
        // Deuxième joueur connecté
        else
        {
            SpawnPlayerForClient(clientId, player2Prefab, spawn2Position);
        }
    }

    private void SpawnPlayerForClient(ulong clientId, GameObject playerPrefab, Transform spawnPoint)
    {
        GameObject playerInstance = Instantiate(
            playerPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        NetworkObject netObj = playerInstance.GetComponent<NetworkObject>();
        netObj.SpawnWithOwnership(clientId);

        AssignPlayerClientRpc(clientId, netObj);

        Debug.Log($"Spawned player for client {clientId}");
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
}