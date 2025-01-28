using UnityEngine;
using Unity.Netcode;

namespace TomAg
{
    public class TakeHammer : NetworkBehaviour, IInteractable
    {
        public void Interact(PlayerInfo playerInfo)
        {
            if (!playerInfo.canTakeHammer)
            {
                Debug.Log("Vous ne pouvez pas ramasser cet item !");
                return;
            }
            if (playerInfo == null)
            {
                Debug.LogError("PlayerInfo is null. Cannot take hammer.");
                return;
            }

            // Active le marteau localement pour le joueur
            playerInfo.EnabledHammer();

            // Active le marteau pour tous les joueurs
            EnableHammerOnAllClientsServerRpc(playerInfo.OwnerClientId);

            Debug.Log("Player has taken the hammer.");
            DestroyHammerOnAllClientsServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void DestroyHammerOnAllClientsServerRpc()
        {
            // Détruit l'objet sur tous les clients
            Destroy(gameObject);
            NotifyDestructionOnAllClientsClientRpc();
        }

        [ClientRpc]
        private void NotifyDestructionOnAllClientsClientRpc()
        {
            if (IsOwner) return;
            Destroy(gameObject);
        }

        // Active le marteau pour tous les joueurs via une ClientRpc
        [ServerRpc(RequireOwnership = false)]
        private void EnableHammerOnAllClientsServerRpc(ulong playerId)
        {
            EnableHammerOnAllClientsClientRpc(playerId);
        }

        [ClientRpc]
        private void EnableHammerOnAllClientsClientRpc(ulong playerId)
        {
            // Trouve le joueur correspondant à l'ID
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
