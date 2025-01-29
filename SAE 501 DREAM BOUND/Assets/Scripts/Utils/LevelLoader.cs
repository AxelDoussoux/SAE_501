using UnityEngine;
using Unity.Netcode;

public class LevelLoader : NetworkBehaviour
{
    [SerializeField] private GameObject prefabToDisable;
    [SerializeField] private GameObject prefabToEnable;

    // Subscribes to the level load event
    private void OnEnable()
    {
        LevelLoaderEvent.OnLevelLoad += HandleLevelLoad;
    }

    // Unsubscribes from the level load event
    private void OnDisable()
    {
        LevelLoaderEvent.OnLevelLoad -= HandleLevelLoad;
    }

    // Handles the level load event
    private void HandleLevelLoad()
    {
        if (IsServer)
        {
            SwitchPrefabsServerRpc();
        }
    }

    // Requests the server to switch prefabs
    [ServerRpc]
    private void SwitchPrefabsServerRpc()
    {
        SwitchPrefabsClientRpc();
    }

    // Switches prefabs on all clients
    [ClientRpc]
    private void SwitchPrefabsClientRpc()
    {
        if (prefabToDisable != null)
        {
            prefabToDisable.SetActive(false);
        }

        if (prefabToEnable != null)
        {
            prefabToEnable.SetActive(true);
        }
    }
}
