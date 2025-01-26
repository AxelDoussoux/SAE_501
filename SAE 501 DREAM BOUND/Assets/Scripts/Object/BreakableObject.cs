using TomAg;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;

public class BreakableObject : NetworkBehaviour, IInteractable
{
    private float _destroyAfterTime = 1.5f;
    [Header("Effects")]
    [SerializeField] private ParticleSystem breakParticles;
    [SerializeField] private GameObject decalPrefab;

    public void Interact(PlayerInfo playerInfo)
    {
        if (!IsServer) return;

        if (playerInfo.HaveHammer)
        {
            if (playerInfo.TryGetComponent<PlayerAnimator>(out PlayerAnimator playerAnimator))
            {
                playerAnimator.HammerBreak();
                StartCoroutine(DestroyObjectCoroutine(playerAnimator));
            }
            Debug.Log($"{gameObject.name} commence à se briser !");
        }
        else
        {
            Debug.Log($"{gameObject.name} n'a pas été détruit ! Il vous manque le marteau...");
        }
    }

    private void DestroyObject(PlayerAnimator playerAnimator)
    {
        Debug.Log($"{gameObject.name} a été détruit !");

        // Spawn effects before destroying the object
        SpawnEffectsClientRpc(transform.position);

        NetworkObject.Destroy(gameObject);
    }

    private IEnumerator DestroyObjectCoroutine(PlayerAnimator playerAnimator)
    {
        yield return new WaitForSeconds(_destroyAfterTime);
        DestroyObject(playerAnimator);
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

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

}
