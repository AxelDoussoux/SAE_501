using TomAg;
using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class BreakableObject : NetworkBehaviour, IInteractable
{
    [SerializeField] private PlayerAnimator _playerAnimator;

    public void Interact(PlayerInfo playerInfo)
    {
        if (playerInfo.HaveHammer)
        {
            StartCoroutine(ActivateBreaking());
            Debug.Log($"{gameObject.name} commence � se briser !");
        }
        else
        {
            Debug.Log($"{gameObject.name} n'a pas �t� d�truit ! Il vous manque le marteau...");
        }
    }

    private IEnumerator ActivateBreaking()
    {
        _playerAnimator._isBreaking = true;
        yield return new WaitForSeconds(0.1f);
        _playerAnimator._isBreaking = false;
    }

    public void OnAnimationBreakEvent()
    {
        Debug.Log($"{gameObject.name} a �t� d�truit !");
        Destroy(gameObject);
    }
}
