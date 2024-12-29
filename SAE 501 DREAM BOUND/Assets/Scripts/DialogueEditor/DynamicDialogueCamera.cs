using UnityEngine;

public class DynamicDialogueCamera : MonoBehaviour
{
    // Suppression de la r�f�rence au joueur dans l'inspecteur
    [SerializeField] private Transform npc; // Le NPC
    [SerializeField] private float distanceFromPlayer = 10f;  // Distance de la cam�ra par rapport au joueur
    [SerializeField] private float heightOffset = 2f;        // Hauteur de la cam�ra
    [SerializeField] private float smoothing = 0.1f;         // Lissage du mouvement de la cam�ra

    private Camera dialogueCamera;  // R�f�rence � la cam�ra de dialogue
    private bool isInDialogue = false;  // Si le joueur est en dialogue
    private Transform player;  // R�f�rence dynamique au joueur qui interagit

    private void Start()
    {
        dialogueCamera = GetComponent<Camera>();
        if (dialogueCamera != null)
        {
            dialogueCamera.gameObject.SetActive(false);  // La cam�ra de dialogue est d�sactiv�e par d�faut
        }
    }

    private void Update()
    {
        if (isInDialogue)
        {
            UpdateCameraPosition();  // Met � jour la position de la cam�ra pendant le dialogue
        }
    }

    public void StartDialogue(Transform playerTransform, Transform npcTransform)
    {
        // Commence la conversation et active la cam�ra de dialogue
        player = playerTransform;  // Assigner dynamiquement le joueur actif
        npc = npcTransform;        // Assigner le NPC

        // Active la cam�ra de dialogue
        dialogueCamera.gameObject.SetActive(true);

        isInDialogue = true;
    }

    public void EndDialogue()
    {
        // Termine la conversation et d�sactive la cam�ra de dialogue
        isInDialogue = false;

        if (dialogueCamera != null)
        {
            dialogueCamera.gameObject.SetActive(false);  // D�sactive la cam�ra de dialogue apr�s la conversation
        }
    }

    private void UpdateCameraPosition()
    {
        if (player == null || npc == null) return;

        // Calculer le point moyen entre le joueur et le NPC
        Vector3 midpoint = (player.position + npc.position) / 2f;

        // Calculer une position derri�re et au-dessus du joueur et du NPC
        Vector3 cameraPosition = midpoint - player.forward * distanceFromPlayer + Vector3.up * heightOffset;

        // Effectuer un lissage pour rendre le mouvement de la cam�ra plus fluide
        transform.position = Vector3.Lerp(transform.position, cameraPosition, smoothing);

        // Faire en sorte que la cam�ra regarde toujours vers le point moyen entre le joueur et le NPC
        transform.LookAt(midpoint);
    }
}