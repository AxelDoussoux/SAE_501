using TomAg;
using UnityEngine;
using Unity.Netcode;

public class BreakableObject : NetworkBehaviour, IInteractable
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem breakParticlePrefab;
    [SerializeField] private GameObject decalPrefab;
    [SerializeField] private float particleLifetime = 2f;
    [SerializeField] private float decalLifetime = 5f;
    [SerializeField] private float decalGroundOffset = 0.01f;

    public void Interact(PlayerInfo playerInfo)
    {
        if (playerInfo.HaveHammer)
        {
            // Request destruction from the server
            RequestDestructionServerRpc();
        }
        else
        {
            Debug.Log($"{gameObject.name} n'a pas été détruit ! Il vous manque le marteau...");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDestructionServerRpc()
    {
        // Server validates and triggers the destruction on all clients
        SpawnEffectsClientRpc(transform.position);

        // Destroy the object on all clients
        NetworkObject.Despawn(true);
    }

    [ClientRpc]
    private void SpawnEffectsClientRpc(Vector3 position)
    {
        // Spawn break particles
        if (breakParticlePrefab != null)
        {
            ParticleSystem particles = Instantiate(breakParticlePrefab, position, Quaternion.identity);
            particles.Play();
            Destroy(particles.gameObject, particleLifetime);
        }

        // Spawn decal on the ground
        if (decalPrefab != null && Physics.Raycast(position, Vector3.down, out RaycastHit hit))
        {
            Vector3 decalPosition = hit.point + (Vector3.up * decalGroundOffset);
            Quaternion decalRotation = Quaternion.LookRotation(hit.normal);

            GameObject decal = Instantiate(decalPrefab, decalPosition, decalRotation);
            StartCoroutine(FadeAndDestroyDecal(decal));
        }

        Debug.Log($"{gameObject.name} a été détruit !");
    }

    private System.Collections.IEnumerator FadeAndDestroyDecal(GameObject decal)
    {
        Renderer[] renderers = decal.GetComponentsInChildren<Renderer>();
        float elapsedTime = 0f;

        while (elapsedTime < decalLifetime)
        {
            float alpha = 1f - (elapsedTime / decalLifetime);

            foreach (Renderer renderer in renderers)
            {
                Color color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(decal);
    }
}