using System.Collections;
using System.Collections.Generic;
using TomAg;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{

    public PlayerController playerController;

    // Called when the object is spawned on the network
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Enables the player controller only for the owner of this object
        playerController.enabled = IsOwner;
    }
}
