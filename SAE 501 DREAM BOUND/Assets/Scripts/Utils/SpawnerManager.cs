using Unity.Netcode;
using UnityEngine;

namespace TomAg
{
    public class SpawnerManager : NetworkBehaviour
    {
        [SerializeField] private GameObject player1Prefab;
        [SerializeField] private GameObject player2Prefab;

        private void Awake()
        {
            if (player1Prefab != null)
            {
                var info1 = player1Prefab.GetComponent<PlayerInfo>();
                if (info1 == null || info1.SpawnPoint == null)
                {
                    Debug.LogError("Player1 prefab missing PlayerInfo or SpawnPoint!");
                }
            }

            if (player2Prefab != null)
            {
                var info2 = player2Prefab.GetComponent<PlayerInfo>();
                if (info2 == null || info2.SpawnPoint == null)
                {
                    Debug.LogError("Player2 prefab missing PlayerInfo or SpawnPoint!");
                }
            }
        }

        [ClientRpc]
        private void AssignPlayerClientRpc(ulong clientId, NetworkObjectReference playerRef)
        {
            if (Unity.Netcode.NetworkManager.Singleton.LocalClientId == clientId)
            {
                if (playerRef.TryGet(out NetworkObject playerObject))
                {
                    Camera playerCamera = playerObject.GetComponentInChildren<Camera>();
                    if (playerCamera != null)
                    {
                        Camera.main?.gameObject.SetActive(false);
                        playerCamera.gameObject.SetActive(true);
                        Debug.Log($"Camera activated for client {clientId} at position {playerObject.transform.position}");
                    }
                }
            }
            else
            {
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

        private void SpawnPlayerForClient(ulong clientId, GameObject playerPrefab, Transform spawnPoint)
        {
            if (playerPrefab == null || spawnPoint == null)
            {
                Debug.LogError($"Missing prefab or spawnpoint for client {clientId}");
                return;
            }

            Debug.Log($"Attempting to spawn player for client {clientId} at position {spawnPoint.position}");

            GameObject playerInstance = Instantiate(
                playerPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            NetworkObject netObj = playerInstance.GetComponent<NetworkObject>();
            if (netObj == null)
            {
                Debug.LogError($"NetworkObject component missing on player instance for client {clientId}");
                Destroy(playerInstance);
                return;
            }

            netObj.SpawnWithOwnership(clientId);
            AssignPlayerClientRpc(clientId, netObj);

            Debug.Log($"Successfully spawned player for client {clientId} at position {playerInstance.transform.position}");
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        private void OnClientConnected(ulong clientId)
        {
            if (!IsServer) return;

            if (clientId == Unity.Netcode.NetworkManager.ServerClientId)
            {
                var playerInfo = player1Prefab.GetComponent<PlayerInfo>();
                if (playerInfo?.SpawnPoint != null)
                {
                    SpawnPlayerForClient(clientId, player1Prefab, playerInfo.SpawnPoint);
                }
                else
                {
                    Debug.LogError("Player1 spawn configuration is invalid!");
                }
            }
            else
            {
                var playerInfo = player2Prefab.GetComponent<PlayerInfo>();
                if (playerInfo?.SpawnPoint != null)
                {
                    SpawnPlayerForClient(clientId, player2Prefab, playerInfo.SpawnPoint);
                }
                else
                {
                    Debug.LogError("Player2 spawn configuration is invalid!");
                }
            }
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