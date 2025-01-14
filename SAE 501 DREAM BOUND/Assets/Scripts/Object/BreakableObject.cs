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

// New component to handle decal lifetime
public class DecalLifetime : MonoBehaviour
{
    private float duration;
    private float startTime;
    private DecalProjector projector;
    private Material material;
    private bool initialized = false;

    public void Initialize(float duration)
    {
        this.duration = duration;
        this.startTime = Time.time;

        projector = GetComponent<DecalProjector>();
        if (projector != null)
        {
            material = new Material(projector.material);
            projector.material = material;
        }

        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        float elapsed = Time.time - startTime;
        if (elapsed >= duration)
        {
            Destroy(gameObject);
            return;
        }

        float alpha = 1 - (elapsed / duration);
        if (projector != null && material != null)
        {
            Color color = material.color;
            material.color = new Color(color.r, color.g, color.b, alpha);
        }
    }
}