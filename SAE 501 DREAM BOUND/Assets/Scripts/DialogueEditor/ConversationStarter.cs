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
    private ulong authorizedPlayerNetworkId; // To store the authorized player's ID

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
                Debug.LogError("DynamicDialogueCamera not found in the scene!");
            }
        }

        if (interactionText == null)
        {
            Debug.LogWarning("interactionText is not assigned in the inspector!");
        }

        if (npc == null)
        {
            Debug.LogWarning("npc (Transform) is not assigned in the inspector!");
        }

        if (myConversation == null)
        {
            Debug.LogWarning("myConversation is not assigned in the inspector!");
        }
    }

    public void Interact(PlayerInfo playerInfo)
    {
        // Check if the player is authorized to interact
        if (playerInfo.NetworkObjectId != authorizedPlayerNetworkId)
        {
            Debug.Log($"Player {playerInfo.NetworkObjectId} is not authorized to interact. Authorized player: {authorizedPlayerNetworkId}");
            return;
        }

        if (!ValidateInteraction(playerInfo)) return;

        isInteracting = true;
        ConversationManager.Instance.StartConversation(myConversation);
        Debug.Log($"Conversation started with player: {playerInfo.name}");

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
            Debug.LogError("myConversation is not assigned in ConversationStarter.");
            return false;
        }

        if (dialogueCamera == null)
        {
            Debug.LogError("dialogueCamera is not assigned in ConversationStarter.");
            return false;
        }

        if (playerInfo == null)
        {
            Debug.LogError("PlayerInfo is null during interaction.");
            return false;
        }

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var playerInfo = other.GetComponent<PlayerInfo>();
        if (playerInfo != null && playerInfo.IsOwner)
        {
            Debug.Log($"PlayerInfo found for {playerInfo.name}!");
            isPlayerInRange = true;
            currentPlayerInfo = playerInfo;
            authorizedPlayerNetworkId = playerInfo.NetworkObjectId; // Store the authorized player's ID

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
                Debug.Log($"'Interact' action assigned for {other.name}");
            }
            else
            {
                Debug.LogError($"PlayerInput not found on {other.name}");
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
                authorizedPlayerNetworkId = 0; // Reset the authorized player's ID
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
            // Check if the current player is the authorized one to interact
            if (currentPlayerInfo.NetworkObjectId == authorizedPlayerNetworkId)
            {
                Interact(currentPlayerInfo);
            }
            else
            {
                Debug.Log($"Player {currentPlayerInfo.NetworkObjectId} is not authorized to interact. Authorized player: {authorizedPlayerNetworkId}");
            }
        }
    }
}