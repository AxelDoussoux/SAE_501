using UnityEngine;
using Unity.Netcode;

public class PressurePlate2 : NetworkBehaviour
{
    public Transform pressurePlate;  // La plaque de pression
    public Transform plateToMove;    // La plaque à faire monter
    public float moveAmount = 1f;    // La distance que la plaque monte (modifiable dans l'inspecteur)
    public float moveSpeed = 2f;     // Vitesse de montée de la plaque

    private Vector3 initialPosition; // Position initiale de la plaque

    // Initialisation pour sauvegarder la position initiale de la plaque
    private void Start()
    {
        initialPosition = plateToMove.localPosition; // Sauvegarde de la position initiale de la plaque
    }

    // Appelé à chaque fois qu'un collider entre dans la zone de la plaque de pression
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && IsServer)  // Se déclenche si le joueur entre en contact
        {
            MovePlateServerRpc(true);  // Appelle la fonction de serveur pour monter la plaque
        }
    }

    // Appelé à chaque fois qu'un collider quitte la zone de la plaque de pression
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && IsServer)  // Se déclenche si le joueur quitte la plaque
        {
            MovePlateServerRpc(false);  // Appelle la fonction de serveur pour abaisser la plaque à la position initiale
        }
    }

    // Fonction RPC pour le serveur qui gère le mouvement de la plaque
    [ServerRpc(RequireOwnership = false)]
    void MovePlateServerRpc(bool playerOnPlate, ServerRpcParams rpcParams = default)
    {
        // Monte ou descend la plaque selon l'état de la plaque de pression
        if (playerOnPlate)
        {
            StartCoroutine(MovePlate(Vector3.up * moveAmount)); // Monte la plaque
        }
        else
        {
            StartCoroutine(MovePlate(Vector3.zero)); // Retourne à la position initiale
        }
    }

    // Coroutine pour déplacer la plaque de manière lisse
    private System.Collections.IEnumerator MovePlate(Vector3 targetPosition)
    {
        Vector3 startPos = plateToMove.localPosition;
        Vector3 endPos = (targetPosition == Vector3.zero) ? initialPosition : startPos + targetPosition;

        float elapsedTime = 0f;

        while (elapsedTime < moveSpeed)
        {
            plateToMove.localPosition = Vector3.Lerp(startPos, endPos, elapsedTime / moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        plateToMove.localPosition = endPos;  // S'assure que la plaque est positionnée à la fin
    }
}
