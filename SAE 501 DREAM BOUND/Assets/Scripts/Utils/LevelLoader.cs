using UnityEngine;
using Unity.Netcode;

public class LevelLoader : NetworkBehaviour
{
    [SerializeField] private GameObject prefabToDisable;
    [SerializeField] private GameObject prefabToEnable;

    private void OnEnable()
    {
        LevelLoaderEvent.OnLevelLoad += HandleLevelLoad;
    }

    private void OnDisable()
    {
        LevelLoaderEvent.OnLevelLoad -= HandleLevelLoad;
    }

    private void HandleLevelLoad()
    {
        if (IsServer)
        {
            SwitchPrefabsServerRpc();
        }
    }

    [ServerRpc]
    private void SwitchPrefabsServerRpc()
    {
        SwitchPrefabsClientRpc();
    }

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


