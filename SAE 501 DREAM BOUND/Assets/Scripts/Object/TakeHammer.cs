using UnityEngine;
using Unity.Netcode;

namespace TomAg
{
    public class TakeHammer : NetworkBehaviour, IInteractable
    {
        // Handles the interaction of a player with the hammer item
        public void Interact(PlayerInfo playerInfo)
        {
            // If the player cannot take the hammer, log and return
            if (!playerInfo.canTakeHammer)
            {
                Debug.Log("You cannot take this item!");
                return;
            }
            if (playerInfo == null)
            {
                Debug.LogError("PlayerInfo is null. Cannot take hammer.");
                return;
            }

            // Enable the hammer locally for the player
            playerInfo.EnabledHammer();

            // Enable the hammer for all players on the network
            EnableHammerOnAllClientsServerRpc(playerInfo.OwnerClientId);

            Debug.Log("Player has taken the hammer.");
            DestroyHammerOnAllClientsServerRpc();
        }

        // ServerRpc to destroy the hammer for all clients
        [ServerRpc(RequireOwnership = false)]
        private void DestroyHammerOnAllClientsServerRpc()
        {
            // Destroy the hammer object on all clients
            Destroy(gameObject);
            NotifyDestructionOnAllClientsClientRpc();
        }

        // ClientRpc to notify all clients about the destruction of the hammer
        [ClientRpc]
        private void NotifyDestructionOnAllClientsClientRpc()
        {
            if (IsOwner) return;
            Destroy(gameObject);
        }

        // ServerRpc to enable hammer on all clients
        [ServerRpc(RequireOwnership = false)]
        private void EnableHammerOnAllClientsServerRpc(ulong playerId)
        {
            EnableHammerOnAllClientsClientRpc(playerId);
        }

        // ClientRpc to enable hammer for the specified player on all clients
        [ClientRpc]
        private void EnableHammerOnAllClientsClientRpc(ulong playerId)
        {
            // Find the player by their network ID and enable the hammer
            foreach (var networkObject in FindObjectsOfType<PlayerInfo>())
            {
                if (networkObject.OwnerClientId == playerId)
                {
                    networkObject.EnabledHammer();
                }
            }
        }
    }
}
