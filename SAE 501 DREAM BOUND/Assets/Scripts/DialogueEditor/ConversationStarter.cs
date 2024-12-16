using UnityEngine;
using UnityEngine.InputSystem;
using DialogueEditor;
using TomAg;

public class ConversationStarter : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCConversation myConversation;

    private PlayerInput playerInput;
    private InputAction interactAction;

    // Impl�mentation de l'interface IInteractable
    public void Interact(PlayerInfo playerInfo)
    {
        if (myConversation != null)
        {
            // D�marrer la conversation avec le NPC
            ConversationManager.Instance.StartConversation(myConversation);
            Debug.Log($"Conversation d�marr�e avec le joueur : {playerInfo.name}");
        }
        else
        {
            Debug.LogWarning("Aucune conversation assign�e dans ConversationStarter.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // V�rifie si le joueur poss�de un PlayerInfo (plus s�r qu'un tag)
        var playerInfo = other.GetComponent<PlayerInfo>();
        if (playerInfo != null)
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

            // Si le joueur a press� le bouton d'interaction
            if (interactAction != null && interactAction.triggered)
            {
                // Appelle la m�thode Interact via l'interface
                Interact(playerInfo);
            }
        }
    }
}
