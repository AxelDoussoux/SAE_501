using UnityEngine;
using Unity.Netcode;

public class MovementCube : NetworkBehaviour
{
    public Vector3 positionB = Vector3.zero; // Target position, visible in Inspector
    private Vector3 positionA; // Initial position
    public float speed = 5f; // Movement speed
    private NetworkVariable<bool> shouldMove = new NetworkVariable<bool>(false);
    private Rigidbody rb; // Rigidbody component

    // Cette NetworkVariable sera utilis�e pour synchroniser positionB entre tous les clients
    private NetworkVariable<Vector3> networkPositionB = new NetworkVariable<Vector3>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // R�cup�re le Rigidbody du cube
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("[CubeMovement] Rigidbody is missing! Please add one to the Cube.");
            return;
        }

        // Sauvegarde la position initiale
        positionA = transform.position;

        // Si c'est le serveur, d�finit la position B sur le serveur
        if (IsServer)
        {
            // Assurez-vous que la position B est en espace global, peu importe le parent
            networkPositionB.Value = positionB; // Utilise la position globale d�finie dans l'inspecteur
        }

        // Ne pas utiliser la physique (on manipule la position manuellement)
        rb.isKinematic = true;

        Debug.Log($"[MovementCube] Initialized - Position A: {positionA}, Position B: {networkPositionB.Value}");
    }

    void Update()
    {
        if (rb == null) return;

        // On utilise networkPositionB pour le mouvement
        Vector3 targetPosition = shouldMove.Value ? networkPositionB.Value : positionA;

        // Si la position est encore distante, d�placer le cube
        if (Vector3.Distance(rb.position, targetPosition) > 0.01f)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
            rb.MovePosition(newPosition);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetMoveStateServerRpc(bool move, ServerRpcParams serverRpcParams = default)
    {
        // Mise � jour de l'�tat de mouvement
        shouldMove.Value = move;
        Debug.Log($"[CubeMovement] SetMoveStateServerRpc called with value: {move}");
    }

    // ServerRpc pour changer la position B, et synchroniser entre tous les clients
    [ServerRpc(RequireOwnership = false)]
    public void SetPositionBServerRpc(Vector3 newPositionB, ServerRpcParams serverRpcParams = default)
    {
        // Si c'est le serveur, on met � jour la position B et synchronise avec les clients
        if (IsServer)
        {
            // Assurez-vous que la position est en espace global, m�me si le cube est un enfant
            Vector3 globalPositionB = newPositionB;

            // Met � jour la position B sur le serveur
            networkPositionB.Value = globalPositionB;
            Debug.Log($"[CubeMovement] New positionB set to: {globalPositionB}");
        }
    }

    // M�thode pour lier la position B, appel�e par les clients si n�cessaire
    public void SetPositionB(Vector3 newPositionB)
    {
        if (IsServer)
        {
            SetPositionBServerRpc(newPositionB);
        }
        else
        {
            // Si ce n'est pas le serveur, demande la mise � jour de la position B
            SetPositionBServerRpc(newPositionB);
        }
    }
}
