using TomAg;
using UnityEngine;
using Unity.Netcode;

public class BreakableObject : NetworkBehaviour, IInteractable
{
    public void Interact(PlayerInfo playerInfo)
    {
        if (playerInfo.HaveHammer)
        {
            Debug.Log($"{gameObject.name} a �t� d�truit !");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"{gameObject.name} n'a pas �t� d�truit ! Il vous manque le marteau...");
        }

    }
}
