using UnityEngine;
using Unity.Netcode;

public class PressurePlate2 : NetworkBehaviour
{
    public Transform pressurePlate;  // La plaque de pression
    public Transform plateToMove;    // La plaque � faire monter
    public float moveAmount = 1f;    // La distance que la plaque monte (modifiable dans l'inspecteur)
    public float moveSpeed = 2f;     // Vitesse de mont�e de la plaque

    private Vector3 initialPosition; // Position initiale de la plaque

    // Initialisation pour sauvegarder la position initiale de la plaque
    private void Start()
    {
        initialPosition = plateToMove.localPosition; // Sauvegarde de la position initiale de la plaque
    }

    // Appel� � chaque fois qu'un collider entre dans la zone de la plaque de pression
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && IsServer)  // Se d�clenche si le joueur entre en contact
        {
            MovePlateServerRpc(true);  // Appelle la fonction de serveur pour monter la plaque
        }
    }

    // Appel� � chaque fois qu'un collider quitte la zone de la plaque de pression
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && IsServer)  // Se d�clenche si le joueur quitte la plaque
        {
            MovePlateServerRpc(false);  // Appelle la fonction de serveur pour abaisser la plaque � la position initiale
        }
    }

    // Fonction RPC pour le serveur qui g�re le mouvement de la plaque
    [ServerRpc(RequireOwnership = false)]
    void MovePlateServerRpc(bool playerOnPlate, ServerRpcParams rpcParams = default)
    {
        // Monte ou descend la plaque selon l'�tat de la plaque de pression
        if (playerOnPlate)
        {
            StartCoroutine(MovePlate(Vector3.up * moveAmount)); // Monte la plaque
        }
        else
        {
            StartCoroutine(MovePlate(Vector3.zero)); // Retourne � la position initiale
        }
    }

    // Coroutine pour d�placer la plaque de mani�re lisse
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

        plateToMove.localPosition = endPos;  // S'assure que la plaque est positionn�e � la fin
    }
}
