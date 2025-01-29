using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace TomAg
{
    public class PlayerRespawn : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<NetworkObject>(out var networkObject))
            {
                var playerInfo = other.GetComponent<PlayerInfo>();
                if (playerInfo != null)
                {
                    TeleportToSpawnPointServerRpc(networkObject.NetworkObjectId, playerInfo.SpawnPoint.position, playerInfo.SpawnPoint.rotation);
                }
                else
                {
                    Debug.LogWarning("PlayerInfo component not found on the player object.");
                }
            }
            else
            {
                Debug.LogWarning("NetworkObject component not found on the player object.");
            }
        }

        public void ForceRespawnAllPlayers()
        {
            if (!IsServer)
            {
                ForceRespawnRequestServerRpc();
                return;
            }
            ForceRespawnAllPlayersServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void ForceRespawnRequestServerRpc()
        {
            ForceRespawnAllPlayersServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void ForceRespawnAllPlayersServerRpc()
        {
            foreach (var spawnedObject in Unity.Netcode.NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
            {
                var playerInfo = spawnedObject.GetComponent<PlayerInfo>();
                if (playerInfo != null && playerInfo.SpawnPoint != null)
                {
                    Debug.Log($"Found player with NetworkObjectId: {spawnedObject.NetworkObjectId}");
                    TeleportToSpawnPointServerRpc(
                        spawnedObject.NetworkObjectId,
                        playerInfo.SpawnPoint.position,
                        playerInfo.SpawnPoint.rotation
                    );
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void TeleportToSpawnPointServerRpc(ulong networkObjectId, Vector3 position, Quaternion rotation)
        {
            if (Unity.Netcode.NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                // Mise à jour de la position sur le serveur
                if (networkObject.TryGetComponent<NetworkTransform>(out var networkTransform))
                {
                    // Force la position sur le NetworkTransform
                    networkTransform.transform.SetPositionAndRotation(position, rotation);
                }
                else
                {
                    Debug.LogWarning("NetworkTransform not found on player");
                }
            }

            // Notifier tous les clients
            TeleportPlayerClientRpc(networkObjectId, position, rotation);
        }

        [ClientRpc]
        private void TeleportPlayerClientRpc(ulong networkObjectId, Vector3 position, Quaternion rotation)
        {
            if (Unity.Netcode.NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                if (networkObject.TryGetComponent<NetworkTransform>(out var networkTransform))
                {
                    Debug.Log($"Teleporting player to Position: {position}, Rotation: {rotation}");
                    // Force la position sur le client aussi
                    networkTransform.transform.SetPositionAndRotation(position, rotation);
                }
                else
                {
                    Debug.LogWarning("NetworkTransform not found on player");
                }
            }
            else
            {
                Debug.LogWarning($"Failed to find NetworkObject with ID {networkObjectId}");
            }
        }

    }
}
