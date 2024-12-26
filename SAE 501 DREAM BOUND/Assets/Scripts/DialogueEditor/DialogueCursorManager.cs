using UnityEngine;
using Unity.Netcode;

public class DialogueCursorManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject npc;
    [SerializeField] private NetworkObject npc2;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private NetworkObject morpheePatrouille;

    private NetworkVariable<bool> morpheePatrouilleActivated = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            Debug.Log("OnNetworkSpawn - Server");
            if (morpheePatrouille != null)
            {
                Debug.Log("Initializing Morphee Patrouille state");
                morpheePatrouille.gameObject.SetActive(false);
                // S'assurer que l'objet est spawned mais désactivé
                if (!morpheePatrouille.IsSpawned)
                {
                    morpheePatrouille.Spawn(false);
                }
            }
            else
            {
                Debug.LogError("Morphee Patrouille reference is null!");
            }
        }
    }

    public void EnableCursor()
    {
        if (!IsOwner) return;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DisableCursor()
    {
        if (!IsOwner) return;

        Debug.Log($"DisableCursor called - morpheePatrouilleActivated: {morpheePatrouilleActivated.Value}");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!morpheePatrouilleActivated.Value && morpheePatrouille != null)
        {
            Debug.Log("Requesting Morphee Patrouille activation");
            RequestActivateMorpheePatrouilleServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestActivateMorpheePatrouilleServerRpc()
    {
        Debug.Log("RequestActivateMorpheePatrouilleServerRpc called");
        if (morpheePatrouille != null)
        {
            Debug.Log($"Current state - IsSpawned: {morpheePatrouille.IsSpawned}, IsActive: {morpheePatrouille.gameObject.activeSelf}");

            if (!morpheePatrouilleActivated.Value)
            {
                Debug.Log("Activating Morphee Patrouille");
                morpheePatrouilleActivated.Value = true;
                ActivateMorpheePatrouilleClientRpc();
            }
        }
        else
        {
            Debug.LogError("Morphee Patrouille is null on server!");
        }
    }

    public void HideNPC1()
    {
        if (!IsServer && !IsHost)
        {
            RequestHideNPC1ServerRpc();
            return;
        }

        HandleHideNPC1();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestHideNPC1ServerRpc()
    {
        HandleHideNPC1();
    }

    private void HandleHideNPC1()
    {
        if (npc != null && npc.IsSpawned)
        {
            Vector3 position = npc.transform.position;
            SpawnParticlesClientRpc(position);
            DespawnNPCClientRpc(npc.NetworkObjectId);
        }
        else
        {
            Debug.LogWarning("NPC GameObject is not assigned or not spawned in DialogueCursorManager.");
        }
    }

    public void HideNPC2()
    {
        if (!IsServer && !IsHost)
        {
            RequestHideNPC2ServerRpc();
            return;
        }

        HandleHideNPC2();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestHideNPC2ServerRpc()
    {
        HandleHideNPC2();
    }

    private void HandleHideNPC2()
    {
        if (npc2 != null && npc2.IsSpawned)
        {
            Vector3 position = npc2.transform.position;
            SpawnParticlesClientRpc(position);
            DespawnNPCClientRpc(npc2.NetworkObjectId);
        }
        else
        {
            Debug.LogWarning("NPC2 GameObject is not assigned or not spawned in DialogueCursorManager.");
        }
    }

    public void HideMorpheePatrouille()
    {
        if (!IsServer && !IsHost)
        {
            RequestHideMorpheePatrouilleServerRpc();
            return;
        }

        HandleHideMorpheePatrouille();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestHideMorpheePatrouilleServerRpc()
    {
        HandleHideMorpheePatrouille();
    }

    private void HandleHideMorpheePatrouille()
    {
        if (morpheePatrouille != null && morpheePatrouille.IsSpawned)
        {
            Vector3 position = morpheePatrouille.transform.position;
            SpawnParticlesClientRpc(position);
            DespawnNPCClientRpc(morpheePatrouille.NetworkObjectId);
            Debug.Log("Morphee Patrouille has disappeared.");
        }
        else
        {
            Debug.LogWarning("Morphee Patrouille GameObject is not assigned or not spawned in DialogueCursorManager.");
        }
    }

    [ClientRpc]
    private void SpawnParticlesClientRpc(Vector3 position)
    {
        if (particlePrefab != null)
        {
            Instantiate(particlePrefab, position, Quaternion.identity);
        }
    }

    [ClientRpc]
    private void ActivateMorpheePatrouilleClientRpc()
    {
        Debug.Log($"ActivateMorpheePatrouilleClientRpc called on client {NetworkManager.LocalClientId}");
        if (morpheePatrouille != null)
        {
            morpheePatrouille.gameObject.SetActive(true);
            Debug.Log($"Morphee Patrouille activated on client {NetworkManager.LocalClientId}");
        }
        else
        {
            Debug.LogError($"Morphee Patrouille is null on client {NetworkManager.LocalClientId}!");
        }
    }

    [ClientRpc]
    private void DespawnNPCClientRpc(ulong networkObjectId)
    {
        NetworkObject networkObject = Unity.Netcode.NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        if (networkObject != null)
        {
            if (IsServer || IsHost)
            {
                networkObject.Despawn(true);
            }
            else
            {
                networkObject.gameObject.SetActive(false);
            }
        }
    }
}