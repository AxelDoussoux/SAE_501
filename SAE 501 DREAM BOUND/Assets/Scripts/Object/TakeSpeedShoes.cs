using UnityEngine;
using Unity.Netcode;

namespace TomAg
{
    public class TakeSpeedShoes : NetworkBehaviour, IInteractable
    {
        // Handles the interaction of a player with the speed shoes item
        public void Interact(PlayerInfo playerInfo)
        {
            // If the player cannot take the speed shoes, log and return
            if (!playerInfo.canTakeSpeedShoes)
            {
                Debug.Log("You cannot take this item!");
                return;
            }
            if (playerInfo == null)
            {
                Debug.LogError("PlayerInfo is null. Cannot take speed shoes.");
                return;
            }

            // Enable the speed shoes locally for the player
            playerInfo.EnabledSpeedShoes();

            // Enable the speed shoes for all players on the network
            EnableSpeedShoesOnAllClientsServerRpc(playerInfo.OwnerClientId);

            Debug.Log("Player has taken the speed shoes.");
            DestroySpeedShoesOnAllClientsServerRpc();
        }

        // ServerRpc to destroy the speed shoes for all clients
        [ServerRpc(RequireOwnership = false)]
        private void DestroySpeedShoesOnAllClientsServerRpc()
        {
            // Destroy the speed shoes object on all clients
            Destroy(gameObject);
            NotifyDestructionOnAllClientsClientRpc();
        }

        // ClientRpc to notify all clients about the destruction of the speed shoes
        [ClientRpc]
        private void NotifyDestructionOnAllClientsClientRpc()
        {
            if (IsOwner) return;
            Destroy(gameObject);
        }

        // ServerRpc to enable speed shoes on all clients
        [ServerRpc(RequireOwnership = false)]
        private void EnableSpeedShoesOnAllClientsServerRpc(ulong playerId)
        {
            EnableSpeedShoesOnAllClientsClientRpc(playerId);
        }

        // ClientRpc to enable speed shoes for the specified player on all clients
        [ClientRpc]
        private void EnableSpeedShoesOnAllClientsClientRpc(ulong playerId)
        {
            // Find the player by their network ID and enable speed shoes
            foreach (var networkObject in FindObjectsOfType<PlayerInfo>())
            {
                if (networkObject.OwnerClientId == playerId)
                {
                    networkObject.EnabledSpeedShoes();
                }
            }
        }
    }
}
