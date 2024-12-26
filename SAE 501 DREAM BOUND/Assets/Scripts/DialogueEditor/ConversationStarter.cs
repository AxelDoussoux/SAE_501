using UnityEngine;
using UnityEngine.InputSystem;
using DialogueEditor;
using TomAg;
using TMPro;

public class ConversationStarter : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCConversation myConversation;
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private string message = "Appuie sur E pour interagir";
    [SerializeField] private DynamicDialogueCamera dialogueCamera;
    [SerializeField] private Transform npc;

    private PlayerInput playerInput;
    private InputAction interactAction;
    private bool isPlayerInRange = false;
    private PlayerInfo currentPlayerInfo;
    private bool isInteracting = false;

    private void Start()
    {
        ValidateReferences();
    }

    private void ValidateReferences()
    {
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

    public void Interact(PlayerInfo playerInfo)
    {
        if (!ValidateInteraction(playerInfo)) return;

        isInteracting = true;
        ConversationManager.Instance.StartConversation(myConversation);
        Debug.Log($"Conversation d�marr�e avec le joueur : {playerInfo.name}");

        dialogueCamera.StartDialogue(playerInfo.transform, npc);

        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    private bool ValidateInteraction(PlayerInfo playerInfo)
    {
        if (myConversation == null)
        {
            Debug.LogError("myConversation n'est pas assign� dans ConversationStarter.");
            return false;
        }

        if (dialogueCamera == null)
        {
            Debug.LogError("dialogueCamera n'est pas assign� dans ConversationStarter.");
            return false;
        }

        if (playerInfo == null)
        {
            Debug.LogError("PlayerInfo est null lors de l'interaction.");
            return false;
        }

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var playerInfo = other.GetComponent<PlayerInfo>();
        if (playerInfo != null && playerInfo.IsOwner)  // V�rifie si c'est le propri�taire local
        {
            Debug.Log($"PlayerInfo trouv� pour {playerInfo.name}!");
            isPlayerInRange = true;
            currentPlayerInfo = playerInfo;

            if (interactionText != null)
            {
                interactionText.text = message;
                interactionText.gameObject.SetActive(true);
            }

            SetupPlayerInput(other);
        }
    }

    private void SetupPlayerInput(Collider other)
    {
        if (playerInput == null)
        {
            playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                interactAction = playerInput.actions["Interact"];
                Debug.Log($"Action 'Interact' assign�e pour {other.name}");
            }
            else
            {
                Debug.LogError($"PlayerInput non trouv� sur {other.name}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var playerInfo = other.GetComponent<PlayerInfo>();
        if (playerInfo != null && playerInfo == currentPlayerInfo)
        {
            CleanupInteraction(playerInfo);
        }
    }

    private void CleanupInteraction(PlayerInfo playerInfo)
    {
        if (isInteracting)
        {
            dialogueCamera?.EndDialogue();
        }

        isInteracting = false;
        isPlayerInRange = false;
        currentPlayerInfo = null;

        if (interactionText != null && playerInfo.IsOwner)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInRange && !isInteracting && interactAction != null &&
            interactAction.triggered && currentPlayerInfo != null &&
            currentPlayerInfo.IsOwner)  // V�rifie si c'est le propri�taire local
        {
            Interact(currentPlayerInfo);
        }
    }
}