using UnityEngine;
using UnityEngine.InputSystem;
using DialogueEditor;
using TomAg;
using TMPro;

public class ConversationStarter : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCConversation myConversation;
    [SerializeField] private TextMeshProUGUI interactionText; // R�f�rence au texte UI
    [SerializeField] private string message = "Appuie sur E pour interagir";
    [SerializeField] private DynamicDialogueCamera dialogueCamera;
    [SerializeField] private Transform npc;  // R�f�rence au NPC

    private PlayerInput playerInput;
    private InputAction interactAction;
    private bool isPlayerInRange = false;
    private PlayerInfo currentPlayerInfo;

    private void Start()
    {
        // V�rification dynamique si les r�f�rences ne sont pas assign�es dans l'inspecteur
        if (dialogueCamera == null)
        {
            dialogueCamera = FindObjectOfType<DynamicDialogueCamera>();
            if (dialogueCamera == null)
            {
                Debug.LogError("DynamicDialogueCamera non trouv� dans la sc�ne !");
            }
        }

        if (interactionText == null)
        {
            Debug.LogWarning("interactionText n'est pas assign� dans l'inspecteur !");
        }

        if (npc == null)
        {
            Debug.LogWarning("npc (Transform) n'est pas assign� dans l'inspecteur !");
        }

        if (myConversation == null)
        {
            Debug.LogWarning("myConversation n'est pas assign� dans l'inspecteur !");
        }
    }

    // Impl�mentation de l'interface IInteractable
    public void Interact(PlayerInfo playerInfo)
    {
        if (myConversation == null)
        {
            Debug.LogError("myConversation n'est pas assign� dans ConversationStarter.");
            return;
        }

        if (dialogueCamera == null)
        {
            Debug.LogError("dialogueCamera n'est pas assign� dans ConversationStarter.");
            return;
        }

        // D�marrer la conversation
        ConversationManager.Instance.StartConversation(myConversation);
        Debug.Log($"Conversation d�marr�e avec le joueur : {playerInfo.name}");

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
            Debug.Log("PlayerInfo trouv� !");
            isPlayerInRange = true;
            currentPlayerInfo = playerInfo; // Stocke la r�f�rence pour une utilisation ult�rieure

            // Affiche le texte d'interaction uniquement pour le client local
            if (interactionText != null && playerInfo.IsOwner)
            {
                interactionText.text = message;
                interactionText.gameObject.SetActive(true);
            }

            // R�cup�rer le PlayerInput si ce n'est pas d�j� fait
            if (playerInput == null)
            {
                playerInput = other.GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    interactAction = playerInput.actions["Interact"];
                    Debug.Log("Action 'Interact' assign�e.");
                }
                else
                {
                    Debug.LogError("PlayerInput non trouv� sur l'objet.");
                }
            }
        }
        else
        {
            Debug.LogError("PlayerInfo non trouv� sur l'objet entrant dans le trigger.");
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
        // V�rifie si le joueur est dans la zone et a press� l'action "Interact"
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
