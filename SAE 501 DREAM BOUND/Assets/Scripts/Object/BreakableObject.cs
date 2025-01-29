using TomAg;
using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class BreakableObject : NetworkBehaviour, IInteractable
{
    private float _destroyAfterTime = 1.5f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem breakParticles;
    [SerializeField] private GameObject decalPrefab;

    public void Interact(PlayerInfo playerInfo)
    {
        if (!IsServer) return; // Only the server handles the interaction

        if (this == null || gameObject == null) return;

        if (playerInfo.HaveHammer)
        {
            if (playerInfo.TryGetComponent<PlayerAnimator>(out PlayerAnimator playerAnimator))
            {
                playerAnimator.HammerBreak();
                StartCoroutine(DestroyObjectCoroutine(playerAnimator));
            }

            Debug.Log($"{gameObject.name} is starting to break!");
        }
        else
        {
            Debug.Log($"{gameObject.name} was not destroyed! You're missing the hammer...");
        }
    }

    private IEnumerator DestroyObjectCoroutine(PlayerAnimator playerAnimator)
    {
        yield return new WaitForSeconds(_destroyAfterTime);
        DestroyObjectServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyObjectServerRpc()
    {
        if (!IsServer) return;

        Debug.Log($"{gameObject.name} has been destroyed!");

        Vector3 position = transform.position;

        // Synchronize on all clients to display effects
        SpawnEffectsClientRpc(position);

        // Synchronize to destroy the object
        DestroyObjectClientRpc();
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

    [ClientRpc]
    private void DestroyObjectClientRpc()
    {
        Destroy(gameObject);
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

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
