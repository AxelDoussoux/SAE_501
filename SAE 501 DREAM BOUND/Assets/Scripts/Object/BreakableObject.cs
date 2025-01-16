using TomAg;
using UnityEngine;
using Unity.Netcode;

public class BreakableObject : NetworkBehaviour, IInteractable
{
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private AnimatorEvent _animEvent;

    private void Awake()
    {
        if (_playerAnimator != null)
        {
            _playerAnimator.isBreaking = false;
        }
        else
        {
            Debug.LogWarning("PlayerAnimator n'est pas assigné dans l'inspecteur.");
        }

        if (_animEvent == null)
        {
            Debug.LogWarning("AnimatorEvent n'est pas assigné dans l'inspecteur.");
        }
    }

    public void Interact(PlayerInfo playerInfo)
    {
        if (playerInfo.HaveHammer)
        {
            _animEvent.OnAnimationEvent += OnAnimationEvent;

            if (playerInfo.TryGetComponent<PlayerAnimator>(out PlayerAnimator playerAnimator))
            {
                playerAnimator.SetBreakingState(true); // Utilisez une méthode pour gérer isBreaking
            }

            Debug.Log($"{gameObject.name} commence à se briser !");
        }
        else
        {
            Debug.Log($"{gameObject.name} n'a pas été détruit ! Il vous manque le marteau...");
        }
    }

    private void OnAnimationEvent(string eventName)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning("EventName est vide ou null dans OnAnimationEvent.");
            return;
        }

        if (eventName == "HammerBreak")
        {
            OnAnimationBreakEvent();
        }

        if (_animEvent != null)
        {
            _animEvent.OnAnimationEvent -= OnAnimationEvent;
        }
    }

    private void OnAnimationBreakEvent()
    {
        Debug.Log($"{gameObject.name} a été détruit !");
        Destroy(gameObject);

        if (_playerAnimator != null)
        {
            _playerAnimator.isBreaking = false;
        }
    }
}
