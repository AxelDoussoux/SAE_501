using UnityEngine;
using Unity.Netcode;

namespace TomAg
{
    public class TakeSpeedShoes : NetworkBehaviour, IInteractable
    {
        public void Interact(PlayerInfo playerInfo)
        {
            if (!playerInfo.canTakeSpeedShoes)
            {
                Debug.Log("Vous ne pouvez pas ramasser cet item !");
                return;
            }
            if (playerInfo == null)
            {
                Debug.LogError("PlayerInfo is null. Cannot take speed shoes.");
                return;
            }

            // Active les chaussures de vitesse localement pour le joueur
            playerInfo.EnabledSpeedShoes();

            // Active les chaussures de vitesse pour tous les joueurs
            EnableSpeedShoesOnAllClientsServerRpc(playerInfo.OwnerClientId);

            Debug.Log("Player has taken the speed shoes.");
            DestroySpeedShoesOnAllClientsServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void DestroySpeedShoesOnAllClientsServerRpc()
        {
            // Détruit les chaussures sur tous les clients
            Destroy(gameObject);
            NotifyDestructionOnAllClientsClientRpc();
        }

        [ClientRpc]
        private void NotifyDestructionOnAllClientsClientRpc()
        {
            if (IsOwner) return;
            Destroy(gameObject);
        }

        // Active les chaussures de vitesse pour tous les joueurs via une ClientRpc
        [ServerRpc(RequireOwnership = false)]
        private void EnableSpeedShoesOnAllClientsServerRpc(ulong playerId)
        {
            EnableSpeedShoesOnAllClientsClientRpc(playerId);
        }

        [ClientRpc]
        private void EnableSpeedShoesOnAllClientsClientRpc(ulong playerId)
        {
            // Trouve le joueur correspondant à l'ID
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
