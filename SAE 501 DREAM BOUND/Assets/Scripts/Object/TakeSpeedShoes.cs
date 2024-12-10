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
                Debug.Log("Vouse ne pouvez pas ramasser cette item !");
                return;
            }
            if (playerInfo == null)
            {
                Debug.LogError("PlayerInfo is null. Cannot take speed shoes.");
                return;
            }

            playerInfo.EnabledSpeedShoes();

            Debug.Log("Player has taken the speed shoes.");
            Destroy(gameObject); // D�truit l'objet une fois r�cup�r�
        }
    }
}