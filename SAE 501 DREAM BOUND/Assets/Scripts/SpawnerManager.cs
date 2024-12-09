using Unity.Netcode;
using UnityEngine;

namespace TomAg
{
    public class SpawnerManager : NetworkBehaviour
    {
        [SerializeField] private GameObject player1Prefab;
        [SerializeField] private GameObject player2Prefab;

        private PlayerInfo playerInfo1;
        private PlayerInfo playerInfo2;

        [ClientRpc]
        private void AssignPlayerClientRpc(ulong clientId, NetworkObjectReference playerRef)
        {
            if (Unity.Netcode.NetworkManager.Singleton.LocalClientId == clientId)
            {
                if (playerRef.TryGet(out NetworkObject playerObject))
                {
                    // Activer la caméra du joueur local uniquement
                    Camera playerCamera = playerObject.GetComponentInChildren<Camera>();
                    if (playerCamera != null)
                    {
                        // Désactiver la caméra principale si elle existe
                        Camera.main?.gameObject.SetActive(false);

                        // Activer la caméra du joueur
                        playerCamera.gameObject.SetActive(true);
                    }

                    Debug.Log($"Client {clientId} assigned to player {playerObject.gameObject.name} with camera activated");
                }
            }
            else
            {
                // Désactiver la caméra des autres joueurs
                if (playerRef.TryGet(out NetworkObject playerObject))
                {
                    Camera playerCamera = playerObject.GetComponentInChildren<Camera>();
                    if (playerCamera != null)
                    {
                        playerCamera.gameObject.SetActive(false);
                    }
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        private void OnClientConnected(ulong clientId)
        {
            if (!IsServer) return;

            if (clientId == GetServerClientId())
            {
                playerInfo1 = player1Prefab.GetComponent<PlayerInfo>();
                SpawnPlayerForClient(clientId, player1Prefab, playerInfo1.SpawnPoint);
            }
            else
            {
                playerInfo2 = player2Prefab.GetComponent<PlayerInfo>();
                SpawnPlayerForClient(clientId, player2Prefab, playerInfo2.SpawnPoint);
            }
        }

        private static ulong GetServerClientId()
        {
            return Unity.Netcode.NetworkManager.ServerClientId;
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
                Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }
    }
}