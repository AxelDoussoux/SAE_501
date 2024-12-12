using UnityEngine;
using UnityEngine.InputSystem;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation myConversation;

    private PlayerInput playerInput;
    private InputAction interactAction;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Récupérer le PlayerInput si ce n'est pas déjà fait
            if (playerInput == null)
            {
                playerInput = other.GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    // Récupérer l'action Interact
                    interactAction = playerInput.actions["Interact"];
                }
            }

            // Vérifier si l'action Interact est déclenchée
            if (interactAction != null && interactAction.triggered)
            {
                ConversationManager.Instance.StartConversation(myConversation);
            }
        }
    }
}