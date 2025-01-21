using TomAg;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;

public class BreakableObject : NetworkBehaviour, IInteractable
{
    private float _destroyAfterTime = 1.5f;

    public void Interact(PlayerInfo playerInfo)
    {
        if (this == null || gameObject == null) return;

        if (playerInfo.HaveHammer)
        {
            if (playerInfo.TryGetComponent<PlayerAnimator>(out PlayerAnimator playerAnimator))
            {
                playerAnimator.HammerBreak();
                StartCoroutine(DestroyObjectCoroutine(playerAnimator));
            }

            Debug.Log($"{gameObject.name} commence � se briser !");
        }
        else
        {
            Debug.Log($"{gameObject.name} n'a pas �t� d�truit ! Il vous manque le marteau...");
        }
    }

    private void DestroyObject(PlayerAnimator playerAnimator)
    {
        Debug.Log($"{gameObject.name} a �t� d�truit !");
        Destroy(gameObject);
    }

    private IEnumerator DestroyObjectCoroutine(PlayerAnimator playerAnimator)
    {
        yield return new WaitForSeconds(_destroyAfterTime);
        DestroyObject(playerAnimator);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

}
