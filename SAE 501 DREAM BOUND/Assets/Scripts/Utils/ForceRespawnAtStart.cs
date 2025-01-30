using Unity.Netcode;
using UnityEngine;

namespace TomAg
{
    public class ForceRespawnAtStart : NetworkBehaviour
    {
        private PlayerRespawn playerRespawn;
        private bool isPlayer2Connected = false;
        
        [SerializeField] private float respawnDelay = 1f; // Délai pour s'assurer que les deux joueurs sont bien spawn

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            playerRespawn = GetComponent<PlayerRespawn>();

            if (playerRespawn == null)
            {
                Debug.LogError("PlayerRespawn component not assigned!");
                return;
            }

            Unity.Netcode.NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        private void OnClientConnected(ulong clientId)
        {
            if (!IsServer) return;

            // Si ce n'est pas le serveur (Player 1) qui se connecte
            if (clientId != Unity.Netcode.NetworkManager.ServerClientId)
            {
                isPlayer2Connected = true;
                Debug.Log("Player 2 connected - Initiating force respawn sequence");

                // Attend un court délai pour s'assurer que les deux joueurs sont spawn
                StartCoroutine(ForceRespawnWithDelay());
            }
        }

        private System.Collections.IEnumerator ForceRespawnWithDelay()
        {
            yield return new WaitForSeconds(respawnDelay);

            if (isPlayer2Connected)
            {
                Debug.Log("Forcing respawn for all players");
                playerRespawn.ForceRespawnAllPlayers();
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