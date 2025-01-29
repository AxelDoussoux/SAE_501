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
    private ulong authorizedPlayerNetworkId; // Pour stocker l'ID du joueur autorisé

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

    public void Interact(PlayerInfo playerInfo)
    {
        // Vérifier si le joueur est autorisé à interagir
        if (playerInfo.NetworkObjectId != authorizedPlayerNetworkId)
        {
            Debug.Log($"Player {playerInfo.NetworkObjectId} n'est pas autorisé à interagir. Joueur autorisé: {authorizedPlayerNetworkId}");
            return;
        }

        if (!ValidateInteraction(playerInfo)) return;

        isInteracting = true;
        ConversationManager.Instance.StartConversation(myConversation);
        Debug.Log($"Conversation démarrée avec le joueur : {playerInfo.name}");

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
            Debug.LogError("myConversation n'est pas assigné dans ConversationStarter.");
            return false;
        }

        if (dialogueCamera == null)
        {
            Debug.LogError("dialogueCamera n'est pas assigné dans ConversationStarter.");
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
        if (playerInfo != null && playerInfo.IsOwner)
        {
            Debug.Log($"PlayerInfo trouvé pour {playerInfo.name}!");
            isPlayerInRange = true;
            currentPlayerInfo = playerInfo;
            authorizedPlayerNetworkId = playerInfo.NetworkObjectId; // Stocker l'ID du joueur autorisé

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
                Debug.Log($"Action 'Interact' assignée pour {other.name}");
            }
            else
            {
                Debug.LogError($"PlayerInput non trouvé sur {other.name}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var playerInfo = other.GetComponent<PlayerInfo>();
        if (playerInfo != null && playerInfo == currentPlayerInfo)
        {
            if (playerInfo.NetworkObjectId == authorizedPlayerNetworkId)
            {
                authorizedPlayerNetworkId = 0; // Réinitialiser l'ID du joueur autorisé
            }
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
            currentPlayerInfo.IsOwner)
        {
            // Vérifier si le joueur actuel est celui autorisé à interagir
            if (currentPlayerInfo.NetworkObjectId == authorizedPlayerNetworkId)
            {
                Interact(currentPlayerInfo);
            }
            else
            {
                Debug.Log($"Player {currentPlayerInfo.NetworkObjectId} n'est pas autorisé à interagir. Joueur autorisé: {authorizedPlayerNetworkId}");
            }
        }
    }
}