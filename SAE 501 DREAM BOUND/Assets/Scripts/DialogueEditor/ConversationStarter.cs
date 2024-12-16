using UnityEngine;
using UnityEngine.InputSystem;
using DialogueEditor;
using TomAg;

public class ConversationStarter : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCConversation myConversation;

    private PlayerInput playerInput;
    private InputAction interactAction;

    // Implémentation de l'interface IInteractable
    public void Interact(PlayerInfo playerInfo)
    {
        if (myConversation != null)
        {
            // Démarrer la conversation avec le NPC
            ConversationManager.Instance.StartConversation(myConversation);
            Debug.Log($"Conversation démarrée avec le joueur : {playerInfo.name}");
        }
        else
        {
            Debug.LogWarning("Aucune conversation assignée dans ConversationStarter.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Vérifie si le joueur possède un PlayerInfo (plus sûr qu'un tag)
        var playerInfo = other.GetComponent<PlayerInfo>();
        if (playerInfo != null)
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

            // Si le joueur a pressé le bouton d'interaction
            if (interactAction != null && interactAction.triggered)
            {
                // Appelle la méthode Interact via l'interface
                Interact(playerInfo);
            }
        }
    }
}
