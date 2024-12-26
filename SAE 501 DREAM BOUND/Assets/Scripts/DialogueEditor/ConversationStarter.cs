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

    private void Start()
    {
        // Vérification dynamique si les références ne sont pas assignées dans l'inspecteur
        if (dialogueCamera == null)
        {
            dialogueCamera = FindObjectOfType<DynamicDialogueCamera>();
            if (dialogueCamera == null)
            {
                Debug.LogError("DynamicDialogueCamera non trouvé dans la scène !");
            }
        }

        if (interactionText == null)
        {
            Debug.LogWarning("interactionText n'est pas assigné dans l'inspecteur !");
        }

        if (npc == null)
        {
            Debug.LogWarning("npc (Transform) n'est pas assigné dans l'inspecteur !");
        }

        if (myConversation == null)
        {
            Debug.LogWarning("myConversation n'est pas assigné dans l'inspecteur !");
        }
    }

    // Implémentation de l'interface IInteractable
    public void Interact(PlayerInfo playerInfo)
    {
        if (myConversation == null)
        {
            Debug.LogError("myConversation n'est pas assigné dans ConversationStarter.");
            return;
        }

        if (dialogueCamera == null)
        {
            Debug.LogError("dialogueCamera n'est pas assigné dans ConversationStarter.");
            return;
        }

        // Démarrer la conversation
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

    private void OnTriggerEnter(Collider other)
    {
        var playerInfo = other.GetComponent<PlayerInfo>();
        if (playerInfo != null)
        {
            Debug.Log("PlayerInfo trouvé !");
            isPlayerInRange = true;
            currentPlayerInfo = playerInfo; // Stocke la référence pour une utilisation ultérieure

            // Affiche le texte d'interaction uniquement pour le client local
            if (interactionText != null && playerInfo.IsOwner)
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
                    Debug.Log("Action 'Interact' assignée.");
                }
                else
                {
                    Debug.LogError("PlayerInput non trouvé sur l'objet.");
                }
            }
        }
        else
        {
            Debug.LogError("PlayerInfo non trouvé sur l'objet entrant dans le trigger.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var playerInfo = other.GetComponent<PlayerInfo>();
        if (playerInfo != null && playerInfo == currentPlayerInfo)
        {
            dialogueCamera?.EndDialogue();
            currentPlayerInfo = null;

            // Cache le texte d'interaction uniquement pour le client local
            if (interactionText != null && playerInfo.IsOwner)
            {
                interactionText.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // Vérifie si le joueur est dans la zone et a pressé l'action "Interact"
        if (isPlayerInRange && interactAction != null && interactAction.triggered)
        {
            if (currentPlayerInfo != null)
            {
                Interact(currentPlayerInfo);
            }
            else
            {
                Debug.LogWarning("currentPlayerInfo est null au moment d'interagir.");
            }
        }
    }
}
