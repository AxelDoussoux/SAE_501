using TomAg;
using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class BreakableObject : NetworkBehaviour, IInteractable
{
    [SerializeField] private PlayerAnimator _playerAnimator;



    AnimatorEvent _animEvent;

    public void Awake()
    {
        _playerAnimator.TryGetComponent(out _animEvent);
    }

    public void Interact(PlayerInfo playerInfo)
    {
        if (playerInfo.HaveHammer)
        {
        _playerAnimator._isBreaking = true;
            _animEvent.onAnimationEvent += onAnimationEvent;
            Debug.Log($"{gameObject.name} commence à se briser !");
        }
        else
        {
            Debug.Log($"{gameObject.name} n'a pas été détruit ! Il vous manque le marteau...");
        }
    }

    private void onAnimationEvent(string arg)
    {
        Debug.Log($"En attente");
        if (arg == "HammerBreak")
        {
            OnAnimationBreakEvent();
        }
        _animEvent.onAnimationEvent -= onAnimationEvent;
    }

    public void OnAnimationBreakEvent()
    {
        Debug.Log($"{gameObject.name} a été détruit !");
        Destroy(gameObject);
        _playerAnimator._isBreaking = false;
    }
}
