using UnityEngine;

public class DynamicDialogueCamera : MonoBehaviour
{
    // Suppression de la référence au joueur dans l'inspecteur
    [SerializeField] private Transform npc; // Le NPC
    [SerializeField] private float distanceFromPlayer = 10f;  // Distance de la caméra par rapport au joueur
    [SerializeField] private float heightOffset = 2f;        // Hauteur de la caméra
    [SerializeField] private float smoothing = 0.1f;         // Lissage du mouvement de la caméra

    private Camera dialogueCamera;  // Référence à la caméra de dialogue
    private bool isInDialogue = false;  // Si le joueur est en dialogue
    private Transform player;  // Référence dynamique au joueur qui interagit

    private void Start()
    {
        dialogueCamera = GetComponent<Camera>();
        if (dialogueCamera != null)
        {
            dialogueCamera.gameObject.SetActive(false);  // La caméra de dialogue est désactivée par défaut
        }
    }

    private void Update()
    {
        if (isInDialogue)
        {
            UpdateCameraPosition();  // Met à jour la position de la caméra pendant le dialogue
        }
    }

    public void StartDialogue(Transform playerTransform, Transform npcTransform)
    {
        // Commence la conversation et active la caméra de dialogue
        player = playerTransform;  // Assigner dynamiquement le joueur actif
        npc = npcTransform;        // Assigner le NPC

        // Active la caméra de dialogue
        dialogueCamera.gameObject.SetActive(true);

        isInDialogue = true;
    }

    public void EndDialogue()
    {
        // Termine la conversation et désactive la caméra de dialogue
        isInDialogue = false;

        if (dialogueCamera != null)
        {
            dialogueCamera.gameObject.SetActive(false);  // Désactive la caméra de dialogue après la conversation
        }
    }

    private void UpdateCameraPosition()
    {
        if (player == null || npc == null) return;

        // Calculer le point moyen entre le joueur et le NPC
        Vector3 midpoint = (player.position + npc.position) / 2f;

        // Calculer une position derrière et au-dessus du joueur et du NPC
        Vector3 cameraPosition = midpoint - player.forward * distanceFromPlayer + Vector3.up * heightOffset;

        // Effectuer un lissage pour rendre le mouvement de la caméra plus fluide
        transform.position = Vector3.Lerp(transform.position, cameraPosition, smoothing);

        // Faire en sorte que la caméra regarde toujours vers le point moyen entre le joueur et le NPC
        transform.LookAt(midpoint);
    }
}