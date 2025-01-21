using TomAg;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering.Universal;

public class BreakableObject : NetworkBehaviour, IInteractable
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem breakParticles;
    [SerializeField] private GameObject decalPrefab;

    public void Interact(PlayerInfo playerInfo)
    {
        if (!IsServer)
        {
            RequestBreakServerRpc(playerInfo.HaveHammer);
            return;
        }
        HandleBreak(playerInfo.HaveHammer);
    }

    private void HandleBreak(bool hasHammer)
    {
        if (hasHammer)
        {
            SpawnEffectsClientRpc(transform.position);
            Debug.Log($"{gameObject.name} a été détruit !");
            // First notify clients that the object is being destroyed
            DestroyObjectClientRpc();
            // Then destroy the object on the server
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"{gameObject.name} n'a pas été détruit ! Il vous manque le marteau...");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestBreakServerRpc(bool hasHammer)
    {
        HandleBreak(hasHammer);
    }

    [ClientRpc]
    private void DestroyObjectClientRpc()
    {
        if (!IsServer)  // Only destroy on clients, server handles its own destruction
        {
            Destroy(gameObject);
        }
    }

    [ClientRpc]
    private void SpawnEffectsClientRpc(Vector3 position)
    {
        if (breakParticles != null)
        {
            ParticleSystem particles = Instantiate(breakParticles, position, Quaternion.identity);
            particles.Play();
        }
        SpawnDecal(position);
    }

    private void SpawnDecal(Vector3 position)
    {
        if (Physics.Raycast(position, Vector3.down, out RaycastHit hit))
        {
            Vector3 decalPosition = hit.point + hit.normal * 0.01f;
            Quaternion decalRotation = Quaternion.LookRotation(-hit.normal);
            Instantiate(decalPrefab, decalPosition, decalRotation);
        }
    }
}