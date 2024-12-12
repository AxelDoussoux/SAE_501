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
            // R�cup�rer le PlayerInput si ce n'est pas d�j� fait
            if (playerInput == null)
            {
                playerInput = other.GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    // R�cup�rer l'action Interact
                    interactAction = playerInput.actions["Interact"];
                }
            }

            // V�rifier si l'action Interact est d�clench�e
            if (interactAction != null && interactAction.triggered)
            {
                ConversationManager.Instance.StartConversation(myConversation);
            }
        }
    }
}