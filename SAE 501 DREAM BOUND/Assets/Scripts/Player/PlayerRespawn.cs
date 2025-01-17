using Unity.Netcode;
using UnityEngine;

namespace TomAg
{
    public class PlayerRespawn : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

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

        [ServerRpc(RequireOwnership = false)]
        private void TeleportToSpawnPointServerRpc(ulong networkObjectId, Vector3 position, Quaternion rotation)
        {
            TeleportPlayerClientRpc(networkObjectId, position, rotation);
        }

        [ClientRpc]
        private void TeleportPlayerClientRpc(ulong networkObjectId, Vector3 position, Quaternion rotation)
        {
            if (Unity.Netcode.NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
            {
                if (networkObject.IsOwner)
                {
                    Transform playerTransform = networkObject.transform;
                    Debug.Log($"Teleporting player to Position: {position}, Rotation: {rotation}");
                    playerTransform.SetPositionAndRotation(position, rotation);
                }
            }
            else
            {
                Debug.LogWarning($"Failed to find NetworkObject with ID {networkObjectId}");
            }
        }
    }
}
