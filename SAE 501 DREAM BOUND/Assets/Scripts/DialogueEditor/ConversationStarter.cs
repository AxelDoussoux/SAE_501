using UnityEngine;
using UnityEngine.InputSystem;
using DialogueEditor;
using TomAg;
using TMPro;

public class ConversationStarter : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCConversation myConversation;
    [SerializeField] private TextMeshProUGUI interactionText; // Référence au texte UI
    [SerializeField] private string message = "Appuie sur E pour interagir";
    [SerializeField] private DynamicDialogueCamera dialogueCamera;
    [SerializeField] private Transform npc;  // Référence au NPC

    private PlayerInput playerInput;
    private InputAction interactAction;
    private bool isPlayerInRange = false;
    private PlayerInfo currentPlayerInfo;

    // Implémentation de l'interface IInteractable
    public void Interact(PlayerInfo playerInfo)
    {
        if (myConversation != null)
        {
            // Démarrer la conversation avec le NPC
            ConversationManager.Instance.StartConversation(myConversation);
            Debug.Log($"Conversation démarrée avec le joueur : {playerInfo.name}");

            // Dynamically assign the player and NPC for dialogue camera
            dialogueCamera.StartDialogue(playerInfo.transform, npc);

            // Cache le texte d'interaction
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Aucune conversation assignée dans ConversationStarter.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var playerInfo = other.GetComponent<PlayerInfo>();
        if (playerInfo != null)
        {
            isPlayerInRange = true;
            currentPlayerInfo = playerInfo; // Stocke la référence pour une utilisation ultérieure

            // Affiche le texte d'interaction
            if (interactionText != null)
            {
                interactionText.text = message;
                interactionText.gameObject.SetActive(true);
            }

            // Récupérer le PlayerInput si ce n'est pas déjà fait
            if (playerInput == null)
            {
                playerInput = other.GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    interactAction = playerInput.actions["Interact"];
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // On termine le dialogue si le joueur sort de la zone
        if (other.GetComponent<PlayerInfo>() == currentPlayerInfo)
        {
            dialogueCamera.EndDialogue();
            currentPlayerInfo = null;
        }

        // Cache le texte d'interaction
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Vérifie si le joueur est dans la zone et a pressé l'action "Interact"
        if (isPlayerInRange && interactAction != null && interactAction.triggered)
        {
            Interact(currentPlayerInfo);
        }
    }
}
