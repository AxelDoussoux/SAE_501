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
                Debug.Log("Vouse ne pouvez pas ramasser cette item !");
                return;
            }
            if (playerInfo == null)
            {
                Debug.LogError("PlayerInfo is null. Cannot take hammer.");
                return;
            }

            playerInfo.EnabledHammer();
            
            Debug.Log("Player has taken the hammer.");
            Destroy(gameObject); // Détruit l'objet une fois récupéré
        }
    }
}