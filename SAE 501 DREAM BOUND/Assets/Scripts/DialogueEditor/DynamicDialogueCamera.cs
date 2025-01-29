using UnityEngine;

public class DynamicDialogueCamera : MonoBehaviour
{
   
    [SerializeField] private Transform npc; 
    [SerializeField] private float distanceFromPlayer = 10f;  
    [SerializeField] private float heightOffset = 2f;       
    [SerializeField] private float smoothing = 0.1f;         

    private Camera dialogueCamera;  
    private bool isInDialogue = false;  
    private Transform player;  

    private void Start()
    {
        dialogueCamera = GetComponent<Camera>();
        if (dialogueCamera != null)
        {
            dialogueCamera.gameObject.SetActive(false);  // The dialogue camera is disabled by default
        }
    }

    private void Update()
    {
        if (isInDialogue)
        {
            UpdateCameraPosition();  // Updates the camera position during the dialogue
        }
    }

    public void StartDialogue(Transform playerTransform, Transform npcTransform)
    {
        // Starts the conversation and activates the dialogue camera
        player = playerTransform;  
        npc = npcTransform;        

        // Activate the dialogue camera
        dialogueCamera.gameObject.SetActive(true);

        isInDialogue = true;
    }

    public void EndDialogue()
    {
        // Ends the conversation and disables the dialogue camera
        isInDialogue = false;

        if (dialogueCamera != null)
        {
            dialogueCamera.gameObject.SetActive(false);  // Disable the dialogue camera after the conversation
        }
    }

    private void UpdateCameraPosition()
    {
        if (player == null || npc == null) return;

        // Calculate the midpoint between the player and the NPC
        Vector3 midpoint = (player.position + npc.position) / 2f;

        // Calculate a position behind and above the player and NPC
        Vector3 cameraPosition = midpoint - player.forward * distanceFromPlayer + Vector3.up * heightOffset;

        // Smooth the movement to make the camera transition more fluid
        transform.position = Vector3.Lerp(transform.position, cameraPosition, smoothing);

        // Ensure the camera always looks at the midpoint between the player and the NPC
        transform.LookAt(midpoint);
    }
}
