using Unity.Netcode;
using UnityEngine;

namespace TomAg
{
    public class TeleportPlayer : NetworkBehaviour
    {
        [SerializeField] private PlayerRespawn playerRespawn;
        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return; // Seulement exécuter sur le serveur

            if (other.TryGetComponent<NetworkObject>(out var networkObject))
            {
                // Vérifier si l'objet a le composant PlayerInfo
                PlayerInfo playerInfo = other.GetComponent<PlayerInfo>();
                if (playerInfo != null && playerInfo.SpawnPoint != null)
                {
                    if (playerRespawn != null)
                    {
                        playerRespawn.TeleportToSpawnPointServerRpc(
                            networkObject.NetworkObjectId,
                            playerInfo.SpawnPoint.position,
                            playerInfo.SpawnPoint.rotation
                        );
                    }
                    else
                    {
                        Debug.LogWarning("PlayerRespawn component not found on the player object.");
                    }
                }
                else
                {
                    Debug.LogWarning("PlayerInfo component not found or SpawnPoint is not set.");
                }
            }
        }
    }
}