using TomAg;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering.Universal;

public class BreakableObject : NetworkBehaviour, IInteractable
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem breakParticles;
    [SerializeField] private GameObject decalPrefab;
    [SerializeField] private float particleLifetime = 2f;
    [SerializeField] private float decalDuration = 5f;

    public void Interact(PlayerInfo playerInfo)
    {
        if (!IsServer)
        {
            RequestBreakServerRpc();
            return;
        }

        if (playerInfo.HaveHammer)
        {
            SpawnEffectsClientRpc(transform.position);
            Debug.Log($"{gameObject.name} a été détruit !");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"{gameObject.name} n'a pas été détruit ! Il vous manque le marteau...");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestBreakServerRpc()
    {
    }

    [ClientRpc]
    private void SpawnEffectsClientRpc(Vector3 position)
    {
        if (breakParticles != null)
        {
            ParticleSystem particles = Instantiate(breakParticles, position, Quaternion.identity);
            particles.Play();
            Destroy(particles.gameObject, particleLifetime);
        }

        SpawnDecal(position);
    }

    private void SpawnDecal(Vector3 position)
    {
        if (Physics.Raycast(position, Vector3.down, out RaycastHit hit))
        {
            Vector3 decalPosition = hit.point + hit.normal * 0.01f;
            Quaternion decalRotation = Quaternion.LookRotation(-hit.normal);

            GameObject decal = Instantiate(decalPrefab, decalPosition, decalRotation);

            // Add a component to handle the decal's lifetime
            DecalLifetime lifetimeHandler = decal.AddComponent<DecalLifetime>();
            lifetimeHandler.Initialize(decalDuration);
        }
    }
}

