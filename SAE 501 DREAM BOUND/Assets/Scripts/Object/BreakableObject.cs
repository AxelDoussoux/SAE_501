using TomAg;
using UnityEngine;
using Unity.Netcode;

public class BreakableObject : NetworkBehaviour, IInteractable
{
    public void Interact(PlayerInfo playerInfo)
    {
        if (playerInfo.HaveHammer)
        {
            Debug.Log($"{gameObject.name} a été détruit !");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"{gameObject.name} n'a pas été détruit ! Il vous manque le marteau...");
        }

    }
}
