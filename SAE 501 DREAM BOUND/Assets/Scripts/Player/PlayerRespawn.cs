using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using System.Collections.Generic;

namespace TomAg
{
    public class PlayerRespawn : NetworkBehaviour
    {
        private Dictionary<ulong, (Vector3 position, Quaternion rotation)> spawnPoints = new Dictionary<ulong, (Vector3, Quaternion)>();

        // Handles player respawn when entering a trigger zone
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

        // Stocke les points de spawn pour chaque joueur
        private void StoreSpawnPoints()
        {
            spawnPoints.Clear();
            foreach (var spawnedObject in Unity.Netcode.NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
            {
                var playerInfo = spawnedObject.GetComponent<PlayerInfo>();
                if (playerInfo != null && playerInfo.SpawnPoint != null)
                {
                    spawnPoints[spawnedObject.NetworkObjectId] = (
                        playerInfo.SpawnPoint.position,
                        playerInfo.SpawnPoint.rotation
                    );
                    Debug.Log($"Stored spawn point for player {spawnedObject.NetworkObjectId}: Position {playerInfo.SpawnPoint.position}");
                }
            }
        }

        // Forces all players to respawn
        public void ForceRespawnAllPlayers()
        {
            if (!IsServer)
            {
                ForceRespawnRequestServerRpc();
                return;
            }
            ForceRespawnAllPlayersServerRpc();
        }

        // Requests server to force respawn all players
        [ServerRpc(RequireOwnership = false)]
        private void ForceRespawnRequestServerRpc()
        {
            ForceRespawnAllPlayersServerRpc();
        }

        // Respawns all players on the server
        [ServerRpc(RequireOwnership = false)]
        private void ForceRespawnAllPlayersServerRpc()
        {
            // Stocke d'abord les points de spawn
            StoreSpawnPoints();

            // Vérifie que nous avons bien les points de spawn pour tous les joueurs
            Debug.Log($"Number of stored spawn points: {spawnPoints.Count}");

            foreach (var spawnedObject in Unity.Netcode.NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
            {
                if (spawnPoints.TryGetValue(spawnedObject.NetworkObjectId, out var spawnPoint))
                {
                    Debug.Log($"Respawning player {spawnedObject.NetworkObjectId} at stored position: {spawnPoint.position}");
                    TeleportToSpawnPointServerRpc(
                        spawnedObject.NetworkObjectId,
                        spawnPoint.position,
                        spawnPoint.rotation
                    );
                }
                else
                {
                    Debug.LogWarning($"No stored spawn point found for player {spawnedObject.NetworkObjectId}");
                }
            }
        }

        // Teleports a player to their spawn point on the server
        [ServerRpc(RequireOwnership = false)]
        public void TeleportToSpawnPointServerRpc(ulong networkObjectId, Vector3 position, Quaternion rotation)
        {
            if (Unity.Netcode.NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                if (networkObject.TryGetComponent<NetworkTransform>(out var networkTransform))
                {
                    networkTransform.transform.SetPositionAndRotation(position, rotation);
                }
                else
                {
                    Debug.LogWarning("NetworkTransform not found on player");
                }
            }
            TeleportPlayerClientRpc(networkObjectId, position, rotation);
        }

        // Teleports a player to their spawn point on all clients
        [ClientRpc]
        private void TeleportPlayerClientRpc(ulong networkObjectId, Vector3 position, Quaternion rotation)
        {
            if (Unity.Netcode.NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                if (networkObject.TryGetComponent<NetworkTransform>(out var networkTransform))
                {
                    Debug.Log($"Teleporting player to Position: {position}, Rotation: {rotation}");
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